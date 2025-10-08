<!-- 由 BMAD™ Core 提供支持 -->

# apply-qa-fixes (应用QA修复)

根据特定故事的QA结果（门禁和评估）实施修复。此任务旨在让开发代理系统性地采纳QA的输出，并应用代码/测试更改，同时只更新故事文件中允许的部分。

## 目的

- 读取一个故事的QA输出（门禁YAML + 评估Markdown）
- 创建一个有优先级的、确定性的修复计划
- 应用代码和测试更改以弥补差距和解决问题
- 仅更新开发代理允许的故事部分

## 输入

```yaml
必需:
  - story_id: '{epic}.{story}' # 例如: "2.2"
  - qa_root: 来自 `.bmad-core/core-config.yaml` 的 `qa.qaLocation` 键 (例如: `docs/project/qa`)
  - story_root: 来自 `.bmad-core/core-config.yaml` 的 `devStoryLocation` 键 (例如: `docs/project/stories`)

可选:
  - story_title: '{title}' # 如果缺失，则从故事的H1标题派生
  - story_slug: '{slug}' # 如果缺失，则从标题派生 (小写，连字符分隔)
```

## 要读取的QA源

- 门禁 (YAML): `{qa_root}/gates/{epic}.{story}-*.yml`
  - 如果有多个，使用修改时间最新的一个
- 评估 (Markdown):
  - 测试设计: `{qa_root}/assessments/{epic}.{story}-test-design-*.md`
  - 可追溯性: `{qa_root}/assessments/{epic}.{story}-trace-*.md`
  - 风险概况: `{qa_root}/assessments/{epic}.{story}-risk-*.md`
  - NFR评估: `{qa_root}/assessments/{epic}.{story}-nfr-*.md`

## 先决条件

- 仓库在本地构建和测试通过 (Deno 2)
- Lint和测试命令可用:
  - `deno lint`
  - `deno test -A`

## 流程 (请勿跳过步骤)

### 0) 加载核心配置并定位故事

- 读取 `.bmad-core/core-config.yaml` 并解析 `qa_root` 和 `story_root`
- 在 `{story_root}/{epic}.{story}.*.md` 中定位故事文件
  - 如果缺失，则停止并请求正确的 `story_id`/路径

### 1) 收集QA发现

- 解析最新的门禁YAML:
  - `gate` (PASS|CONCERNS|FAIL|WAIVED)
  - `top_issues[]` 包含 `id`, `severity`, `finding`, `suggested_action`
  - `nfr_validation.*.status` 和说明
  - `trace` 覆盖范围摘要/差距
  - `test_design.coverage_gaps[]`
  - `risk_summary.recommendations.must_fix[]` (如果存在)
- 读取任何存在的评估Markdown文件，并提取明确的差距/建议

### 2) 构建确定性的修复计划 (按优先级排序)

按顺序应用，优先级最高的先行:

1. `top_issues` 中的高严重性项目 (安全/性能/可靠性/可维护性)
2. NFR状态: 所有FAIL必须修复 → 然后是CONCERNS
3. 测试设计 `coverage_gaps` (如果指定，优先处理P0场景)
4. 未覆盖的需求追溯 (AC级别)
5. 风险 `must_fix` 建议
6. 中等严重性问题，然后是低严重性问题

指南:

- 优先使用测试来弥补覆盖差距，可以在代码更改之前或同时进行
- 保持更改最小化和目标明确；遵循项目架构和TS/Deno规则

### 3) 应用更改

- 根据计划实施代码修复
- 添加缺失的测试以弥补覆盖差距 (单元测试优先；根据AC要求进行集成测试)
- 通过 `deps.ts` 保持导入集中化 (参见 `docs/project/typescript-rules.md`)
- 遵循 `src/core/di.ts` 中的DI边界和现有模式

### 4) 验证

- 运行 `deno lint` 并修复问题
- 运行 `deno test -A` 直到所有测试通过
- 迭代直到全部通过

### 5) 更新故事 (仅限允许的部分)

关键: 开发代理仅被授权更新故事文件的这些部分。不要修改任何其他部分 (例如: QA结果、故事、验收标准、开发说明、测试):

- 任务 / 子任务复选框 (将您添加的任何修复子任务标记为完成)
- 开发代理记录 →
  - 使用的代理模型 (如果更改)
  - 调试日志参考 (命令/结果，例如lint/测试)
  - 完成说明列表 (更改了什么，为什么，如何更改)
  - 文件列表 (所有添加/修改/删除的文件)
- 变更日志 (新的带日期的条目，描述应用的修复)
- 状态 (见下面的规则)

状态规则:

- 如果门禁为PASS且所有已识别的差距都已弥补 → 设置 `Status: Ready for Done`
- 否则 → 设置 `Status: Ready for Review` 并通知QA重新运行审查

### 6) 不要编辑门禁文件

- 开发人员不修改门禁YAML。如果修复解决了问题，请求QA重新运行 `review-story` 来更新门禁

## 阻塞条件

- 缺少 `.bmad-core/core-config.yaml`
- 找不到 `story_id` 对应的故事文件
- 找不到QA产物 (门禁和评估都没有)
  - 停止并请求QA至少生成一个门禁文件 (或者仅在有明确的开发者提供的修复列表时继续)

## 完成清单

- deno lint: 0 个问题
- deno test -A: 所有测试通过
- 所有高严重性的 `top_issues` 已解决
- NFR FAIL → 已解决; CONCERNS 已最小化或记录
- 覆盖差距已弥补或已明确记录并说明理由
- 故事已更新 (仅限允许的部分)，包括文件列表和变更日志
- 状态已根据状态规则设置

## 示例: 故事 2.2

假设门禁 `docs/project/qa/gates/2.2-*.yml` 显示

- `coverage_gaps`: 返回操作行为未经测试 (AC2)
- `coverage_gaps`: 集中化依赖强制执行未经测试 (AC4)

修复计划:

- 添加一个测试，确保工具包菜单的“返回”操作能返回到主菜单
- 添加一个静态测试，验证服务/视图的导入是否通过 `deps.ts`
- 重新运行lint/测试，并相应更新开发代理记录 + 文件列表

## 关键原则

- 确定性的、风险优先的排序
- 最小化、可维护的更改
- 测试验证行为并弥补差距
- 严格遵守允许的故事更新区域
- 门禁所有权归QA所有；开发通过状态信号表示准备就绪
