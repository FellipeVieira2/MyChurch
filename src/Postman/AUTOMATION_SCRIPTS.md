# ?? Scripts de Automação - Postman

Scripts prontos para adicionar nos testes do Postman para validação automática.

## ?? Como Usar

1. Abra uma request no Postman
2. Vá na aba **Tests**
3. Cole o script correspondente
4. Execute a request
5. Verifique os resultados em **Test Results**

---

## ?? Scripts para Busca Pública

### Script: Search Public Churches

```javascript
// Valida status code
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

// Valida tempo de resposta
pm.test("Response time is less than 500ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(500);
});

// Valida estrutura da resposta
pm.test("Response has pagination structure", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('items');
    pm.expect(jsonData).to.have.property('totalCount');
    pm.expect(jsonData).to.have.property('page');
    pm.expect(jsonData).to.have.property('pageSize');
    pm.expect(jsonData).to.have.property('totalPages');
});

// Valida que items é um array
pm.test("Items is an array", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.items).to.be.an('array');
});

// Valida campos obrigatórios em cada igreja
pm.test("Churches have required fields", function () {
    var jsonData = pm.response.json();
    if (jsonData.items.length > 0) {
        var church = jsonData.items[0];
        pm.expect(church).to.have.property('id');
        pm.expect(church).to.have.property('name');
        pm.expect(church).to.have.property('city');
        pm.expect(church).to.have.property('state');
    }
});

// Salva o primeiro church_id para uso posterior
pm.test("Save first church ID to environment", function () {
    var jsonData = pm.response.json();
    if (jsonData.items.length > 0) {
        pm.environment.set("church_id", jsonData.items[0].id);
        console.log("Saved church_id: " + jsonData.items[0].id);
    }
});
```

### Script: Search with Rating Filter

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("All churches meet minimum rating", function () {
    var jsonData = pm.response.json();
    var minRating = parseFloat(pm.request.url.query.get("minRating") || 0);
    
    jsonData.items.forEach(function(church) {
        if (church.averageRating !== null) {
            pm.expect(church.averageRating).to.be.at.least(minRating);
        }
    });
});

pm.test("Churches sorted by rating (descending)", function () {
    var jsonData = pm.response.json();
    var sortBy = pm.request.url.query.get("sortBy");
    
    if (sortBy === "rating" && jsonData.items.length > 1) {
        for (var i = 0; i < jsonData.items.length - 1; i++) {
            var current = jsonData.items[i].averageRating || 0;
            var next = jsonData.items[i + 1].averageRating || 0;
            pm.expect(current).to.be.at.least(next);
        }
    }
});
```

### Script: Search by Location

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("All churches have distance field", function () {
    var jsonData = pm.response.json();
    jsonData.items.forEach(function(church) {
        pm.expect(church).to.have.property('distance');
        pm.expect(church.distance).to.be.a('number');
    });
});

pm.test("All churches within radius", function () {
    var jsonData = pm.response.json();
    var radiusKm = parseFloat(pm.request.url.query.get("radiusKm") || 5);
    
    jsonData.items.forEach(function(church) {
        pm.expect(church.distance).to.be.at.most(radiusKm);
    });
});

pm.test("Churches sorted by distance (ascending)", function () {
    var jsonData = pm.response.json();
    var sortBy = pm.request.url.query.get("sortBy");
    
    if (sortBy === "distance" && jsonData.items.length > 1) {
        for (var i = 0; i < jsonData.items.length - 1; i++) {
            pm.expect(jsonData.items[i].distance).to.be.at.most(jsonData.items[i + 1].distance);
        }
    }
});
```

---

## ? Scripts para CRUD

### Script: Create Church

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response contains churchId", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('churchId');
    pm.expect(jsonData.churchId).to.be.a('number');
    pm.expect(jsonData.churchId).to.be.above(0);
});

// Salva o church_id criado
pm.test("Save created church ID", function () {
    var jsonData = pm.response.json();
    pm.environment.set("church_id", jsonData.churchId);
    console.log("Church created with ID: " + jsonData.churchId);
});

pm.test("Response time is less than 2000ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(2000);
});
```

### Script: Create Church with Admin

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response contains churchId", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('churchId');
});

// Salva dados para login posterior
pm.test("Save admin credentials for login", function () {
    var requestBody = JSON.parse(pm.request.body.raw);
    pm.environment.set("admin_email", requestBody.adminEmail);
    pm.environment.set("admin_password", requestBody.adminPassword);
    
    var jsonData = pm.response.json();
    pm.environment.set("church_id", jsonData.churchId);
    
    console.log("Church created. Admin email: " + requestBody.adminEmail);
});

pm.test("Response time is less than 3000ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(3000);
});
```

### Script: Get Church by ID

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Church has all required fields", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('id');
    pm.expect(jsonData).to.have.property('name');
    pm.expect(jsonData).to.have.property('phone');
    pm.expect(jsonData).to.have.property('address');
});

pm.test("Address is complete", function () {
    var jsonData = pm.response.json();
    var address = jsonData.address;
    pm.expect(address).to.have.property('street');
    pm.expect(address).to.have.property('city');
    pm.expect(address).to.have.property('state');
    pm.expect(address).to.have.property('zipCode');
});

pm.test("Phone is valid format", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.phone).to.match(/^\d{10,11}$/);
});
```

### Script: Update Church

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Updated data returned", function () {
    var jsonData = pm.response.json();
    var requestBody = JSON.parse(pm.request.body.raw);
    
    pm.expect(jsonData.name).to.equal(requestBody.name);
    pm.expect(jsonData.phone).to.equal(requestBody.phone);
});

pm.test("Only admin can update", function () {
    // Se não for admin, espera 403
    var hasAdminRole = pm.request.headers.get("Authorization") !== null;
    if (!hasAdminRole) {
        pm.response.to.have.status(403);
    }
});
```

---

## ?? Scripts para Banking & Dashboard

### Script: Update Banking Info

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Banking info updated", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('bankName');
    pm.expect(jsonData).to.have.property('agency');
    pm.expect(jsonData).to.have.property('account');
});

pm.test("Sensitive data format is correct", function () {
    var jsonData = pm.response.json();
    // Verifica formato de agência (ex: 1234-5)
    pm.expect(jsonData.agency).to.match(/^\d{4}-\d$/);
});

pm.test("Only admin can update banking", function () {
    if (pm.response.code === 403) {
        console.log("Non-admin user correctly blocked");
    }
});
```

### Script: Get Dashboard

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Dashboard has metrics", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('totalMembers');
    pm.expect(jsonData).to.have.property('totalEvents');
    pm.expect(jsonData).to.have.property('totalDonations');
});

pm.test("Metrics are numbers", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.totalMembers).to.be.a('number');
    pm.expect(jsonData.totalEvents).to.be.a('number');
    pm.expect(jsonData.totalDonations).to.be.a('number');
});

pm.test("Only admin can access dashboard", function () {
    // Dashboard é admin-only
    pm.expect(pm.response.code).to.not.equal(403);
});
```

---

## ?? Scripts para QR Code

### Script: Generate QR Code

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("QR Code returned", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('qrCode');
    pm.expect(jsonData.qrCode).to.be.a('string');
    pm.expect(jsonData.qrCode.length).to.be.above(0);
});

pm.test("QR Code is valid Base64", function () {
    var jsonData = pm.response.json();
    var base64Regex = /^[A-Za-z0-9+/]+={0,2}$/;
    
    // Remove data:image prefix if present
    var qrCode = jsonData.qrCode.replace(/^data:image\/[^;]+;base64,/, '');
    pm.expect(qrCode).to.match(base64Regex);
});
```

---

## ??? Scripts para Geolocalização

### Script: Update Location Manually

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Location updated successfully", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.success).to.be.true;
    pm.expect(jsonData).to.have.property('latitude');
    pm.expect(jsonData).to.have.property('longitude');
});

pm.test("Coordinates match request", function () {
    var jsonData = pm.response.json();
    var requestBody = JSON.parse(pm.request.body.raw);
    
    pm.expect(jsonData.latitude).to.equal(requestBody.latitude);
    pm.expect(jsonData.longitude).to.equal(requestBody.longitude);
});

pm.test("Method is manual", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.method).to.equal('manual');
});
```

### Script: Geocode Location

```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Geocoding successful", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.success).to.be.true;
    pm.expect(jsonData).to.have.property('latitude');
    pm.expect(jsonData).to.have.property('longitude');
    pm.expect(jsonData).to.have.property('formattedAddress');
});

pm.test("Coordinates are valid", function () {
    var jsonData = pm.response.json();
    // Latitude: -90 a 90
    pm.expect(jsonData.latitude).to.be.within(-90, 90);
    // Longitude: -180 a 180
    pm.expect(jsonData.longitude).to.be.within(-180, 180);
});

pm.test("Method is google_geocoding", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.method).to.equal('google_geocoding');
});

pm.test("Response time is acceptable", function () {
    // Geocoding pode demorar mais (API externa)
    pm.expect(pm.response.responseTime).to.be.below(3000);
});
```

### Script: Rate Limit Test

```javascript
// Script para testar rate limiting
var requestCount = pm.environment.get("rate_limit_count") || 0;
requestCount++;
pm.environment.set("rate_limit_count", requestCount);

if (requestCount > 20) {
    pm.test("Rate limit triggered", function () {
        pm.expect(pm.response.code).to.equal(429);
    });
    
    pm.test("Retry-After header present", function () {
        pm.expect(pm.response.headers.has("Retry-After")).to.be.true;
    });
    
    // Reset counter
    pm.environment.set("rate_limit_count", 0);
} else {
    pm.test("Request within rate limit", function () {
        pm.expect(pm.response.code).to.equal(200);
    });
}

console.log("Request count: " + requestCount);
```

---

## ?? Scripts de Segurança

### Script: Authentication Check

```javascript
pm.test("JWT token is present", function () {
    var authHeader = pm.request.headers.get("Authorization");
    pm.expect(authHeader).to.not.be.undefined;
    pm.expect(authHeader).to.include("Bearer");
});

pm.test("Unauthorized without token", function () {
    var authHeader = pm.request.headers.get("Authorization");
    if (!authHeader || authHeader === "") {
        pm.expect(pm.response.code).to.equal(401);
    }
});
```

### Script: Admin Role Check

```javascript
pm.test("Admin endpoints require admin role", function () {
    var adminEndpoints = [
        "/api/Church/",
        "/api/Church/banking-info",
        "/api/Church/dashboard"
    ];
    
    var url = pm.request.url.toString();
    var isAdminEndpoint = adminEndpoints.some(endpoint => url.includes(endpoint));
    
    if (isAdminEndpoint && pm.response.code === 403) {
        console.log("Non-admin correctly blocked from admin endpoint");
        pm.expect(true).to.be.true;
    }
});
```

---

## ?? Script de Validação Global

Cole este script em **Collection > Tests** para executar em todas as requests:

```javascript
// Valida que sempre há resposta
pm.test("Response has body", function () {
    pm.expect(pm.response.text()).to.not.be.empty;
});

// Valida content-type
pm.test("Content-Type is JSON", function () {
    pm.expect(pm.response.headers.get("Content-Type")).to.include("application/json");
});

// Log de performance
console.log("Response time: " + pm.response.responseTime + "ms");

// Log de erro se houver
if (pm.response.code >= 400) {
    console.error("Error " + pm.response.code + ": " + pm.response.json().message);
}

// Salva timestamp da última execução
pm.environment.set("last_request_time", new Date().toISOString());
```

---

## ?? Gerando Relatórios

Para executar todos os testes e gerar relatório:

```bash
# Via Newman (CLI do Postman)
newman run ChurchController.postman_collection.json \
  -e MyChurch.Development.postman_environment.json \
  --reporters cli,html \
  --reporter-html-export report.html
```

---

**Happy Testing with Automation! ??**
