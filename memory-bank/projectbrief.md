# Project Brief

## Project Overview
**Kafka Microservices Demo** - A comprehensive demonstration of microservices architecture using Apache Kafka as a message broker, built with .NET 9 and Docker.

## Complexity Level
**Level 2-3** (Moderate to Complex)
- Multiple microservices with distributed architecture
- Message-driven communication via Apache Kafka
- Docker containerization and orchestration
- Database persistence with PostgreSQL
- Event-driven patterns and asynchronous processing

## Project Type
- **Type**: Distributed Microservices Architecture Demo
- **Language**: C# (.NET 9)
- **Infrastructure**: Docker, Kafka, PostgreSQL
- **Pattern**: Event-driven microservices with message broker

## Core Architecture Components

### Services
1. **Order Service** (Port 5001)
   - Handles order creation and management
   - Produces `OrderCreatedEvent` to Kafka
   - REST API with Swagger documentation

2. **Inventory Service** (Port 5002) 
   - Consumes `OrderCreatedEvent` from Kafka
   - Manages product inventory and reservations
   - Produces `InventoryReservedEvent`

3. **Notification Service** (Port 5003)
   - Consumes multiple Kafka events
   - Sends notifications to customers
   - Handles order confirmations and inventory updates

### Infrastructure
- **Kafka Broker** (Port 9092) - Message broker for service communication
- **Zookeeper** (Port 2181) - Kafka coordination service  
- **Kafka UI** (Port 8080) - Web interface for monitoring
- **PostgreSQL** (Port 5432) - Database for all services

## Current Project State
- ✅ Complete microservices implementation
- ✅ Docker containerization with docker-compose
- ✅ REST APIs with Swagger documentation
- ✅ Database persistence and migrations
- ✅ Event publishing/consuming patterns
- ⚠️ Kafka consumers currently in simulation mode (not fully connected)
- ✅ Comprehensive demo scripts for testing

## Key Technologies
- **.NET 9** - Application framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM for database access
- **Apache Kafka** - Message broker
- **PostgreSQL** - Relational database
- **Docker & Docker Compose** - Containerization
- **Swagger/OpenAPI** - API documentation

## Project Goals
1. Demonstrate microservices communication patterns
2. Show event-driven architecture benefits
3. Provide working examples of Kafka integration
4. Illustrate containerized deployment strategies
5. Create educational resource for microservices best practices
