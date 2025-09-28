# AI Coding With Bmad-Method

## 一 项目介绍

## 二 安装

## 三 使用方法

## 四 了解运行机制

## 五 软件架构

## 六 代理调用命令 (IDE内使用)

运行`bmad install`后，您的选定AI开发工具被配置为使用BMAD代理。您使用直接在工具界面（例如聊天提示）中的命令与这些代理交互。

确切的语法（`@`、`/`等）取决于特定的AI工具，但底层结构是一致的。

### 通用语法

调用代理的常见模式是：

`[前缀]<代理名称> <任务名称> [--选项 值]`

- **`[前缀]`**：AI工具用于识别命令的字符，如`@`或`/`。
- **`<代理名称>`**：您想要调用的代理ID（例如，`pm`、`dev`、`architect`）。这些在`.bmad/agents/`目录中定义。
- **`<任务名称>`**：您希望代理执行的特定任务（例如，`create-prd`、`implement-feature`）。这些在`.bmad/tasks/`目录中定义。
- **`[--选项 值]`**：可选参数，如输入文件或输出路径。

### 核心代理命令

#### 1. 项目管理器 (`pm`)

**用途**：用于创建PRD、产品策略、功能优先级排序、路线图规划和利益相关者沟通。

**可用命令**：
- `*help`：显示可用命令列表
- `*correct-course`：执行correct-course任务
- `*create-brownfield-epic`：运行brownfield-create-epic.md任务
- `*create-brownfield-prd`：使用brownfield-prd-tmpl.yaml模板运行create-doc.md任务
- `*create-brownfield-story`：运行brownfield-create-story.md任务
- `*create-epic`：为brownfield项目创建epic（任务brownfield-create-epic）
- `*create-prd`：使用prd-tmpl.yaml模板运行create-doc.md任务
- `*create-story`：根据需求创建用户故事（任务brownfield-create-story）
- `*doc-out`：将完整文档输出到当前目标文件
- `*shard-prd`：对提供的prd.md运行shard-doc.md任务（如果未找到则询问）
- `*yolo`：切换Yolo模式
- `*exit`：退出（确认）

#### 2. 开发者 (`dev`)

**用途**：用于代码实现、调试、重构和开发最佳实践。

**可用命令**：
- `*help`：显示可用命令列表
- `*develop-story`：开发故事
- `*explain`：详细解释所做的工作
- `*review-qa`：运行`apply-qa-fixes.md`任务
- `*run-tests`：执行linting和测试
- `*exit`：作为开发者告别，然后放弃此角色

#### 3. 架构师 (`architect`)

**用途**：用于系统设计、架构文档、技术选择、API设计和基础设施规划。

**可用命令**：
- `*help`：显示可用命令列表
- `*create-backend-architecture`：使用architecture-tmpl.yaml模板创建文档
- `*create-brownfield-architecture`：使用brownfield-architecture-tmpl.yaml模板创建文档
- `*create-front-end-architecture`：使用front-end-architecture-tmpl.yaml模板创建文档
- `*create-full-stack-architecture`：使用fullstack-architecture-tmpl.yaml模板创建文档
- `*doc-out`：将完整文档输出到当前目标文件
- `*document-project`：执行document-project.md任务
- `*execute-checklist {checklist}`：运行execute-checklist任务（默认→architect-checklist）
- `*research {topic}`：执行create-deep-research-prompt任务
- `*shard-prd`：对提供的architecture.md运行shard-doc.md任务（如果未找到则询问）
- `*yolo`：切换Yolo模式
- `*exit`：作为架构师告别，然后放弃此角色

#### 4. 业务分析师 (`analyst`)

**用途**：用于市场研究、头脑风暴、竞争分析、创建项目简报、初始项目发现和记录现有项目（brownfield）。

**可用命令**：
- `*help`：显示可用命令列表
- `*brainstorm {topic}`：促进结构化头脑风暴会议
- `*create-competitor-analysis`：使用competitor-analysis-tmpl.yaml模板创建文档
- `*create-project-brief`：使用project-brief-tmpl.yaml模板创建文档
- `*doc-out`：将完整文档输出到当前目标文件
- `*elicit`：运行advanced-elicitation任务
- `*perform-market-research`：使用market-research-tmpl.yaml模板创建文档
- `*research-prompt {topic}`：执行create-deep-research-prompt.md任务
- `*yolo`：切换Yolo模式
- `*exit`：作为业务分析师告别，然后放弃此角色

#### 5. 测试架构师 (`qa`)

**用途**：用于全面的测试架构审查、质量门决策和代码改进。

**可用命令**：
- `*help`：显示可用命令列表
- `*gate {story}`：执行qa-gate任务以在qa.qaLocation/gates/目录中写入/更新质量门决策
- `*nfr-assess {story}`：执行nfr-assess任务以验证非功能性需求
- `*review {story}`：自适应、风险感知的全面审查
- `*risk-profile {story}`：执行risk-profile任务以生成风险评估矩阵
- `*test-design {story}`：执行test-design任务以创建全面的测试场景
- `*trace {story}`：执行trace-requirements任务以使用Given-When-Then映射需求到测试
- `*exit`：作为测试架构师告别，然后放弃此角色

#### 6. Scrum Master (`sm`)

**用途**：用于故事创建、epic管理、回顾会议和敏捷过程指导。

**可用命令**：
- `*help`：显示可用命令列表
- `*correct-course`：执行correct-course.md任务
- `*draft`：执行create-next-story.md任务
- `*story-checklist`：使用story-draft-checklist.md检查清单执行execute-checklist.md任务
- `*exit`：作为Scrum Master告别，然后放弃此角色

#### 7. 产品负责人 (`po`)

**用途**：用于待办事项管理、故事细化、验收标准、冲刺规划和优先级决策。

**可用命令**：
- `*help`：显示可用命令列表
- `*correct-course`：执行correct-course任务
- `*create-epic`：为brownfield项目创建epic（任务brownfield-create-epic）
- `*create-story`：根据需求创建用户故事（任务brownfield-create-story）
- `*doc-out`：将完整文档输出到当前目标文件
- `*execute-checklist-po`：运行execute-checklist任务（检查清单po-master-checklist）
- `*shard-doc {document} {destination}`：对可选提供的文档运行shard-doc任务到指定目标
- `*validate-story-draft {story}`：对提供的故事文件运行validate-next-story任务
- `*yolo`：切换Yolo模式（开将跳过文档部分确认）
- `*exit`：退出（确认）

#### 8. UX专家 (`ux-expert`)

**用途**：用于UI/UX设计、线框图、原型、前端规范和用户体验优化。

**可用命令**：
- `*help`：显示可用命令列表
- `*create-front-end-spec`：使用front-end-spec-tmpl.yaml模板运行create-doc.md任务
- `*generate-ui-prompt`：运行generate-ai-frontend-prompt.md任务
- `*exit`：作为UX专家告别，然后放弃此角色

#### 9. BMad Master (`bmad-master`)

**用途**：当您需要跨所有领域的全面专业知识、运行不需要角色的1次性任务，或只是想使用同一代理处理多项任务时。

**可用命令**：
- `*help`：显示这些列出的命令
- `*create-doc {template}`：执行create-doc任务（无模板=仅显示依赖项/templates下列出的可用模板）
- `*doc-out`：将完整文档输出到当前目标文件
- `*document-project`：执行document-project.md任务
- `*execute-checklist {checklist}`：运行execute-checklist任务（无检查清单=仅显示依赖项/checklist下列出的可用检查清单）
- `*kb`：切换KB模式关闭（默认）或打开，打开时将加载和引用{root}/data/bmad-kb.md并与用户对话回答问题
- `*shard-doc {document} {destination}`：对可选提供的文档运行shard-doc任务到指定目标
- `*task {task}`：执行任务，如果未找到或未指定，则仅列出下面列出的依赖项/tasks
- `*yolo`：切换Yolo模式
- `*exit`：退出（确认）

#### 10. BMad Orchestrator (`bmad-orchestrator`)

**用途**：用于工作流协调、多代理任务、角色切换指导和不确定咨询哪个专家时。

**可用命令**：
- `*help`：显示此指南和可用代理及工作流
- `*agent`：转换为专门代理（如果未指定名称则列出）
- `*chat-mode`：启动会话模式以获得详细帮助
- `*checklist`：执行检查清单（如果未指定名称则列出）
- `*doc-out`：输出完整文档
- `*kb-mode`：加载完整BMad知识库
- `*party-mode`：与所有代理进行群聊
- `*status`：显示当前上下文、活动代理和进度
- `*task`：运行特定任务（如果未指定名称则列出）
- `*yolo`：切换跳过确认模式
- `*exit`：返回BMad或退出会话

### 在聊天式AI助手中的示例（例如Cursor、Gemini CLI）

这些工具通常使用`@`来提及代理。

**示例1：要求项目管理器（`pm`）创建PRD。**

> `@pm create-prd --input project-brief.md`

- `@pm`：调用**项目管理器**代理。
- `create-prd`：指示其执行**创建PRD**任务。

**示例2：要求开发者（`dev`）实现功能。**

> `@dev implement-feature --story story-123.md --output ./src/new-feature.js`

- `@dev`：调用**开发者**代理。
- `implement-feature`：指示其执行**实现功能**任务。

**示例3：要求架构师（`architect`）设计系统。**

> `@architect design-architecture --requirements prd.md`

- `@architect`：调用**架构师**代理。
- `design-architecture`：指示其执行**设计架构**任务。

### 在专用CLI中的示例（例如OpenCode）

更专业的CLI可能使用结构化命令格式。

**示例1：运行完整工作流。**

> `opencode run workflow:greenfield-fullstack`

**示例2：调用代理执行任务。**

> `opencode agent dev --task create-doc --input requirements.md --template component-spec.yaml`

## 七 你的贡献
