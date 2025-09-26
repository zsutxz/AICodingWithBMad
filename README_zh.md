# BMAD-METHOD™: 通用AI代理框架

[![Version](https://img.shields.io/npm/v/bmad-method?color=blue&label=version)](https://www.npmjs.com/package/bmad-method)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Node.js Version](https://img.shields.io/badge/node-%3E%3D20.0.0-brightgreen)](https://nodejs.org)
[![Discord](https://img.shields.io/badge/Discord-Join%20Community-7289da?logo=discord&logoColor=white)](https://discord.gg/gk8jAdXWmj)

基于代理驱动的敏捷开发基础，被称为敏捷AI驱动开发的突破方法，但远不止于此。通过专业化的AI专业知识转换任何领域：软件开发、娱乐、创意写作、商业策略到个人健康等。

**[在YouTube上订阅BMadCode](https://www.youtube.com/@BMadCode?sub_confirmation=1)**

**[加入我们的Discord社区](https://discord.gg/gk8jAdXWmj)** - 一个不断成长的AI爱好者社区！获得帮助、分享想法、探索AI代理和框架、协作技术项目、享受爱好并互相帮助成功。无论您是在BMad上遇到困难、构建自己的代理，还是只想聊聊最新的AI - 我们都在这里为您服务！**某些移动设备和VPN在加入discord时可能有问题，这是discord的问题 - 如果邀请链接不起作用，请尝试使用您自己的网络或其他网络，或非VPN。**

⭐ **如果您觉得这个项目有帮助或有用，请在右上角给它一个星标！** 这有助于其他人发现BMAD-METHOD™，您也会收到更新通知！

## 概述

**BMAD-METHOD™的两个关键创新：**

**1. 代理规划：** 专用代理（分析师、产品经理、架构师）与您协作创建详细、一致的PRD和架构文档。通过高级提示工程和人在回路优化，这些规划代理产生的综合规范远超通用AI任务生成。

**2. 上下文工程化开发：** Scrum Master代理然后将这些详细计划转换为超详细的开发故事，其中包含开发代理所需的一切 - 完整的上下文、实现细节和直接嵌入到故事文件中的架构指导。

这种两阶段方法消除了AI辅助开发中的**规划不一致**和**上下文丢失**这两大问题。您的开发代理打开一个故事文件，完全理解要构建什么、如何构建以及为什么构建。

**📖 [在用户指南中查看完整工作流程](docs/user-guide.md)** - 规划阶段、开发周期和所有代理角色

### 技术架构

BMAD-METHOD™建立在模块化架构上，将关注点分离为不同的组件：

- **代理**：具有定义角色和功能的专业化AI角色
- **代理团队**：为特定用例捆绑的代理集合
- **工作流**：复杂任务的预定义代理交互序列
- **模板**：嵌入AI指令的可重用文档结构
- **任务**：代理可以执行的特定操作
- **检查清单**：质量保证和验证程序
- **数据**：知识库和领域特定信息

## 快速导航

### 理解BMad工作流

**在深入之前，请查看这些解释BMad工作原理的关键工作流图：**

1. **[规划工作流（Web UI）](docs/user-guide.md#the-planning-workflow-web-ui)** - 如何创建PRD和架构文档
2. **[核心开发周期（IDE）](docs/user-guide.md#the-core-development-cycle-ide)** - SM、Dev和QA代理如何通过故事文件协作

> ⚠️ **这些图解释了90%的BMad方法代理敏捷流困惑** - 理解PRD+架构创建以及SM/Dev/QA工作流以及代理如何通过故事文件传递笔记是至关重要的 - 这也解释了为什么这不是任务管理器或简单的任务运行器！

### 您想要做什么？

- **[使用全栈敏捷AI团队安装和构建软件](#快速开始)** → 快速开始说明
- **[学习如何使用BMad](docs/user-guide.md)** → 完整的用户指南和演练
- **[查看可用的AI代理](/bmad-core/agents)** → 团队的专业角色
- **[探索非技术用途](#-超越软件开发---扩展包)** → 创意写作、商业、健康、教育
- **[创建我自己的AI代理](docs/expansion-packs.md)** → 为您的领域构建代理
- **[浏览现成的扩展包](expansion-packs/)** → 游戏开发、DevOps、基础设施以及获得灵感的想法和示例
- **[理解架构](docs/core-architecture.md)** → 技术深入探讨
- **[加入社区](https://discord.gg/gk8jAdXWmj)** → 获得帮助和分享想法

## 核心组件

### 代理

BMAD-METHOD™配备了一套全面的专业化代理：

- **分析师**：市场研究、竞争对手分析和项目简报创建
- **产品经理（PM）**：创建详细的产品需求文档（PRD）
- **架构师**：设计系统架构和技术规范
- **UX专家**：设计用户界面和体验流程
- **Scrum Master（SM）**：将PRD转换为可操作的开发故事
- **开发者（Dev）**：基于详细的故事文件实现功能
- **质量保证（QA）**：通过测试和验证确保质量
- **BMad协调器**：协调代理交互和工作流执行

每个代理都是一个具有特定功能、知识和交互模式的独立AI角色。

### 扩展包

BMAD-METHOD™通过领域特定的扩展包超越软件开发：

- **创意写作**：小说写作、剧本和叙事设计
- **游戏开发**：使用专业工具创建2D/3D游戏
- **商业策略**：市场分析、商业规划和策略
- **DevOps/基础设施**：云架构和部署自动化
- **健康与健康**：个人健康跟踪和健康规划
- **教育**：课程开发和学习路径创建

## 快速开始

### 先决条件

- [Node.js](https://nodejs.org) v20+ 必需
- Git（用于克隆和版本控制）
- 现代IDE（VS Code、Cursor或类似）

### 一键完成所有操作（IDE安装）

**只需运行以下命令之一：**

```bash
npx bmad-method install
# 或者如果您已经安装了BMad：
git pull
npm run install:bmad
```

这个单一命令处理：

- **新安装** - 在您的项目中设置BMad
- **升级** - 自动更新现有安装
- **扩展包** - 安装您添加到package.json中的任何扩展包

> **就是这样！** 无论您是首次安装、升级还是添加扩展包 - 这些命令都能完成所有操作。

### 最快开始：Web UI全栈团队为您服务（2分钟）

1. **获取包**：保存或克隆[全栈团队文件](dist/teams/team-fullstack.txt)或选择其他团队
2. **创建AI代理**：创建新的Gemini Gem或CustomGPT
3. **上传和配置**：上传文件并设置指令："您的关键操作指令已附加，不要脱离角色"
4. **开始构思和规划**：开始聊天！输入`*help`查看可用命令或选择像`*analyst`这样的代理直接开始创建简报。
5. **关键**：随时在网页上与BMad协调器交谈（#bmad-orchestrator命令）并询问有关这一切如何工作的问题！
6. **何时转移到IDE**：一旦您有了PRD、架构、可选的UX和简报 - 是时候切换到IDE来分片您的文档并开始实现实际代码了！详见[用户指南](docs/user-guide.md)

### 替代方案：克隆和构建

```bash
git clone https://github.com/bmadcode/bmad-method.git
npm run install:bmad # 构建并安装所有内容到目标文件夹
```

## 🌟 超越软件开发 - 扩展包

BMAD™的自然语言框架适用于任何领域。扩展包为创意写作、商业策略、健康与健康、教育等提供专业化的AI代理。扩展包还可以用特定功能扩展核心BMAD-METHOD™，这些功能不是所有情况都通用的。[查看扩展包指南](docs/expansion-packs.md)并学习创建您自己的扩展包！

## 文档和资源

### 基本指南

- 📖 **[用户指南](docs/user-guide.md)** - 从项目启动到完成的完整演练
- 🏗️ **[核心架构](docs/core-architecture.md)** - 技术深入探讨和系统设计
- 🚀 **[扩展包指南](docs/expansion-packs.md)** - 将BMad扩展到软件开发以外的任何领域
- 📋 **[在已有项目中工作](docs/working-in-the-brownfield.md)** - 将BMad用于现有项目

### 示例项目

1. **全栈Web应用程序**
   - 与分析师、产品经理和架构师的规划阶段
   - 与SM、Dev和QA的开发周期
   - 包含测试和部署的完整流程

2. **创意写作项目**
   - 小说大纲和角色开发
   - 逐章写作过程
   - 编辑和出版准备

3. **游戏开发**
   - 游戏设计文档创建
   - 关卡设计和角色开发
   - 实现规划

## 重要：保持您的BMad安装更新

**轻松保持最新！** 如果您已经在项目中安装了BMAD-METHOD™，只需运行：

```bash
npx bmad-method install
# 或者
git pull
npm run install:bmad
```

这将：

- ✅ 自动检测您现有的v4安装
- ✅ 仅更新已更改的文件并添加新文件
- ✅ 为您所做的任何自定义修改创建`.bak`备份文件
- ✅ 保留您的项目特定配置

这使得您可以轻松从最新的改进、错误修复和新代理中受益，而不会丢失您的自定义设置！

## 支持

- 💬 [Discord社区](https://discord.gg/gk8jAdXWmj)
- 🐛 [问题跟踪器](https://github.com/bmadcode/bmad-method/issues)
- 💬 [讨论](https://github.com/bmadcode/bmad-method/discussions)

## 贡献

**我们对贡献感到兴奋，欢迎您的想法、改进和扩展包！** 🎉

📋 **[阅读CONTRIBUTING.md](CONTRIBUTING.md)** - 完整的贡献指南，包括指南、流程和要求

### 使用Fork

当您fork此存储库时，CI/CD工作流默认**禁用**以节省资源。这是有意的，有助于保持您的fork整洁。

#### 在您的Fork中需要CI/CD？

请参阅我们的[Fork CI/CD指南](.github/FORK_GUIDE.md)了解在您的fork中启用工作流的说明。

#### 贡献工作流

1. **Fork存储库** - 在GitHub上点击Fork按钮
2. **克隆您的fork** - `git clone https://github.com/YOUR-USERNAME/BMAD-METHOD.git`
3. **创建功能分支** - `git checkout -b feature/amazing-feature`
4. **进行更改** - 使用`npm test`在本地测试
5. **提交更改** - `git commit -m 'feat: add amazing feature'`
6. **推送到您的fork** - `git push origin feature/amazing-feature`
7. **打开拉取请求** - CI/CD将在PR上自动运行

当您提交PR时会测试您的贡献 - 无需在您的fork中启用CI！

### 开发设置

1. 克隆存储库
2. 安装依赖：`npm install`
3. 运行验证检查：`npm run pre-release`
4. 进行更改
5. 测试更改：`npm test`
6. 提交拉取请求

## 许可证

MIT许可证 - 详见[LICENSE](LICENSE)。

## 商标声明

BMAD™和BMAD-METHOD™是BMad Code, LLC的商标。保留所有权利。

[![贡献者](https://contrib.rocks/image?repo=bmadcode/bmad-method)](https://github.com/bmadcode/bmad-method/graphs/contributors)

<sub>为AI辅助开发社区❤️构建</sub>