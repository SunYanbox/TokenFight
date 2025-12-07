using TokenFight.Core.Databases.Interfaces;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;
using TokenFight.Core.Interfaces.Factories;
using TokenFight.Core.Interfaces.FStream;
using TokenFight.Core.Interfaces.Game;
using TokenFight.Core.Interfaces.Systems;
using TokenFight.Core.Models.Utils.Systems;

namespace TokenFight.Core.Models;

/// <summary> 游戏系统注册表 | 只读 | 单例 </summary>
public record GameSystemRegistry(
    // 数据库
    IDatabaseServer DatabaseServer,
    ILocalLog LocalLog,
    // 系统
    IEventSystem EventSystem,
    IdGenerateSystem IdGenerateSystem,
    IGlobalResourcesSystem GlobalResourcesSystem,
    IActionListSystem ActionListSystem,
    IActorPositionSystem ActorPositionSystem,
    IActorPoolSystem ActorPoolSystem,
    IActorManagerSystem ActorManagerSystem,
    IActionManagerSystem ActionManagerSystem,
    IActorFactorySystem ActorFactorySystem,
    IDungeonFactorySystem DungeonFactorySystem);
    
    