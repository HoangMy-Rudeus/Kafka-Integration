# MemoriPilot: System Architect

## Overview
This file contains the architectural decisions and design patterns for the MemoriPilot project.

## Architectural Decisions

- Implemented microservices with clear domain boundaries (Order, Inventory, Notification)
- Chose Apache Kafka as central message broker for inter-service communication
- Applied database-per-service pattern with PostgreSQL instances
- Used .NET 9 and ASP.NET Core for consistent development experience
- Implemented event sourcing pattern with OrderCreated and InventoryReserved events
- Selected Docker Compose for local development environment orchestration
- Created comprehensive demo scripts for system validation
- Established shared library for common models and interfaces



1. **Decision 1**: Description of the decision and its rationale.
2. **Decision 2**: Description of the decision and its rationale.
3. **Decision 3**: Description of the decision and its rationale.



## Design Considerations

- Event-driven architecture enables loose coupling between services
- Database-per-service pattern ensures service autonomy
- Kafka provides reliable message delivery and event replay capabilities
- Docker containerization ensures consistent deployment environments
- REST APIs provide synchronous access while events handle asynchronous workflows
- Simulated Kafka consumers allow development without full Kafka setup
- PostgreSQL provides ACID transactions within service boundaries
- Swagger documentation ensures API discoverability and testing



## Components

### Order Service

Handles order creation and management

**Responsibilities:**

- Accept order creation requests
- Validate order data
- Persist orders to database
- Publish OrderCreated events to Kafka
- Provide order status and history APIs

### Inventory Service

Manages product inventory and reservations

**Responsibilities:**

- Track inventory levels
- Process inventory reservations
- Consume OrderCreated events
- Publish InventoryReserved events
- Provide inventory status APIs

### Notification Service

Handles customer notifications and communications

**Responsibilities:**

- Consume order and inventory events
- Generate customer notifications
- Store notification history
- Provide notification retrieval APIs
- Support test notification endpoints

### Kafka Message Broker

Central event streaming platform

**Responsibilities:**

- Route events between services
- Provide event persistence and replay
- Handle message ordering and delivery
- Support multiple consumers per topic

### Shared Library

Common components across services

**Responsibilities:**

- Define shared data models
- Provide event schema definitions
- Abstract Kafka producer/consumer interfaces
- Handle message serialization



