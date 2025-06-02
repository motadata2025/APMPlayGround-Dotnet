# 📦 **APMPlayGround-Dotnet**

This project demonstrates a production-ready **.NET 8 / .NET 9** microservice setup using:
✅ DotNetCore.CAP (distributed event system)  
✅ MySQL for data + CAP tables  
✅ RabbitMQ as the message broker  
✅ Runs on both **Windows** & **Linux**  
✅ Optional **Docker Compose** for easy local setup

---

## 🔧 **1️⃣ Prerequisites**

✅ [.NET SDK 8.0 or 9.0](https://dotnet.microsoft.com/download)  
✅ [Docker](https://www.docker.com/) + [Docker Compose](https://docs.docker.com/compose/) installed

---

## 🐳 **2️⃣ Docker Compose Setup**

Here’s the **docker-compose.yml** you can place at your project root:

```yaml
services:
  mysql:
    image: mysql:8.0
    container_name: cap-mysql
    environment:
      MYSQL_ROOT_PASSWORD: root_password
      MYSQL_DATABASE: capdb
      MYSQL_USER: capuser
      MYSQL_PASSWORD: cap_password
    ports:
      - "3306:3306"
    volumes:
      - mysql_data:/var/lib/mysql

  rabbitmq:
    image: rabbitmq:3-management
    container_name: cap-rabbitmq
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: admin
      RABBITMQ_DEFAULT_PASS: admin

volumes:
  mysql_data:
```

### 👉 Start services:
```bash
docker compose up -d
```

✅ MySQL: `localhost:3306`  
✅ RabbitMQ UI: `http://localhost:15672` (admin/admin)

---

## 🐬 **3️⃣ MySQL Database Setup**

Connect to MySQL:
```bash
mysql -h 127.0.0.1 -P 3306 -u capuser -p
# Password: cap_password
```

Run:
```sql
CREATE DATABASE IF NOT EXISTS capdb;

USE capdb;

CREATE TABLE IF NOT EXISTS Orders (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    OrderDate DATETIME NOT NULL,
    CustomerName VARCHAR(255),
    TotalAmount DECIMAL(10,2),
    Status VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS Payments (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    OrderId CHAR(36),
    PaymentStatus VARCHAR(50)
);
```

---

## ⚙ **4️⃣ Configuration Files**

### appsettings.json
```json
{
  "ConnectionStrings": {
    "Default": "server=localhost;port=3306;database=capdb;user=capuser;password=cap_password;"
  },
  "CAP": {
    "RabbitMQ": {
      "HostName": "localhost",
      "UserName": "admin",
      "Password": "admin"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## 🛠 **5️⃣ Build & Run**

### Build (multi-target .NET 8 + .NET 9)
```bash
dotnet build
```

### Run on Windows:
```bash
dotnet run --framework net8.0
```

### Run on Linux:
```bash
dotnet run --framework net8.0
```

---

## 📦 **6️⃣ Publish as Self-contained App**

### For Windows:
```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

### For Linux:
```bash
dotnet publish -c Release -r linux-x64 --self-contained true
```

Result in:
```
bin/Release/net8.0/{runtime}/publish/
```

---

## 🔗 **7️⃣ API Overview**

| Endpoint                       | Description                                     |
|---------------------------------|-------------------------------------------------|
| POST /order/generate?count=10  | Generate N orders + publish events              |
| GET /order/all                 | List all orders + payment details if completed  |
| POST /payment/pay              | Publish payment events                          |
| POST /trace/generate-spans     | Generate trace events for testing               |

Swagger available at:
```
http://localhost:{port}/swagger
```

---

## 📋 **8️⃣ Project Package References**

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFrameworks>net8.0;net9.0</TargetFrameworks>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="DotNetCore.CAP" Version="8.3.5" />
    <PackageReference Include="DotNetCore.CAP.MySql" Version="8.3.5" />
    <PackageReference Include="DotNetCore.CAP.RabbitMQ" Version="8.3.5" />
    <PackageReference Include="Dapper" Version="2.0.123" />
    <PackageReference Include="MySql.Data" Version="8.0.32" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="8.1.2" />
  </ItemGroup>
</Project>
```

---

## ✅ **Checklist**

- [ ] Docker Compose running (MySQL + RabbitMQ)
- [ ] DB + tables initialized
- [ ] App builds + runs on both Windows & Linux
- [ ] API available + testable via Swagger

---

### 🧩 **Bonus: Things You Can Add**
✅ Dockerfile for containerized app  
✅ CI/CD pipeline (GitHub Actions, Azure Pipelines)  
✅ Health checks + monitoring (Prometheus, Grafana)  
✅ Logging/export setups (ELK stack, OpenTelemetry)

---

**Need me to generate any of those? Just ask! 💬✨**