-- ============================================
-- Script: Inserir APENAS Plano Gratuito
-- Descrição: Adiciona somente o plano Free (gratuito)
-- ============================================

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
    1,                         -- Apenas 1 igreja (sem filiais)
    NOW(),                     -- Data de criação
    NULL                       -- Data de atualização
);

-- Verificar se foi criado
SELECT * FROM postgres.plan WHERE name = 'Free';
