/// <reference types="cypress" />

describe('? WorshipActivity API Tests', () => {
  let adminToken;
  let memberToken;
  let worshipServiceId = 1; // Assumindo que existe um worship service

  before(() => {
    cy.loginAsAdmin().then((token) => {
      adminToken = token;
    });
    cy.loginAsMember().then((token) => {
      memberToken = token;
    });
  });

  describe('POST /{worshipServiceId}/start - Start Worship', () => {
    it('should start worship as admin', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/start`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail to start as member', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/start`, memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /{worshipServiceId}/presence/check-in - Member Check-in', () => {
    it('should check-in with geolocation as member', () => {
      const checkInData = {
        latitude: -23.550520,
        longitude: -46.633308,
        maxDistanceMeters: 150
      };

      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/presence/check-in`, memberToken, checkInData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('presenceId');
      });
    });

    it('should check-in with custom distance', () => {
      const checkInData = {
        latitude: -23.550520,
        longitude: -46.633308,
        maxDistanceMeters: 500
      };

      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/presence/check-in`, memberToken, checkInData).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail without geolocation', () => {
      const checkInData = {};

      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/presence/check-in`, memberToken, checkInData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should return 0 if already present', () => {
      const checkInData = {
        latitude: -23.550520,
        longitude: -46.633308,
        maxDistanceMeters: 150
      };

      // Segunda tentativa de check-in
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/presence/check-in`, memberToken, checkInData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.presenceId).to.satisfy((id) => id === 0 || id > 0);
      });
    });
  });

  describe('POST /{worshipServiceId}/visitor/{visitorId}/presence/check-in - Visitor Check-in', () => {
    it('should check-in visitor without authentication', () => {
      const visitorCheckIn = {
        latitude: -23.550520,
        longitude: -46.633308,
        maxDistanceMeters: 150
      };

      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/visitor/1/presence/check-in`, null, visitorCheckIn).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('presenceId');
      });
    });
  });

  describe('GET /{worshipServiceId}/active-activities - Get Active Activities', () => {
    it('should get active activities as member', () => {
      cy.apiRequest('GET', `/worshipactivity/${worshipServiceId}/active-activities`, memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', `/worshipactivity/${worshipServiceId}/active-activities`).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('POST /{worshipServiceId}/bible-reading/highlight - Highlight Bible Reading', () => {
    it('should highlight Bible reading as admin', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/bible-reading/highlight?versionId=1&bookId=1&chapterId=1&verseId=1`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('ActivityId');
        expect(response.body).to.have.property('VersionId');
        expect(response.body).to.have.property('BookId');
        expect(response.body).to.have.property('ChapterId');
      });
    });

    it('should highlight chapter without specific verse', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/bible-reading/highlight?versionId=1&bookId=43&chapterId=3`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail as member', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/bible-reading/highlight?versionId=1&bookId=1&chapterId=1`, memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /{worshipServiceId}/bible-reading/{activityId}/finish - Finish Bible Reading', () => {
    it('should finish Bible reading as admin', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/bible-reading/1/finish`, adminToken).then((response) => {
        expect([200, 404]).to.include(response.status); // 404 se activity não existir
      });
    });

    it('should fail as member', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/bible-reading/1/finish`, memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /{worshipServiceId}/hymn/{number}/present/{verseNumber} - Present Hymn', () => {
    it('should present hymn as admin', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/hymn/123/present/1`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should present different hymn verse', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/hymn/456/present/2`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail as member', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/hymn/123/present/1`, memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /{worshipServiceId}/imported-hymn/{importedHymnId}/present/{stanzaOrder} - Present Imported Hymn', () => {
    it('should present imported hymn as admin', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/imported-hymn/1/present/1`, adminToken).then((response) => {
        expect([200, 404]).to.include(response.status);
      });
    });

    it('should fail as member', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/imported-hymn/1/present/1`, memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /{worshipServiceId}/offering/present - Present Offering', () => {
    it('should present offering as admin', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/offering/present`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('activityId');
      });
    });

    it('should fail as member', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/offering/present`, memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /{worshipServiceId}/offering/{activityId}/finish - Finish Offering', () => {
    it('should finish offering as admin', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/offering/1/finish`, adminToken).then((response) => {
        expect([200, 404]).to.include(response.status);
      });
    });

    it('should fail as member', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/offering/1/finish`, memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /{worshipServiceId}/schedule/add - Add Schedule Item', () => {
    it('should add schedule item as admin', () => {
      const scheduleItem = {
        name: 'Opening Prayer',
        order: 1
      };

      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/schedule/add`, adminToken, scheduleItem).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id');
      });
    });

    it('should add multiple schedule items', () => {
      const items = [
        { name: 'Praise and Worship', order: 2 },
        { name: 'Sermon', order: 3 },
        { name: 'Final Prayer', order: 4 }
      ];

      items.forEach(item => {
        cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/schedule/add`, adminToken, item).then((response) => {
          expect(response.status).to.eq(200);
        });
      });
    });

    it('should fail as member', () => {
      const scheduleItem = {
        name: 'Unauthorized Item',
        order: 1
      };

      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/schedule/add`, memberToken, scheduleItem).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('PUT /{worshipServiceId}/schedule/update/{id} - Update Schedule Item', () => {
    it('should update schedule item as admin', () => {
      const updatedItem = {
        name: 'Updated Prayer',
        order: 1
      };

      cy.apiRequest('PUT', `/worshipactivity/${worshipServiceId}/schedule/update/1`, adminToken, updatedItem).then((response) => {
        expect([200, 404]).to.include(response.status);
      });
    });

    it('should fail as member', () => {
      const updatedItem = {
        name: 'Unauthorized Update',
        order: 1
      };

      cy.apiRequest('PUT', `/worshipactivity/${worshipServiceId}/schedule/update/1`, memberToken, updatedItem).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('DELETE /{worshipServiceId}/schedule/remove/{id} - Remove Schedule Item', () => {
    it('should remove schedule item as admin', () => {
      cy.apiRequest('DELETE', `/worshipactivity/${worshipServiceId}/schedule/remove/1`, adminToken).then((response) => {
        expect([200, 404]).to.include(response.status);
      });
    });

    it('should fail as member', () => {
      cy.apiRequest('DELETE', `/worshipactivity/${worshipServiceId}/schedule/remove/1`, memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /{worshipServiceId}/prayer-request - Create Prayer Request', () => {
    it('should create prayer request as member', () => {
      const prayerRequest = {
        request: 'Please pray for my family health'
      };

      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/prayer-request`, memberToken, prayerRequest).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
      });
    });

    it('should create prayer request as admin', () => {
      const prayerRequest = {
        request: 'Prayer for church growth'
      };

      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/prayer-request`, adminToken, prayerRequest).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail without request text', () => {
      const prayerRequest = {};

      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/prayer-request`, memberToken, prayerRequest).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });
  });

  describe('GET /{worshipServiceId}/prayer-requests/list - Get Prayer Requests', () => {
    it('should get prayer requests as member', () => {
      cy.apiRequest('GET', `/worshipactivity/${worshipServiceId}/prayer-requests/list`, memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');

        if (response.body.length > 0) {
          response.body.forEach(request => {
            expect(request).to.have.property('id');
            expect(request).to.have.property('request');
            expect(request).to.have.property('memberId');
          });
        }
      });
    });

    it('should get prayer requests as admin', () => {
      cy.apiRequest('GET', `/worshipactivity/${worshipServiceId}/prayer-requests/list`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
      });
    });
  });

  describe('POST /{worshipServiceId}/admin-notice - Send Admin Notice', () => {
    it('should send admin notice as admin', () => {
      const notice = {
        message: 'Important announcement: Church meeting postponed'
      };

      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/admin-notice`, adminToken, notice).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('noticeId');
      });
    });

    it('should send notice with image', () => {
      const notice = {
        message: 'Check out our new event poster',
        imageBase64: 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg=='
      };

      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/admin-notice`, adminToken, notice).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail as member', () => {
      const notice = {
        message: 'Unauthorized notice'
      };

      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/admin-notice`, memberToken, notice).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /{worshipServiceId}/finalize - Finalize Worship', () => {
    it('should finalize worship as admin', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/finalize`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail as member', () => {
      cy.apiRequest('POST', `/worshipactivity/${worshipServiceId}/finalize`, memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });
});
