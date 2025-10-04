/// <reference types="cypress" />

describe('? Church Reviews & Rating API Tests', () => {
  let memberToken;
  let adminToken;
  let testChurchId = 1; // Assumindo que existe uma igreja com ID 1

  before(() => {
    cy.loginAsAdmin().then((token) => {
      adminToken = token;
    });
    cy.loginAsMember().then((token) => {
      memberToken = token;
    });
  });

  describe('GET /church/public/search - Search Churches with Ratings', () => {
    it('should search churches publicly without authentication', () => {
      cy.apiRequest('GET', '/church/public/search?page=1&pageSize=10').then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('items');
        expect(response.body).to.have.property('totalCount');
        
        if (response.body.items.length > 0) {
          response.body.items.forEach(church => {
            expect(church).to.have.property('id');
            expect(church).to.have.property('name');
            expect(church).to.have.property('averageRating');
            expect(church).to.have.property('totalReviews');
          });
        }
      });
    });

    it('should filter churches by minimum rating', () => {
      cy.apiRequest('GET', '/church/public/search?minRating=4&page=1&pageSize=10').then((response) => {
        expect(response.status).to.eq(200);
        
        if (response.body.items.length > 0) {
          response.body.items.forEach(church => {
            expect(church.averageRating).to.be.at.least(4);
          });
        }
      });
    });

    it('should filter by minimum review count', () => {
      cy.apiRequest('GET', '/church/public/search?minReviewCount=5&page=1&pageSize=10').then((response) => {
        expect(response.status).to.eq(200);
        
        if (response.body.items.length > 0) {
          response.body.items.forEach(church => {
            expect(church.totalReviews).to.be.at.least(5);
          });
        }
      });
    });

    it('should search by name', () => {
      cy.apiRequest('GET', '/church/public/search?name=Baptist&page=1&pageSize=10').then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should search by city', () => {
      cy.apiRequest('GET', '/church/public/search?city=São Paulo&page=1&pageSize=10').then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should search by state', () => {
      cy.apiRequest('GET', '/church/public/search?state=SP&page=1&pageSize=10').then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should sort by rating descending', () => {
      cy.apiRequest('GET', '/church/public/search?sortBy=Rating&sortDirection=desc&page=1&pageSize=10').then((response) => {
        expect(response.status).to.eq(200);
        
        if (response.body.items.length > 1) {
          // Verify descending order
          for (let i = 0; i < response.body.items.length - 1; i++) {
            expect(response.body.items[i].averageRating).to.be.at.least(response.body.items[i + 1].averageRating);
          }
        }
      });
    });

    it('should sort by relevance', () => {
      cy.apiRequest('GET', '/church/public/search?sortBy=Relevance&page=1&pageSize=10').then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should paginate results', () => {
      cy.apiRequest('GET', '/church/public/search?page=1&pageSize=5').then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.items.length).to.be.lte(5);
      });
    });
  });

  describe('GET /church/public/nearby - Get Nearby Churches', () => {
    it('should get nearby churches by coordinates', () => {
      // Coordenadas de São Paulo
      const lat = -23.550520;
      const lng = -46.633308;
      const radiusKm = 10;

      cy.apiRequest('GET', `/church/public/nearby?lat=${lat}&lng=${lng}&radiusKm=${radiusKm}`).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
        
        if (response.body.length > 0) {
          response.body.forEach(church => {
            expect(church).to.have.property('id');
            expect(church).to.have.property('name');
            expect(church).to.have.property('distance');
            expect(church).to.have.property('averageRating');
            expect(church.distance).to.be.lte(radiusKm);
          });
        }
      });
    });

    it('should limit results with max parameter', () => {
      const lat = -23.550520;
      const lng = -46.633308;
      const radiusKm = 50;
      const max = 5;

      cy.apiRequest('GET', `/church/public/nearby?lat=${lat}&lng=${lng}&radiusKm=${radiusKm}&max=${max}`).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.length).to.be.lte(max);
      });
    });

    it('should use default radius of 5km', () => {
      const lat = -23.550520;
      const lng = -46.633308;

      cy.apiRequest('GET', `/church/public/nearby?lat=${lat}&lng=${lng}`).then((response) => {
        expect(response.status).to.eq(200);
        
        if (response.body.length > 0) {
          response.body.forEach(church => {
            expect(church.distance).to.be.lte(5);
          });
        }
      });
    });

    it('should fail without coordinates', () => {
      cy.apiRequest('GET', '/church/public/nearby').then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });
  });

  describe('POST /church/{id}/review - Create Review (if implemented)', () => {
    it('should create review as member', () => {
      const reviewData = {
        rating: 5,
        comment: 'Excellent church! Great community and wonderful services.',
        attendedDate: new Date().toISOString()
      };

      cy.apiRequest('POST', `/church/${testChurchId}/review`, memberToken, reviewData).then((response) => {
        // Se endpoint existir
        if (response.status !== 404) {
          expect([200, 201]).to.include(response.status);
          expect(response.body).to.have.property('id');
        }
      });
    });

    it('should create review with minimum rating', () => {
      const reviewData = {
        rating: 1,
        comment: 'Needs improvement'
      };

      cy.apiRequest('POST', `/church/${testChurchId}/review`, memberToken, reviewData).then((response) => {
        if (response.status !== 404) {
          expect([200, 201]).to.include(response.status);
        }
      });
    });

    it('should fail with rating above 5', () => {
      const invalidReview = {
        rating: 6,
        comment: 'Invalid rating'
      };

      cy.apiRequest('POST', `/church/${testChurchId}/review`, memberToken, invalidReview).then((response) => {
        if (response.status !== 404) {
          cy.validateErrorResponse(response, 400);
        }
      });
    });

    it('should fail with rating below 1', () => {
      const invalidReview = {
        rating: 0,
        comment: 'Invalid rating'
      };

      cy.apiRequest('POST', `/church/${testChurchId}/review`, memberToken, invalidReview).then((response) => {
        if (response.status !== 404) {
          cy.validateErrorResponse(response, 400);
        }
      });
    });
  });

  describe('GET /church/{id}/reviews - Get Church Reviews (if implemented)', () => {
    it('should get church reviews publicly', () => {
      cy.apiRequest('GET', `/church/${testChurchId}/reviews?page=1&pageSize=10`).then((response) => {
        if (response.status !== 404) {
          expect(response.status).to.eq(200);
          expect(response.body).to.have.property('items');
          
          if (response.body.items.length > 0) {
            response.body.items.forEach(review => {
              expect(review).to.have.property('rating');
              expect(review).to.have.property('comment');
              expect(review).to.have.property('memberName');
              expect(review).to.have.property('createdAt');
              expect(review.rating).to.be.within(1, 5);
            });
          }
        }
      });
    });

    it('should filter reviews by rating', () => {
      cy.apiRequest('GET', `/church/${testChurchId}/reviews?rating=5&page=1&pageSize=10`).then((response) => {
        if (response.status !== 404) {
          expect(response.status).to.eq(200);
          
          if (response.body.items.length > 0) {
            response.body.items.forEach(review => {
              expect(review.rating).to.eq(5);
            });
          }
        }
      });
    });

    it('should sort reviews by date descending', () => {
      cy.apiRequest('GET', `/church/${testChurchId}/reviews?sortBy=Date&sortDirection=desc&page=1&pageSize=10`).then((response) => {
        if (response.status !== 404) {
          expect(response.status).to.eq(200);
        }
      });
    });
  });

  describe('Church Rating Statistics', () => {
    it('should include rating statistics in church details', () => {
      cy.apiRequest('GET', `/church/${testChurchId}`, memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id', testChurchId);
        
        // Check if rating info is included
        if (response.body.averageRating !== undefined) {
          expect(response.body.averageRating).to.be.a('number');
          expect(response.body.averageRating).to.be.within(0, 5);
        }
        
        if (response.body.totalReviews !== undefined) {
          expect(response.body.totalReviews).to.be.a('number');
          expect(response.body.totalReviews).to.be.at.least(0);
        }
      });
    });
  });
});
