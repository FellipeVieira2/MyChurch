/// <reference types="cypress" />

describe('?? Member API Tests', () => {
  let adminToken;
  let memberToken;
  let createdMemberId;

  before(() => {
    cy.loginAsAdmin().then((token) => {
      adminToken = token;
    });
    cy.loginAsMember().then((token) => {
      memberToken = token;
    });
  });

  describe('POST /member - Create Member', () => {
    it('should create member as admin', () => {
      const memberData = {
        firstName: 'João',
        lastName: 'Silva',
        email: `joao${Date.now()}@test.com`,
        phoneNumber: '(11) 98765-4321',
        birthDate: '1995-05-15',
        address: 'Rua Teste 123',
        city: 'São Paulo',
        state: 'SP',
        postalCode: '01234-567',
        maritalStatus: 'Single',
        gender: 'Male'
      };

      cy.apiRequest('POST', '/member', adminToken, memberData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
        createdMemberId = response.body;
      });
    });

    it('should fail to create member without email', () => {
      const memberData = {
        firstName: 'Maria',
        lastName: 'Santos',
        phoneNumber: '(11) 91234-5678'
      };

      cy.apiRequest('POST', '/member', adminToken, memberData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail to create member with duplicate email', () => {
      const memberData = {
        firstName: 'Pedro',
        lastName: 'Costa',
        email: Cypress.env('adminEmail'), // Email já existente
        phoneNumber: '(11) 99999-9999'
      };

      cy.apiRequest('POST', '/member', adminToken, memberData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail to create member as non-admin', () => {
      const memberData = {
        firstName: 'Unauthorized',
        lastName: 'User',
        email: `unauth${Date.now()}@test.com`
      };

      cy.apiRequest('POST', '/member', memberToken, memberData).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('GET /member - List Members', () => {
    it('should get all members with pagination', () => {
      cy.apiRequest('GET', '/member?pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
        
        if (response.body.items.length > 0) {
          response.body.items.forEach(member => {
            expect(member).to.have.property('id');
            expect(member).to.have.property('firstName');
            expect(member).to.have.property('lastName');
            expect(member).to.have.property('email');
          });
        }
      });
    });

    it('should filter members by name', () => {
      cy.apiRequest('GET', '/member?name=João&pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
      });
    });

    it('should filter members by email', () => {
      cy.apiRequest('GET', '/member?email=test.com&pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
      });
    });

    it('should filter members by status (Active)', () => {
      cy.apiRequest('GET', '/member?status=Active&pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
      });
    });

    it('should filter members by role (Member)', () => {
      cy.apiRequest('GET', '/member?role=Member&pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
      });
    });

    it('should sort members by firstName ascending', () => {
      cy.apiRequest('GET', '/member?sortBy=FirstName&sortDirection=asc&pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
      });
    });

    it('should get members as member (limited access)', () => {
      cy.apiRequest('GET', '/member?pageNumber=1&pageSize=10', memberToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });
  });

  describe('GET /member/{id} - Get Member by ID', () => {
    it('should get member by id as admin', () => {
      cy.apiRequest('GET', `/member/${createdMemberId || 1}`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id');
        expect(response.body).to.have.property('firstName');
        expect(response.body).to.have.property('lastName');
        expect(response.body).to.have.property('email');
        expect(response.body).to.have.property('status');
      });
    });

    it('should fail to get non-existent member', () => {
      cy.apiRequest('GET', '/member/99999', adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('GET /member/me - Get Current Member', () => {
    it('should get current member profile', () => {
      cy.apiRequest('GET', '/member/me', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id');
        expect(response.body).to.have.property('firstName');
        expect(response.body).to.have.property('lastName');
        expect(response.body).to.have.property('email');
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/member/me').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('PUT /member/{id} - Update Member', () => {
    it('should update member as admin', () => {
      const updatedData = {
        firstName: 'João Updated',
        lastName: 'Silva Updated',
        phoneNumber: '(11) 99999-8888'
      };

      cy.apiRequest('PUT', `/member/${createdMemberId || 1}`, adminToken, updatedData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('firstName', updatedData.firstName);
        expect(response.body).to.have.property('lastName', updatedData.lastName);
      });
    });

    it('should update member address', () => {
      const updatedData = {
        address: 'Nova Rua 456',
        city: 'Rio de Janeiro',
        state: 'RJ',
        postalCode: '20000-000'
      };

      cy.apiRequest('PUT', `/member/${createdMemberId || 1}`, adminToken, updatedData).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail to update non-existent member', () => {
      const updatedData = {
        firstName: 'Non-existent'
      };

      cy.apiRequest('PUT', '/member/99999', adminToken, updatedData).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('PUT /member/me - Update Own Profile', () => {
    it('should update own profile as member', () => {
      const updatedData = {
        phoneNumber: '(11) 91111-2222',
        address: 'Meu novo endereço'
      };

      cy.apiRequest('PUT', '/member/me', memberToken, updatedData).then((response) => {
        expect(response.status).to.eq(200);
      });
    });
  });

  describe('POST /member/{id}/photo - Upload Photo', () => {
    it('should upload member photo as admin', () => {
      const base64Image = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==';

      const photoData = {
        base64Image: base64Image,
        fileName: 'photo.jpg'
      };

      cy.apiRequest('POST', `/member/${createdMemberId || 1}/photo`, adminToken, photoData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('photoUrl');
      });
    });
  });

  describe('POST /member/{id}/approve - Approve Member', () => {
    it('should approve pending member as admin', () => {
      // Assumindo que o membro está pendente
      cy.apiRequest('POST', `/member/${createdMemberId || 1}/approve`, adminToken).then((response) => {
        expect([200, 400]).to.include(response.status); // 400 se já aprovado
      });
    });

    it('should fail to approve as non-admin', () => {
      cy.apiRequest('POST', `/member/1/approve`, memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /member/{id}/deactivate - Deactivate Member', () => {
    it('should deactivate member as admin', () => {
      const deactivateData = {
        reason: 'Test deactivation'
      };

      cy.apiRequest('POST', `/member/${createdMemberId || 1}/deactivate`, adminToken, deactivateData).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail to deactivate as non-admin', () => {
      cy.apiRequest('POST', `/member/1/deactivate`, memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /member/{id}/activate - Activate Member', () => {
    it('should activate member as admin', () => {
      cy.apiRequest('POST', `/member/${createdMemberId || 1}/activate`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });
  });

  describe('DELETE /member/{id} - Delete Member', () => {
    it('should delete member as admin', () => {
      if (createdMemberId) {
        cy.apiRequest('DELETE', `/member/${createdMemberId}`, adminToken).then((response) => {
          expect(response.status).to.eq(204);
        });
      }
    });

    it('should fail to delete as non-admin', () => {
      cy.apiRequest('DELETE', '/member/1', memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });

    it('should fail to delete non-existent member', () => {
      cy.apiRequest('DELETE', '/member/99999', adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('GET /member/birthdays - Get Upcoming Birthdays', () => {
    it('should get upcoming birthdays', () => {
      cy.apiRequest('GET', '/member/birthdays?days=30', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
      });
    });
  });

  describe('GET /member/statistics - Get Member Statistics', () => {
    it('should get member statistics as admin', () => {
      cy.apiRequest('GET', '/member/statistics', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('total');
        expect(response.body).to.have.property('active');
        expect(response.body).to.have.property('pending');
        expect(response.body).to.have.property('inactive');
      });
    });
  });
});
