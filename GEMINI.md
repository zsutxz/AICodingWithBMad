### Project Overview

This repository contains the **BMAD-METHOD™**, a universal AI agent framework designed to guide AI agents like Gemini in performing complex, domain-specific tasks. It provides a structured yet flexible set of prompts, templates, and workflows that ensure predictable, high-quality outcomes.

The project's core is located in `BMAD-METHOD/bmad-core`, which acts as the "brain" containing all definitions for agents, teams, workflows, tasks, and knowledge bases. A suite of Node.js scripts in `BMAD-METHOD/tools` processes these components, most notably by bundling them into single, context-rich text files for use in web-based AI environments.

The system is designed for two primary environments:
1.  **Local IDEs (like Cursor or VS Code):** Users can interact directly with the agent markdown files.
2.  **Web UIs (like Gemini or ChatGPT):** Users upload a pre-built bundle from the `dist` directory, which provides the AI with the full context of an agent team and its capabilities.

### Building and Running

This is a Node.js project. Key commands are managed via npm scripts defined in `BMAD-METHOD/package.json`.

*   **Install Dependencies:**
    ```bash
    npm install
    ```

*   **Validate and Check Code:** The `pre-release` script is the primary command for ensuring code quality. It validates the project structure, checks formatting, and lints the code.
    ```bash
    npm run pre-release
    ```

*   **Build Bundles:** The build script processes the `bmad-core` components and generates bundled `.txt` files in the `dist` directory for use in web UIs.
    ```bash
    npm run build
    ```

*   **Run Tests:** To run the project's tests:
    ```bash
    npm test
    ```

### Development Conventions

The project emphasizes code quality and a structured workflow.

*   **Code Style:** Code formatting is managed by Prettier and linting by ESLint. These are enforced automatically on commit using Husky pre-commit hooks.
*   **Contribution Workflow:** Contributions are made via forks and pull requests. The typical workflow is:
    1.  Fork the repository.
    2.  Create a feature branch.
    3.  Make changes.
    4.  Run `npm run pre-release` to ensure checks pass.
    5.  Commit changes and push to the fork.
    6.  Open a pull request.
*   **Core Development Cycle:** The framework promotes a cyclical development process managed by specialized agents:
    1.  **Planning:** Analyst, PM, and Architect agents collaborate to produce project briefs, PRDs, and architecture documents.
    2.  **Execution:** A Scrum Master (SM) agent drafts detailed stories, and a Developer (Dev) agent implements them sequentially.
    3.  **Quality Assurance:** A QA agent can be involved to provide code review and quality checks.
