using Microsoft.Extensions.DependencyInjection;
using TokenFight.Core.Databases;
using TokenFight.Core.Databases.Interfaces;
using TokenFight.Core.Interfaces.Entities.Masters;
using TokenFight.Core.Interfaces.Events;
using TokenFight.Core.Interfaces.Factories;
using TokenFight.Core.Interfaces.FStream;
using TokenFight.Core.Interfaces.Systems;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Entities.Masters;
using TokenFight.Core.Models.Events;
using TokenFight.Core.Models.FStream;
using TokenFight.Core.Models.Systems;
using TokenFight.Core.Models.Utils.Systems;
using TokenFight.Reflection;
using TokenFight.Reflection.AutoActor;
using TokenFight.Reflection.AutoDungeon;

namespace TokenFight.DI;

public static class ServiceRegistry
{
    public static void RegisterAllServices(IServiceCollection services)
    {
        // 数据库注册
        services.AddSingleton<IDatabaseServer, DatabaseServer>();
        
        // 系统注册
        services.AddSingleton<ILocalLog, LocalLog>();
        services.AddSingleton<IEventSystem, EventSystem>();
        services.AddSingleton<IGlobalResourcesSystem, GlobalResourcesSystem>();
        services.AddSingleton<IActionListSystem, ActionListSystem>();
        services.AddSingleton<IActorManagerSystem, ActorManagerSystem>();
        services.AddSingleton<IActionManagerSystem, ActionManagerSystem>();
        services.AddSingleton<IActorPoolSystem, ActorPoolSystem>();
        services.AddSingleton<IActorPositionSystem, ActorPositionSystem>();
        services.AddSingleton<IEventOutputSystem, EventOutputSystem>();
        
        services.AddSingleton<IActorFactorySystem, ActorFactorySystem>();
        services.AddSingleton<IDungeonFactorySystem, DungeonFactorySystem>();
        
        // 单例上下文注册
        services.AddSingleton<IdGenerateSystem>();
        services.AddSingleton<GameSystemRegistry>();
        
        // masters注册
        services.AddScoped(typeof(IEnumTypeMaster<>), typeof(EnumTypeMaster<>));
        services.AddScoped<IHealthMaster, HealthMaster>();
        services.AddScoped<IEnergyMaster, EnergyMaster>();
        services.AddScoped<IActionValueMaster, ActionValueMaster>();
        services.AddScoped<ISkillMaster, SkillMaster>();
        services.AddScoped<IEffectMaster, EffectMaster>();
        services.AddScoped<IRelationshipMaster, RelationshipMaster>();
        services.AddScoped<IShieldMaster, ShieldMaster>();
        services.AddScoped<ILifeCycleMaster, LifeCycleMaster>();
        
        // // 副本注册
        // services.AddKeyedScoped<IDungeon, BaseDungeon>(BaseDungeon.Id);
    }

    public static void InitAllSystems(IServiceProvider serviceProvider)
    {
        serviceProvider.GetService<IDatabaseServer>()?.Init();
        serviceProvider.GetService<ILocalLog>()?.Init();
        serviceProvider.GetService<IEventSystem>()?.Init();
        serviceProvider.GetService<IActorManagerSystem>()?.Init();
        serviceProvider.GetService<IActionManagerSystem>()?.Init();
        serviceProvider.GetService<IActorPoolSystem>()?.Init();
        serviceProvider.GetService<IActorPositionSystem>()?.Init();
        serviceProvider.GetService<IGlobalResourcesSystem>()?.Init();
        serviceProvider.GetService<IActionListSystem>()?.Init();
        serviceProvider.GetService<IEventOutputSystem>()?.Init();
        // 反射工厂系统
        serviceProvider.GetService<IActorFactorySystem>()?.Init();
        serviceProvider.GetService<IDungeonFactorySystem>()?.Init();
        
        var systemRegistry  = serviceProvider.GetService<GameSystemRegistry>();

        AutoSysRegistryInitUtil.Initialize(systemRegistry!);
    }
}