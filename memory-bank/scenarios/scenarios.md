# User Scenarios Index

## Overview
This directory contains user scenarios that demonstrate the workflow and logic of the Kafka Microservices Demo application. Each scenario is designed for specific actors using different touchpoints to understand various aspects of the system.

## Available Scenarios

### 1. Order Processing Workflow
- **File**: `scenario/developer/api/order-processing-workflow.md`
- **Actor**: Developer
- **Touchpoint**: REST API
- **System Scheme**: C1
- **Purpose**: Understand the complete end-to-end order processing flow through REST APIs and Kafka events

**Key Learning Points**:
- How to create orders via REST API
- Event-driven communication between services
- Database persistence across service boundaries
- Notification generation from multiple event sources

### 2. System Architecture Understanding  
- **File**: `scenario/architect/documentation/system-architecture-understanding.md`
- **Actor**: Software Architect
- **Touchpoint**: Documentation & Monitoring Tools
- **System Scheme**: C1
- **Purpose**: Deep architectural analysis of system design, patterns, and scalability considerations

**Key Learning Points**:
- Service boundaries and responsibilities
- Event-driven architecture patterns
- Technology choices and trade-offs
- Scalability and resilience strategies

### 3. Learning Microservices Through Demo
- **File**: `scenario/student/demo/learning-microservices-demo.md`
- **Actor**: Student
- **Touchpoint**: Demo Scripts & Interactive Tools
- **System Scheme**: C2
- **Purpose**: Hands-on learning experience for understanding microservices and event-driven architecture

**Key Learning Points**:
- Interactive exploration of microservices
- Understanding asynchronous processing
- Event monitoring and debugging
- Practical skills development

## Scenario Usage Guide

### For Developers
Start with the **Order Processing Workflow** scenario to understand:
- How to interact with the system via APIs
- The complete business flow from order creation to notifications
- Event-driven patterns in practice
- Testing and validation approaches

### For Architects
Focus on the **System Architecture Understanding** scenario to analyze:
- Architectural decisions and trade-offs
- Scalability and performance considerations
- Technology evaluation criteria
- Design pattern implementations

### For Students/Learners
Begin with the **Learning Microservices Demo** scenario for:
- Step-by-step guided exploration
- Hands-on experience with tools
- Conceptual understanding building
- Troubleshooting and experimentation

## Cross-Scenario Learning Path

### Beginner Path
1. **Start**: Student demo scenario for basic understanding
2. **Practice**: Developer API scenario for hands-on experience
3. **Analyze**: Architect documentation scenario for deeper insights

### Advanced Path
1. **Architecture First**: Architect scenario for design understanding
2. **Implementation**: Developer scenario for practical application
3. **Teaching**: Student scenario for knowledge validation

## Integration with Memory Bank

### System Schemes Referenced
- **C1 (System Architecture)**: High-level system design and event flows
- **C2 (Component Architecture)**: Detailed service breakdowns and interactions

### Related Documentation
- **Technical Context**: `memory-bank/techContext.md` - Technology stack details
- **System Patterns**: `memory-bank/systemPatterns.md` - Design pattern implementations
- **Product Context**: `memory-bank/productContext.md` - Business value and use cases

### Demo Scripts Referenced
- `scripts/simple-notification-demo.ps1` - Basic workflow demonstration
- `scripts/complete-notification-demo.ps1` - Comprehensive system showcase
- `scripts/notification-examples.ps1` - Multiple scenario testing

## Extending Scenarios

### Adding New Scenarios
1. Create directory structure: `scenario/<actor>/<touchpoint>/`
2. Follow the scenario template with all required fields
3. Update this index file with the new scenario
4. Link to relevant system schemes (C1/C2)

### Suggested Additional Scenarios
- **DevOps Engineer / Container Management**: Docker orchestration and scaling
- **QA Tester / Testing Tools**: Integration and load testing approaches
- **Product Manager / Business Dashboard**: Business metrics and monitoring
- **Security Analyst / Security Tools**: Security considerations and implementation

## Validation and Updates

### Scenario Validation
- All scenarios should be tested with current system implementation
- Demo scripts should execute successfully
- API endpoints should be accessible and functional
- Documentation links should be current and accurate

### Regular Updates
- Update scenarios when system features change
- Revise acceptance criteria based on implementation updates
- Maintain consistency with Memory Bank documentation
- Ensure demo scripts remain functional

## Usage Tips

### Getting Maximum Value
- Read scenarios in conjunction with system schemes (C1/C2)
- Execute demo scripts while following scenario steps
- Experiment with variations of the described workflows
- Use scenarios as templates for your own system exploration

### Troubleshooting
- Ensure Docker services are running before following scenarios
- Check service health via Swagger UIs before API interactions
- Review container logs if scenarios don't work as expected
- Verify Kafka UI accessibility for event monitoring steps
