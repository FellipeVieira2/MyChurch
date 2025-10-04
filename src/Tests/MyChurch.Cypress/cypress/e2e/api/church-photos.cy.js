/// <reference types="cypress" />

describe('??? Church Photo Gallery - Sistema de Fotos da Igreja', () => {
  let authToken;
  let adminUserId;
  let churchId;
  let photoId;

  before(() => {
    // Login como admin para obter token
    cy.request({
      method: 'POST',
      url: `${Cypress.config('baseUrl')}/api/auth/login`,
      body: {
        identifier: Cypress.env('adminEmail'),
        password: Cypress.env('adminPassword')
      }
    }).then((response) => {
      expect(response.status).to.eq(200);
      authToken = response.body.token;
      adminUserId = response.body.member.id;
      churchId = response.body.member.churchId;
      
      cy.log('? Logged in as admin');
      cy.log(`Church ID: ${churchId}`);
    });
  });

  describe('?? Upload de Fotos', () => {
    it('Deve permitir que membro faça upload de foto', () => {
      // Criar uma imagem base64 simples (1x1 pixel branco)
      const base64Image = 'data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAABAAEDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlbaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9/KKKKAP/2Q==';

      cy.request({
        method: 'POST',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/upload`,
        headers: {
          Authorization: `Bearer ${authToken}`
        },
        body: {
          photoBase64: base64Image,
          caption: 'Foto de teste - Fachada da igreja',
          category: 'Exterior'
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('id');
        expect(response.body).to.have.property('photoUrl');
        expect(response.body.caption).to.eq('Foto de teste - Fachada da igreja');
        expect(response.body.category).to.eq('Exterior');
        expect(response.body.isApproved).to.eq(false); // Precisa aprovação
        
        photoId = response.body.id;
        cy.log(`? Photo uploaded with ID: ${photoId}`);
      });
    });

    it('Deve fazer upload de múltiplas categorias', () => {
      const categories = ['Interior', 'Worship', 'Events', 'Community', 'Facilities'];
      const base64Image = 'data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAABAAEDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlbaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9/KKKKAP/2Q==';

      categories.forEach((category) => {
        cy.request({
          method: 'POST',
          url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/upload`,
          headers: {
            Authorization: `Bearer ${authToken}`
          },
          body: {
            photoBase64: base64Image,
            caption: `Foto de ${category}`,
            category: category
          }
        }).then((response) => {
          expect(response.status).to.eq(200);
          expect(response.body.category).to.eq(category);
          cy.log(`? Uploaded ${category} photo`);
        });
      });
    });
  });

  describe('?? Buscar Galeria de Fotos', () => {
    it('Deve buscar todas as fotos aprovadas da igreja', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos`,
        failOnStatusCode: false
      }).then((response) => {
        expect(response.status).to.be.oneOf([200, 404]); // Pode não ter fotos aprovadas ainda
        
        if (response.status === 200) {
          expect(response.body).to.have.property('photos');
          expect(response.body).to.have.property('totalPhotos');
          expect(response.body).to.have.property('categoriesCount');
          cy.log(`Total de fotos: ${response.body.totalPhotos}`);
        }
      });
    });

    it('Deve filtrar fotos por categoria', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos?category=Exterior`,
        failOnStatusCode: false
      }).then((response) => {
        expect(response.status).to.be.oneOf([200, 404]);
        
        if (response.status === 200 && response.body.photos.length > 0) {
          response.body.photos.forEach((photo) => {
            expect(photo.category).to.eq('Exterior');
          });
        }
      });
    });

    it('Deve buscar apenas fotos em destaque', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos?onlyFeatured=true`,
        failOnStatusCode: false
      }).then((response) => {
        expect(response.status).to.be.oneOf([200, 404]);
        
        if (response.status === 200 && response.body.photos.length > 0) {
          response.body.photos.forEach((photo) => {
            expect(photo.isFeatured).to.be.true;
          });
        }
      });
    });
  });

  describe('? Moderação de Fotos (Admin)', () => {
    it('Deve buscar fotos pendentes de aprovação', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/pending`,
        headers: {
          Authorization: `Bearer ${authToken}`
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.be.an('array');
        
        if (response.body.length > 0) {
          cy.log(`?? ${response.body.length} fotos pendentes`);
          response.body.forEach((photo) => {
            expect(photo.isApproved).to.be.false;
            expect(photo.isRejected).to.be.false;
          });
        }
      });
    });

    it('Deve aprovar uma foto', () => {
      cy.request({
        method: 'POST',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/${photoId}/approve`,
        headers: {
          Authorization: `Bearer ${authToken}`
        },
        body: {
          setAsFeatured: false
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.isApproved).to.be.true;
        expect(response.body.isRejected).to.be.false;
        cy.log('? Foto aprovada com sucesso');
      });
    });

    it('Deve aprovar e marcar foto como destaque', () => {
      // Fazer upload de nova foto primeiro
      const base64Image = 'data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAABAAEDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlbaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9/KKKKAP/2Q==';

      cy.request({
        method: 'POST',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/upload`,
        headers: {
          Authorization: `Bearer ${authToken}`
        },
        body: {
          photoBase64: base64Image,
          caption: 'Foto destaque',
          category: 'Exterior'
        }
      }).then((uploadResponse) => {
        const newPhotoId = uploadResponse.body.id;

        // Aprovar e marcar como destaque
        cy.request({
          method: 'POST',
          url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/${newPhotoId}/approve`,
          headers: {
            Authorization: `Bearer ${authToken}`
          },
          body: {
            setAsFeatured: true,
            displayOrder: 1
          }
        }).then((response) => {
          expect(response.status).to.eq(200);
          expect(response.body.isFeatured).to.be.true;
          expect(response.body.displayOrder).to.eq(1);
          cy.log('? Foto marcada como destaque');
        });
      });
    });

    it('Deve rejeitar uma foto', () => {
      // Fazer upload de nova foto primeiro
      const base64Image = 'data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAABAAEDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlbaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9/KKKKAP/2Q==';

      cy.request({
        method: 'POST',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/upload`,
        headers: {
          Authorization: `Bearer ${authToken}`
        },
        body: {
          photoBase64: base64Image,
          caption: 'Foto para rejeitar',
          category: 'Other'
        }
      }).then((uploadResponse) => {
        const newPhotoId = uploadResponse.body.id;

        // Rejeitar foto
        cy.request({
          method: 'POST',
          url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/${newPhotoId}/reject`,
          headers: {
            Authorization: `Bearer ${authToken}`
          },
          body: {
            reason: 'Conteúdo inadequado para testes'
          }
        }).then((response) => {
          expect(response.status).to.eq(204); // NoContent
          cy.log('? Foto rejeitada');
        });
      });
    });
  });

  describe('?? Sistema de Likes', () => {
    it('Deve curtir uma foto', () => {
      cy.request({
        method: 'POST',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/${photoId}/like`,
        headers: {
          Authorization: `Bearer ${authToken}`
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.isLiked).to.be.true;
        expect(response.body.totalLikes).to.be.greaterThan(0);
        cy.log(`?? Photo liked. Total likes: ${response.body.totalLikes}`);
      });
    });

    it('Deve descurtir uma foto (toggle)', () => {
      // Curtir primeiro
      cy.request({
        method: 'POST',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/${photoId}/like`,
        headers: {
          Authorization: `Bearer ${authToken}`
        }
      });

      // Descurtir
      cy.request({
        method: 'POST',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/${photoId}/like`,
        headers: {
          Authorization: `Bearer ${authToken}`
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.isLiked).to.be.false;
        cy.log(`?? Photo unliked. Total likes: ${response.body.totalLikes}`);
      });
    });
  });

  describe('?? Estatísticas e Categorias', () => {
    it('Deve retornar contagem de fotos por categoria', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos`,
        failOnStatusCode: false
      }).then((response) => {
        if (response.status === 200) {
          expect(response.body.categoriesCount).to.be.an('array');
          
          if (response.body.categoriesCount.length > 0) {
            response.body.categoriesCount.forEach((category) => {
              expect(category).to.have.property('category');
              expect(category).to.have.property('categoryDisplay');
              expect(category).to.have.property('count');
              cy.log(`${category.categoryDisplay}: ${category.count} fotos`);
            });
          }
        }
      });
    });
  });

  describe('?? Segurança e Autorização', () => {
    it('Não deve permitir que não-admin aprove fotos', () => {
      // Tentar aprovar sem ser admin (simulando erro esperado)
      cy.request({
        method: 'POST',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/${photoId}/approve`,
        failOnStatusCode: false,
        headers: {
          Authorization: 'Bearer fake-token'
        },
        body: {}
      }).then((response) => {
        expect(response.status).to.be.oneOf([401, 403]); // Unauthorized or Forbidden
        cy.log('?? Não-admin bloqueado corretamente');
      });
    });

    it('Deve permitir upload público para visitantes', () => {
      // Upload sem autenticação (como visitante)
      const base64Image = 'data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAABAAEDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlbaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9/KKKKAP/2Q==';

      cy.request({
        method: 'POST',
        url: `${Cypress.config('baseUrl')}/api/church/${churchId}/photos/upload/visitor/1`,
        body: {
          photoBase64: base64Image,
          caption: 'Foto enviada por visitante',
          category: 'Community'
        },
        failOnStatusCode: false
      }).then((response) => {
        expect(response.status).to.be.oneOf([200, 201]);
        cy.log('?? Visitante conseguiu fazer upload');
      });
    });
  });
});
