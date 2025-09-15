# Kafka Microservices Demo

A comprehensive demonstration of microservices architecture using Apache Kafka as a message broker, built with .NET 9 and Docker.

## 🏗️ Architecture Overview

This project demonstrates a real-world microservices architecture with the following components:

```
┌─────────────────┐    ┌──────────────┐    ┌─────────────────────┐
│   Order Service │───▶│    Kafka     │───▶│  Inventory Service  │
│   (Producer)    │    │  (Message    │    │   (Consumer)        │
│                 │    │   Broker)    │    │                     │
└─────────────────┘    │              │    └─────────────────────┘
                       │              │
                       │              │    ┌─────────────────────┐
                       │              │───▶│ Notification Service│
                       │              │    │   (Consumer)        │
                       └──────────────┘    └─────────────────────┘
```

### Services

1. **Order Service** (Port 5001)
   - Handles order creation
   - Produces `OrderCreatedEvent` to Kafka
   - REST API for order management

2. **Inventory Service** (Port 5002)
   - Consumes `OrderCreatedEvent`
   - Manages product inventory
   - Reserves inventory for orders
   - Produces `InventoryReservedEvent`

3. **Notification Service** (Port 5003)
   - Consumes multiple events
   - Sends notifications to customers
   - Handles order confirmations and inventory updates

4. **Kafka Infrastructure**
   - Kafka Broker (Port 9092)
   - Zookeeper (Port 2181)
   - Kafka UI (Port 8080) - Web interface for monitoring

## 🚀 Quick Start

### Prerequisites

- Docker Desktop
- .NET 9 SDK (for local development)
- PowerShell or Bash terminal

### Running with Docker

1. **Clone and navigate to the project:**

   ```powershell
   cd d:\entertainment\kafka
   ```

2. **Start the entire system:**

   ```powershell
   docker-compose up --build
   ```

   This will start:
   - Zookeeper
   - Kafka broker
   - Kafka UI (web interface)
   - All three microservices

3. **Verify services are running:**
   - Kafka UI: <http://localhost:8080>
   - Order Service API: <http://localhost:5001>
   - Inventory Service API: <http://localhost:5002>
   - Notification Service API: <http://localhost:5003>

4. **Access Swagger Documentation:**
   - Order Service: <http://localhost:5001/swagger>
   - Inventory Service: <http://localhost:5002/swagger>
   - Notification Service: <http://localhost:5003/swagger>

### Running Locally (Development)

1. **Start Kafka infrastructure only:**

   ```powershell
   docker-compose up -d zookeeper kafka kafka-ui
   ```

2. **Run services locally:**

   ```powershell
   # Terminal 1 - Order Service
   cd src/KafkaMicroservices.OrderService
   dotnet run

   # Terminal 2 - Inventory Service
   cd src/KafkaMicroservices.InventoryService
   dotnet run

   # Terminal 3 - Notification Service
   cd src/KafkaMicroservices.NotificationService
   dotnet run
   ```

## 📋 Demo Scenarios

### Scenario 1: Create an Order

1. **Create a new order:**

   ```powershell
   $body = @{
       CustomerId = "CUST001"
       Items = @(
           @{
               ProductId = "PROD001"
               ProductName = "Laptop"
               Quantity = 2
               Price = 999.99
           },
           @{
               ProductId = "PROD002"
               ProductName = "Mouse"
               Quantity = 1
               Price = 29.99
           }
       )
   } | ConvertTo-Json -Depth 3

   Invoke-RestMethod -Uri "http://localhost:5001/api/orders" -Method POST -Body $body -ContentType "application/json"
   ```

2. **Check the logs to see the event flow:**
   - Order Service logs: Order created and event published
   - Inventory Service logs: Inventory reservation
   - Notification Service logs: Customer notification sent

### Scenario 2: View Inventory

```powershell
# Get all inventory
Invoke-RestMethod -Uri "http://localhost:5002/api/inventory" -Method GET

# Get specific product inventory
Invoke-RestMethod -Uri "http://localhost:5002/api/inventory/PROD001" -Method GET
```

### Scenario 3: View Notifications

```powershell
# Get notifications for a customer
Invoke-RestMethod -Uri "http://localhost:5003/api/notifications/CUST001" -Method GET
```

## 🔧 Development Guide

### Project Structure

```
d:\entertainment\kafka\
├── src\
│   ├── KafkaMicroservices.Shared\          # Shared models and contracts
│   ├── KafkaMicroservices.OrderService\    # Order management service
│   ├── KafkaMicroservices.InventoryService\ # Inventory management service
│   └── KafkaMicroservices.NotificationService\ # Notification service
├── docker\                                 # Docker configurations
├── docs\                                   # Documentation
├── scripts\                                # Demo and setup scripts
├── docker-compose.yml                      # Docker orchestration
└── README.md                              # This file
```

### Key Technologies

- **.NET 9**: Latest version of .NET
- **ASP.NET Core**: Web API framework  
- **Swagger/OpenAPI**: API documentation and testing
- **Apache Kafka**: Message broker
- **Docker**: Containerization
- **Confluent.Kafka**: .NET Kafka client library

### Message Flow

1. **Order Creation:**

   ```
   HTTP POST → Order Service → OrderCreatedEvent → Kafka Topic: order-created
   ```

2. **Inventory Processing:**

   ```
   Kafka Topic: order-created → Inventory Service → InventoryReservedEvent → Kafka Topic: inventory-reserved
   ```

3. **Notification Processing:**

   ```
   Kafka Topic: order-created → Notification Service → Customer Notification
   Kafka Topic: inventory-reserved → Notification Service → Inventory Update Notification
   ```

## 🐛 Debugging Guide

### Common Issues and Solutions

#### 1. Kafka Connection Issues

**Symptom:** Services can't connect to Kafka

```
Error: Connection to broker failed
```

**Solutions:**

- Ensure Kafka is running: `docker-compose ps`
- Check Kafka health: `docker-compose logs kafka`
- Verify port 9092 is accessible: `netstat -an | findstr 9092`

#### 2. Service Startup Issues

**Symptom:** Service fails to start

```
Error: Unable to bind to port
```

**Solutions:**

- Check if ports are in use: `netstat -an | findstr 5001`
- Kill processes using the ports
- Change ports in `appsettings.json` or `docker-compose.yml`

#### 3. Docker Build Issues

**Symptom:** Docker build fails

```
Error: Unable to restore packages
```

**Solutions:**

- Clear Docker cache: `docker system prune -a`
- Rebuild without cache: `docker-compose build --no-cache`
- Check internet connection for package downloads

### Debugging Tools

#### 1. Kafka UI (Recommended)

- URL: <http://localhost:8080>
- View topics, messages, consumer groups
- Monitor message flow in real-time

#### 2. Docker Logs

```powershell
# View logs for specific service
docker-compose logs order-service
docker-compose logs inventory-service
docker-compose logs notification-service

# Follow logs in real-time
docker-compose logs -f kafka
```

#### 3. Service Health Checks

```powershell
# Check Order Service
Invoke-RestMethod -Uri "http://localhost:5001/api/orders" -Method GET

# Check Inventory Service
Invoke-RestMethod -Uri "http://localhost:5002/api/inventory" -Method GET

# Check Notification Service
Invoke-RestMethod -Uri "http://localhost:5003/api/notifications/test" -Method GET
```

#### 4. Kafka Command Line Tools

```bash
# List topics
docker exec kafka kafka-topics --bootstrap-server localhost:9092 --list

# Consume messages from a topic
docker exec kafka kafka-console-consumer --bootstrap-server localhost:9092 --topic order-created --from-beginning

# Produce test message
docker exec kafka kafka-console-producer --bootstrap-server localhost:9092 --topic order-created
```

### Performance Monitoring

#### 1. Application Metrics

- Monitor service logs for performance indicators
- Use Application Insights or Prometheus for production

#### 2. Kafka Metrics

- Use Kafka UI for basic monitoring
- Monitor consumer lag
- Check message throughput

### Troubleshooting Checklist

✅ **Before Starting:**

- [ ] Docker Desktop is running
- [ ] No other services using ports 5001-5003, 8080, 9092, 2181
- [ ] Sufficient disk space (>2GB for Docker images)

✅ **If Issues Occur:**

- [ ] Check Docker container status: `docker-compose ps`
- [ ] Review service logs: `docker-compose logs [service-name]`
- [ ] Verify Kafka topics exist in Kafka UI
- [ ] Test individual service endpoints
- [ ] Check network connectivity between containers

✅ **For Development:**

- [ ] .NET 8 SDK installed
- [ ] NuGet packages restored
- [ ] Environment variables set correctly
- [ ] Local Kafka instance accessible

## 🔄 Message Schemas

### OrderCreatedEvent

```json
{
  "eventId": "guid",
  "timestamp": "datetime",
  "eventType": "OrderCreatedEvent",
  "order": {
    "id": "guid",
    "customerId": "string",
    "items": [
      {
        "productId": "string",
        "productName": "string",
        "quantity": "number",
        "price": "decimal"
      }
    ],
    "totalAmount": "decimal",
    "createdAt": "datetime",
    "status": "string"
  }
}
```

### InventoryReservedEvent

```json
{
  "eventId": "guid",
  "timestamp": "datetime",
  "eventType": "InventoryReservedEvent",
  "orderId": "guid",
  "reservedItems": [
    {
      "productId": "string",
      "quantityReserved": "number"
    }
  ]
}
```

## 🎯 Next Steps

1. **Add Kafka Packages:** Install `Confluent.Kafka` NuGet package to enable actual Kafka communication
2. **Database Integration:** Add Entity Framework for persistent storage
3. **Authentication:** Implement JWT authentication across services
4. **API Gateway:** Add an API Gateway for unified access
5. **Monitoring:** Integrate Application Insights or ELK stack
6. **Circuit Breaker:** Implement resilience patterns
7. **Event Sourcing:** Add event store for full audit trail

## 📚 Additional Resources

- [Apache Kafka Documentation](https://kafka.apache.org/documentation/)
- [Confluent.Kafka .NET Client](https://docs.confluent.io/kafka-clients/dotnet/current/overview.html)
- [Microservices Patterns](https://microservices.io/patterns/)
- [.NET Microservices Guide](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/)

---

**Happy Coding! 🚀**
