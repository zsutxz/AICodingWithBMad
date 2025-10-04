# Repository Guidelines

## Project Structure & Module Organization
- Root: docs, README, LICENSE; main code in `BMAD-METHOD/`.
- `BMAD-METHOD/`: source, CLI tools (`tools/`), core (`bmad-core/`).
- `BMAD_ZH/`: Chinese translations; `docs/`, `agents/`, `expansion-packs/` for auxiliary content.
- Tests live next to code or in `__tests__` folders under their modules.

## Build, Test, and Development Commands
- `cd BMAD-METHOD && npm install` — install deps.
- `cd BMAD-METHOD && npm run build` — build artifacts (uses `tools/cli.js`).
- `cd BMAD-METHOD && npm test` or `npx jest` — run unit tests.
- `npm run lint`, `npm run lint:fix` — run/fix ESLint.
- `npm run format`, `npm run format:check` — run Prettier.

## Coding Style & Naming Conventions
- JavaScript/Node.js with Node >= 20.10.0.
- Indentation: 2 spaces. Use `const`/`let` (no `var`).
- Filenames: `kebab-case` for scripts/YAML; `PascalCase` for major modules.
- Follow ESLint and Prettier configs in `BMAD-METHOD/`.

## Testing Guidelines
- Framework: `jest` (devDependency in `BMAD-METHOD/package.json`).
- Place tests near code or in `__tests__` and name as `*.test.js` or `*.spec.js`.
- Aim for unit coverage on CLI helpers. Run `cd BMAD-METHOD && npm test`.

## Commit & Pull Request Guidelines
- Commit messages: imperative present tense (e.g., `Fix: validate config parsing`).
- PRs: short description, link issue if present, include screenshots for UI changes.
- Run `npm run format` and `npm run lint` before submitting.

## Security & Configuration Tips
- Do not commit secrets; use environment variables or ignored local config files.
- Respect `.gitignore` and avoid checking `node_modules`/local env files.

Quick onboarding: run `cd BMAD-METHOD && npm install && npm run build` to get started.
