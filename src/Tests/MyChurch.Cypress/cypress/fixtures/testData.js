/// <reference types="cypress" />

/**
 * Fixtures para testes da API MyChurch
 * Use estes dados para testes consistentes
 */

module.exports = {
  validChurch: {
    name: 'Igreja Batista Central',
    cnpj: '12345678901234',
    address: 'Av. Paulista, 1000',
    city: 'São Paulo',
    state: 'SP',
    postalCode: '01310-100',
    phoneNumber: '(11) 3000-0000',
    email: 'contato@igrejabatista.com.br',
    description: 'Igreja evangélica batista localizada no centro de São Paulo'
  },

  validMember: {
    firstName: 'Carlos',
    lastName: 'Oliveira',
    email: 'carlos.oliveira@test.com',
    phoneNumber: '(11) 98888-7777',
    birthDate: '1985-03-20',
    address: 'Rua das Flores, 250',
    city: 'São Paulo',
    state: 'SP',
    postalCode: '04567-890',
    maritalStatus: 'Married',
    gender: 'Male'
  },

  validCashFlowCategory: {
    name: 'Dízimos',
    description: 'Categoria para lançamento de dízimos'
  },

  validCashFlowEntry: {
    amount: 1000.00,
    date: '2025-01-15T10:00:00.000Z',
    description: 'Dízimo do mês de Janeiro',
    type: 0 // Income
  },

  validEvent: {
    title: 'Culto de Domingo',
    description: 'Culto dominical às 19h',
    date: '2025-02-02T19:00:00.000Z',
    finishDate: '2025-02-02T21:00:00.000Z',
    location: 'Templo Principal',
    requiresParticipantList: true,
    eventType: 1, // WorshipService
    worshipTheme: 'O Amor de Deus'
  },

  validJourney: {
    title: 'Jornada do Novo Convertido',
    description: 'Programa de discipulado para novos membros',
    iconUrl: 'https://example.com/icon.png',
    isActive: true,
    isDefault: true,
    stages: [
      {
        title: 'Conhecendo a Bíblia',
        description: 'Introdução às Escrituras Sagradas',
        orderIndex: 1,
        type: 'Reading'
      }
    ]
  },

  validDonation: {
    amount: 150.00,
    donationType: 'Tithe',
    paymentMethod: 'CreditCard',
    description: 'Dízimo de Janeiro 2025'
  },

  bibleReferences: {
    john3_16: {
      versionName: 'ARC',
      bookName: 'João',
      chapterNumber: 3,
      verseNumber: 16
    },
    genesis1_1: {
      versionName: 'ARC',
      bookName: 'Gênesis',
      chapterNumber: 1,
      verseNumber: 1
    },
    psalm23_1: {
      versionName: 'ARC',
      bookName: 'Salmos',
      chapterNumber: 23,
      verseNumber: 1
    }
  }
};
