**项目概述（针对 PM 命令）**
- 代码组织：主要模块在 `bmad-core/`，公共资源在 `common/`，可选扩展在 `expansion-packs/`，文档在 `docs/`。
- pm 代理定义：位于 `bmad-core/agents/pm.md`（以及中文 `pm_zh.md`），文件内嵌完整 YAML 配置，包含 `activation-instructions`、`commands`、`dependencies` 等。

**pm 命令调用流程（逐步说明）**
1. 激活与初始化
- 触发：用户或系统请求以 pm 身份运行（例如 CLI、IDE 插件或上层 orchestrator）。
- 步骤：加载 `bmad-core/agents/pm.md`，读取文件顶部的 YAML 块。
- 动作：按照 `activation-instructions`：先读取 `.\bmad-core\core-config.yaml`，采用 `agent` 与 `persona`，然后（通常）输出问候并运行 `*help` 列表。

2. 命令解析与映射
- 触发：用户输入以 `*` 开头的命令（例如 `*create-prd`）。
- 步骤：解析 `pm.md` 中的 `commands` 列表，查找与请求匹配的条目。
- 动作：确定要执行的任务（task）和/或模板（template），例如 `create-prd` 映射到 `tasks/create-doc.md` + `templates/prd-tmpl.yaml`。

3. 加载依赖文件
- 触发：解析后需要执行 task/template/checklist。
- 步骤：从 `dependencies` 字段解析对应路径（相对根目录 `bmad-core/`）。
- 动作：只在真实执行时按需加载文件（遵循 YAML 中的 IDE-FILE-RESOLUTION 规则）。

4. 执行任务工作流
- 触发：任务（位于 `bmad-core/tasks/`）被调用。
- 步骤：读取相应的 task 文件（Markdown），按任务内定义的步骤执行。
- 动作：若任务定义 `elicit=true`，则必须与用户进行交互，按指定格式询问并等待回应；否则可以自动执行并生成文档或输出。

5. 输出与持久化
- 触发：任务完成或生成中间产物。
- 步骤：执行 `doc-out` 或任务内定义的存储步骤，将结果写入目标位置（例如当前工作目录或指定输出文件）。
- 动作：若需要，返回操作日志、生成的文档、或提示用户下一步操作。

**错误处理与约束**
- 只按文件内部 YAML 指示加载依赖，避免自动加载其他 agent 文件。
- 严格遵守 `elicit=true` 的交互需求，不可绕过。
- `agent.customization` 优先级高，若存在冲突则以其为准。

**示例：执行 `*create-prd` 的具体流程**
- 用户输入 `*create-prd`。
- 系统从 `pm.md` 中解析 `create-prd -> tasks/create-doc.md + templates/prd-tmpl.yaml`。
- 加载 `bmad-core/tasks/create-doc.md`，读取任务步骤；加载 `bmad-core/templates/prd-tmpl.yaml` 作为填充模板。
- 若任务需要用户提供需求或确认（elicit），则提示用户并等待输入；否则使用默认或上下文信息生成 PRD 文档。
- 生成后执行 `doc-out` 将文档输出到目标文件，并返回完成信息。

**文件参考（仓库位置）**
- `bmad-core/agents/pm.md`（主实现，包含 YAML 配置）
- `bmad-core/agents/pm_zh.md`（中文说明）
- 任务目录：`bmad-core/tasks/*.md`
- 模板目录：`bmad-core/templates/*.yaml`
- 核心配置：`bmad-core/core-config.yaml`

**建议与注意事项**
- 若实现中有 orchestrator/CLI 层负责实际运行 agent，请确保该层在执行任务前遵循 `activation-instructions` 中的初始化顺序。
- 在自动化执行时，谨慎处理需要用户交互的任务（`elicit=true`），为自动化场景提供安全回退或明确的参数传递接口。

