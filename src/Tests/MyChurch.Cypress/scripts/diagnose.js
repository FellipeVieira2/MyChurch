const http = require('http');

console.log('?? Iniciando diagnóstico do ambiente de testes...\n');

// 1. Testar conexão com API
function testAPI() {
  return new Promise((resolve) => {
    console.log('1?? Testando conexão com API...');
    
    const options = {
      hostname: 'localhost',
      port: 5210,
      path: '/swagger/index.html',
      method: 'GET',
      timeout: 5000
    };

    const req = http.request(options, (res) => {
      if (res.statusCode === 200) {
        console.log('   ? API está respondendo (http://localhost:5210)');
        resolve(true);
      } else {
        console.log(`   ? API retornou status: ${res.statusCode}`);
        resolve(false);
      }
    });

    req.on('timeout', () => {
      console.log('   ? Timeout ao conectar na API (5s)');
      console.log('   ?? Certifique-se de que a API está rodando:');
      console.log('      dotnet run --project Web/MyChurch.Api.Web/MyChurch.Api.Web.csproj');
      resolve(false);
    });

    req.on('error', (error) => {
      console.log('   ? Erro ao conectar na API:', error.message);
      console.log('   ?? A API está rodando em http://localhost:5210?');
      resolve(false);
    });

    req.end();
  });
}

// 2. Testar endpoint de health (se existir)
async function testHealth() {
  return new Promise((resolve) => {
    console.log('\n2?? Testando endpoint de health...');
    
    const options = {
      hostname: 'localhost',
      port: 5210,
      path: '/health',
      method: 'GET',
      timeout: 5000
    };

    const req = http.request(options, (res) => {
      if (res.statusCode === 200) {
        console.log('   ? Health check OK');
        resolve(true);
      } else if (res.statusCode === 404) {
        console.log('   ??  Health endpoint não configurado (opcional)');
        resolve(true);
      } else {
        console.log(`   ? Health check falhou (status: ${res.statusCode})`);
        resolve(false);
      }
    });

    req.on('timeout', () => {
      console.log('   ? Timeout no health check');
      resolve(false);
    });

    req.on('error', () => {
      console.log('   ??  Health endpoint não disponível (opcional)');
      resolve(true);
    });

    req.end();
  });
}

// 3. Testar endpoint de login
async function testLogin() {
  return new Promise((resolve) => {
    console.log('\n3?? Testando endpoint de login...');
    
    const postData = JSON.stringify({
      identifier: 'admin@test.com',
      password: 'Test@123456'
    });

    const options = {
      hostname: 'localhost',
      port: 5210,
      path: '/api/auth/login',
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Content-Length': Buffer.byteLength(postData)
      },
      timeout: 10000
    };

    const req = http.request(options, (res) => {
      let data = '';
      
      res.on('data', (chunk) => {
        data += chunk;
      });

      res.on('end', () => {
        if (res.statusCode === 200) {
          console.log('   ? Login funcionando');
          try {
            const response = JSON.parse(data);
            if (response.token) {
              console.log('   ? Token JWT gerado com sucesso');
            }
            if (response.member) {
              console.log(`   ? Usuário: ${response.member.name} (ID: ${response.member.id})`);
            }
          } catch (e) {
            console.log('   ??  Resposta OK mas não é JSON válido');
          }
          resolve(true);
        } else {
          console.log(`   ? Login falhou (status: ${res.statusCode})`);
          console.log('   ?? Resposta:', data.substring(0, 200));
          console.log('\n   ?? Possíveis causas:');
          console.log('      - Usuário admin@test.com não existe no banco');
          console.log('      - Senha incorreta');
          console.log('      - Migrations não executadas');
          console.log('\n   ?? Solução:');
          console.log('      1. Execute: dotnet ef database update --project Infrastructure/MyChurch.Infrastructure');
          console.log('      2. Crie usuário admin no banco (ver TROUBLESHOOTING.md)');
          resolve(false);
        }
      });
    });

    req.on('timeout', () => {
      console.log('   ? Timeout no endpoint de login (10s)');
      console.log('   ?? O banco de dados está acessível?');
      resolve(false);
    });

    req.on('error', (error) => {
      console.log('   ? Erro ao testar login:', error.message);
      resolve(false);
    });

    req.write(postData);
    req.end();
  });
}

// 4. Testar endpoint de busca de igrejas
async function testChurchSearch() {
  return new Promise((resolve) => {
    console.log('\n4?? Testando endpoint de busca de igrejas...');
    
    const options = {
      hostname: 'localhost',
      port: 5210,
      path: '/api/church/search/nearby?latitude=-23.5505&longitude=-46.6333&radiusKm=10',
      method: 'GET',
      timeout: 10000
    };

    const req = http.request(options, (res) => {
      let data = '';
      
      res.on('data', (chunk) => {
        data += chunk;
      });

      res.on('end', () => {
        if (res.statusCode === 200) {
          console.log('   ? Endpoint de busca funcionando');
          try {
            const response = JSON.parse(data);
            console.log(`   ?? ${response.totalResults || 0} igrejas encontradas`);
            if (response.churches && response.churches.length > 0) {
              console.log(`   ???  Primeira igreja: ${response.churches[0].name}`);
            } else {
              console.log('   ??  Nenhuma igreja cadastrada (normal em ambiente novo)');
            }
          } catch (e) {
            console.log('   ??  Resposta OK mas não é JSON válido');
          }
          resolve(true);
        } else if (res.statusCode === 404) {
          console.log('   ? Endpoint não encontrado');
          console.log('   ?? ChurchSearchController não está registrado?');
          resolve(false);
        } else {
          console.log(`   ? Busca falhou (status: ${res.statusCode})`);
          console.log('   ?? Resposta:', data.substring(0, 200));
          resolve(false);
        }
      });
    });

    req.on('timeout', () => {
      console.log('   ? Timeout no endpoint de busca (10s)');
      resolve(false);
    });

    req.on('error', (error) => {
      console.log('   ? Erro ao testar busca:', error.message);
      resolve(false);
    });

    req.end();
  });
}

// Executar diagnóstico completo
async function runDiagnosis() {
  console.log('?'.repeat(60));
  console.log('  DIAGNÓSTICO DO AMBIENTE DE TESTES CYPRESS');
  console.log('?'.repeat(60) + '\n');

  const apiOk = await testAPI();
  
  if (!apiOk) {
    console.log('\n? Diagnóstico interrompido: API não está acessível\n');
    console.log('?? PRÓXIMOS PASSOS:');
    console.log('   1. Inicie a API:');
    console.log('      cd Web/MyChurch.Api.Web');
    console.log('      dotnet run');
    console.log('   2. Execute este diagnóstico novamente');
    console.log('\n');
    process.exit(1);
  }

  await testHealth();
  const loginOk = await testLogin();
  await testChurchSearch();

  console.log('\n' + '?'.repeat(60));
  console.log('  RESUMO DO DIAGNÓSTICO');
  console.log('?'.repeat(60) + '\n');

  if (loginOk) {
    console.log('? Ambiente pronto para testes!');
    console.log('\n?? Execute os testes:');
    console.log('   npx cypress open');
    console.log('   ou');
    console.log('   npx cypress run');
  } else {
    console.log('? Ambiente com problemas');
    console.log('\n?? Consulte o arquivo TROUBLESHOOTING.md para mais detalhes');
  }

  console.log('\n');
}

runDiagnosis();
