# TokenFight

> 一款基于 .NET 9.0 的回合制策略战斗游戏 | 作者: Suntion

## 项目概览

TokenFight 是一款受《崩坏：星穹铁道》启发的回合制战斗游戏，采用组件化架构设计。核心玩法围绕行动值驱动的回合制战斗系统展开，支持角色技能、效果（Buff/Debuff）、属性修改、能量管理和抽卡等机制。

### 技术栈

- **语言**: C# 13+ (LangVersion latest)
- **框架**: .NET 9.0
- **依赖注入**: Microsoft.Extensions.DependencyInjection
- **数据存储**: 本地 JSON 文件
- **项目类型**: 单文件发布可执行程序 + 库项目

## 解决方案结构

```
TokenFight.sln
├── TokenFight.Game/                    # 主游戏入口 (Exe)
│   ├── Actors/
│   │   ├── Players/                    # 玩家角色实现
│   │   └── Enemies/                    # 敌人角色实现
│   ├── Dungeons/                       # 副本/战斗流程控制器
│   ├── data/                           # 运行时数据 (JSON)
│   └── Program.cs                      # 入口点 & 服务编排
│
├── Libraries/
│   ├── TokenFight.Core/                # 核心战斗逻辑库
│   │   ├── Consoles/                   # 控制台显示与交互
│   │   ├── Constants/                  # 游戏常量定义
│   │   ├── Databases/                  # JSON 数据库服务
│   │   ├── Enums/                      # 枚举定义
│   │   ├── Helpers/                    # 战斗辅助工具
│   │   ├── Interfaces/                 # 核心接口定义
│   │   ├── Models/                     # 数据模型 (角色/技能/效果等)
│   │   └── ReflectionAttribute/        # 自定义反射特性
│   │
│   ├── TokenFight.DI/                 # 依赖注入模块
│   │   └── ServiceRegistry.cs          # 服务注册与系统初始化
│   │
│   ├── TokenFight.Reflection/          # 反射工具库
│   │   ├── AutoFactorySystem.cs        # 泛型自动工厂系统
│   │   ├── AutoRegister/               # 工厂注册实现 (Actor/Dungeon/Skill)
│   │   └── AutoSysRegistryInitUtil.cs  # 自动初始化系统注册表
│   │
│   ├── TokenFight.UI/                  # 用户界面库
│   │   └── Examples/                   # UI 示例
│   │
│   └── TokenFight.UI.Widgets/          # UI 组件库 (WPF)
│       └── Class1.cs
└── data/                               # 全局游戏数据 (角色/副本/抽卡/模板)
```

## 核心架构

### 1. 反射驱动的自动注册系统

通过自定义特性（Attribute）实现角色、副本、技能的自动发现与注册：

| 特性 | 用途 | 注册目标 |
|------|------|----------|
| `[AutoActor]` | 标记角色类 | `IActorFactorySystem` |
| `[AutoDungeon]` | 标记副本类 | `IDungeonFactorySystem` |
| `[AutoSkill]` | 标记技能类 | `ISkillFactorySystem` |
| `[AutoSysRegistryInit]` | 自动注入 `GameSystemRegistry` | 静态属性注入 |

```csharp
[AutoActor(Id = "PlayerXiEr_希尔251121b", Team = TeamType.Player)]
public class PlayerXiEr0 : PlayerActor { ... }
```

### 2. JSON 数据驱动

所有游戏数据（角色属性、副本配置、抽卡奖励、物品模板、用户档案）均以 JSON 格式存储在 `data/` 目录下，程序启动时由 `DatabaseServer` 自动加载。

### 3. 依赖注入 (DI)

`ServiceRegistry` 集中管理所有服务的注册与初始化：
- **单例服务**: 数据库、事件系统、日志、战斗系统、工厂系统
- **Scoped 服务**: 各类 Master（血量、能量、技能、效果等）
- **初始化流程**: 注册 → 构建容器 → 初始化系统 → 自动注册工厂

### 4. 事件系统

基于 `IEventSystem` 的事件驱动架构，支持战斗中各类事件（伤害、死亡、行动结束等）的订阅与广播。

### 5. GameSystemRegistry

全局系统注册表，以 record 类型集中持有所有核心系统的引用，通过 DI 注入到各个组件中。

## 战斗系统特性

- **行动值驱动回合**: 基于行动值排序的回合制战斗
- **技能系统**: 普攻、战技、终结技、天赋
- **效果系统**: Buff/Debuff、标记、属性增益
- **能量管理**: 终结技能量充能机制
- **属性系统**: 基于 `IAttrSet` 的属性修改框架
- **推条/拉条**: 行动值操控机制
- **追加攻击 & 额外回合**: 角色特殊机制（如希尔的击杀再动）
- **波次与战利品**: 副本波次管理、Token收益

## 快速开始

### 前置要求

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### 构建

```bash
# 构建整个解决方案
dotnet build

# 运行游戏
dotnet run --project TokenFight.Game/TokenFight.Game.csproj

# 发布单文件
dotnet publish TokenFight.Game/TokenFight.Game.csproj -c Release
```

### 添加角色

1. 在 `data/actors/` 目录下创建角色的 JSON 数据文件
2. 在 `TokenFight.Game/Actors/Players/` (或 `Enemies/`) 下创建角色类
3. 使用 `[AutoActor]` 特性标记类
4. 在 `GameIdTableConst` 中注册 ID 常量

## 开发指南

参见 [AGENTS.md](./AGENTS.md) 获取详细的代码风格约定、构建命令和开发工作流。

## 架构规划

参见 [架构设计文档](./docs/architecture/design.md) 了解 ECS + 状态机架构改造计划。

## 项目里程碑

| 版本 | 说明 |
|------|------|
| v25.12.1 | 自动化工厂系统与角色注册机制 |
| v25.12.2 | 副本系统与收益机制 |
| v25.12.3 | 移除冗余命名空间引用 |
| v25.12-1.0.3 ~ 1.0.4 | 用户登录注册 & UI重构 |
| v25.12-1.0.5 ~ 1.0.7 | 数据保存、角色属性初始化、数据模型重构 |
| v25.12-1.0.8 ~ 1.0.10 | 抽卡系统实现 |
| v25.12-1.0.11 | 重构抽卡系统并优化UI布局 |
| 近期 | 移除 Terminal.Gui UI，引入 WPF UI 组件库 |
