# Contributing to TokenFight

## Commit Convention

This project uses [Conventional Commits](https://www.conventionalcommits.org/).

A commit-formatting skill is available at `.claude/skills/commit-formatter-zh-cn/`. It is automatically loaded by Claude Code, Opencode, and other compatible AI coding tools — simply ask them to "write a commit" and the skill will guide you through generating a compliant commit message.

## Code Formatting

Run `dotnet format` before pushing to ensure code style compliance. A CI workflow (`.github/workflows/ci.yml`) also verifies formatting on every push and pull request via `dotnet format --verify-no-changes`.
