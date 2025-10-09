# Project Overview

This project, BMAD-METHOD™, is a Universal AI Agent Framework designed for Agile AI-driven development. It provides a structured environment for creating, managing, and coordinating AI agents to perform complex tasks, particularly in software development.

The core of the framework is the **BMAD Orchestrator**, an AI agent that acts as a central controller. The Orchestrator can transform into various specialized agents, each with its own unique skills and tasks. These agents, defined in Markdown files, collaborate to perform tasks like analysis, architecture design, development, and project management.

The project is a Node.js application and uses a command-line interface for interaction.

## Building and Running

**Installation:**

```bash
npm install
```

**Running Checks:**

```bash
npm run pre-release
```

**Running Tests:**

```bash
npm test
```

**Running the CLI:**

```bash
node tools/cli.js
```

## Development Conventions

*   **Commit Messages:** The project follows the Conventional Commits specification.
*   **Linting:** The project uses ESLint for code linting.
*   **Formatting:** The project uses Prettier for code formatting.
*   **Testing:** The project uses Jest for testing.
*   **Contribution:** Contributions are welcome and should follow the guidelines in `CONTRIBUTING.md`.
