-- ============================================
-- Script: Inserir Plano Gratuito
-- Descrição: Adiciona o plano Free (gratuito) na tabela de planos
-- Data: 2025-01-XX
-- ============================================

-- Inserir plano gratuito
INSERT INTO postgres.plan (
    name,
    price,
    max_members,
    max_events,
    max_storage_gb,
    branches,
    created,
    updated
)
VALUES (
    'Free',                    -- Nome do plano
    0.00,                      -- Preço: R$ 0,00 (gratuito)
    50,                        -- Máximo de 50 membros
    10,                        -- Máximo de 10 eventos por mês
    1,                         -- 1 GB de armazenamento
    1,                         -- Apenas 1 igreja (matriz)
    NOW(),                     -- Data de criação
    NULL                       -- Data de atualização (null inicialmente)
)
ON CONFLICT DO NOTHING;        -- Evita duplicação caso já exista

-- Verificar se o plano foi criado
SELECT 
    id,
    name,
    price,
    max_members,
    max_events,
    max_storage_gb,
    branches,
    created
FROM postgres.plan
WHERE name = 'Free';

-- ============================================
-- OPCIONAL: Adicionar planos pagos para comparação
-- ============================================

-- Plano Basic (Básico)
INSERT INTO postgres.plan (
    name,
    price,
    max_members,
    max_events,
    max_storage_gb,
    branches,
    created,
    updated
)
VALUES (
    'Basic',                   -- Nome do plano
    49.90,                     -- Preço: R$ 49,90/mês
    200,                       -- Máximo de 200 membros
    50,                        -- Máximo de 50 eventos por mês
    10,                        -- 10 GB de armazenamento
    1,                         -- Apenas 1 igreja (matriz)
    NOW(),                     -- Data de criação
    NULL                       -- Data de atualização
)
ON CONFLICT DO NOTHING;

-- Plano Premium
INSERT INTO postgres.plan (
    name,
    price,
    max_members,
    max_events,
    max_storage_gb,
    branches,
    created,
    updated
)
VALUES (
    'Premium',                 -- Nome do plano
    99.90,                     -- Preço: R$ 99,90/mês
    1000,                      -- Máximo de 1000 membros
    200,                       -- Eventos ilimitados (ou número alto)
    50,                        -- 50 GB de armazenamento
    5,                         -- Até 5 filiais
    NOW(),                     -- Data de criação
    NULL                       -- Data de atualização
)
ON CONFLICT DO NOTHING;

-- Plano Enterprise (Empresarial)
INSERT INTO postgres.plan (
    name,
    price,
    max_members,
    max_events,
    max_storage_gb,
    branches,
    created,
    updated
)
VALUES (
    'Enterprise',              -- Nome do plano
    299.90,                    -- Preço: R$ 299,90/mês
    9999,                      -- Membros ilimitados (número muito alto)
    9999,                      -- Eventos ilimitados
    500,                       -- 500 GB de armazenamento
    99,                        -- Filiais ilimitadas (número muito alto)
    NOW(),                     -- Data de criação
    NULL                       -- Data de atualização
)
ON CONFLICT DO NOTHING;

-- ============================================
-- Verificar todos os planos criados
-- ============================================
SELECT 
    id,
    name,
    price,
    max_members,
    max_events,
    max_storage_gb,
    branches,
    created
FROM postgres.plan
ORDER BY price ASC;

-- ============================================
-- Estatísticas dos planos
-- ============================================
SELECT 
    COUNT(*) as total_planos,
    SUM(CASE WHEN price = 0 THEN 1 ELSE 0 END) as planos_gratuitos,
    SUM(CASE WHEN price > 0 THEN 1 ELSE 0 END) as planos_pagos
FROM postgres.plan;
