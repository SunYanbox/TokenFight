using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Entities.Masters;
using TokenFight.Core.ReflectionAttribute;
using TokenFight.UI.Core.Profile;

namespace TokenFight.UI.Core;

[AutoSysRegistryInit]
public sealed class GameWindow: Window
{
    public static GameSystemRegistry? GameSystemRegistry { protected get; set; }
    private readonly Tab _loginTab;
    private readonly Tab _profileTab;
    private readonly Tab _gachaTab;
    private TabView _tabView;
    private string _currentProfileIdCache;
    private bool _currentProfileIdChange;
    
    private EnumTypeMaster<GameWindowState> _gameWindowState = new([GameWindowState.NotLogin]);
    
    public GameWindow()
    {
        Title = "TokenFight - 未登录";
        _currentProfileIdCache = string.Empty;
        _tabView = new TabView
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        // 登录页面
        _loginTab = new Tab
        {
            View = new LoginView(GameSystemRegistry!.DatabaseServer),
            DisplayText = "登录"
        };
        _tabView.AddTab(_loginTab, true);
        // 存档信息页面
        _profileTab = new Tab
        {
            View = new ProfileView(GameSystemRegistry!.DatabaseServer),
            DisplayText = "账号信息"
        };
        Add(_tabView);
        // 抽卡界面
        _gachaTab = new Tab
        {
            View = new GachaView(GameSystemRegistry!.DatabaseServer),
            DisplayText = "抽卡"
        };
        
        SubViewLayout += (_, _) =>
        {
            UpdateState();
            UpdateTabView();
        };
    }

    private void UpdateState()
    {
        string loginAccount = GameSystemRegistry?.DatabaseServer?.CurrentProfile?.Account ?? "";
        string oldAccount = _currentProfileIdCache;
        
        _currentProfileIdCache = loginAccount;

        if (oldAccount != loginAccount)
        {
            if (string.IsNullOrEmpty(_currentProfileIdCache))
            {
                _gameWindowState.Clear();
                _gameWindowState.Add(GameWindowState.NotLogin);
            }
            else
            {
                _gameWindowState.Remove(GameWindowState.NotLogin);
                _gameWindowState.Add(GameWindowState.Login);
            }
            _currentProfileIdChange = true;
        }
    }

    private void UpdateTabView()
    {
        if (!_currentProfileIdChange) return;
        if (_gameWindowState.Contains(GameWindowState.NotLogin))
        {
            _tabView.RemoveTab(_profileTab);
            _tabView.RemoveTab(_gachaTab);
            
            _tabView.AddTab(_loginTab, true);
            _loginTab.SetNeedsDraw();
            Title = "TokenFight - 未登录";
        }

        if (_gameWindowState.Contains(GameWindowState.Login))
        {
            _tabView.RemoveTab(_loginTab);
            
            _tabView.AddTab(_profileTab, true);
            _tabView.AddTab(_gachaTab, false);
            _profileTab.SetNeedsDraw();
            Title = $"TokenFight - {_currentProfileIdCache}";
        }
        _currentProfileIdChange = false;
    }
}