/// <reference types="cypress" />

describe('?? Auth API Tests', () => {
  const apiUrl = Cypress.env('apiUrl');

  describe('POST /auth/login', () => {
    it('should login successfully with valid admin credentials', () => {
      cy.request({
        method: 'POST',
        url: `${apiUrl}/auth/login`,
        body: {
          email: Cypress.env('adminEmail'),
          password: Cypress.env('adminPassword')
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('token');
        expect(response.body.token).to.be.validToken();
        expect(response.body).to.have.property('refreshToken');
        expect(response.body).to.have.property('expiration');
      });
    });

    it('should login successfully with valid member credentials', () => {
      cy.request({
        method: 'POST',
        url: `${apiUrl}/auth/login`,
        body: {
          email: Cypress.env('memberEmail'),
          password: Cypress.env('memberPassword')
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('token');
        expect(response.body.token).to.be.validToken();
      });
    });

    it('should fail with invalid email', () => {
      cy.request({
        method: 'POST',
        url: `${apiUrl}/auth/login`,
        body: {
          email: 'invalid@email.com',
          password: 'wrongpassword'
        },
        failOnStatusCode: false
      }).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });

    it('should fail with invalid password', () => {
      cy.request({
        method: 'POST',
        url: `${apiUrl}/auth/login`,
        body: {
          email: Cypress.env('adminEmail'),
          password: 'wrongpassword'
        },
        failOnStatusCode: false
      }).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });

    it('should fail with missing email', () => {
      cy.request({
        method: 'POST',
        url: `${apiUrl}/auth/login`,
        body: {
          password: 'somepassword'
        },
        failOnStatusCode: false
      }).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail with missing password', () => {
      cy.request({
        method: 'POST',
        url: `${apiUrl}/auth/login`,
        body: {
          email: Cypress.env('adminEmail')
        },
        failOnStatusCode: false
      }).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });
  });

  describe('POST /auth/refresh-token', () => {
    let refreshToken;

    before(() => {
      cy.request({
        method: 'POST',
        url: `${apiUrl}/auth/login`,
        body: {
          email: Cypress.env('adminEmail'),
          password: Cypress.env('adminPassword')
        }
      }).then((response) => {
        refreshToken = response.body.refreshToken;
      });
    });

    it('should refresh token successfully', () => {
      cy.request({
        method: 'POST',
        url: `${apiUrl}/auth/refresh-token`,
        body: {
          refreshToken: refreshToken
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('token');
        expect(response.body.token).to.be.validToken();
      });
    });

    it('should fail with invalid refresh token', () => {
      cy.request({
        method: 'POST',
        url: `${apiUrl}/auth/refresh-token`,
        body: {
          refreshToken: 'invalid-token'
        },
        failOnStatusCode: false
      }).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });
});
