/// <reference types="cypress" />

describe('?? Feed API Tests', () => {
  let adminToken;
  let memberToken;
  let createdPostId;

  before(() => {
    cy.loginAsAdmin().then((token) => {
      adminToken = token;
    });
    cy.loginAsMember().then((token) => {
      memberToken = token;
    });
  });

  describe('POST /feed - Create Feed Post', () => {
    it('should create post as admin', () => {
      const postData = {
        content: `Test post created at ${Date.now()}`,
        title: 'Test Post',
        imageBase64: 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg=='
      };

      cy.apiRequest('POST', '/feed', adminToken, postData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
        createdPostId = response.body;
      });
    });

    it('should create post with text only', () => {
      const postData = {
        content: 'Simple text post without image',
        title: 'Text Only Post'
      };

      cy.apiRequest('POST', '/feed', adminToken, postData).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should create post with long content', () => {
      const postData = {
        content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. '.repeat(10),
        title: 'Long Post'
      };

      cy.apiRequest('POST', '/feed', adminToken, postData).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail without content', () => {
      const invalidData = {
        title: 'No Content'
      };

      cy.apiRequest('POST', '/feed', adminToken, invalidData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail as member', () => {
      const postData = {
        content: 'Unauthorized post',
        title: 'Test'
      };

      cy.apiRequest('POST', '/feed', memberToken, postData).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });

    it('should fail without authentication', () => {
      const postData = {
        content: 'Unauthenticated post',
        title: 'Test'
      };

      cy.apiRequest('POST', '/feed', null, postData).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('GET /feed - Get All Feed Posts', () => {
    it('should get all posts as member', () => {
      cy.apiRequest('GET', '/feed', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('items');
        expect(response.body).to.have.property('page');
        expect(response.body).to.have.property('totalCount');
        expect(response.body.items).to.be.an('array');

        if (response.body.items.length > 0) {
          response.body.items.forEach(post => {
            expect(post).to.have.property('postId');
            expect(post).to.have.property('content');
            expect(post).to.have.property('title');
            expect(post).to.have.property('createdAt');
            expect(post).to.have.property('likesCount');
          });
        }
      });
    });

    it('should get all posts as admin', () => {
      cy.apiRequest('GET', '/feed', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.items).to.be.an('array');
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/feed').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('GET /feed/{id} - Get Feed Post by ID', () => {
    it('should get post by id as member', () => {
      if (createdPostId) {
        cy.apiRequest('GET', `/feed/${createdPostId}`, memberToken).then((response) => {
          expect(response.status).to.eq(200);
          expect(response.body).to.have.property('postId', createdPostId);
          expect(response.body).to.have.property('content');
          expect(response.body).to.have.property('title');
          expect(response.body).to.have.property('likesCount');
          expect(response.body).to.have.property('isLikedByCurrentUser');
        });
      }
    });

    it('should get post as admin', () => {
      if (createdPostId) {
        cy.apiRequest('GET', `/feed/${createdPostId}`, adminToken).then((response) => {
          expect(response.status).to.eq(200);
        });
      }
    });

    it('should fail to get non-existent post', () => {
      cy.apiRequest('GET', '/feed/99999', memberToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/feed/1').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('PUT /feed/{id} - Update Feed Post', () => {
    it('should update post as admin', () => {
      if (createdPostId) {
        const updatedData = {
          content: `Updated content at ${Date.now()}`,
          title: 'Updated Title'
        };

        cy.apiRequest('PUT', `/feed/${createdPostId}`, adminToken, updatedData).then((response) => {
          expect(response.status).to.eq(200);
          expect(response.body).to.have.property('postId', createdPostId);
          expect(response.body).to.have.property('content', updatedData.content);
        });
      }
    });

    it('should update only content', () => {
      if (createdPostId) {
        const updatedData = {
          content: 'Only content updated'
        };

        cy.apiRequest('PUT', `/feed/${createdPostId}`, adminToken, updatedData).then((response) => {
          expect(response.status).to.eq(200);
        });
      }
    });

    it('should fail to update non-existent post', () => {
      const updatedData = {
        content: 'Updated content'
      };

      cy.apiRequest('PUT', '/feed/99999', adminToken, updatedData).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });

    it('should fail as member', () => {
      const updatedData = {
        content: 'Unauthorized update'
      };

      cy.apiRequest('PUT', `/feed/${createdPostId || 1}`, memberToken, updatedData).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });
  });

  describe('POST /feed/{id}/like - Add Like', () => {
    it('should add like as member', () => {
      if (createdPostId) {
        cy.apiRequest('POST', `/feed/${createdPostId}/like`, memberToken).then((response) => {
          expect(response.status).to.eq(204);
        });
      }
    });

    it('should add like as admin', () => {
      if (createdPostId) {
        cy.apiRequest('POST', `/feed/${createdPostId}/like`, adminToken).then((response) => {
          expect(response.status).to.eq(204);
        });
      }
    });

    it('should handle duplicate like gracefully', () => {
      if (createdPostId) {
        // Second like from same user
        cy.apiRequest('POST', `/feed/${createdPostId}/like`, memberToken).then((response) => {
          expect([204, 400]).to.include(response.status); // 400 if already liked
        });
      }
    });

    it('should fail to like non-existent post', () => {
      cy.apiRequest('POST', '/feed/99999/like', memberToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('POST', `/feed/${createdPostId || 1}/like`).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('DELETE /feed/{id}/like - Remove Like', () => {
    it('should remove like as member', () => {
      if (createdPostId) {
        cy.apiRequest('DELETE', `/feed/${createdPostId}/like`, memberToken).then((response) => {
          expect(response.status).to.eq(204);
        });
      }
    });

    it('should handle removing non-existent like gracefully', () => {
      if (createdPostId) {
        // Second removal
        cy.apiRequest('DELETE', `/feed/${createdPostId}/like`, memberToken).then((response) => {
          expect([204, 404]).to.include(response.status);
        });
      }
    });

    it('should fail to unlike non-existent post', () => {
      cy.apiRequest('DELETE', '/feed/99999/like', memberToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('DELETE', `/feed/${createdPostId || 1}/like`).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('DELETE /feed/{id} - Delete Feed Post', () => {
    it('should delete post as admin', () => {
      if (createdPostId) {
        cy.apiRequest('DELETE', `/feed/${createdPostId}`, adminToken).then((response) => {
          expect(response.status).to.eq(204);
        });
      }
    });

    it('should fail to delete non-existent post', () => {
      cy.apiRequest('DELETE', '/feed/99999', adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });

    it('should fail as member', () => {
      cy.apiRequest('DELETE', '/feed/1', memberToken).then((response) => {
        cy.validateErrorResponse(response, 403);
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('DELETE', '/feed/1').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });
});
