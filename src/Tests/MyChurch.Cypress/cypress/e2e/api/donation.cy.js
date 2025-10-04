/// <reference types="cypress" />

describe('?? Donation API Tests', () => {
  let adminToken;
  let memberToken;
  let createdDonationId;

  before(() => {
    cy.loginAsAdmin().then((token) => {
      adminToken = token;
    });
    cy.loginAsMember().then((token) => {
      memberToken = token;
    });
  });

  describe('POST /donation - Create Donation', () => {
    it('should create tithe donation as member', () => {
      const donationData = {
        amount: 100.00,
        donationType: 'Tithe',
        paymentMethod: 'CreditCard',
        description: 'Tithe of January 2025'
      };

      cy.apiRequest('POST', '/donation', memberToken, donationData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
        createdDonationId = response.body;
      });
    });

    it('should create offering donation', () => {
      const donationData = {
        amount: 50.00,
        donationType: 'Offering',
        paymentMethod: 'Pix',
        description: 'Sunday offering'
      };

      cy.apiRequest('POST', '/donation', memberToken, donationData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
      });
    });

    it('should create general donation', () => {
      const donationData = {
        amount: 200.00,
        donationType: 'General',
        paymentMethod: 'BankTransfer',
        description: 'General donation for church renovation'
      };

      cy.apiRequest('POST', '/donation', memberToken, donationData).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should create campaign donation', () => {
      const donationData = {
        amount: 75.00,
        donationType: 'Campaign',
        paymentMethod: 'CreditCard',
        description: 'Christmas campaign donation',
        campaignId: 1
      };

      cy.apiRequest('POST', '/donation', memberToken, donationData).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail without amount', () => {
      const invalidData = {
        donationType: 'Tithe',
        paymentMethod: 'CreditCard'
      };

      cy.apiRequest('POST', '/donation', memberToken, invalidData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail with negative amount', () => {
      const invalidData = {
        amount: -50.00,
        donationType: 'Tithe',
        paymentMethod: 'CreditCard'
      };

      cy.apiRequest('POST', '/donation', memberToken, invalidData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail without authentication', () => {
      const donationData = {
        amount: 100.00,
        donationType: 'Tithe',
        paymentMethod: 'CreditCard'
      };

      cy.apiRequest('POST', '/donation', null, donationData).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('GET /donation/paid - Get All Paid Donations', () => {
    it('should get all paid donations for member', () => {
      cy.apiRequest('GET', '/donation/paid?Page=1&PageSize=10', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('items');
        expect(response.body).to.have.property('page');
        expect(response.body).to.have.property('pageSize');
        expect(response.body).to.have.property('totalCount');
        expect(response.body.items).to.be.an('array');
      });
    });

    it('should filter donations by description', () => {
      cy.apiRequest('GET', '/donation/paid?Description=Tithe&Page=1&PageSize=10', memberToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should filter donations by value', () => {
      cy.apiRequest('GET', '/donation/paid?Value=100&Page=1&PageSize=10', memberToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should filter donations by date', () => {
      const date = new Date().toISOString().split('T')[0];
      cy.apiRequest('GET', `/donation/paid?Date=${date}&Page=1&PageSize=10`, memberToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should paginate results', () => {
      cy.apiRequest('GET', '/donation/paid?Page=1&PageSize=5', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.pageSize).to.be.lte(5);
      });
    });

    it('should get donations as admin', () => {
      cy.apiRequest('GET', '/donation/paid?Page=1&PageSize=10', adminToken).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/donation/paid?Page=1&PageSize=10').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('GET /donation/transfer-balance - Get Transfer Balance', () => {
    it('should get transfer balance as admin', () => {
      cy.apiRequest('GET', '/donation/transfer-balance', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('availableBalance');
        expect(response.body).to.have.property('totalDonations');
        expect(response.body).to.have.property('platformFee');
        expect(response.body.availableBalance).to.be.a('number');
      });
    });

    it('should fail as member', () => {
      cy.apiRequest('GET', '/donation/transfer-balance', memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/donation/transfer-balance').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('POST /donation/transfer - Transfer Church Balance', () => {
    it('should transfer available balance as admin', () => {
      const transferData = {
        amount: 50.00,
        description: 'Transfer to church bank account'
      };

      cy.apiRequest('POST', '/donation/transfer', adminToken, transferData).then((response) => {
        expect([200, 400]).to.include(response.status); // 400 if insufficient balance
        
        if (response.status === 200) {
          expect(response.body).to.have.property('success');
          expect(response.body).to.have.property('transactionId');
          expect(response.body.success).to.be.true;
        }
      });
    });

    it('should fail to transfer without amount', () => {
      const invalidData = {
        description: 'No amount specified'
      };

      cy.apiRequest('POST', '/donation/transfer', adminToken, invalidData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail to transfer negative amount', () => {
      const invalidData = {
        amount: -100.00,
        description: 'Negative transfer'
      };

      cy.apiRequest('POST', '/donation/transfer', adminToken, invalidData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail to transfer more than available', () => {
      const invalidData = {
        amount: 99999999.99,
        description: 'Excessive transfer'
      };

      cy.apiRequest('POST', '/donation/transfer', adminToken, invalidData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail as member', () => {
      const transferData = {
        amount: 50.00,
        description: 'Unauthorized transfer'
      };

      cy.apiRequest('POST', '/donation/transfer', memberToken, transferData).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });

    it('should fail without authentication', () => {
      const transferData = {
        amount: 50.00,
        description: 'Unauthenticated transfer'
      };

      cy.apiRequest('POST', '/donation/transfer', null, transferData).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });
});
