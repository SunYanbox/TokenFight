# 贡献指南

## 提交规范

本项目采用 [Conventional Commits](https://www.conventionalcommits.org/) 规范。

项目预置了提交信息格式化 SKILL，位于 `.claude/skills/commit-formatter-zh-cn/`。该 SKILL 会被 Claude Code、Opencode 等兼容工具自动加载，提交时直接说"帮我写提交"或"写 commit"即可触发。

## 代码格式化

推送前请运行 `dotnet format` 以确保代码风格合规。CI 工作流（`.github/workflows/ci.yml`）也会在每次推送和拉取请求时通过 `dotnet format --verify-no-changes` 检查格式化。
