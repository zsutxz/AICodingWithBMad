高层概览

- 输入 `/Analyst`（或带前缀的等价写法）由 IDE/CLI 捕获并映射到一个 agent 标识；该 agent 的定义通常存放为 Markdown + YAML（如 `bmad-core/agents/analyst.md`）。

分层调用流程（自底向上）

1) 输入层
- 用户在 IDE 或聊天框内输入命令，形式常见为 `[前缀]<agent> <task> [--options]`，例如 `/Analyst create-competitor-analysis --input brief.md`。
- 前缀字符（如 `/` 或 `@`）由目标 IDE 的配置决定。

2) IDE/CLI 的解析与映射
- 安装器把 agent 注册到 IDE 配置中（例如 `opencode.json` 等），将 agent id 映射到一个 prompt 文件引用（例如 `"prompt": "{file:./.bmad-core/agents/analyst.md}"`）。
- 代码位置（示例）：`BMAD-METHOD/tools/installer/lib/ide-setup.js`。

3) 资源解析与依赖装载
- 找到 agent 文件后，依赖解析器会读取该 agent 的 Markdown 并从中提取 YAML 配置（包含 `commands`、`dependencies` 等），然后加载所需的任务、模板、数据等资源。
- 关键模块：`BMAD-METHOD/tools/lib/dependency-resolver.js`，函数 `resolveAgentDependencies` / `loadResource`。

4) Prompt（提示）拼装
- 对于 web 捆绑，`web-builder.js` 会把 agent 本体和依赖拼装成一个带 START/END 标签的 bundle（见 `BMAD-METHOD/tools/builders/web-builder.js` 中的 `buildAgentBundle` / `generateWebInstructions`）。
- 对于 IDE 集成，IDE 会在发送请求给 LLM 时把 agent 文件内容或引用注入为 prompt 的一部分。

5) 内部命令到 task 的映射
- agent YAML 中定义了 `commands` 列表和 `dependencies`。当用户指定具体 task（如 `create-competitor-analysis`）时，运行时/LLM 会在 agent 的范围内查找对应命令或把相应 task 文件加入 prompt（一致地将依赖注入以便 LLM 使用）。

6) LLM 执行与运行时权限
- 拼装好的 prompt 发送给 LLM，LLM 根据 agent 的角色与指令生成响应。
- 若 IDE/集成配置了工具权限（`tools: { write: true, edit: true, bash: true }` 等），agent/IDE 可基于 LLM 的输出执行写文件或运行命令。安装器会把这些能力写入 IDE 配置（见 `ide-setup.js` 中写入逻辑）。

7) 团队/多代理场景
- 团队捆绑会合并多个 agent 的配置并去重依赖（`resolveTeamDependencies`），用于多角色协作或 orchestrator 场景。
- 关键代码：`BMAD-METHOD/tools/lib/dependency-resolver.js` 中 `resolveTeamDependencies`。

关键文件参考

- 命令与示例：`BMAD_ZH/docs/commands.md`
- agent 定义示例：`BMAD-METHOD/bmad-core/agents/analyst.md` 或 `BMAD_ZH/agents/analyst.md`
- IDE 安装/配置逻辑：`BMAD-METHOD/tools/installer/lib/ide-setup.js`
- 依赖解析器：`BMAD-METHOD/tools/lib/dependency-resolver.js`
- Web bundle / prompt 生成：`BMAD-METHOD/tools/builders/web-builder.js`
- YAML 提取工具：`BMAD-METHOD/tools/lib/yaml-utils.js`

简短总结

- `/Analyst` 的识别首先依赖 IDE 的前缀/映射配置；映射到 agent 后，BMAD 的解析/构建逻辑（或预构建的 bundle）会把 agent + 依赖装入 prompt，随后 LLM 执行并返回结果；IDE/运行时负责将结果展示或基于权限执行副作用（写文件、运行命令）。

后续我可以：
- 直接展示 `dependency-resolver.js` 如何解析并返回资源对象；或
- 演示 `ide-setup.js` 怎样把 agent 写入某个 IDE 配置（例如 `opencode.json`）。



IDE installer config (snippet):

installation-options:
  full:
    name: Complete BMad Core
    description: Copy the entire .bmad-core folder with all agents, templates, and tools
    action: copy-folder
    source: bmad-core
  single-agent:
    name: Single Agent
    description: Select and install a single agent with its dependencies
    action: copy-agent
ide-configurations:
  cursor:
    name: Cursor
    rule-dir: .cursor/rules/bmad/
    format: multi-file
    command-suffix: .mdc
    instructions: |
      # To use BMad agents in Cursor:
      # 1. Press Ctrl+L (Cmd+L on Mac) to open the chat
      # 2. Type @agent-name (e.g., "@dev", "@pm", "@architect")
      # 3. The agent will adopt that persona for the conversation
  claude-code:
    name: Claude Code
    rule-dir: .claude/commands/BMad/
    format: multi-file
    command-suffix: .md
    instructions: |
      # To use BMad agents in Claude Code:
      # 1. Type /agent-name (e.g., "/dev", "/pm", "/architect")
      # 2. Claude will switch to that agent's persona
  iflow-cli:
    name: iFlow CLI
    rule-dir: .iflow/commands/BMad/
    format: multi-file
    command-suffix: .md
    instructions: |
      # To use BMad agents in iFlow CLI:
      # 1. Type /agent-name (e.g., "/dev", "/pm", "/architect")
      # 2. iFlow will switch to that agent's persona
  crush:
    name: Crush
    rule-dir: .crush/commands/BMad/
    format: multi-file
    command-suffix: .md
    instructions: |
      # To use BMad agents in Crush:
      # 1. Press CTRL + P and press TAB
      # 2. Select agent or task
      # 3. Crush will switch to that agent's persona / task
  windsurf:
    name: Windsurf
    rule-dir: .windsurf/workflows/
    format: multi-file
    command-suffix: .md
    instructions: |
      # To use BMad agents in Windsurf:
      # 1. Type /agent-name (e.g., "/dev", "/pm")
      # 2. Windsurf will adopt that agent's persona
  trae:
    name: Trae
    rule-dir: .trae/rules/
    format: multi-file
    command-suffix: .md
    instructions: |
      # To use BMad agents in Trae:
      # 1. Type @agent-name (e.g., "@dev", "@pm", "@architect")
      # 2. Trae will adopt that agent's persona
  roo:
    name: Roo Code
    format: custom-modes
    file: .roomodes
    instructions: |
      # To use BMad agents in Roo Code:
      # 1. Open the mode selector (usually in the status bar)
      # 2. Select any bmad-{agent} mode (e.g., "bmad-dev", "bmad-pm")
      # 3. The AI will adopt that agent's full personality and capabilities
  cline:
    name: Cline
    rule-dir: .clinerules/
    format: multi-file
    command-suffix: .md
    instructions: |
      # To use BMad agents in Cline:
      # 1. Open the Cline chat panel in VS Code
      # 2. Type @agent-name (e.g., "@dev", "@pm", "@architect")
      # 3. The agent will adopt that persona for the conversation
      # 4. Rules are stored in .clinerules/ directory in your project
  gemini:
    name: Gemini CLI
    rule-dir: .gemini/commands/BMad/
    format: multi-file
    command-suffix: .toml
    instructions: |
      # To use BMad agents with the Gemini CLI:
      # 1. The installer creates a `BMad` folder in `.gemini/commands`.
      # 2. This adds custom commands for each agent and task.
      # 3. Type /BMad:agents:<agent-name> (e.g., "/BMad:agents:dev", "/BMad:agents:pm") or /BMad:tasks:<task-name> (e.g., "/BMad:tasks:create-doc").
      # 4. The agent will adopt that persona for the conversation or preform the task.
  github-copilot:
    name: Github Copilot
    rule-dir: .github/chatmodes/
    format: multi-file
    command-suffix: .md
    instructions: |
      # To use BMad agents with Github Copilot:
      # 1. The installer creates a .github/chatmodes/ directory in your project
      # 2. Open the Chat view (`鈱冣寴I` on Mac, `Ctrl+Alt+I` on Windows/Linux) and select **Agent** from the chat mode selector.
      # 3. The agent will adopt that persona for the conversation
      # 4. Requires VS Code 1.101+ with `chat.agent.enabled: true` in settings
      # 5. Agent files are stored in .github/chatmodes/
      # 6. Use `*help` to see available commands and agents
  kilo:
    name: Kilo Code
    format: custom-modes
    file: .kilocodemodes
    instructions: |
      # To use BMAD鈩?agents in Kilo Code:
      # 1. Open the mode selector in VSCode
      # 2. Select a bmad-{agent} mode (e.g. "bmad-dev")
      # 3. The AI adopts that agent's persona and capabilities

  qwen-code:
    name: Qwen Code
    rule-dir: .qwen/bmad-method/
    format: single-file
    command-suffix: .md
    instructions: |
      # To use BMad agents with Qwen Code:
      # 1. The installer creates a .qwen/bmad-method/ directory in your project.
      # 2. It concatenates all agent files into a single QWEN.md file.
      # 3. Simply mention the agent in your prompt (e.g., "As *dev, ...").
      # 4. The Qwen Code CLI will automatically have the context for that agent.

  auggie-cli:
    name: Auggie CLI (Augment Code)
    format: multi-location
    locations:
      user:
        name: User Commands (Global)
        rule-dir: ~/.augment/commands/bmad/
        description: Available across all your projects (user-wide)
      workspace:
        name: Workspace Commands (Project)
        rule-dir: ./.augment/commands/bmad/
        description: Stored in your repository and shared with your team
    command-suffix: .md
    instructions: |
      # To use BMad agents in Auggie CLI (Augment Code):
      # 1. Type /bmad:agent-name (e.g., "/bmad:dev", "/bmad:pm", "/bmad:architect")
      # 2. The agent will adopt that persona for the conversation
      # 3. Commands are available based on your selected location(s)

  codex:
    name: Codex CLI
    format: project-memory
    file: AGENTS.md
    instructions: |
      # To use BMAD agents with Codex CLI:
      # 1. The installer updates/creates AGENTS.md at your project root with BMAD agents and tasks.
      # 2. Run `codex` in your project. Codex automatically reads AGENTS.md as project memory.
      # 3. Mention agents in your prompt (e.g., "As dev, please implement ...") or reference tasks.
      # 4. You can further customize global Codex behavior via ~/.codex/config.toml.

  codex-web:
    name: Codex Web Enabled
    format: project-memory
    file: AGENTS.md
    instructions: |
      # To enable BMAD agents for Codex Web (cloud):
      # 1. The installer updates/creates AGENTS.md and ensures `.bmad-core` is NOT ignored by git.
      # 2. Commit `.bmad-core/` and `AGENTS.md` to your repository.
      # 3. Open the repo in Codex Web and reference agents naturally (e.g., "As dev, ...").
      # 4. Re-run this installer to refresh agent sections when the core changes.

  opencode:
    name: OpenCode CLI
    format: jsonc-config
    file: opencode.jsonc
    instructions: |
      # To use BMAD agents with OpenCode CLI:
      # 1. The installer creates/updates `opencode.jsonc` at your project root.
      # 2. It ensures the BMAD core instructions file is referenced: `./.bmad-core/core-config.yaml`.
      # 3. If an existing `opencode.json` or `opencode.jsonc` is present, it is preserved and only `instructions` are minimally merged.
      # 4. Run `opencode` in this project to use your configured agents and commands.

