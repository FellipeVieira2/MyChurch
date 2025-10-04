const { defineConfig } = require('cypress');

module.exports = defineConfig({
  e2e: {
    baseUrl: 'http://localhost:5210',
    supportFile: 'cypress/support/e2e.js',
    specPattern: 'cypress/e2e/**/*.cy.{js,jsx,ts,tsx}',
    video: true,
    screenshotOnRunFailure: true,
    viewportWidth: 1280,
    viewportHeight: 720,
    requestTimeout: 10000,
    responseTimeout: 30000,
    defaultCommandTimeout: 10000,
    
    env: {
      // Credenciais para testes (usar dados de teste, não produção!)
      adminEmail: 'admin@test.com',
      adminPassword: 'Test@123456',
      memberEmail: 'member@test.com',
      memberPassword: 'Test@123456'
    },

    setupNodeEvents(on, config) {
      // Implementar plugins aqui
      require('cypress-mochawesome-reporter/plugin')(on);
      return config;
    },

    retries: {
      runMode: 2,
      openMode: 0
    }
  },

  reporter: 'cypress-mochawesome-reporter',
  reporterOptions: {
    reportDir: 'cypress/reports',
    charts: true,
    reportPageTitle: 'MyChurch API Tests',
    embeddedScreenshots: true,
    inlineAssets: true,
    saveAllAttempts: false
  }
});
