# TokenFight 项目代理指南

本文档为代码代理提供在 TokenFight 项目中工作的指南，包含构建命令、代码风格约定和开发工作流。

## 项目概览

TokenFight 是一个基于 .NET 9.0 的游戏项目，使用 C# 开发。项目结构：

- `TokenFight.Game/` - 主游戏可执行项目
- `Libraries/TokenFight.Core/` - 核心战斗逻辑库
- `Libraries/TokenFight.UI/` - 用户界面库  
- `Libraries/TokenFight.UI.Widgets/` - UI 组件库
- `Libraries/TokenFight.DI/` - 依赖注入模块
- `Libraries/TokenFight.Reflection/` - 反射工具库

## 构建、测试和运行命令

### 构建命令
```bash
# 构建整个解决方案
dotnet build

# 构建特定项目
dotnet build TokenFight.Game/TokenFight.Game.csproj
dotnet build Libraries/TokenFight.Core/TokenFight.Core.csproj

# 重新构建（清理后构建）
dotnet clean
dotnet build

# 发布构建（单文件发布）
dotnet publish TokenFight.Game/TokenFight.Game.csproj -c Release
```

### 运行命令
```bash
# 运行游戏
dotnet run --project TokenFight.Game/TokenFight.Game.csproj

# 以调试模式运行
dotnet run --project TokenFight.Game/TokenFight.Game.csproj --configuration Debug
```

### 包管理
```bash
# 还原 NuGet 包
dotnet restore

# 添加 NuGet 包
dotnet add package <PackageName> --version <Version>

# 列出项目引用
dotnet list package
```

## 代码风格指南

### 命名约定
- **命名空间**: `TokenFight.<模块>.<子模块>` (例如: `TokenFight.Core.Constants`)
- **类名**: PascalCase，使用名词或名词短语
- **接口名**: PascalCase，以 "I" 开头
- **方法名**: PascalCase，使用动词或动词短语
- **属性名**: PascalCase
- **字段名**: PascalCase，不使用下划线前缀
- **局部变量**: camelCase
- **常量**: PascalCase（全部大写仅用于字面常量）

### 文件组织
- 每个文件一个主类，文件名与类名匹配
- 目录结构按功能模块组织
- 工具类放在 `Helpers/` 目录
- 常量类放在 `Constants/` 目录
- 数据模型放在 `Models/` 目录
- 接口放在 `Interfaces/` 目录

### 导入约定
- 使用 `using` 语句按功能分组，空行分隔
- 系统命名空间优先，然后项目命名空间
- 保持导入有序：
```csharp
// 系统命名空间
using System;
using System.Collections.Generic;
using System.Linq;

// 第三方库

// 项目命名空间
using TokenFight.Core.Constants;
using TokenFight.Core.Interfaces;
using TokenFight.Core.Models;
```

### 类型使用
- 启用 Nullable 引用类型：`<Nullable>enable</Nullable>`
- 使用最新的 C# 语言版本：`<LangVersion>latest</LangVersion>`
- 启用隐式 using：`<ImplicitUsings>enable</ImplicitUsings>`
- 优先使用记录类型（record）表示不可变数据
- 在适当的地方使用泛型

### 注释和文档
- **XML 文档注释**: 所有公共类、方法、属性必须包含
- **代码注释**: 在复杂逻辑处添加中文注释
- **区域划分**: 使用 `#region` 和 `#endregion` 组织相关代码块
- **示例 XML 注释**:
```csharp
/// <summary>
/// 游戏中一些固定的常量
/// </summary>
public static class GameConst
{
    /// <summary> 默认行动距离 </summary>
    public const double DefaultActionDistance = 10000.00;
}
```

### 错误处理
- 使用 try-catch 处理可能失败的代码
- 在适当的地方重新抛出异常
- 记录异常信息到控制台
- 示例模式：
```csharp
try
{
    // 业务逻辑
}
catch (Exception ex)
{
    Console.WriteLine($"操作失败：{ex.Message}");
    throw; // 或处理异常
}
```

### 格式化规则
- 使用 4 空格缩进（非制表符）
- 方法之间空一行
- 类成员按访问修饰符排序：public > internal > protected > private
- 花括号在同一行开始（K&R 风格）
- 行长度尽量保持在 120 字符以内

## 项目配置

### 目标框架和设置
- **目标框架**: .NET 9.0
- **输出类型**: 
  - 库项目: `Library`
  - 主游戏: `Exe` （单文件发布）
- **运行时**: win-x64
- **发布配置**: 启用单文件发布和自包含

### 依赖管理
- 项目引用使用相对路径
- NuGet 包使用显式版本
- 保持依赖项最新但稳定

## 开发工作流

### 1. 代码修改前
- 运行 `dotnet build` 确保当前代码可编译
- 检查相关测试（如有）

### 2. 代码修改中
- 遵循上述代码风格指南
- 添加必要的 XML 文档注释
- 保持向后兼容性
- 避免不必要的重构

### 3. 代码修改后
- 运行 `dotnet build` 验证编译
- 确保无警告和错误
- 如果添加了新功能，考虑是否需要测试

### 4. 验证更改
- 运行游戏测试核心功能
- 检查控制台输出是否有异常
- 验证依赖注入正常工作

## 测试策略

### 当前状态
项目目前没有单元测试框架。添加测试时建议：
1. 创建独立的测试项目
2. 使用 xUnit 或 NUnit 框架
3. 在测试项目中引用要测试的库

### 添加测试的示例命令
```bash
# 创建 xUnit 测试项目
dotnet new xunit -n TokenFight.Core.Tests

# 添加项目引用
dotnet add TokenFight.Core.Tests reference Libraries/TokenFight.Core/

# 运行测试
dotnet test --filter "FullyQualifiedName~TestClassName"
```

## 性能和优化

### 内存管理
- 使用 `IDisposable` 接口管理非托管资源
- 避免频繁的对象分配
- 使用对象池管理游戏对象

### 性能考虑
- 游戏循环中避免 LINQ 查询
- 预计算常量值
- 使用结构体（struct）代替小类

## 工具和扩展

### 推荐的开发工具
- Visual Studio 或 Rider
- .NET 9.0 SDK
- Git 进行版本控制

### 有用的扩展
- C# 扩展（VS Code）
- NuGet Package Manager
- GitLens（可选）

## 故障排除

### 常见问题
1. **构建失败**: 检查 NuGet 包还原状态 `dotnet restore`
2. **运行时错误**: 检查依赖注入注册和配置
3. **发布问题**: 验证目标运行时（win-x64）

### 调试技巧
- 使用 `Console.WriteLine` 进行日志记录
- 检查异常堆栈跟踪
- 验证服务注册顺序

---

*最后更新: 2026-04-11*  
*基于 TokenFight 项目结构分析生成*