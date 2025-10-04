// ***********************************************
// Custom commands for API testing
// ***********************************************

import { faker } from '@faker-js/faker';

// Login commands
Cypress.Commands.add('loginAsAdmin', () => {
  cy.request({
    method: 'POST',
    url: `${Cypress.env('apiUrl')}/auth/login`,
    body: {
      email: Cypress.env('adminEmail'),
      password: Cypress.env('adminPassword')
    }
  }).then((response) => {
    expect(response.status).to.eq(200);
    expect(response.body).to.have.property('token');
    cy.wrap(response.body.token).as('adminToken');
    return response.body.token;
  });
});

Cypress.Commands.add('loginAsMember', () => {
  cy.request({
    method: 'POST',
    url: `${Cypress.env('apiUrl')}/auth/login`,
    body: {
      email: Cypress.env('memberEmail'),
      password: Cypress.env('memberPassword')
    }
  }).then((response) => {
    expect(response.status).to.eq(200);
    expect(response.body).to.have.property('token');
    cy.wrap(response.body.token).as('memberToken');
    return response.body.token;
  });
});

// API request with auth
Cypress.Commands.add('apiRequest', (method, endpoint, token = null, body = null) => {
  const headers = {
    'Content-Type': 'application/json'
  };
  
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const options = {
    method: method,
    url: `${Cypress.env('apiUrl')}${endpoint}`,
    headers: headers,
    failOnStatusCode: false
  };

  if (body) {
    options.body = body;
  }

  return cy.request(options);
});

// Generate fake data
Cypress.Commands.add('generateFakeChurch', () => {
  return {
    name: faker.company.name() + ' Church',
    cnpj: faker.string.numeric(14),
    address: faker.location.streetAddress(),
    city: faker.location.city(),
    state: faker.location.state({ abbreviated: true }),
    postalCode: faker.location.zipCode(),
    phoneNumber: faker.phone.number(),
    email: faker.internet.email(),
    description: faker.lorem.paragraph()
  };
});

Cypress.Commands.add('generateFakeMember', () => {
  return {
    firstName: faker.person.firstName(),
    lastName: faker.person.lastName(),
    email: faker.internet.email(),
    phoneNumber: faker.phone.number(),
    birthDate: faker.date.birthdate({ min: 18, max: 80, mode: 'age' }).toISOString().split('T')[0],
    address: faker.location.streetAddress(),
    city: faker.location.city(),
    state: faker.location.state({ abbreviated: true }),
    postalCode: faker.location.zipCode()
  };
});

Cypress.Commands.add('generateFakeCashFlowEntry', () => {
  return {
    amount: parseFloat(faker.finance.amount({ min: 10, max: 1000, dec: 2 })),
    date: faker.date.recent().toISOString(),
    description: faker.finance.transactionDescription(),
    type: faker.helpers.arrayElement([0, 1]) // 0 = Entrada, 1 = Saída
  };
});

// Cleanup commands
Cypress.Commands.add('cleanupTestData', (token) => {
  // Implementar limpeza de dados de teste se necessário
  cy.log('Cleaning up test data...');
});

// Validation helpers
Cypress.Commands.add('validatePaginatedResponse', (response) => {
  expect(response.body).to.have.property('items');
  expect(response.body).to.have.property('pageNumber');
  expect(response.body).to.have.property('totalPages');
  expect(response.body).to.have.property('totalCount');
  expect(response.body).to.have.property('hasPreviousPage');
  expect(response.body).to.have.property('hasNextPage');
  expect(response.body.items).to.be.an('array');
});

Cypress.Commands.add('validateErrorResponse', (response, expectedStatus) => {
  expect(response.status).to.eq(expectedStatus);
  expect(response.body).to.have.property('statusCode');
  expect(response.body).to.have.property('message');
});

// Wait for API to be ready
Cypress.Commands.add('waitForApi', () => {
  cy.request({
    url: `${Cypress.env('apiUrl')}/health`,
    failOnStatusCode: false,
    retryOnStatusCodeFailure: true,
    timeout: 30000
  }).then((response) => {
    if (response.status !== 200) {
      cy.wait(2000);
      cy.waitForApi();
    }
  });
});
