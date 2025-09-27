# Repository Guidelines

## Project Structure & Module Organization
- `src/` — primary source files. `tests/` — unit/integration tests. `assets/` — static files, examples, and docs.
- Place new features under `src/<feature>/` with a clear module entry (e.g. `src/auth/index.*`).

## Build, Test, and Development Commands
- `npm install` — install dependencies.
- `npm run build` — produce distributable output.
- `npm test` — run project tests.
- `npm start` — run locally (if applicable).

## Coding Style & Naming Conventions
- Use 2-space indentation for JS/TS. Wrap long lines at ~100 chars.
- Files: `kebab-case` for filenames, `camelCase` for variables, `PascalCase` for React components/classes.
- Run `npm run lint` or `npx eslint .` before committing if present.

## Testing Guidelines
- Tests live in `tests/` mirroring `src/` (e.g., `src/foo` → `tests/foo.test.js`).
- Use the repository test runner (e.g., Jest / Mocha). Aim for meaningful unit tests and one integration test per major feature.
- Run `npm test` locally; CI will run the same commands.

## Commit & Pull Request Guidelines
- Follow concise commit messages: `<type>: Short description` (e.g., `fix: handle null response`).
- PRs must include description, linked issue (if any), and screenshots for UI changes. Keep PRs focused and small.

## Security & Configuration Tips
- Do not commit secrets or credentials. Use environment files (`.env`) and update `.gitignore` as needed.
- Sanitize inputs and validate external data at the module boundary.

## Agent Notes
- This repo supports contributor agents: document changes, run tests, and prefer minimal, targeted edits.
