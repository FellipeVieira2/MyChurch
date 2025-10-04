// ***********************************************************
// Support file - Runs before every test file
// ***********************************************************

import './commands';
import 'cypress-mochawesome-reporter/register';

// Global before hook
before(() => {
  cy.log('?? Starting MyChurch API Tests');
  // cy.waitForApi(); // Uncomment if health endpoint is implemented
});

// Global after hook
after(() => {
  cy.log('? MyChurch API Tests Completed');
});

// Handle uncaught exceptions
Cypress.on('uncaught:exception', (err, runnable) => {
  // returning false here prevents Cypress from failing the test
  return false;
});

// Add custom assertions
chai.Assertion.addMethod('validToken', function () {
  const token = this._obj;
  new chai.Assertion(token).to.be.a('string');
  new chai.Assertion(token).to.have.length.greaterThan(20);
  new chai.Assertion(token).to.match(/^[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+$/);
});
