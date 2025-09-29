# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This repository contains the BMAD-METHOD™ (Breakthrough Method of Agile AI-driven Development), a framework for AI-assisted development using specialized agents. The project provides a structured approach to software development with dedicated AI agents for different roles (Analyst, PM, Architect, Developer, QA, etc.).

The key innovations of BMAD-METHOD are:
1. **Agentic Planning**: Dedicated agents collaborate to create detailed, consistent PRDs and Architecture documents
2. **Context-Engineered Development**: Scrum Master transforms plans into detailed development stories with full context

## Repository Structure

- `BMAD-METHOD/` - Main framework code
  - `bmad-core/` - Core agent definitions and resources
    - `agents/` - Individual agent definitions (analyst.md, dev.md, pm.md, etc.)
    - `agent-teams/` - Team configurations bundling multiple agents
    - `tasks/` - Reusable task definitions
    - `templates/` - Document templates (PRDs, architecture docs, etc.)
    - `checklists/` - Quality assurance checklists
    - `data/` - Knowledge base and domain-specific information
    - `workflows/` - Prescribed sequences of agent interactions
  - `tools/` - Build tools and utilities
    - `builders/` - Agent and team building scripts
    - `installer/` - Installation utilities
    - `cli.js` - Main command-line interface
  - `dist/` - Built output for web deployment
    - `agents/` - Packaged individual agents
    - `teams/` - Packaged agent teams
  - `docs/` - Documentation files
- `BMAD_ZH/` - Chinese translations of agents and documentation
  - `agents/` - Chinese versions of agent definitions
- `AICodingWithBMad.md` - Main project documentation in Chinese

## Development Commands

- Install dependencies: `npm install`
- Build the project: `npm run build`
- Build agents only: `npm run build:agents`
- Build teams only: `npm run build:teams`
- Validate configurations: `npm run validate`
- Format code: `npm run format`
- Lint code: `npm run lint`
- Run pre-release checks: `npm run pre-release`
- List available agents: `npm run list:agents`
- Run tests: `npm test`
- Fix formatting and linting issues: `npm run fix`

## Architecture Overview

The BMAD-METHOD follows a modular architecture with these key components:

1. **Agents**: Specialized AI personas with defined roles (Analyst, PM, Architect, Dev, QA, etc.)
2. **Agent Teams**: Collections of agents bundled for specific use cases
3. **Workflows**: Prescribed sequences of agent interactions
4. **Templates**: Reusable document structures with embedded AI instructions
5. **Tasks**: Specific actions that agents can perform
6. **Checklists**: Quality assurance procedures

The system uses a build process that packages agent definitions and their dependencies into distributable bundles for different environments (IDE, Web UI).

## Key Development Concepts

- **Agentic Planning**: Dedicated agents collaborate to create detailed PRDs and Architecture documents
- **Context-Engineered Development**: Scrum Master transforms plans into detailed development stories
- **Two-Phase Approach**: Separates planning (inconsistent) from execution (context loss)
- **Web UI vs IDE Workflow**: Planning can be done in web UI for cost efficiency, then switched to IDE for development

## Core Workflow

1. **Planning Phase**: Analyst → PM → Architect (creates PRD and Architecture)
2. **Development Cycle**: Scrum Master → Developer → QA (iterative implementation)
3. **Document Sharding**: PO shards PRD and Architecture into epics/stories
4. **Implementation**: Dev agent implements stories one at a time with full context

## File Conventions

- Agent definitions: YAML frontmatter with markdown content
- Team configurations: YAML files defining agent collections
- Templates: Markdown with embedded processing directives
- Tasks: Markdown files with step-by-step instructions
- Dependencies: Defined in YAML headers of agent files

## Testing

- Run all tests: `npm test`
- Run a single test file: `npx jest path/to/test-file.test.js`
- Run tests matching a pattern: `npx jest -t "pattern"`
- The project uses Jest for testing
- Tests are located alongside the code they test
- Test files have the `.test.js` extension

## Contributing

- Follow the existing code style
- Run `npm run pre-release` before submitting pull requests
- Ensure all tests pass
- Update documentation as needed