/// <reference types="cypress" />

describe('?? Subscription API Tests', () => {
  let platformAdminToken;
  let adminToken;
  let memberToken;
  let createdSubscriptionId;

  before(() => {
    // Platform Admin
    cy.request({
      method: 'POST',
      url: `${Cypress.env('apiUrl')}/auth/login`,
      body: {
        email: 'platformadmin@test.com',
        password: 'Test@123456'
      },
      failOnStatusCode: false
    }).then((response) => {
      if (response.status === 200) {
        platformAdminToken = response.body.token;
      }
    });

    cy.loginAsAdmin().then((token) => {
      adminToken = token;
    });
    
    cy.loginAsMember().then((token) => {
      memberToken = token;
    });
  });

  describe('POST /subscription - Create Subscription', () => {
    it('should create subscription for church', () => {
      const subscriptionData = {
        planId: 1, // Basic plan
        paymentMethod: 'CreditCard'
      };

      cy.apiRequest('POST', '/subscription', adminToken, subscriptionData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
        createdSubscriptionId = response.body;
      });
    });

    it('should fail without plan id', () => {
      const invalidData = {
        paymentMethod: 'CreditCard'
      };

      cy.apiRequest('POST', '/subscription', adminToken, invalidData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail as member', () => {
      const subscriptionData = {
        planId: 1,
        paymentMethod: 'CreditCard'
      };

      cy.apiRequest('POST', '/subscription', memberToken, subscriptionData).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('GET /subscription/{id} - Get Subscription', () => {
    it('should get subscription as admin', () => {
      if (createdSubscriptionId) {
        cy.apiRequest('GET', `/subscription/${createdSubscriptionId}`, adminToken).then((response) => {
          expect(response.status).to.eq(200);
        });
      }
    });

    it('should fail to get non-existent subscription', () => {
      cy.apiRequest('GET', '/subscription/99999', adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/subscription/1').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('POST /subscription/change-plan - Change Church Plan', () => {
    it('should upgrade plan as admin', () => {
      const changePlanData = {
        newPlanId: 2 // Premium plan
      };

      cy.apiRequest('POST', '/subscription/change-plan', adminToken, changePlanData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('success');
        expect(response.body).to.have.property('message');
        expect(response.body).to.have.property('newPlanName');
      });
    });

    it('should downgrade plan as admin', () => {
      const changePlanData = {
        newPlanId: 1 // Basic plan
      };

      cy.apiRequest('POST', '/subscription/change-plan', adminToken, changePlanData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.success).to.be.true;
      });
    });

    it('should fail to change to same plan', () => {
      const changePlanData = {
        newPlanId: 1 // Already on basic
      };

      cy.apiRequest('POST', '/subscription/change-plan', adminToken, changePlanData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail to change to non-existent plan', () => {
      const changePlanData = {
        newPlanId: 99999
      };

      cy.apiRequest('POST', '/subscription/change-plan', adminToken, changePlanData).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });

    it('should fail as member', () => {
      const changePlanData = {
        newPlanId: 2
      };

      cy.apiRequest('POST', '/subscription/change-plan', memberToken, changePlanData).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('PUT /subscription/{id} - Update Subscription', () => {
    it('should update subscription as platform admin', function() {
      if (!platformAdminToken || !createdSubscriptionId) {
        this.skip();
      }

      const updatedData = {
        status: 'Active'
      };

      cy.apiRequest('PUT', `/subscription/${createdSubscriptionId}`, platformAdminToken, updatedData).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail as regular admin', () => {
      cy.apiRequest('PUT', '/subscription/1', adminToken, {}).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('DELETE /subscription/{id} - Cancel Subscription', () => {
    it('should cancel subscription as platform admin', function() {
      if (!platformAdminToken) {
        this.skip();
      }

      cy.apiRequest('DELETE', '/subscription/1', platformAdminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail as regular admin', () => {
      cy.apiRequest('DELETE', '/subscription/1', adminToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });

    it('should fail to cancel non-existent subscription', function() {
      if (!platformAdminToken) {
        this.skip();
      }

      cy.apiRequest('DELETE', '/subscription/99999', platformAdminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });
});
