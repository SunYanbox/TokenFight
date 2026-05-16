# 架构设计文档

> 记录 TokenFight 项目的架构决策、设计方案与演进路线。

## ECS + 状态机 架构改造计划

### 背景

当前战斗系统采用传统的面向对象继承体系，大量系统（血量、能量、技能、效果、行动值等）通过 DI 和 `GameSystemRegistry` 耦合在一起，导致：

- 系统间职责边界模糊，`Master` 类群（HpMaster、EnergyMaster、SkillMaster 等）相互引用，牵一发而动全身
- 新增角色机制需要修改多个现有系统，扩展性差
- 没有统一的数据视图，调试和排查问题困难

### 目标架构

将核心战斗层改造为 **ECS (Entity-Component-System) + 状态机 + 事件系统** 架构

### 设计原则

1. **ECS 管数据，状态机管行为** — Component 只存数据，System 只写逻辑，StateMachine 控制实体行为流转
2. **System 无状态** — 所有 System 可复用，不持有实例字段，输入 = Entity + World，输出 = 修改 Component
3. **逐步替换** — 不一次性重写，先抽取 Hp / Energy 为 ECS，再迁移技能、效果等复杂系统
4. **事件桥接** — 改造期间 `IEventSystem` 继续作为 ECS System 与外部 UI/DI 的通信层

### 相关链接

- [GameSystemRegistry](../../Libraries/TokenFight.Core/Consoles/GameSystemRegistry.cs)
- [ServiceRegistry](../../Libraries/TokenFight.DI/ServiceRegistry.cs)
- [README.md 核心架构](../../README.md#核心架构)
