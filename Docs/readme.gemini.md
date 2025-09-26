# BMAD-METHOD 项目架构与命令调用流程分析

本文档旨在分析 BMAD-METHOD 项目的核心架构，并以项目经理（PM）为例，详细说明其命令和任务的调用流程。

## 1. 核心架构概述

BMAD-METHOD 项目采用了一种高度模块化、由 AI Agent 驱动的软件开发流程。其核心思想是将复杂的软件开发过程分解为由不同角色的 Agent 按顺序执行的、定义明确的任务。整个架构主要由以下几个关键组件构成：

*   **Agents (`bmad-core/agents/*.md`)**:
    这是对 AI Agent 的定义文件。每个文件（如 `pm.md`）都描述了一个特定角色（如项目经理）的**身份、个性、技能和可用命令**。这本质上是指导 LLM 如何行动的、高度结构化的 Persona 或提示。

*   **Tasks (`bmad-core/tasks/*.md`)**:
    这是 Agent 可以执行的具体工作指令。每个文件（如 `brownfield-create-epic.md`）都包含完成一项特定任务的详细步骤、要求和成功标准。它相当于 Agent 的“函数”或“方法”。

*   **Workflows (`bmad-core/workflows/*.yaml`)**:
    这是整个流程的“编排器”。YAML 文件（如 `greenfield-fullstack.yaml`）定义了一个完整的端到端开发流程。它通过 `sequence` 字段指定了一系列步骤，明确了哪个 `agent` 在哪个阶段介入、需要什么输入（`requires`）、产出什么文档（`creates`），以及流程的条件分支。

*   **Templates (`bmad-core/templates/*.yaml`)**:
    这些是任务输出文档的结构化模板。例如，PM 在创建产品需求文档（PRD）时，会使用 `prd-tmpl.yaml` 来确保文档的格式和章节完整性。

*   **CLI (`tools/cli.js`)**:
    从 `package.json` 和 `cli.js` 的内容来看，此命令行工具主要扮演**构建和验证**的角色，例如将 Agents 和 Teams 的定义打包成 Web bundles，或校验配置文件的有效性。实际的 Agent 任务流转似乎由一个更高层的运行时环境（例如特定的 IDE 插件或聊天机器人）来解释和执行，而不是直接通过这个 CLI 工具。

## 2. 命令调用流程 (以 PM 为例)

为了具体说明命令调用流程，我们以 `greenfield-fullstack.yaml` 工作流中 **PM Agent** 的执行环节为例。

整个流程是一个链式反应，前一个 Agent 的输出是后一个 Agent 的输入。

#### **步骤 1: 工作流启动与前置任务**

1.  用户或一个外部协调器启动 `greenfield-fullstack` 工作流。
2.  工作流的第一个 Agent `analyst` 被激活。
3.  `analyst` 执行其任务（例如进行市场研究、头脑风暴），最终产出 `project-brief.md`（项目简报）。

#### **步骤 2: Handoff - 任务交接到 PM**

1.  工作流根据 `sequence` 定义，在 `analyst` 完成后，将任务流转到 `pm` Agent。
2.  工作流明确指出 `pm` 的任务是 `creates: prd.md`（创建 PRD 文档），并且 `requires: project-brief.md`（需要项目简报作为输入）。

#### **步骤 3: PM Agent 激活与任务解析**

1.  系统加载 `pm` 的定义文件 `bmad-core/agents/pm.md`。
2.  从该文件中，系统了解到 PM 的身份是“产品策略师”，其可用命令之一是 `create-prd`。
3.  `pm.md` 的 `commands` 部分进一步指示 `create-prd` 命令需要运行 `create-doc.md` 这个通用任务，并结合使用 `prd-tmpl.yaml` 模板。

#### **步骤 4: Prompt 生成与 LLM 调用**

这是核心环节。系统会将以下所有信息动态地组合成一个完整的、上下文丰富的 Prompt，然后发送给大语言模型（LLM）：

*   **Agent Persona**: 来自 `pm.md` 的角色定义，告诉 LLM 要扮演一个数据驱动、关注用户、善于沟通的 PM。
*   **Task Instructions**: 来自 `common/tasks/create-doc.md` 的具体指令，告诉 LLM 创建文档需要遵循哪些步骤。
*   **Output Template**: 来自 `bmad-core/templates/prd-tmpl.yaml` 的文档结构，告诉 LLM 输出的 PRD 必须包含哪些章节和格式。
*   **Input Context**: 来自上一步 `analyst` 产出的 `project-brief.md` 的全部内容，作为撰写 PRD 的核心依据和素材。

#### **步骤 5: 产出与流程推进**

1.  LLM 根据这个精心构建的 Prompt，生成一份完整的 `prd.md` 文档。
2.  系统保存这份文档。
3.  工作流完成此步骤，并将 `prd.md` 作为下一个 Agent (`ux-expert`) 的输入，继续推进整个开发流程。

## 3. 总结

BMAD-METHOD 的调用流程是一个高度自动化和结构化的链式调用系统：

**用户/协调器 → 启动 Workflow → 按顺序激活 Agent → Agent 加载 Persona、Task 和 Template → 结合上下文生成 Prompt → LLM 执行并生成结果 → 结果成为下一个 Agent 的输入**

这种架构的优势在于将复杂的软件工程流程标准化、模块化，通过 AI Agent 的协同工作，确保了每个环节的专业性和最终产出物的一致性与高质量。
