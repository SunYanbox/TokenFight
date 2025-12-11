using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TokenFight.Core.Models;
using TokenFight.Core.ReflectionAttribute;
using TokenFight.UI.Core.Profile;

namespace TokenFight.UI.Core;

[AutoSysRegistryInit]
public sealed class GameWindow: Window
{
    public static GameSystemRegistry? GameSystemRegistry { protected get; set; }
    private LoginView _loginView;
    private Tab _loginTab;
    private ProfileView _profileView;
    private Tab _profileTab;
    private TabView _tabView;
    private string _currentProfileIdCache;
    private bool _currentProfileIdChange;
    
    private GameWindowState _gameWindowState = GameWindowState.NotLogin;
    
    public GameWindow()
    {
        _currentProfileIdCache = string.Empty;
        _tabView = new TabView
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        // 登录页面
        _loginView = new LoginView(GameSystemRegistry!.DatabaseServer)
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        _loginTab = new Tab
        {
            View = _loginView,
            DisplayText = "登录"
        };
        _tabView.AddTab(_loginTab, true);
        // 存档信息页面
        _profileView = new ProfileView(GameSystemRegistry!.DatabaseServer)
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        _profileTab = new Tab
        {
            View = _profileView,
            DisplayText = "账号信息"
        };
        Add(_tabView);
        
        SubViewLayout += (_, _) =>
        {
            UpdateState();
            UpdateTabView();
            UpdateTitle();
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
                _gameWindowState = GameWindowState.NotLogin;
            }
            else
            {
                _gameWindowState = GameWindowState.Login;
            }
            _currentProfileIdChange = true;
        }
    }

    private void UpdateTabView()
    {
        if (!_currentProfileIdChange) return;
        switch (_gameWindowState)
        {
            case GameWindowState.NotLogin:
                _tabView.RemoveTab(_profileTab);
                _loginTab.SetNeedsDraw();
                _tabView.AddTab(_loginTab, true);
                break;
            case GameWindowState.Login:
                _tabView.RemoveTab(_loginTab);
                _profileTab.SetNeedsDraw();
                _tabView.AddTab(_profileTab, true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _currentProfileIdChange = false;
    }

    private void UpdateTitle()
    {
        switch (_gameWindowState)
        {
            case GameWindowState.NotLogin:
                Title = "TokenFight - 未登录";
                break;
            case GameWindowState.Login:
                Title = $"TokenFight - {_currentProfileIdCache}";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}