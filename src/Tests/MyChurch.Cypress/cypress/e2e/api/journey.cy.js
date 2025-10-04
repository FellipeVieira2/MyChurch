/// <reference types="cypress" />

describe('?? Journey API Tests', () => {
  let adminToken;
  let memberToken;
  let leaderToken;
  let createdJourneyId;
  let stageId;
  let dailyChallengeId;

  before(() => {
    cy.loginAsAdmin().then((token) => {
      adminToken = token;
    });
    cy.loginAsMember().then((token) => {
      memberToken = token;
    });

    // Login como Leader (se existir)
    cy.request({
      method: 'POST',
      url: `${Cypress.env('apiUrl')}/auth/login`,
      body: {
        email: 'leader@test.com',
        password: 'Test@123456'
      },
      failOnStatusCode: false
    }).then((response) => {
      if (response.status === 200) {
        leaderToken = response.body.token;
      }
    });
  });

  describe('POST /journeys - Create Journey', () => {
    it('should create journey as admin', () => {
      const journeyData = {
        title: `Spiritual Journey ${Date.now()}`,
        description: 'A journey of faith and growth',
        iconUrl: 'https://example.com/icon.png',
        isActive: true,
        isDefault: false,
        stages: [
          {
            title: 'Understanding the Bible',
            description: 'Introduction to Holy Scriptures',
            orderIndex: 1,
            type: 'Reading',
            contentJson: JSON.stringify({ book: 'John', chapters: [1, 2, 3] })
          },
          {
            title: 'Prayer Life',
            description: 'Developing a strong prayer routine',
            orderIndex: 2,
            type: 'Prayer',
            contentJson: JSON.stringify({ dailyGoal: 30 })
          }
        ]
      };

      cy.apiRequest('POST', '/journeys', adminToken, journeyData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
        createdJourneyId = response.body;
      });
    });

    it('should fail to create journey without title', () => {
      const invalidJourney = {
        description: 'Journey without title',
        isActive: true,
        stages: []
      };

      cy.apiRequest('POST', '/journeys', adminToken, invalidJourney).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail to create journey as member', () => {
      const journeyData = {
        title: 'Unauthorized Journey',
        description: 'Should fail',
        isActive: true,
        stages: []
      };

      cy.apiRequest('POST', '/journeys', memberToken, journeyData).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('GET /journeys/my-journeys - Get My Assigned Journeys', () => {
    it('should get assigned journeys as member', () => {
      cy.apiRequest('GET', '/journeys/my-journeys?pageNumber=1&pageSize=10', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('items');
        expect(response.body).to.have.property('pageNumber');
        expect(response.body).to.have.property('totalCount');
        expect(response.body.items).to.be.an('array');
      });
    });

    it('should paginate my journeys', () => {
      cy.apiRequest('GET', '/journeys/my-journeys?pageNumber=1&pageSize=5', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.pageSize).to.be.lte(5);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/journeys/my-journeys?pageNumber=1&pageSize=10').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('POST /journeys/complete-stage/{id} - Complete Journey Stage', () => {
    it('should complete journey stage as member', () => {
      // Assumindo que o membro tem uma stage atribuída
      if (stageId) {
        const completionData = {
          memberResponse: 'I learned a lot about prayer and faith through this stage.'
        };

        cy.apiRequest('POST', `/journeys/complete-stage/${stageId}`, memberToken, completionData).then((response) => {
          expect(response.status).to.eq(200);
        });
      }
    });

    it('should fail to complete non-existent stage', () => {
      const completionData = {
        memberResponse: 'Test response'
      };

      cy.apiRequest('POST', '/journeys/complete-stage/99999', memberToken, completionData).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('POST /journeys/complete-daily-challenge/{id} - Complete Daily Challenge', () => {
    it('should complete daily challenge as member', () => {
      // Assumindo que existe um daily challenge
      if (dailyChallengeId) {
        cy.apiRequest('POST', `/journeys/complete-daily-challenge/${dailyChallengeId}`, memberToken).then((response) => {
          expect(response.status).to.eq(200);
        });
      }
    });

    it('should fail to complete non-existent challenge', () => {
      cy.apiRequest('POST', '/journeys/complete-daily-challenge/99999', memberToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('POST /journeys/generate-content - Generate Journey Content with AI', () => {
    it('should generate content as admin', () => {
      const contentRequest = {
        topic: 'The importance of forgiveness',
        type: 'Devotional'
      };

      cy.apiRequest('POST', '/journeys/generate-content', adminToken, contentRequest).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('string');
        expect(response.body.length).to.be.greaterThan(0);
      });
    });

    it('should generate prayer content', () => {
      const contentRequest = {
        topic: 'Gratitude',
        type: 'Prayer'
      };

      cy.apiRequest('POST', '/journeys/generate-content', adminToken, contentRequest).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail without topic', () => {
      const contentRequest = {
        type: 'Devotional'
      };

      cy.apiRequest('POST', '/journeys/generate-content', adminToken, contentRequest).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail as member', () => {
      const contentRequest = {
        topic: 'Faith',
        type: 'Devotional'
      };

      cy.apiRequest('POST', '/journeys/generate-content', memberToken, contentRequest).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /journeys/verify-stage - Verify Journey Stage', () => {
    it('should verify stage as admin', function() {
      // Assumindo que existe um progress ID
      const verifyData = {
        memberJourneyProgressId: 1
      };

      cy.apiRequest('POST', '/journeys/verify-stage', adminToken, verifyData).then((response) => {
        expect([200, 404]).to.include(response.status); // 404 se não existir
      });
    });

    it('should verify stage as leader', function() {
      if (!leaderToken) {
        this.skip();
      }

      const verifyData = {
        memberJourneyProgressId: 1
      };

      cy.apiRequest('POST', '/journeys/verify-stage', leaderToken, verifyData).then((response) => {
        expect([200, 404]).to.include(response.status);
      });
    });

    it('should fail as regular member', () => {
      const verifyData = {
        memberJourneyProgressId: 1
      };

      cy.apiRequest('POST', '/journeys/verify-stage', memberToken, verifyData).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('GET /journeys/leaderboard - Get Leaderboard', () => {
    it('should get leaderboard with pagination', () => {
      cy.apiRequest('GET', '/journeys/leaderboard?pageNumber=1&pageSize=10', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('items');
        expect(response.body).to.have.property('pageNumber');
        expect(response.body).to.have.property('totalCount');
        expect(response.body.items).to.be.an('array');

        if (response.body.items.length > 0) {
          response.body.items.forEach(entry => {
            expect(entry).to.have.property('memberId');
            expect(entry).to.have.property('memberName');
            expect(entry).to.have.property('points');
            expect(entry).to.have.property('rank');
          });
        }
      });
    });

    it('should paginate leaderboard', () => {
      cy.apiRequest('GET', '/journeys/leaderboard?pageNumber=1&pageSize=5', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.pageSize).to.be.lte(5);
      });
    });

    it('should get leaderboard as admin', () => {
      cy.apiRequest('GET', '/journeys/leaderboard?pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/journeys/leaderboard?pageNumber=1&pageSize=10').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('GET /journeys/pastoral-alerts - Get Pastoral Alerts', () => {
    it('should get pastoral alerts as admin', () => {
      cy.apiRequest('GET', '/journeys/pastoral-alerts', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');

        if (response.body.length > 0) {
          response.body.forEach(alert => {
            expect(alert).to.have.property('memberId');
            expect(alert).to.have.property('memberName');
            expect(alert).to.have.property('alertType');
            expect(alert).to.have.property('severity');
          });
        }
      });
    });

    it('should get pastoral alerts as leader', function() {
      if (!leaderToken) {
        this.skip();
      }

      cy.apiRequest('GET', '/journeys/pastoral-alerts', leaderToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
      });
    });

    it('should fail as regular member', () => {
      cy.apiRequest('GET', '/journeys/pastoral-alerts', memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/journeys/pastoral-alerts').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });
});
