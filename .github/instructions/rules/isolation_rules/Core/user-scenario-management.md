---
description: "Guidelines for managing user scenarios in the Memory Bank"
globs: "user-scenario-management.md"
alwaysApply: false
---
# MEMORY BANK USER SCENARIO MANAGEMENT

## Overview
Memory Bank now tracks **user scenarios**—goal-oriented stories that bind tasks to real user value.

## 📑 Scenario Template
| Field | Description |
|-------|-------------|
| **Actor** | Person or role performing the actions |
| **Touchpoint** | UI or interface they use (VS Code, Web, CLI, API) |
| **Story** | *As a <actor> I want <goal> so that <benefit>* |
| **Steps** | Ordered list of user actions |
| **Acceptance Criteria** | Conditions signalling success |
| **Protocols / Services** | Which services & protocols each step invokes |
| **Notes** | Optional clarifications |

## 🧭 Scenario Lifecycle
```mermaid
graph TD
    Draft["Draft Scenario"] --> Review["Peer Review"]
    Review --> Approve{"Approved?"}
    Approve -->|"Yes"| Implement["Implement Feature"]
    Approve -->|"No"| Revise["Revise Scenario"]
    Implement --> Validate["Validate Against Scenario"]
    Validate --> UpdateMB["Update Memory Bank"]
    Revise --> Draft
````

## 🌐 Interaction Flow

```mermaid
graph LR
    Actor["Actor"] --> Client["Touchpoint"]
    Client --> Service["Service API (HTTPS/WebSocket)"]
    Service --> Memory["Memory Bank Files"]
    Memory --> Service
    Service --> Client
    Client --> Actor
```

## 🗂 Storage Conventions

Store files under `memory-bank/scenarios/scenario/<actor>/<touchpoint>/`.
Example filename: `writer-dashboard-article-submission.md`.
Update `memory-bank/scenarios/scenarios.md` index on every addition.

## 💡 Checklist

* [ ] Scenario documented with all fields
* [ ] YAML front matter passes lint
* [ ] Peer review finished
* [ ] Linked from all relevant tasks
* [ ] Acceptance criteria validated in QA
