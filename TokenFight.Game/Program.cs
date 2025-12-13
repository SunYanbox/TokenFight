using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using TokenFight.Core.Constants;
using TokenFight.Core.Databases.Models.Profiles;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Attrs;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;
using TokenFight.Core.Interfaces.Factories;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Attrs;
using TokenFight.Core.Models.Entities.Actors;
using TokenFight.Core.Models.Events.Contexts;
using TokenFight.Core.Models.Game;
using TokenFight.DI;
using TokenFight.UI.Core;



var services = new ServiceCollection();




ServiceRegistry.RegisterAllServices(services);

ServiceProvider serviceProvider = services.BuildServiceProvider();

ServiceRegistry.InitAllSystems(serviceProvider);

var systemRegistryGlobal = serviceProvider.GetService<GameSystemRegistry>()!;


try
{
    ConfigurationManager.RuntimeConfig = """{ "Theme": "Amber Phosphor" }""";
    ConfigurationManager.Enable(ConfigLocations.All);

    // With using statement for automatic disposal
    using IApplication app = Application.Create().Init();
    using var window = new GameWindow();
    app.Run(window);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}






try
{
    var autoActorManageSystem = serviceProvider.GetService<IActorFactorySystem>();

    var systemRegistry = serviceProvider.GetService<GameSystemRegistry>()!;

    var autoDungeon = serviceProvider.GetService<IDungeonFactorySystem>()!;
    BaseDungeon battleFlow = autoDungeon.CreateInstance(GameIdTableConst.BattleFlow, [systemRegistry, "dungeon1"])!;

    battleFlow.OnLoad(new Profile("null", "null"));

    battleFlow.InitActorPool();

    systemRegistry.ActorPoolSystem.AddPlayer(new Lazy<IActor>(() =>
        autoActorManageSystem!.CreateInstance(GameIdTableConst.PlayerXiEr0, [20, systemRegistry])));

    void HandleGameStart(IContext context)
    {
        if (context is EnterGameContext)
        {
            PlayerActor[] players = ActorHelper.GetActorsByTeamWithLifeAndValid(TeamType.Player).OfType<PlayerActor>().ToArray();

            foreach (PlayerActor player in players)
            {
                player.AttrSet.SetAttr(new AttrModifyData
                {
                    Id = "环境紊流1",
                    Type = AttrModifyType.Base,
                    ModifyData = new Dictionary<int, double>
                    {
                        { IAttrSet.ToInt(AttrType.CriticalRate), 0.6 },
                        { IAttrSet.ToInt(AttrType.CriticalDamage), 1.5 }
                    },
                    IsTemp = false
                });
            }
        }
        systemRegistry.EventSystem.Unsubscribe(EventType.EnterGame, HandleGameStart);
    }

    systemRegistry.EventSystem.Subscribe(EventType.EnterGame, HandleGameStart);

    battleFlow.Main();

    Console.WriteLine("服务解析成功。");
}
catch (Exception ex)
{
    Console.WriteLine($"服务解析失败： {ex.Message}\n\t{ex.StackTrace}");
    throw;
}