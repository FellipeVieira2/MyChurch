/// <reference types="cypress" />

describe('?? CashFlow API Tests', () => {
  let adminToken;
  let memberToken;
  let categoryId;
  let entryId;

  before(() => {
    cy.loginAsAdmin().then((token) => {
      adminToken = token;
    });
    cy.loginAsMember().then((token) => {
      memberToken = token;
    });
  });

  describe('POST /cashflow/categories - Create Category', () => {
    it('should create cash flow category as admin', () => {
      const category = {
        name: `Test Category ${Date.now()}`,
        description: 'Category for testing purposes'
      };

      cy.apiRequest('POST', '/cashflow/categories', adminToken, category).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
        categoryId = response.body;
      });
    });

    it('should fail to create category without name', () => {
      const category = {
        description: 'Category without name'
      };

      cy.apiRequest('POST', '/cashflow/categories', adminToken, category).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail to create category as member', () => {
      const category = {
        name: 'Unauthorized Category',
        description: 'Should fail'
      };

      cy.apiRequest('POST', '/cashflow/categories', memberToken, category).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('GET /cashflow/categories - List Categories', () => {
    it('should get all categories with pagination', () => {
      cy.apiRequest('GET', '/cashflow/categories?pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
      });
    });

    it('should filter categories by name', () => {
      cy.apiRequest('GET', '/cashflow/categories?name=Test&pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
      });
    });

    it('should sort categories by name ascending', () => {
      cy.apiRequest('GET', '/cashflow/categories?sortBy=Name&sortDirection=asc&pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
      });
    });

    it('should get categories as member', () => {
      cy.apiRequest('GET', '/cashflow/categories?pageNumber=1&pageSize=10', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        cy.validatePaginatedResponse(response);
      });
    });
  });

  describe('PUT /cashflow/categories/{id} - Update Category', () => {
    it('should update category as admin', () => {
      const updatedCategory = {
        name: `Updated Category ${Date.now()}`,
        description: 'Updated description'
      };

      cy.apiRequest('PUT', `/cashflow/categories/${categoryId}`, adminToken, updatedCategory).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id', categoryId);
        expect(response.body).to.have.property('name', updatedCategory.name);
      });
    });

    it('should fail to update non-existent category', () => {
      const updatedCategory = {
        name: 'Non-existent Category'
      };

      cy.apiRequest('PUT', '/cashflow/categories/99999', adminToken, updatedCategory).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('POST /cashflow - Create Entry', () => {
    it('should create income entry as admin', () => {
      const entry = {
        amount: 500.00,
        date: new Date().toISOString(),
        description: 'Test Income Entry',
        type: 0, // Income
        categoryId: categoryId
      };

      cy.apiRequest('POST', '/cashflow', adminToken, entry).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
        entryId = response.body;
      });
    });

    it('should create expense entry as admin', () => {
      const entry = {
        amount: 200.00,
        date: new Date().toISOString(),
        description: 'Test Expense Entry',
        type: 1, // Expense
        categoryId: categoryId
      };

      cy.apiRequest('POST', '/cashflow', adminToken, entry).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
      });
    });

    it('should fail to create entry without amount', () => {
      const entry = {
        date: new Date().toISOString(),
        description: 'Entry without amount',
        type: 0,
        categoryId: categoryId
      };

      cy.apiRequest('POST', '/cashflow', adminToken, entry).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail to create entry with invalid category', () => {
      const entry = {
        amount: 100.00,
        date: new Date().toISOString(),
        description: 'Entry with invalid category',
        type: 0,
        categoryId: 99999
      };

      cy.apiRequest('POST', '/cashflow', adminToken, entry).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('GET /cashflow - List Entries', () => {
    it('should get all entries with pagination', () => {
      cy.apiRequest('GET', '/cashflow?pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('items');
        expect(response.body).to.have.property('totalIncome');
        expect(response.body).to.have.property('totalExpense');
        expect(response.body).to.have.property('balance');
      });
    });

    it('should filter entries by type (Income)', () => {
      cy.apiRequest('GET', '/cashflow?type=0&pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        response.body.items.forEach(entry => {
          expect(entry.type).to.eq(0);
        });
      });
    });

    it('should filter entries by type (Expense)', () => {
      cy.apiRequest('GET', '/cashflow?type=1&pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        response.body.items.forEach(entry => {
          expect(entry.type).to.eq(1);
        });
      });
    });

    it('should filter entries by date range', () => {
      const startDate = new Date('2024-01-01').toISOString();
      const endDate = new Date().toISOString();

      cy.apiRequest('GET', `/cashflow?startDate=${startDate}&endDate=${endDate}&pageNumber=1&pageSize=10`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should filter entries by category', () => {
      cy.apiRequest('GET', `/cashflow?categoryId=${categoryId}&pageNumber=1&pageSize=10`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        response.body.items.forEach(entry => {
          expect(entry.categoryId).to.eq(categoryId);
        });
      });
    });

    it('should sort entries by date descending', () => {
      cy.apiRequest('GET', '/cashflow?sortBy=Date&sortDirection=desc&pageNumber=1&pageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });
  });

  describe('GET /cashflow/{id} - Get Entry by ID', () => {
    it('should get entry by id', () => {
      cy.apiRequest('GET', `/cashflow/${entryId}`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id', entryId);
        expect(response.body).to.have.property('amount');
        expect(response.body).to.have.property('date');
        expect(response.body).to.have.property('type');
      });
    });

    it('should fail to get non-existent entry', () => {
      cy.apiRequest('GET', '/cashflow/99999', adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('PUT /cashflow/{id} - Update Entry', () => {
    it('should update entry as admin', () => {
      const updatedEntry = {
        amount: 600.00,
        description: 'Updated Entry'
      };

      cy.apiRequest('PUT', `/cashflow/${entryId}`, adminToken, updatedEntry).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('amount', 600.00);
      });
    });
  });

  describe('GET /cashflow/balance - Get Balance', () => {
    it('should get church cash flow balance', () => {
      cy.apiRequest('GET', '/cashflow/balance', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
      });
    });

    it('should get balance as member', () => {
      cy.apiRequest('GET', '/cashflow/balance', memberToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });
  });

  describe('DELETE /cashflow/{id} - Delete Entry', () => {
    it('should delete entry as admin', () => {
      cy.apiRequest('DELETE', `/cashflow/${entryId}`, adminToken).then((response) => {
        expect(response.status).to.eq(204);
      });
    });

    it('should fail to delete non-existent entry', () => {
      cy.apiRequest('DELETE', '/cashflow/99999', adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('DELETE /cashflow/categories/{id} - Delete Category', () => {
    it('should delete category as admin', () => {
      cy.apiRequest('DELETE', `/cashflow/categories/${categoryId}`, adminToken).then((response) => {
        expect(response.status).to.eq(204);
      });
    });

    it('should fail to delete non-existent category', () => {
      cy.apiRequest('DELETE', '/cashflow/categories/99999', adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });
});
