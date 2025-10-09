# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

This repository contains the BMAD-METHOD™ (Breakthrough Method of Agile AI-driven Development), a universal AI agent framework for any domain. The framework provides specialized AI agents for software development, creative writing, business strategy, and more.

## High-Level Architecture

The BMAD-METHOD follows a modular architecture with these core components:

1. **Agents**: Specialized AI personas with defined roles (Analyst, PM, Architect, Scrum Master, Developer, QA, etc.)
2. **Agent Teams**: Collections of agents bundled for specific use cases
3. **Workflows**: Prescribed sequences of agent interactions for complex tasks
4. **Templates**: Reusable document structures with embedded AI instructions
5. **Tasks**: Specific actions that agents can perform
6. **Checklists**: Quality assurance and validation procedures
7. **Data**: Knowledge base and domain-specific information

The system uses a dependency resolution system where each agent declares its dependencies (templates, tasks, etc.) in YAML configuration files.

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

1. **Planning Phase**: Use Analyst, PM, and Architect agents to create detailed PRDs and Architecture documents
2. **Development Cycle**: Use Scrum Master to transform plans into actionable stories, then Developer to implement
3. **Quality Assurance**: Use QA agent for testing and validation throughout the process

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