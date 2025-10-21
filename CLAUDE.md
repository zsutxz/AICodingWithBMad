# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

This repository is a documentation and educational wrapper around the BMAD-METHOD™ (Breakthrough Method of Agile AI-driven Development), a universal AI agent framework. The actual framework code lives in the `BMAD-METHOD/` submodule (https://github.com/bmadcode/BMAD-METHOD), while this repository provides Chinese documentation, examples, and usage guides.

## Repository Structure

- `BMAD-METHOD/` - Git submodule containing the core framework
  - `bmad-core/` - Core framework components
    - `agents/` - Agent definitions (analyst.md, pm.md, architect.md, dev.md, qa.md, etc.)
    - `tasks/` - Task definitions (create-doc.md, shard-doc.md, etc.)
    - `templates/` - Document templates (prd-tmpl.yaml, architecture-tmpl.yaml, etc.)
    - `checklists/` - Quality assurance checklists
    - `data/` - Knowledge base files
  - `expansion-packs/` - Domain-specific extensions
  - `tools/` - Build tools, installer, and CLI utilities
  - `dist/` - Built web bundles for agents
- Root level documentation (Chinese):
  - `AICodingWithBMad.md` - Comprehensive guide
  - `BMAD_Commands.md` - Command reference
  - `BMAD_FrameWork.md` - Framework architecture details
  - `AGENTS.md` - Agent listing

## High-Level Architecture

The BMAD-METHOD follows a modular, dependency-driven architecture centered around the BMAD Orchestrator:

1. **BMAD Orchestrator**: The central AI agent that serves as the system's entry point and controller. It can dynamically transform into any specialized agent based on user commands (e.g., `/pm`, `/architect`). The Orchestrator is responsible for parsing user requirements and coordinating the execution of tasks.
2. **Specialized Agents**: AI personas defined in Markdown+YAML files with specific roles, skills, and pre-defined tasks. Examples include Analyst, Project Manager, Architect, Developer, and QA agents, each with distinct capabilities.
3. **Workflows**: Pre-defined sequences of agent interactions and tasks that automate common development processes (e.g., greenfield-fullstack.yaml for new project development).
4. **Tasks**: Specific executable instructions that agents can perform.
5. **Templates**: YAML-based document structures ensuring consistent outputs for PRDs, architecture documents, and other artifacts.
6. **Dependencies**: Each agent declares dependencies (tasks, templates, data) that are dynamically loaded.
7. **Dependency Resolution**: The system (in `tools/lib/dependency-resolver.js`) recursively loads all dependencies and constructs a "super prompt" combining agent persona, task instructions, templates, and context.

**Key Innovation**: When you invoke an agent (e.g., `/pm create-prd`), the system doesn't just send the command to the LLM. Instead, it:
1. Loads the agent definition from `bmad-core/agents/pm.md`
2. Resolves all dependencies (tasks, templates, checklists)
3. Combines them into a context-rich "super prompt"
4. Sends this enriched prompt to the LLM for consistent, high-quality output

## Common Development Commands

### Installation and Setup
```bash
# Interactive installation
npx bmad-method install

# Install with specific options
npx bmad-method install -f -i claude-code

# Update existing installation
npx bmad-method install
```

### Building and Validation
```bash
# Build web bundles for agents and teams
npm run build

# Build only agent bundles
npm run build:agents

# Build only team bundles
npm run build:teams

# Validate agent and team configurations
npm run validate

# List all available agents
npm run list:agents
```

### Code Quality
```bash
# Format code
npm run format

# Lint code
npm run lint

# Fix linting issues
npm run lint:fix

# Run all validation checks
npm run pre-release
```

### Version Management
```bash
# Bump patch version
npm run version:patch

# Bump minor version
npm run version:minor

# Bump major version
npm run version:major
```

## Development Workflow

The framework supports structured development workflows that guide projects from concept to completion:

1. **Planning Phase**: Use Analyst, PM, and Architect agents to create detailed PRDs and Architecture documents. The Analyst agent performs market research, the PM agent creates product requirements, and the Architect agent designs the system architecture.

2. **Development Cycle**: Use the Scrum Master (SM) agent to transform high-level plans into actionable user stories and tasks. The SM agent then coordinates with the Developer agent to implement the features, following agile development practices.

3. **Quality Assurance**: Use the QA agent for comprehensive testing and validation throughout the development process. The QA agent can perform various types of testing and ensure the implementation meets the requirements.

4. **Workflow Automation**: The framework includes pre-defined workflows (in `bmad-core/workflows/`) that automate common development patterns. These workflows can be invoked via CLI commands and coordinate multiple agents to complete complex tasks end-to-end.

## Key Directories

- `bmad-core/`: Core framework components
- `bmad-core/agents/`: Agent definitions
- `bmad-core/agent-teams/`: Agent team configurations
- `bmad-core/workflows/`: Workflow definitions
- `bmad-core/templates/`: Document templates
- `bmad-core/tasks/`: Task definitions
- `bmad-core/checklists/`: Quality checklists
- `bmad-core/data/`: Knowledge base and data files
- `expansion-packs/`: Domain-specific extensions
- `dist/`: Built web bundles
- `tools/`: Build and development tools

## IDE Integration

The framework supports multiple IDEs including Claude Code. When working with Claude Code:

1. Use slash commands for agent interactions
2. Reference agents by their roles (e.g., /pm, /architect, /dev)
3. Follow the prescribed workflows for consistent results
4. Utilize the Test Architect (QA agent) for quality assurance

## Agent Interaction Patterns

In Claude Code, interact with agents using slash commands:
- `/analyst` - Market research and project briefs
- `/pm` - Product requirements documents
- `/architect` - System architecture design
- `/sm` - Scrum Master for story creation
- `/dev` - Developer for implementation
- `/qa` - Quality assurance and testing

Each agent has specific capabilities and should be used for their designated roles in the development workflow.