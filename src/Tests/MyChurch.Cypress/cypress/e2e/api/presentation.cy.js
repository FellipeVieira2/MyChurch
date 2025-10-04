/// <reference types="cypress" />

describe('?? Presentation API Tests', () => {
  let adminToken;
  let memberToken;
  let createdPresentationId;
  let createdSlideId;

  before(() => {
    cy.loginAsAdmin().then((token) => {
      adminToken = token;
    });
    cy.loginAsMember().then((token) => {
      memberToken = token;
    });
  });

  describe('POST /presentation - Create Presentation', () => {
    it('should create presentation as admin', () => {
      const presentationData = {
        name: `Test Presentation ${Date.now()}`,
        description: 'Presentation for worship service'
      };

      cy.apiRequest('POST', '/presentation', adminToken, presentationData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id');
        expect(response.body).to.have.property('name', presentationData.name);
        expect(response.body).to.have.property('description', presentationData.description);
        expect(response.body).to.have.property('slides');
        expect(response.body.slides).to.be.an('array');
        createdPresentationId = response.body.id;
      });
    });

    it('should create presentation as member', () => {
      const presentationData = {
        name: 'Member Presentation',
        description: 'Created by member'
      };

      cy.apiRequest('POST', '/presentation', memberToken, presentationData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id');
      });
    });

    it('should fail without name', () => {
      const invalidData = {
        description: 'No name'
      };

      cy.apiRequest('POST', '/presentation', adminToken, invalidData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail without authentication', () => {
      const presentationData = {
        name: 'Unauthorized Presentation',
        description: 'Should fail'
      };

      cy.apiRequest('POST', '/presentation', null, presentationData).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('GET /presentation - Get All Presentations', () => {
    it('should get all presentations as admin', () => {
      cy.apiRequest('GET', '/presentation', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');

        if (response.body.length > 0) {
          response.body.forEach(presentation => {
            expect(presentation).to.have.property('id');
            expect(presentation).to.have.property('name');
            expect(presentation).to.have.property('slides');
          });
        }
      });
    });

    it('should get all presentations as member', () => {
      cy.apiRequest('GET', '/presentation', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/presentation').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('GET /presentation/{id} - Get Presentation by ID', () => {
    it('should get presentation by id', () => {
      cy.apiRequest('GET', `/presentation/${createdPresentationId}`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id', createdPresentationId);
        expect(response.body).to.have.property('name');
        expect(response.body).to.have.property('currentSlideIndex');
        expect(response.body).to.have.property('isLive');
      });
    });

    it('should fail to get non-existent presentation', () => {
      cy.apiRequest('GET', '/presentation/99999', adminToken).then((response) => {
        expect(response.status).to.eq(404);
      });
    });
  });

  describe('POST /presentation/{presentationId}/slide - Add Slide', () => {
    it('should add Bible verse slide', () => {
      const slideData = {
        contentType: 0, // BibleVerse
        contentReferenceJson: JSON.stringify({
          versionId: 1,
          bookId: 1,
          chapterId: 1,
          verseId: 1
        }),
        orderIndex: 1
      };

      cy.apiRequest('POST', `/presentation/${createdPresentationId}/slide`, adminToken, slideData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id');
        expect(response.body).to.have.property('orderIndex', 1);
        expect(response.body).to.have.property('contentType', 0);
        createdSlideId = response.body.id;
      });
    });

    it('should add hymn slide', () => {
      const slideData = {
        contentType: 1, // Hymn
        contentReferenceJson: JSON.stringify({
          hymnNumber: 123,
          verseNumber: 1
        }),
        orderIndex: 2
      };

      cy.apiRequest('POST', `/presentation/${createdPresentationId}/slide`, adminToken, slideData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('contentType', 1);
      });
    });

    it('should add announcement slide', () => {
      const slideData = {
        contentType: 2, // Announcement
        contentReferenceJson: JSON.stringify({
          text: 'Important announcement for the church',
          title: 'Church News'
        }),
        orderIndex: 3
      };

      cy.apiRequest('POST', `/presentation/${createdPresentationId}/slide`, adminToken, slideData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('contentType', 2);
      });
    });

    it('should add image slide', () => {
      const slideData = {
        contentType: 3, // Image
        contentReferenceJson: JSON.stringify({
          imageUrl: 'https://example.com/image.jpg',
          caption: 'Beautiful landscape'
        }),
        orderIndex: 4
      };

      cy.apiRequest('POST', `/presentation/${createdPresentationId}/slide`, adminToken, slideData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('contentType', 3);
      });
    });

    it('should fail to add slide to non-existent presentation', () => {
      const slideData = {
        contentType: 0,
        contentReferenceJson: '{}',
        orderIndex: 1
      };

      cy.apiRequest('POST', '/presentation/99999/slide', adminToken, slideData).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('PUT /presentation/slide/{id} - Update Slide', () => {
    it('should update slide content', () => {
      const updatedData = {
        contentReferenceJson: JSON.stringify({
          versionId: 1,
          bookId: 2,
          chapterId: 3,
          verseId: 16
        })
      };

      cy.apiRequest('PUT', `/presentation/slide/${createdSlideId}`, adminToken, updatedData).then((response) => {
        expect(response.status).to.eq(204);
      });
    });

    it('should fail to update non-existent slide', () => {
      const updatedData = {
        contentReferenceJson: '{}'
      };

      cy.apiRequest('PUT', '/presentation/slide/99999', adminToken, updatedData).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('POST /presentation/{presentationId}/live/start - Start Live Presentation', () => {
    it('should start live presentation', () => {
      cy.apiRequest('POST', `/presentation/${createdPresentationId}/live/start`, adminToken, {}).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail to start live with non-existent presentation', () => {
      cy.apiRequest('POST', '/presentation/99999/live/start', adminToken, {}).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('POST /presentation/{presentationId}/live/next - Next Slide', () => {
    it('should go to next slide', () => {
      cy.apiRequest('POST', `/presentation/${createdPresentationId}/live/next`, adminToken, {}).then((response) => {
        expect(response.status).to.eq(200);
      });
    });
  });

  describe('POST /presentation/{presentationId}/live/prev - Previous Slide', () => {
    it('should go to previous slide', () => {
      cy.apiRequest('POST', `/presentation/${createdPresentationId}/live/prev`, adminToken, {}).then((response) => {
        expect(response.status).to.eq(200);
      });
    });
  });

  describe('POST /presentation/{presentationId}/live/goto/{targetIndex} - Go to Slide', () => {
    it('should go to specific slide index', () => {
      cy.apiRequest('POST', `/presentation/${createdPresentationId}/live/goto/2`, adminToken, {}).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should go to first slide', () => {
      cy.apiRequest('POST', `/presentation/${createdPresentationId}/live/goto/0`, adminToken, {}).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail with invalid index', () => {
      cy.apiRequest('POST', `/presentation/${createdPresentationId}/live/goto/999`, adminToken, {}).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });
  });

  describe('POST /presentation/{presentationId}/live/end - End Live Presentation', () => {
    it('should end live presentation', () => {
      cy.apiRequest('POST', `/presentation/${createdPresentationId}/live/end`, adminToken, {}).then((response) => {
        expect(response.status).to.eq(200);
      });
    });

    it('should fail to end non-existent presentation', () => {
      cy.apiRequest('POST', '/presentation/99999/live/end', adminToken, {}).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('DELETE /presentation/slide/{id} - Delete Slide', () => {
    it('should delete slide', () => {
      if (createdSlideId) {
        cy.apiRequest('DELETE', `/presentation/slide/${createdSlideId}`, adminToken).then((response) => {
          expect(response.status).to.eq(204);
        });
      }
    });

    it('should fail to delete non-existent slide', () => {
      cy.apiRequest('DELETE', '/presentation/slide/99999', adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });
});
