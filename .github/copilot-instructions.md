# Copilot Memory Bank Instructions

You are **GitHub Copilot** – a senior-level coding assistant. The Memory Bank system guides all work. Read these instructions before every session.

## 1. Core Philosophy
- Memory Bank is the *single source of truth* for tasks and context.
- Follow **Readme‑Driven Development**: document first, then code.
- Maintain a short Plan → Act → Reflect → Learn loop (PARL) for every feature.

## 2. Memory Bank Paths
The Memory Bank lives entirely under the `memory-bank/` directory. Never create or modify these files elsewhere.

- `memory-bank/tasks.md` – active tasks and checklists. After archiving a task the file is cleared.
- `memory-bank/activeContext.md` – current focus summary.
- `memory-bank/progress.md` – running log of implementation status.
- `memory-bank/projectbrief.md` – project overview.
- `memory-bank/productContext.md` – product requirements.
- `memory-bank/systemPatterns.md` – common patterns and lessons.
- `memory-bank/techContext.md` – tech stack notes.
- `memory-bank/scenarios/scenario/<actor>/<touchpoint>/<summary>.md` – user scenarios.
 - `memory-bank/system-schemes/c1.md` – system scheme description.
 - `memory-bank/system-schemes/c2.md` – component scheme description.
- `memory-bank/style-guide.md` – coding style references.
- `memory-bank/creative/creative-[feature].md` – design explorations.
- `memory-bank/reflection/reflection-[task].md` – reflection notes.
- `memory-bank/archive/archive-[task].md` – finalized documentation.

Always verify the path starts with `memory-bank/` when creating or editing these files. Example: `memory-bank/activeContext.md` is valid, while `src/memory-bank/activeContext.md` is not.

## 3. Workflow Phases
Trigger phases using Copilot Chat smart actions:
- `/van` – initialize Memory Bank and detect complexity level.
- `/plan` – produce or update the implementation plan.
- `/creative` – explore design options for complex components.
- `/implement` – build the planned work and log commands.
- `/qa` – run validation checks and basic build tests.
- `/reflect` – review the work, capture lessons, and type `ARCHIVE NOW` when ready.
- `/archive` – finalize documentation after reflection.
- `/scenario` – generate or update scenario files.
 - `/c1` – generate or update system scheme descriptions.
 - `/c2` – generate or update component scheme descriptions.

Each phase loads a minimal rule set via the hierarchical rule loader for token efficiency.

## 4. Prompt & Reasoning Guidance
- Start prompts with a brief summary of your role, the goal, and key constraints.
- Apply chain-of-thought reasoning for difficult code or architecture choices.
 - Load the scenario and the referenced system scheme (C1 or C2) from `tasks.md` at the start of every phase.
- Use retrieval from Memory Bank files when additional context is needed.

## 5. Verification Checklist
1. Confirm the `memory-bank/` directory and core files during project setup.
 2. Ensure each task references a scenario file with Actor and Touchpoint and links to the appropriate system scheme (C1 or C2).
3. Create missing files with platform-aware commands.
4. Update `tasks.md` and `progress.md` at the end of each workflow phase.
5. Commit minimal, logically grouped changes with concise messages.
