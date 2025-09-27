# 代码库扁平化工具

BMAD-METHOD™ 包含一个功能强大的代码库扁平化工具，旨在将您的项目文件准备好以上传到 Web AI 工具中供 AI 模型使用。该工具将您的整个代码库聚合到一个 XML 文件中，使您可以轻松地与 AI 助手共享项目上下文，以进行分析、调试或开发辅助。

## 功能

- **AI 优化输出**：生成专为 AI 模型设计的纯净 XML 格式
- **智能过滤**：自动遵循 `.gitignore` 模式以排除不必要的文件，并可选地支持项目级的 `.bmad-flattenignore` 以在计划扁平化现有存储库以进行外部更新和分析时进行额外排除
- **二进制文件检测**：智能识别并排除二进制文件，专注于源代码
- **进度跟踪**：实时进度指示器和全面的完成统计信息
- **灵活输出**：可自定义的输出文件位置和命名

## 用法

```bash
# 基本用法 - 在当前目录中创建 flattened-codebase.xml
npx bmad-method flatten

# 指定自定义输入目录
npx bmad-method flatten --input /path/to/source/directory
npx bmad-method flatten -i /path/to/source/directory

# 指定自定义输出文件
npx bmad-method flatten --output my-project.xml
npx bmad-method flatten -o /path/to/output/codebase.xml

# 结合输入和输出选项
npx bmad-method flatten --input /path/to/source --output /path/to/output/codebase.xml
```

## 示例输出

该工具将显示进度并提供全面的摘要：

```text
📊 完成摘要：
✅ 成功将 156 个文件处理到 flattened-codebase.xml
📁 输出文件：/path/to/your/project/flattened-codebase.xml
📏 总源大小：2.3 MB
📄 生成的 XML 大小：2.1 MB
📝 总代码行数：15,847
🔢 估计令牌数：542,891
📊 文件细分：142 个文本文件，14 个二进制文件，0 个错误
```

生成的 XML 文件以结构化格式包含您项目的基于文本的源文件，AI 模型可以轻松解析和理解，非常适合代码审查、架构讨论或在您的 BMAD-METHOD™ 项目中获得 AI 辅助。

## 高级用法和选项

- CLI 选项
  - `-i, --input <path>`：要扁平化的目录。默认值：当前工作目录或在交互式运行时自动检测到的项目根目录。
  - `-o, --output <path>`：输出文件路径。默认值：在所选目录中的 `flattened-codebase.xml`。
- 交互模式
  - 如果您未传递 `--input` 和 `--output` 并且终端是交互式的 (TTY)，该工具将尝试检测您的项目根目录（通过查找 `.git`、`package.json` 等标记）并提示您确认或覆盖路径。
  - 在非交互式上下文（例如 CI）中，它将静默地优先使用检测到的根目录；否则，它将回退到当前目录和默认文件名。
- 文件发现和忽略
  - 在 git 存储库中时使用 `git ls-files` 以提高速度和正确性；否则回退到基于 glob 的扫描。
  - 应用您的 `.gitignore` 以及一组精选的默认忽略模式（例如 `node_modules`、构建输出、缓存、日志、IDE 文件夹、锁文件、大型媒体/二进制文件、`.env*` 和先前生成的 XML 输出）。
  - 支持在项目根目录处可选的 `.bmad-flattenignore` 文件以获取其他忽略模式（gitignore 样式）。如果存在，其规则将在 `.gitignore` 和默认值之后应用。

## `.bmad-flattenignore` 示例

在项目根目录中创建一个 `.bmad-flattenignore` 文件，以排除必须保留在 git 中但不应包含在扁平化 XML 中的文件：

```text
seeds/**
scripts/private/**
**/*.snap
```

- 二进制处理
  - 检测并从 XML 内容中排除二进制文件。它们在最终摘要中计数，但未嵌入输出中。
- XML 格式和安全性
  - UTF-8 编码文件，根元素为 `<files>`。
  - 每个文本文件都作为 `<file path="relative/path">` 元素发出，其内容包装在 `<![CDATA[ ... ]]>` 中。
  - 该工具通过拆分 CDATA 来安全地处理内容中出现的 `]]>`，以保持正确性。
  - 文件内容按原样保留，并在 XML 内部缩进以提高可读性。
- 性能
  - 并发性根据您的 CPU 和工作负载大小自动选择。无需配置。
  - 在 git 存储库中运行可提高发现性能。

## 最小 XML 示例

```xml
<?xml version="1.0" encoding="UTF-8"?>
<files>
  <file path="src/index.js"><![CDATA[
    // 你的源代码内容
  ]]></file>
</files>
```
