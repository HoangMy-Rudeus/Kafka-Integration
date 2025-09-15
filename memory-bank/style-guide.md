# Style Guide

## Documentation Standards

### Markdown Formatting
- **Headers**: Always include blank lines before and after headers
- **Lists**: Surround all lists with blank lines
- **Code Blocks**: Use proper language identifiers
- **Links**: Use descriptive link text, avoid bare URLs
- **Line Endings**: End files with single newline

### Code Style (.NET/C#)
- **Naming**: PascalCase for classes, camelCase for variables
- **Async/Await**: Use async/await pattern consistently
- **Dependency Injection**: Constructor injection preferred
- **Error Handling**: Use try-catch with specific exception handling
- **Logging**: Structured logging with appropriate levels

### API Design
- **REST Endpoints**: Use noun-based URLs with HTTP verbs
- **Response Format**: Consistent JSON structure
- **Status Codes**: Appropriate HTTP status codes
- **Documentation**: Comprehensive Swagger/OpenAPI specs
- **Versioning**: Plan for API versioning strategy

### Service Architecture
- **Service Boundaries**: Clear domain-driven boundaries
- **Event Naming**: Past tense (OrderCreated, InventoryReserved)
- **Data Ownership**: Single service owns each data entity
- **Communication**: Async messaging preferred over sync calls
- **Configuration**: Environment-specific settings externalized

### Docker & Infrastructure
- **Container Images**: Multi-stage builds for optimization
- **Environment Variables**: Use for configuration
- **Health Checks**: Implement proper health endpoints
- **Logging**: Container-friendly logging patterns
- **Security**: Non-root user execution

### Testing Strategy
- **Unit Tests**: Test business logic and service methods
- **Integration Tests**: Test service-to-service communication
- **End-to-End Tests**: Full workflow validation
- **Load Testing**: Performance under realistic conditions
- **Contract Testing**: API contract validation

### Git & Version Control
- **Commit Messages**: Clear, descriptive commit messages
- **Branch Strategy**: Feature branches with descriptive names
- **Pull Requests**: Include description and testing notes
- **Tags**: Semantic versioning for releases
- **Documentation**: Keep README.md updated

### Monitoring & Observability
- **Logging Levels**: DEBUG, INFO, WARN, ERROR appropriately
- **Correlation IDs**: Track requests across services
- **Metrics**: Business and technical metrics
- **Health Checks**: Service status and dependencies
- **Alerting**: Appropriate thresholds and notifications
