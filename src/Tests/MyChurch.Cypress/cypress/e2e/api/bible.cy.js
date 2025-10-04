/// <reference types="cypress" />

describe('?? Bible API Tests', () => {
  let adminToken;
  let memberToken;

  before(() => {
    cy.loginAsAdmin().then((token) => {
      adminToken = token;
    });
    cy.loginAsMember().then((token) => {
      memberToken = token;
    });
  });

  describe('GET /bible/versions', () => {
    it('should get all bible versions as admin', () => {
      cy.apiRequest('GET', '/bible/versions', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
        expect(response.body.length).to.be.greaterThan(0);
        
        response.body.forEach(version => {
          expect(version).to.have.property('id');
          expect(version).to.have.property('name');
          expect(version).to.have.property('abbreviation');
        });
      });
    });

    it('should get all bible versions as member', () => {
      cy.apiRequest('GET', '/bible/versions', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/bible/versions').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('GET /bible/versions/{versionId}/books', () => {
    let versionId;

    before(() => {
      cy.apiRequest('GET', '/bible/versions', adminToken).then((response) => {
        versionId = response.body[0].id;
      });
    });

    it('should get all books of a version', () => {
      cy.apiRequest('GET', `/bible/versions/${versionId}/books`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
        expect(response.body.length).to.eq(66); // 66 books in the Bible
        
        response.body.forEach(book => {
          expect(book).to.have.property('id');
          expect(book).to.have.property('name');
          expect(book).to.have.property('bookOrder');
          expect(book).to.have.property('testament');
        });
      });
    });

    it('should fail with invalid version id', () => {
      cy.apiRequest('GET', `/bible/versions/99999/books`, adminToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });

  describe('GET /bible/books/{bookId}/chapters', () => {
    let bookId;

    before(() => {
      cy.apiRequest('GET', '/bible/versions', adminToken).then((versionResponse) => {
        const versionId = versionResponse.body[0].id;
        cy.apiRequest('GET', `/bible/versions/${versionId}/books`, adminToken).then((bookResponse) => {
          bookId = bookResponse.body[0].id; // Genesis
        });
      });
    });

    it('should get all chapters of a book', () => {
      cy.apiRequest('GET', `/bible/books/${bookId}/chapters`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
        expect(response.body.length).to.be.greaterThan(0);
        
        response.body.forEach(chapter => {
          expect(chapter).to.have.property('id');
          expect(chapter).to.have.property('chapterNumber');
        });
      });
    });
  });

  describe('GET /bible/chapters/{chapterId}/verses', () => {
    let chapterId;

    before(() => {
      cy.apiRequest('GET', '/bible/versions', adminToken).then((versionResponse) => {
        const versionId = versionResponse.body[0].id;
        cy.apiRequest('GET', `/bible/versions/${versionId}/books`, adminToken).then((bookResponse) => {
          const bookId = bookResponse.body[0].id;
          cy.apiRequest('GET', `/bible/books/${bookId}/chapters`, adminToken).then((chapterResponse) => {
            chapterId = chapterResponse.body[0].id;
          });
        });
      });
    });

    it('should get all verses of a chapter', () => {
      cy.apiRequest('GET', `/bible/chapters/${chapterId}/verses`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
        expect(response.body.length).to.be.greaterThan(0);
        
        response.body.forEach(verse => {
          expect(verse).to.have.property('id');
          expect(verse).to.have.property('verseNumber');
          expect(verse).to.have.property('text');
        });
      });
    });
  });

  describe('GET /bible/versions/{versionId}/books/{bookName}/chapters/{chapterNumber}/verses', () => {
    let versionId;

    before(() => {
      cy.apiRequest('GET', '/bible/versions', adminToken).then((response) => {
        versionId = response.body[0].id;
      });
    });

    it('should get verses by reference (John 3)', () => {
      cy.apiRequest('GET', `/bible/versions/${versionId}/books/João/chapters/3/verses`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
        expect(response.body.length).to.be.greaterThan(0);
      });
    });

    it('should get verses by reference (Genesis 1)', () => {
      cy.apiRequest('GET', `/bible/versions/${versionId}/books/Gênesis/chapters/1/verses`, adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
      });
    });
  });

  describe('POST /bible/favorites', () => {
    let versionId;

    before(() => {
      cy.apiRequest('GET', '/bible/versions', memberToken).then((response) => {
        versionId = response.body[0].id;
      });
    });

    it('should add favorite verse', () => {
      const favoriteVerse = {
        versionId: versionId,
        bookName: 'João',
        chapterNumber: 3,
        verseNumber: 16
      };

      cy.apiRequest('POST', '/bible/favorites', memberToken, favoriteVerse).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.a('number');
        cy.wrap(response.body).as('favoriteVerseId');
      });
    });

    it('should fail to add duplicate favorite', () => {
      const favoriteVerse = {
        versionId: versionId,
        bookName: 'João',
        chapterNumber: 3,
        verseNumber: 16
      };

      cy.apiRequest('POST', '/bible/favorites', memberToken, favoriteVerse).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });
  });

  describe('GET /bible/favorites', () => {
    it('should get all favorite verses', () => {
      cy.apiRequest('GET', '/bible/favorites', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
        
        if (response.body.length > 0) {
          response.body.forEach(favorite => {
            expect(favorite).to.have.property('id');
            expect(favorite).to.have.property('verse');
            expect(favorite).to.have.property('createdAt');
          });
        }
      });
    });
  });

  describe('DELETE /bible/favorites/{favoriteVerseId}', () => {
    it('should remove favorite verse', function() {
      if (this.favoriteVerseId) {
        cy.apiRequest('DELETE', `/bible/favorites/${this.favoriteVerseId}`, memberToken).then((response) => {
          expect(response.status).to.eq(200);
        });
      }
    });

    it('should fail to remove non-existent favorite', () => {
      cy.apiRequest('DELETE', '/bible/favorites/99999', memberToken).then((response) => {
        cy.validateErrorResponse(response, 404);
      });
    });
  });
});
