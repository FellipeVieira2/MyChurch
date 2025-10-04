-- ???????????????????????????????????????????????????????????????
--  SCRIPT PARA CRIAR USUÁRIOS DE TESTE PARA CYPRESS
-- ???????????????????????????????????????????????????????????????
-- 
-- IMPORTANTE: Use este script em AMBIENTE DE DESENVOLVIMENTO apenas!
-- Senhas estão em BCrypt (seguro)
--
-- Usuários criados:
--   - admin@test.com / Test@123456 (Admin)
--   - member@test.com / Test@123456 (Membro)
--
-- ???????????????????????????????????????????????????????????????

-- 1. Criar igreja de teste (se não existir)
DO $$
DECLARE
    v_church_id INT;
    v_address_id INT;
BEGIN
    -- Criar endereço (SEM campo complement - não está na migration!)
    INSERT INTO postgres.address (
        street, city, state, zip_code, country, neighborhood, number
    )
    VALUES (
        'Rua Teste', 'São Paulo', 'SP', '01000-000', 'Brasil', 'Centro', '123'
    )
    ON CONFLICT DO NOTHING
    RETURNING id INTO v_address_id;
    
    -- Se já existe, pegar ID
    IF v_address_id IS NULL THEN
        SELECT id INTO v_address_id 
        FROM postgres.address 
        WHERE street = 'Rua Teste' 
        LIMIT 1;
    END IF;

    -- Criar igreja
    INSERT INTO postgres.church (
        name, phone, description, address_id, created, platform_fee,
        latitude, longitude, denomination, is_verified
    )
    VALUES (
        'Igreja Teste Cypress',
        '11999999999',
        'Igreja para testes automatizados',
        v_address_id,
        NOW(),
        0.05,
        -23.5505,
        -46.6333,
        'Batista',
        true
    )
    ON CONFLICT DO NOTHING
    RETURNING id INTO v_church_id;
    
    -- Se já existe, pegar ID
    IF v_church_id IS NULL THEN
        SELECT id INTO v_church_id 
        FROM postgres.church 
        WHERE name = 'Igreja Teste Cypress' 
        LIMIT 1;
    END IF;
    
    RAISE NOTICE 'Igreja ID: %', v_church_id;
    
    -- 2. Criar usuário ADMIN de teste
    INSERT INTO postgres.member (
        name, email, phone, birth_date, church_id, role, 
        password_hash, created, is_active, member_since
    )
    VALUES (
        'Admin Teste Cypress',
        'admin@test.com',
        '11999999999',
        '1990-01-01',
        v_church_id,
        0,  -- Role.Admin = 0
        '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyPgMqL.kP7W',  -- Test@123456
        NOW(),
        true,
        NOW()
    )
    ON CONFLICT (email) DO UPDATE 
    SET 
        password_hash = '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyPgMqL.kP7W',
        church_id = v_church_id,
        role = 0,
        is_active = true;
    
    RAISE NOTICE '? Admin criado: admin@test.com / Test@123456';
    
    -- 3. Criar usuário MEMBRO de teste
    INSERT INTO postgres.member (
        name, email, phone, birth_date, church_id, role, 
        password_hash, created, is_active, member_since
    )
    VALUES (
        'Membro Teste Cypress',
        'member@test.com',
        '11888888888',
        '1995-05-05',
        v_church_id,
        2,  -- Role.Member = 2
        '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyPgMqL.kP7W',  -- Test@123456
        NOW(),
        true,
        NOW()
    )
    ON CONFLICT (email) DO UPDATE 
    SET 
        password_hash = '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyPgMqL.kP7W',
        church_id = v_church_id,
        role = 2,
        is_active = true;
    
    RAISE NOTICE '? Membro criado: member@test.com / Test@123456';
    
END $$;

-- 4. Verificar usuários criados
SELECT 
    m.id,
    m.name,
    m.email,
    m.role,
    c.name as church_name,
    CASE 
        WHEN m.password_hash LIKE '$2a$%' THEN '? BCrypt'
        WHEN m.password_hash LIKE '$2b$%' THEN '? BCrypt'
        WHEN m.password_hash LIKE '$2y$%' THEN '? BCrypt'
        ELSE '? Formato antigo'
    END as password_format
FROM postgres.member m
JOIN postgres.church c ON c.id = m.church_id
WHERE m.email IN ('admin@test.com', 'member@test.com')
ORDER BY m.email;

-- ???????????????????????????????????????????????????????????????
--  RESULTADO ESPERADO:
-- ???????????????????????????????????????????????????????????????
--
--  id  |          name           |      email       | role |      church_name      | password_format
-- -----+-------------------------+------------------+------+-----------------------+-----------------
--   X  | Admin Teste Cypress     | admin@test.com   |    0 | Igreja Teste Cypress  | ? BCrypt
--   Y  | Membro Teste Cypress    | member@test.com  |    2 | Igreja Teste Cypress  | ? BCrypt
--
-- ???????????????????????????????????????????????????????????????

-- 5. Teste de login (opcional - apenas para validar hash)
DO $$
DECLARE
    v_hash TEXT;
BEGIN
    SELECT password_hash INTO v_hash
    FROM postgres.member
    WHERE email = 'admin@test.com';
    
    IF v_hash LIKE '$2a$%' OR v_hash LIKE '$2b$%' OR v_hash LIKE '$2y$%' THEN
        RAISE NOTICE '? Hash BCrypt válido encontrado';
    ELSE
        RAISE WARNING '? Hash não é BCrypt! Formato: %', SUBSTRING(v_hash, 1, 10);
    END IF;
END $$;
