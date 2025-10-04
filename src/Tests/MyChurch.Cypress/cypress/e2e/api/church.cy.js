/// <reference types="cypress" />

describe('? Church API Tests', () => {
  let platformAdminToken;
  let adminToken;
  let createdChurchId;

  before(() => {
    // Assumindo que existe um PlatformAdmin para criar igrejas
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
  });

  describe('POST /church - Create Church', () => {
    it('should create church with admin member as platform admin', function() {
      if (!platformAdminToken) {
        this.skip();
      }

      const churchData = {
        name: `Test Church ${Date.now()}`,
        cnpj: `${Math.floor(Math.random() * 100000000000000)}`,
        address: 'Test Address 123',
        city: 'Test City',
        state: 'SP',
        postalCode: '12345-678',
        phoneNumber: '(11) 98765-4321',
        email: `church${Date.now()}@test.com`,
        description: 'Test church for API testing',
        adminMember: {
          firstName: 'Admin',
          lastName: 'Test',
          email: `admin${Date.now()}@test.com`,
          phoneNumber: '(11) 91234-5678',
          birthDate: '1990-01-01',
          address: 'Admin Address 456',
          city: 'Test City',
          state: 'SP',
          postalCode: '12345-678'
        },
        planId: 1
      };

      cy.apiRequest('POST', '/church', platformAdminToken, churchData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('churchId');
        expect(response.body).to.have.property('adminMemberId');
        createdChurchId = response.body.churchId;
      });
    });

    it('should fail to create church without name', function() {
      if (!platformAdminToken) {
        this.skip();
      }

      const churchData = {
        cnpj: '12345678901234',
        address: 'Test Address',
        city: 'Test City',
        state: 'SP',
        postalCode: '12345-678'
      };

      cy.apiRequest('POST', '/church', platformAdminToken, churchData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });
  });

  describe('GET /church/{id} - Get Church by ID', () => {
    it('should get church by id as admin', () => {
      cy.apiRequest('GET', `/church/${createdChurchId || 1}`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id');
        expect(response.body).to.have.property('name');
        expect(response.body).to.have.property('cnpj');
        expect(response.body).to.have.property('address');
        expect(response.body).to.have.property('city');
        expect(response.body).to.have.property('state');
      });
    });

    it('should fail to get non-existent church', () => {
      cy.apiRequest('GET', '/church/99999', adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/church/1').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('PUT /church/{id} - Update Church', () => {
    it('should update church as admin', () => {
      const updatedData = {
        name: `Updated Church ${Date.now()}`,
        description: 'Updated description for testing'
      };

      cy.apiRequest('PUT', `/church/${createdChurchId || 1}`, adminToken, updatedData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('name', updatedData.name);
      });
    });

    it('should update church address', () => {
      const updatedData = {
        address: 'New Test Address 789',
        city: 'New City',
        state: 'RJ',
        postalCode: '98765-432'
      };

      cy.apiRequest('PUT', `/church/${createdChurchId || 1}`, adminToken, updatedData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('address', updatedData.address);
        expect(response.body).to.have.property('city', updatedData.city);
      });
    });

    it('should fail to update non-existent church', () => {
      const updatedData = {
        name: 'Non-existent Church'
      };

      cy.apiRequest('PUT', '/church/99999', adminToken, updatedData).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('GET /church/my-church - Get My Church', () => {
    it('should get current user church as admin', () => {
      cy.apiRequest('GET', '/church/my-church', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id');
        expect(response.body).to.have.property('name');
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/church/my-church').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('POST /church/{id}/logo - Upload Logo', () => {
    it('should upload church logo as admin', () => {
      const base64Image = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==';

      const logoData = {
        base64Image: base64Image,
        fileName: 'logo.png'
      };

      cy.apiRequest('POST', `/church/${createdChurchId || 1}/logo`, adminToken, logoData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('logoUrl');
      });
    });

    it('should fail to upload without image', () => {
      const logoData = {
        fileName: 'logo.png'
      };

      cy.apiRequest('POST', `/church/${createdChurchId || 1}/logo`, adminToken, logoData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });
  });

  describe('GET /church/statistics - Get Church Statistics', () => {
    it('should get church statistics as admin', () => {
      cy.apiRequest('GET', '/church/statistics', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('totalMembers');
        expect(response.body).to.have.property('activeMembers');
        expect(response.body).to.have.property('totalEvents');
        expect(response.body).to.have.property('totalDonations');
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/church/statistics').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('GET /church/search - Search Churches', () => {
    it('should search churches by name', function() {
      if (!platformAdminToken) {
        this.skip();
      }

      cy.apiRequest('GET', '/church/search?name=Test&pageNumber=1&pageSize=10', platformAdminToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
      });
    });

    it('should search churches by city', function() {
      if (!platformAdminToken) {
        this.skip();
      }

      cy.apiRequest('GET', '/church/search?city=Test&pageNumber=1&pageSize=10', platformAdminToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
      });
    });

    it('should search churches by state', function() {
      if (!platformAdminToken) {
        this.skip();
      }

      cy.apiRequest('GET', '/church/search?state=SP&pageNumber=1&pageSize=10', platformAdminToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
      });
    });
  });

  describe('DELETE /church/{id} - Delete Church', () => {
    it('should delete church as platform admin', function() {
      if (!platformAdminToken || !createdChurchId) {
        this.skip();
      }

      cy.apiRequest('DELETE', `/church/${createdChurchId}`, platformAdminToken).then((response) => {
        expect(response.status).to.eq(204);
      });
    });

    it('should fail to delete as regular admin', () => {
      cy.apiRequest('DELETE', `/church/1`, adminToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });
});
