using Microsoft.Extensions.DependencyInjection;
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
using TokenFight.DI;
using Terminal.Gui;
using TokenFight.UI.Core;
using Attribute = Terminal.Gui.Attribute;

var services = new ServiceCollection();




ServiceRegistry.RegisterAllServices(services);

ServiceProvider serviceProvider = services.BuildServiceProvider();

ServiceRegistry.InitAllSystems(serviceProvider);

try
{
    Application.Init();
    
    // 创建自定义颜色方案 - 黑色背景
    var blackBackgroundScheme = new ColorScheme
    {
        Normal = new Attribute(Color.White, Color.Black),     // 白字黑底
        HotNormal = new Attribute(Color.BrightYellow, Color.Black),
        Focus = new Attribute(Color.Black, Color.Gray),
        HotFocus = new Attribute(Color.BrightRed, Color.Gray),
        Disabled = new Attribute(Color.Gray, Color.Black)
    };
        
    // 设置Application.Top的颜色方案（影响所有子控件）
    Application.Top.ColorScheme = blackBackgroundScheme;
        
    // 创建主窗口
    var window = new Window("我的应用")
    {
        X = 0,
        Y = 1, // 给菜单栏留出空间
        Width = Dim.Fill(),
        Height = Dim.Fill(),
        ColorScheme = ColorSchemePreset.WhiteBackgroundScheme
    };
        
    Application.Top.Add(window);
    Application.Top.Add();
        
    Application.Run();
    Application.Shutdown();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}






try
{
    var autoActorManageSystem = serviceProvider.GetService<IActorFactorySystem>();
    Console.WriteLine(string.Join(", ", autoActorManageSystem?.GetAllIds() ?? []));
    
    GameSystemRegistry systemRegistry = serviceProvider.GetService<GameSystemRegistry>()!;
    
    var autoDungeon = serviceProvider.GetService<IDungeonFactorySystem>()!;
    var battleFlow = autoDungeon.CreateInstance(GameIdTableConst.BattleFlow, [systemRegistry, "dungeon1"])!;

    battleFlow.OnLoad(new Profile
    {
        Account = "null",
        Password = "null",
        Token = 0,
        Items = []
    });
    
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