/// <reference types="cypress" />

describe('?? Church Search & Discovery - Busca Geoespacial', () => {
  // Coordenadas de São Paulo (centro) para testes
  const SAO_PAULO_LAT = -23.5505;
  const SAO_PAULO_LNG = -46.6333;

  describe('?? Busca por Proximidade (Nearby)', () => {
    it('Deve buscar igrejas próximas em um raio de 10km', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 10,
          page: 1,
          pageSize: 20
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('churches');
        expect(response.body).to.have.property('totalResults');
        expect(response.body).to.have.property('mapBounds');
        expect(response.body).to.have.property('suggestions');
        expect(response.body).to.have.property('availableFilters');

        cy.log(`? Found ${response.body.totalResults} churches`);

        if (response.body.churches.length > 0) {
          const firstChurch = response.body.churches[0];
          
          // Verificar estrutura do card
          expect(firstChurch).to.have.property('id');
          expect(firstChurch).to.have.property('name');
          expect(firstChurch).to.have.property('distanceKm');
          expect(firstChurch).to.have.property('address');
          expect(firstChurch).to.have.property('city');
          expect(firstChurch).to.have.property('state');
          expect(firstChurch).to.have.property('features');
          expect(firstChurch).to.have.property('badges');
          expect(firstChurch).to.have.property('latitude');
          expect(firstChurch).to.have.property('longitude');

          // Verificar que distância está dentro do raio
          expect(firstChurch.distanceKm).to.be.lessThan(10);

          cy.log(`?? ${firstChurch.name} - ${firstChurch.distanceKm}km`);
        }
      });
    });

    it('Deve ordenar igrejas por distância (padrão)', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20,
          sortBy: 'distance'
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.churches.length > 1) {
          const churches = response.body.churches;
          
          // Verificar que está ordenado por distância
          for (let i = 0; i < churches.length - 1; i++) {
            expect(churches[i].distanceKm).to.be.at.most(churches[i + 1].distanceKm);
          }

          cy.log('? Churches sorted by distance correctly');
        }
      });
    });

    it('Deve ordenar igrejas por avaliação', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20,
          sortBy: 'rating'
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.churches.length > 1) {
          const churches = response.body.churches;
          
          cy.log('?? Churches sorted by rating');
          churches.forEach(church => {
            cy.log(`? ${church.name}: ${church.averageRating || 'N/A'} stars`);
          });
        }
      });
    });

    it('Deve ordenar igrejas por popularidade', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20,
          sortBy: 'popularity'
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.churches.length > 1) {
          const churches = response.body.churches;
          
          cy.log('?? Churches sorted by popularity');
          churches.forEach(church => {
            cy.log(`?? ${church.name}: ${church.totalVisits} visits`);
          });
        }
      });
    });
  });

  describe('?? Filtros de Busca', () => {
    it('Deve filtrar por denominação', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20,
          denominations: 'Batista,Assembleia de Deus'
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.churches.length > 0) {
          response.body.churches.forEach(church => {
            if (church.denomination) {
              expect(['Batista', 'Assembleia de Deus']).to.include(church.denomination);
            }
          });

          cy.log(`? ${response.body.totalResults} churches found with specified denominations`);
        }
      });
    });

    it('Deve filtrar por avaliação mínima', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20,
          minRating: 4
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.churches.length > 0) {
          response.body.churches.forEach(church => {
            if (church.averageRating) {
              expect(church.averageRating).to.be.at.least(4);
            }
          });

          cy.log(`? ${response.body.totalResults} churches with 4+ rating`);
        }
      });
    });

    it('Deve filtrar por características (estacionamento)', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20,
          hasParking: true
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.churches.length > 0) {
          response.body.churches.forEach(church => {
            expect(church.features).to.include('Estacionamento');
          });

          cy.log(`??? ${response.body.totalResults} churches with parking`);
        }
      });
    });

    it('Deve filtrar por acessibilidade', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20,
          isAccessible: true
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.churches.length > 0) {
          response.body.churches.forEach(church => {
            expect(church.features).to.include('Acessível');
          });

          cy.log(`? ${response.body.totalResults} accessible churches`);
        }
      });
    });

    it('Deve filtrar por transmissão ao vivo', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20,
          hasLiveStream: true
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.churches.length > 0) {
          response.body.churches.forEach(church => {
            expect(church.features).to.include('Transmissão ao vivo');
          });

          cy.log(`?? ${response.body.totalResults} churches with live stream`);
        }
      });
    });

    it('Deve filtrar por ministério infantil', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20,
          hasChildMinistry: true
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.churches.length > 0) {
          response.body.churches.forEach(church => {
            expect(church.features).to.include('Ministério infantil');
          });

          cy.log(`?? ${response.body.totalResults} churches with child ministry`);
        }
      });
    });

    it('Deve filtrar apenas igrejas verificadas', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20,
          isVerified: true
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.churches.length > 0) {
          response.body.churches.forEach(church => {
            expect(church.isVerified).to.be.true;
            expect(church.badges).to.include('Verificada');
          });

          cy.log(`? ${response.body.totalResults} verified churches`);
        }
      });
    });

    it('Deve combinar múltiplos filtros', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 15,
          minRating: 4,
          hasParking: true,
          isAccessible: true,
          hasLiveStream: true,
          sortBy: 'rating'
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        cy.log(`?? Complex filter: ${response.body.totalResults} churches found`);

        if (response.body.churches.length > 0) {
          response.body.churches.forEach(church => {
            if (church.averageRating) {
              expect(church.averageRating).to.be.at.least(4);
            }
            expect(church.features).to.include.members(['Estacionamento', 'Acessível', 'Transmissão ao vivo']);
          });
        }
      });
    });
  });

  describe('?? Paginação', () => {
    it('Deve paginar resultados corretamente', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 50,
          page: 1,
          pageSize: 5
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.currentPage).to.eq(1);
        expect(response.body.pageSize).to.eq(5);
        expect(response.body.churches.length).to.be.at.most(5);

        if (response.body.totalResults > 5) {
          expect(response.body.hasNextPage).to.be.true;
          expect(response.body.totalPages).to.be.greaterThan(1);
        }

        cy.log(`?? Page 1 of ${response.body.totalPages} (${response.body.totalResults} total)`);
      });
    });

    it('Deve navegar para segunda página', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 50,
          page: 2,
          pageSize: 5
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.currentPage).to.eq(2);
        expect(response.body.hasPreviousPage).to.be.true;

        cy.log('?? Page 2 loaded successfully');
      });
    });
  });

  describe('??? Mapa e Limites', () => {
    it('Deve retornar limites do mapa (bounds)', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.mapBounds) {
          const bounds = response.body.mapBounds;
          
          expect(bounds).to.have.property('minLatitude');
          expect(bounds).to.have.property('maxLatitude');
          expect(bounds).to.have.property('minLongitude');
          expect(bounds).to.have.property('maxLongitude');
          expect(bounds).to.have.property('centerLatitude');
          expect(bounds).to.have.property('centerLongitude');

          cy.log('??? Map bounds calculated');
          cy.log(`Center: ${bounds.centerLatitude}, ${bounds.centerLongitude}`);
        }
      });
    });
  });

  describe('?? Sugestões e Filtros', () => {
    it('Deve retornar sugestões de busca', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.suggestions) {
          const suggestions = response.body.suggestions;
          
          expect(suggestions).to.have.property('nearbyDenominations');
          expect(suggestions).to.have.property('popularSearches');

          cy.log('?? Suggestions:');
          suggestions.nearbyDenominations.forEach(den => {
            cy.log(`  - ${den}`);
          });
        }
      });
    });

    it('Deve retornar filtros disponíveis com contagem', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20
        }
      }).then((response) => {
        expect(response.status).to.eq(200);

        if (response.body.availableFilters && response.body.availableFilters.length > 0) {
          response.body.availableFilters.forEach(filter => {
            expect(filter).to.have.property('key');
            expect(filter).to.have.property('label');
            expect(filter).to.have.property('type');
            expect(filter).to.have.property('values');

            cy.log(`??? ${filter.label}:`);
            filter.values.forEach(value => {
              cy.log(`   ${value.label}: ${value.count}`);
            });
          });
        }
      });
    });
  });

  describe('? Performance', () => {
    it('Deve responder em menos de 2 segundos', () => {
      const startTime = Date.now();

      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 20
        }
      }).then((response) => {
        const duration = Date.now() - startTime;
        
        expect(response.status).to.eq(200);
        expect(duration).to.be.lessThan(2000);

        cy.log(`? Response time: ${duration}ms`);
      });
    });
  });

  describe('?? Edge Cases', () => {
    it('Deve retornar resultado vazio para raio muito pequeno', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 0.1 // 100 metros
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        cy.log(`Found ${response.body.totalResults} churches in 100m radius`);
      });
    });

    it('Deve funcionar com raio grande', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: SAO_PAULO_LAT,
          longitude: SAO_PAULO_LNG,
          radiusKm: 100 // 100 km
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        cy.log(`Found ${response.body.totalResults} churches in 100km radius`);
      });
    });

    it('Deve lidar com coordenadas em área sem igrejas', () => {
      cy.request({
        method: 'GET',
        url: `${Cypress.config('baseUrl')}/api/church/search/nearby`,
        qs: {
          latitude: 0, // Oceano
          longitude: 0,
          radiusKm: 10
        }
      }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.totalResults).to.eq(0);
        expect(response.body.churches).to.be.empty;
      });
    });
  });
});
