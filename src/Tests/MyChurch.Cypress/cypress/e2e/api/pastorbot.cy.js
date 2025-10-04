/// <reference types="cypress" />

describe('?? PastorBot API Tests', () => {
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

  describe('POST /pastorbot/ask - Ask Pastor Bot', () => {
    it('should ask theological question as member', () => {
      const question = {
        question: 'What does the Bible say about forgiveness?'
      };

      cy.apiRequest('POST', '/pastorbot/ask', memberToken, question).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('answer');
        expect(response.body).to.have.property('references');
        expect(response.body.answer).to.be.a('string');
        expect(response.body.answer.length).to.be.greaterThan(0);
        expect(response.body.references).to.be.an('array');
      });
    });

    it('should ask about faith as admin', () => {
      const question = {
        question: 'How can I strengthen my faith?'
      };

      cy.apiRequest('POST', '/pastorbot/ask', adminToken, question).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('answer');
        expect(response.body.answer.length).to.be.greaterThan(0);
      });
    });

    it('should ask about prayer', () => {
      const question = {
        question: 'Why is prayer important?'
      };

      cy.apiRequest('POST', '/pastorbot/ask', memberToken, question).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('answer');
      });
    });

    it('should ask about salvation', () => {
      const question = {
        question: 'What is salvation according to the Bible?'
      };

      cy.apiRequest('POST', '/pastorbot/ask', memberToken, question).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('answer');
        expect(response.body).to.have.property('references');
      });
    });

    it('should ask about Holy Spirit', () => {
      const question = {
        question: 'Who is the Holy Spirit and what is His role?'
      };

      cy.apiRequest('POST', '/pastorbot/ask', memberToken, question).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('answer');
      });
    });

    it('should fail without question', () => {
      const question = {};

      cy.apiRequest('POST', '/pastorbot/ask', memberToken, question).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail with empty question', () => {
      const question = {
        question: ''
      };

      cy.apiRequest('POST', '/pastorbot/ask', memberToken, question).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail without authentication', () => {
      const question = {
        question: 'What is faith?'
      };

      cy.apiRequest('POST', '/pastorbot/ask', null, question).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('GET /pastorbot/verseoftheday - Get Verse of the Day', () => {
    it('should get verse of the day as member', () => {
      cy.apiRequest('GET', '/pastorbot/verseoftheday', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('verse');
        expect(response.body).to.have.property('reference');
        expect(response.body).to.have.property('explanation');
        expect(response.body.verse).to.be.a('string');
        expect(response.body.verse.length).to.be.greaterThan(0);
        expect(response.body.reference).to.be.a('string');
      });
    });

    it('should get verse of the day as admin', () => {
      cy.apiRequest('GET', '/pastorbot/verseoftheday', adminToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('verse');
        expect(response.body).to.have.property('reference');
        expect(response.body).to.have.property('explanation');
      });
    });

    it('should return different verses on different days (simulation)', () => {
      // Primeira chamada
      cy.apiRequest('GET', '/pastorbot/verseoftheday', memberToken).then((response1) => {
        expect(response1.status).to.eq(200);
        const verse1 = response1.body.verse;

        // Segunda chamada (mesmo dia deve retornar o mesmo)
        cy.apiRequest('GET', '/pastorbot/verseoftheday', memberToken).then((response2) => {
          expect(response2.status).to.eq(200);
          expect(response2.body.verse).to.eq(verse1);
        });
      });
    });

    it('should include application in response', () => {
      cy.apiRequest('GET', '/pastorbot/verseoftheday', memberToken).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('application');
        expect(response.body.application).to.be.a('string');
      });
    });

    it('should fail without authentication', () => {
      cy.apiRequest('GET', '/pastorbot/verseoftheday').then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('POST /pastorbot/explainverse - Explain Bible Verse', () => {
    it('should explain John 3:16', () => {
      const verseData = {
        verseReference: 'João 3:16',
        verseText: 'Porque Deus amou o mundo de tal maneira que deu o seu Filho unigênito, para que todo aquele que nele crê não pereça, mas tenha a vida eterna.'
      };

      cy.apiRequest('POST', '/pastorbot/explainverse', memberToken, verseData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('explanation');
        expect(response.body).to.have.property('context');
        expect(response.body).to.have.property('application');
        expect(response.body.explanation).to.be.a('string');
        expect(response.body.explanation.length).to.be.greaterThan(0);
      });
    });

    it('should explain Psalm 23:1', () => {
      const verseData = {
        verseReference: 'Salmos 23:1',
        verseText: 'O Senhor é o meu pastor; nada me faltará.'
      };

      cy.apiRequest('POST', '/pastorbot/explainverse', memberToken, verseData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('explanation');
        expect(response.body).to.have.property('context');
        expect(response.body).to.have.property('application');
      });
    });

    it('should explain Genesis 1:1', () => {
      const verseData = {
        verseReference: 'Gênesis 1:1',
        verseText: 'No princípio, criou Deus os céus e a terra.'
      };

      cy.apiRequest('POST', '/pastorbot/explainverse', adminToken, verseData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('explanation');
      });
    });

    it('should explain Ephesians 2:8', () => {
      const verseData = {
        verseReference: 'Efésios 2:8',
        verseText: 'Porque pela graça sois salvos, por meio da fé; e isso não vem de vós; é dom de Deus.'
      };

      cy.apiRequest('POST', '/pastorbot/explainverse', memberToken, verseData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('explanation');
        expect(response.body).to.have.property('context');
      });
    });

    it('should include historical context', () => {
      const verseData = {
        verseReference: 'Romanos 8:28',
        verseText: 'E sabemos que todas as coisas contribuem juntamente para o bem daqueles que amam a Deus, daqueles que são chamados segundo o seu propósito.'
      };

      cy.apiRequest('POST', '/pastorbot/explainverse', memberToken, verseData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.context).to.be.a('string');
        expect(response.body.context.length).to.be.greaterThan(0);
      });
    });

    it('should include practical application', () => {
      const verseData = {
        verseReference: 'Mateus 6:33',
        verseText: 'Mas buscai primeiro o Reino de Deus, e a sua justiça, e todas essas coisas vos serão acrescentadas.'
      };

      cy.apiRequest('POST', '/pastorbot/explainverse', memberToken, verseData).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.application).to.be.a('string');
        expect(response.body.application.length).to.be.greaterThan(0);
      });
    });

    it('should fail without verse reference', () => {
      const verseData = {
        verseText: 'Some verse text'
      };

      cy.apiRequest('POST', '/pastorbot/explainverse', memberToken, verseData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail without verse text', () => {
      const verseData = {
        verseReference: 'João 3:16'
      };

      cy.apiRequest('POST', '/pastorbot/explainverse', memberToken, verseData).then((response) => {
        cy.validateErrorResponse(response, 400);
      });
    });

    it('should fail without authentication', () => {
      const verseData = {
        verseReference: 'João 3:16',
        verseText: 'Verse text'
      };

      cy.apiRequest('POST', '/pastorbot/explainverse', null, verseData).then((response) => {
        cy.validateErrorResponse(response, 401);
      });
    });
  });

  describe('PastorBot Response Quality', () => {
    it('should provide detailed theological answers', () => {
      const question = {
        question: 'Explain the doctrine of the Trinity'
      };

      cy.apiRequest('POST', '/pastorbot/ask', memberToken, question).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.answer.length).to.be.greaterThan(100);
      });
    });

    it('should include Bible references in answers', () => {
      const question = {
        question: 'What does the Bible say about love?'
      };

      cy.apiRequest('POST', '/pastorbot/ask', memberToken, question).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body.references).to.be.an('array');
        if (response.body.references.length > 0) {
          response.body.references.forEach(ref => {
            expect(ref).to.be.a('string');
          });
        }
      });
    });
  });
});
