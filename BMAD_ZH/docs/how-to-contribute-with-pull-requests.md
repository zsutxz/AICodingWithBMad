# 如何通过拉取请求进行贡献

**GitHub 和拉取请求新手？** 本指南将逐步引导您了解基础知识。

## 什么是拉取请求？

拉取请求 (PR) 是您在 GitHub 上提议对项目进行更改的方式。可以将其视为“这是我想做的一些更改 - 请审查并考虑将它们添加到主项目中。”

## 开始之前

⚠️ **重要提示**：请保持您的贡献小而专注！我们更喜欢许多小的、清晰的更改，而不是一个巨大的更改。

**提交 PR 前的要求：**

- **对于错误修复**：使用[错误报告模板](https://github.com/bmadcode/bmad-method/issues/new?template=bug_report.md)创建一个问题
- **对于新功能**：
  1. 在 Discord [#general-dev 频道](https://discord.gg/gk8jAdXWmj)中讨论
  2. 使用[功能请求模板](https://github.com/bmadcode/bmad-method/issues/new?template=feature_request.md)创建一个问题
- **对于大的更改**：始终先打开一个问题以讨论一致性

## 分步指南

### 1. Fork 存储库

1.  转到 [BMad-Method 存储库](https://github.com/bmadcode/bmad-method)
2.  单击右上角的“Fork”按钮
3.  这将创建您自己的项目副本

### 2. 克隆您的 Fork

```bash
# 将 YOUR-USERNAME 替换为您的实际 GitHub 用户名
git clone https://github.com/YOUR-USERNAME/bmad-method.git
cd bmad-method
```

### 3. 创建一个新分支

**切勿直接在 `main` 分支上工作！** 始终为您的更改创建一个新分支：

```bash
# 创建并切换到一个新分支
git checkout -b fix/typo-in-readme
# 或
git checkout -b feature/add-new-agent
```

**分支命名提示：**

- `fix/description` - 用于错误修复
- `feature/description` - 用于新功能
- `docs/description` - 用于文档更改

### 4. 进行更改

- 编辑您想要更改的文件
- 保持更改小而专注
- 如果可能，测试您的更改

### 5. 提交您的更改

```bash
# 添加您的更改
git add .

# 使用清晰的消息提交
git commit -m "Fix typo in README.md"
```

**好的提交消息：**

- “修复安装说明中的拼写错误”
- “为新 Agent 用法添加示例”
- “更新文档中的损坏链接”

**不好的提交消息：**

- “东西”
- “更改”
- “更新”

### 6. 推送到您的 Fork

```bash
# 将您的分支推送到您的 fork
git push origin fix/typo-in-readme
```

### 7. 创建拉取请求

1.  在 GitHub 上转到您的 fork
2.  您会看到一个绿色的“Compare & pull request”按钮 - 单击它
3.  选择正确的目标分支：
    *   **`next` 分支** 用于大多数贡献（功能、文档、增强功能）
    *   **`main` 分支** 仅用于关键修复
4.  使用 CONTRIBUTING.md 中的模板填写 PR 描述：
    *   **什么**：描述更改的 1-2 句话
    *   **为什么**：解释原因的 1-2 句话
    *   **如何**：关于实施的 2-3 个要点
    *   **测试**：您如何测试
5.  引用相关的问题编号（例如，“Fixes #123”）

### 8. 等待审查

- 维护人员将审查您的 PR
- 他们可能会要求更改
- 请耐心并积极响应反馈

## 什么是好的拉取请求？

✅ **好的 PR：**

- 一次只更改一件事
- 有清晰、描述性的标题
- 在描述中解释了什么和为什么
- 仅包含需要更改的文件

❌ **避免：**

- 更改整个文件的格式
- 在一个 PR 中进行多个不相关的更改
- 将您的整个项目/存储库复制到 PR 中
- 没有解释的更改

## 要避免的常见错误

1.  **不要重新格式化整个文件** - 只更改必要的内容
2.  **不要包含不相关的更改** - 每个 PR 坚持一个修复/功能
3.  **不要在问题中粘贴代码** - 创建一个合适的 PR
4.  **不要提交您的整个项目** - 贡献特定的改进

## 需要帮助？

- 💬 加入我们的 [Discord 社区](https://discord.gg/gk8jAdXWmj) 以获得实时帮助：
  - **#general-dev** - 技术问题和功能讨论
  - **#bugs-issues** - 在提交问题之前获得有关错误的帮助
- 💬 在 [GitHub Discussions](https://github.com/bmadcode/bmad-method/discussions) 中提问
- 🐛 使用[错误报告模板](https://github.com/bmadcode/bmad-method/issues/new?template=bug_report.md)报告错误
- 💡 使用[功能请求模板](https://github.com/bmadcode/bmad-method/issues/new?template=feature_request.md)建议功能
- 📖 阅读完整的[贡献指南](../CONTRIBUTING.md)

## 示例：好的与不好的 PR

### 😀 好的 PR 示例

**标题**：“修复安装指南的损坏链接”
**更改**：一个文件，一行更改
**描述**：“README.md 中的链接指向了错误的文件。已更新为指向正确的安装指南。”

### 😞 不好的 PR 示例

**标题**：“更新”
**更改**：50 个文件，整个代码库重新格式化
**描述**：“进行了一些改进”

---

**记住**：我们随时为您提供帮助！不要害怕提问。每个专家都曾经是初学者。
