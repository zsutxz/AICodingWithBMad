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
- agent 定义示例：`BMAD_ZH/agents/analyst.md`
- IDE 安装/配置逻辑：`BMAD-METHOD/tools/installer/lib/ide-setup.js`
- 依赖解析器：`BMAD-METHOD/tools/lib/dependency-resolver.js`
- Web bundle / prompt 生成：`BMAD-METHOD/tools/builders/web-builder.js`
- YAML 提取工具：`BMAD-METHOD/tools/lib/yaml-utils.js`

简短总结

- `/Analyst` 的识别首先依赖 IDE 的前缀/映射配置；映射到 agent 后，BMAD 的解析/构建逻辑（或预构建的 bundle）会把 agent + 依赖装入 prompt，随后 LLM 执行并返回结果；IDE/运行时负责将结果展示或基于权限执行副作用（写文件、运行命令）。

## 将解析过的prompt传给LLM的详细过程

### 1. IDE/CLI捕获命令并映射到代理文件

当用户在IDE或CLI中输入命令（如`/Analyst create-competitor-analysis`）时：
- IDE/CLI首先根据预定义的前缀规则（如`/`或`@`）识别这是一个BMAD命令
- 系统根据命令中的agent名称（如`Analyst`）在配置中查找对应的代理文件路径
- 对于不同的IDE，配置方式不同：
  - 在OpenCode中，配置存储在`opencode.jsonc`文件中，将agent名称映射到`.bmad-core/agents/analyst.md`文件
  - 在Claude Code中，代理文件存储在`.claude/commands/BMad/`目录下
  - 在Cursor中，代理文件存储在`.cursor/rules/bmad/`目录下
- 系统定位到代理文件后，准备加载该代理及其依赖项

### 2. 依赖解析器加载代理及其所有依赖项

依赖解析过程由`DependencyResolver`类处理（位于`BMAD-METHOD/tools/lib/dependency-resolver.js`）：
- 系统读取代理文件（如`analyst.md`）并解析其中的YAML配置部分
- YAML配置中包含`dependencies`字段，定义了该代理需要的所有依赖资源，包括：
  - `tasks`：代理可以执行的任务文件
  - `templates`：文档模板文件
  - `checklists`：检查清单文件
  - `data`：数据文件
  - `utils`：工具文件
- 依赖解析器递归加载所有依赖项：
  - 首先从`bmad-core`目录查找资源
  - 如果未找到，则从`common`目录查找
  - 所有找到的资源都被缓存以避免重复加载
- 系统构建一个完整的依赖树，包含代理本身和所有相关资源

### 3. 系统构建包含角色、任务、模板和上下文的超级提示

超级提示的构建过程是BMAD-METHOD的核心创新之一：
- 系统将代理的角色定义（Persona）作为提示的核心部分，包括：
  - 代理的名称、角色和个性特征
  - 核心原则和行为准则
  - 可用命令列表
- 将用户请求的具体任务指令添加到提示中：
  - 如果用户指定了任务（如`create-competitor-analysis`），系统会加载相应的任务文件
  - 任务文件包含详细的执行步骤和指导原则
- 将相关的模板内容整合到提示中：
  - 根据任务要求加载相应的模板文件
  - 模板定义了输出文档的结构和格式
- 添加上下文信息：
  - 用户提供的输入材料（如项目简介、需求文档等）
  - 前一个代理生成的输出文档
  - 系统配置信息

### 4. 将超级提示发送给LLM进行处理

构建好的超级提示通过以下方式发送给LLM：
- 在IDE集成环境中，系统将整个超级提示作为用户输入发送给LLM
- 在Web环境中，系统使用`web-builder.js`将代理和依赖项打包成带有START/END标签的bundle
- 提示中包含明确的指令，指导LLM如何解析和使用各种资源：
  - 代理角色定义指导LLM如何扮演特定角色
  - 任务指令告诉LLM需要执行什么操作
  - 模板结构确保输出格式的一致性
  - 上下文信息为LLM提供必要的背景知识
- 系统还包含错误处理和重试机制，确保LLM能够成功处理提示

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
  claude-code:
    name: Claude Code
    rule-dir: .claude/commands/BMad/
    format: multi-file
    command-suffix: .md
    instructions: |
      # To use BMad agents in Claude Code:
      # 1. Type /agent-name (e.g., "/dev", "/pm", "/architect")
      # 2. Claude will switch to that agent's persona
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

      
## 六 软件架构

AI Coding With Bmad-Method 遵循模块化架构，包含以下关键组件：

1. **代理(Agents)**：专门的AI角色，每个都有定义的角色（分析师、项目经理、架构师、开发人员、QA等）。

2. **代理团队(Agent Teams)**：为特定用例捆绑的代理集合。

3. **工作流(Workflows)**：预定义的代理交互序列。

4. **模板(Templates)**：具有嵌入式AI指令的可重用文档结构。

5. **任务(Tasks)**：代理可以执行的特定操作。

6. **检查清单(Checklists)**：质量保证程序。

核心架构特点：

- **敏捷规划**：专用代理协作创建详细的PRD和架构文档。
- **上下文工程开发**：Scrum Master将计划转换为详细的故事。
- **两阶段方法**：将规划（不一致）与执行（上下文丢失）分离。
- **Web UI与IDE工作流**：规划可以在Web UI中完成以节省成本，然后切换到IDE进行开发。

该系统使用构建过程将代理定义及其依赖项打包成适用于不同环境（IDE、Web UI）的可分发包。


## 七 你的贡献

我们欢迎社区对 AI Coding With Bmad-Method 项目的贡献！您可以通过以下方式参与：

### 贡献方式

1. **报告问题**：如果您发现bug或有改进建议，请在GitHub上提交issue。

2. **提交代码**：如果您想贡献代码，请：
   - Fork本项目
   - 创建功能分支
   - 提交您的更改
   - 发起Pull Request

3. **改进文档**：帮助我们完善文档，使其更清晰易懂。

4. **添加新代理**：开发新的AI代理以扩展BMAD-METHOD的功能。

5. **创建模板**：为常见任务创建新的模板。

### 开发环境设置

1. 克隆项目仓库
2. 安装必要的依赖
3. 配置AI工具和API密钥
4. 运行测试确保一切正常工作

### 代码规范

- 遵循项目现有的代码风格
- 为新功能添加适当的测试
- 更新相关文档
- 确保所有测试通过后再提交

### 社区准则

- 保持尊重和专业
- 提供有建设性的反馈
- 遵循项目的代码规范
- 保持开放的心态接受反馈

感谢您对本项目的贡献！