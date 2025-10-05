-- ============================================
-- Script: Atualizar para Novo Modelo de Negócio
-- Descrição: Remove planos desnecessários e ajusta para Free + Premium
-- ============================================

-- 1. Desabilita planos antigos (Basic, Enterprise) mas mantém dados históricos
UPDATE postgres.plan 
SET 
    name = name || ' (DEPRECATED)',
    updated = NOW()
WHERE name IN ('Basic', 'Enterprise');

-- 2. Atualiza plano Free (mantém como está)
UPDATE postgres.plan 
SET 
    max_members = 50,
    max_events = 10,
    max_storage_gb = 1,
    branches = 1,
    shows_ads = true,                -- ?? Mostra ads
    can_promote_church = false,      -- ?? Não pode promover
    can_promote_events = false,      -- ?? Não pode promover eventos
    has_advanced_analytics = false,  -- ?? Sem analytics avançados
    has_priority_support = false,    -- ?? Sem suporte prioritário
    has_verified_badge = false,      -- ?? Sem badge verificado
    updated = NOW()
WHERE name = 'Free';

-- 3. Atualiza/Cria plano Premium
INSERT INTO postgres.plan (
    name,
    price,
    max_members,
    max_events,
    max_storage_gb,
    branches,
    shows_ads,
    can_promote_church,
    can_promote_events,
    has_advanced_analytics,
    has_priority_support,
    has_verified_badge,
    created,
    updated
)
VALUES (
    'Premium',                 -- Nome
    79.90,                     -- Preço: R$ 79,90/mês
    999999,                    -- Membros ilimitados
    999999,                    -- Eventos ilimitados
    100,                       -- 100 GB de armazenamento
    10,                        -- Até 10 filiais
    false,                     -- ?? SEM ads internos
    true,                      -- ?? PODE promover igreja
    true,                      -- ?? PODE promover eventos
    true,                      -- ?? Analytics avançados
    true,                      -- ?? Suporte prioritário
    true,                      -- ?? Badge verificado
    NOW(),
    NULL
)
ON CONFLICT DO NOTHING;

-- Se já existe, atualiza
UPDATE postgres.plan 
SET 
    price = 79.90,
    max_members = 999999,
    max_events = 999999,
    max_storage_gb = 100,
    branches = 10,
    shows_ads = false,
    can_promote_church = true,
    can_promote_events = true,
    has_advanced_analytics = true,
    has_priority_support = true,
    has_verified_badge = true,
    updated = NOW()
WHERE name = 'Premium' AND name NOT LIKE '%(DEPRECATED)%';

-- 4. Verificar planos ativos
SELECT 
    id,
    name,
    price,
    max_members,
    max_events,
    max_storage_gb,
    branches,
    shows_ads,
    can_promote_church,
    can_promote_events,
    has_advanced_analytics,
    has_priority_support,
    has_verified_badge
FROM postgres.plan
WHERE name NOT LIKE '%(DEPRECATED)%'
ORDER BY price ASC;

-- ============================================
-- RESULTADO ESPERADO: 
-- 1. Free (R$ 0,00) - Onboarding básico + Com Ads
-- 2. Premium (R$ 79,90) - Full Features + Sem Ads + Promoções
-- ============================================
