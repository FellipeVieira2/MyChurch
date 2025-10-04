/// <reference types="cypress" />

describe('?? Event API Tests', () => {
  let adminToken;
  let memberToken;
  let createdEventId;
  let createdWorshipId;

  before(() => {
    cy.loginAsAdmin().then((token) => {
      adminToken = token;
    });
    cy.loginAsMember().then((token) => {
      memberToken = token;
    });
  });

  describe('POST /event - Create Event', () => {
    it('should create general event as admin', () => {
      const eventData = {
        title: `Test Event ${Date.now()}`,
        description: 'Event for testing purposes',
        date: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(), // 7 days from now
        finishDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000 + 2 * 60 * 60 * 1000).toISOString(), // +2 hours
        location: 'Main Hall',
        requiresParticipantList: true,
        eventType: 0 // General
      };

      cy.apiRequest('POST', '/event', adminToken, eventData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
        createdEventId = response.body;
      });
    });

    it('should create worship service event as admin', () => {
      const worshipData = {
        title: 'Sunday Worship Service',
        description: 'Weekly Sunday service',
        date: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000).toISOString(),
        finishDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000 + 2 * 60 * 60 * 1000).toISOString(),
        location: 'Main Temple',
        requiresParticipantList: false,
        eventType: 1, // WorshipService
        worshipTheme: 'God\'s Love'
      };

      cy.apiRequest('POST', '/event', adminToken, worshipData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
        createdWorshipId = response.body;
      });
    });

    it('should create recurring weekly event as admin', () => {
      const recurringEvent = {
        title: 'Weekly Bible Study',
        description: 'Every Wednesday',
        date: new Date(Date.now() + 1 * 24 * 60 * 60 * 1000).toISOString(),
        finishDate: new Date(Date.now() + 1 * 24 * 60 * 60 * 1000 + 1.5 * 60 * 60 * 1000).toISOString(),
        location: 'Room 101',
        requiresParticipantList: true,
        eventType: 2, // Meeting
        recurrenceType: 2, // Weekly
        frequency: 1
      };

      cy.apiRequest('POST', '/event', adminToken, recurringEvent).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
      });
    });

    it('should fail to create event without title', () => {
      const invalidEvent = {
        date: new Date().toISOString(),
        finishDate: new Date(Date.now() + 2 * 60 * 60 * 1000).toISOString(),
        eventType: 0
      };

      cy.apiRequest('POST', '/event', adminToken, invalidEvent).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail to create event as member', () => {
      const eventData = {
        title: 'Unauthorized Event',
        date: new Date().toISOString(),
        finishDate: new Date(Date.now() + 2 * 60 * 60 * 1000).toISOString(),
        eventType: 0
      };

      cy.apiRequest('POST', '/event', memberToken, eventData).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('GET /event/{id} - Get Event by ID', () => {
    it('should get event by id as admin', () => {
      cy.apiRequest('GET', `/event/${createdEventId}`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id', createdEventId);
        expect(response.body).to.have.property('title');
        expect(response.body).to.have.property('date');
        expect(response.body).to.have.property('eventType');
      });
    });

    it('should get event as member', () => {
      cy.apiRequest('GET', `/event/${createdEventId}`, memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id', createdEventId);
      });
    });

    it('should fail to get non-existent event', () => {
      cy.apiRequest('GET', '/event/99999', adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', `/event/${createdEventId}`).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('PUT /event/{id} - Update Event', () => {
    it('should update event as admin', () => {
      const updatedData = {
        title: `Updated Event ${Date.now()}`,
        description: 'Updated description',
        location: 'Updated Location'
      };

      cy.apiRequest('PUT', `/event/${createdEventId}`, adminToken, updatedData).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should update event dates', () => {
      const updatedData = {
        date: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
        finishDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000 + 3 * 60 * 60 * 1000).toISOString()
      };

      cy.apiRequest('PUT', `/event/${createdEventId}`, adminToken, updatedData).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail to update non-existent event', () => {
      const updatedData = {
        title: 'Non-existent Event'
      };

      cy.apiRequest('PUT', '/event/99999', adminToken, updatedData).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });

    it('should fail to update as member', () => {
      const updatedData = {
        title: 'Unauthorized Update'
      };

      cy.apiRequest('PUT', `/event/${createdEventId}`, memberToken, updatedData).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('GET /event/calendar - Get Events for Calendar', () => {
    it('should get events for current month', () => {
      const now = new Date();
      const year = now.getFullYear();
      const month = now.getMonth() + 1;

      cy.apiRequest('GET', `/event/calendar?year=${year}&month=${month}`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
      });
    });

    it('should get events for specific month', () => {
      cy.apiRequest('GET', '/event/calendar?year=2025&month=12', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
      });
    });

    it('should fail without year parameter', () => {
      cy.apiRequest('GET', '/event/calendar?month=1', adminToken).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/event/calendar?year=2025&month=1').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('GET /event/worship - Get All Worship Services', () => {
    it('should get all worship services with pagination', () => {
      cy.apiRequest('GET', '/event/worship?page=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('items');
        expect(response.body).to.have.property('page');
        expect(response.body).to.have.property('pageSize');
        expect(response.body).to.have.property('totalCount');
      });
    });

    it('should filter worship by title', () => {
      cy.apiRequest('GET', '/event/worship?title=Sunday&page=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should filter worship by theme', () => {
      cy.apiRequest('GET', '/event/worship?theme=Love&page=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should filter worship by date range', () => {
      const startTime = new Date().toISOString();
      const endTime = new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString();

      cy.apiRequest('GET', `/event/worship?startTime=${startTime}&endTime=${endTime}&page=1&pageSize=10`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should filter only past worship services', () => {
      cy.apiRequest('GET', '/event/worship?onlyPast=true&page=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should filter by status', () => {
      cy.apiRequest('GET', '/event/worship?status=1&page=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });
  });

  describe('GET /event/worship/{id} - Get Worship by ID', () => {
    it('should get worship service by id', () => {
      if (createdWorshipId) {
        cy.apiRequest('GET', `/event/worship/${createdWorshipId}`, adminToken).then((response) => {
          expect(response.status).to.eq(200);
          expect(response.body).to.have.property('id', createdWorshipId);
          expect(response.body).to.have.property('worshipTheme');
        });
      }
    });

    it('should fail to get non-existent worship', () => {
      cy.apiRequest('GET', '/event/worship/99999', adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('DELETE /event/{id} - Delete Event', () => {
    it('should delete event as admin', () => {
      if (createdEventId) {
        cy.apiRequest('DELETE', `/event/${createdEventId}`, adminToken).then((response) => {
          expect(response.status).to.eq(204);
        });
      }
    });

    it('should fail to delete as member', () => {
      cy.apiRequest('DELETE', '/event/1', memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });

    it('should fail to delete non-existent event', () => {
      cy.apiRequest('DELETE', '/event/99999', adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });
});
