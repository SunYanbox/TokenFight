using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TokenFight.Core.Databases.Interfaces;

namespace TokenFight.UI.Core.Profile;

public class ProfileView: View
{
    private readonly IDatabaseServer _databaseServer;
    private readonly Label _accountLabel;
    private readonly Label _tokenLabel;
    private bool _queryLogout;
    
    public ProfileView(IDatabaseServer databaseServer)
    {
        _databaseServer = databaseServer;
        var accountLabel = new Label{ Text = "账号: " };
        _accountLabel = new Label
        {
            X = Pos.Right(accountLabel),
            Y = Pos.Y(accountLabel)
        };
        var tokenLabel = new Label()
        {
            Text = "Token: ",
            X = Pos.X(accountLabel),
            Y = Pos.Bottom(accountLabel)
        };
        _tokenLabel = new Label
        {
            X = Pos.Right(tokenLabel),
            Y = Pos.Y(tokenLabel)
        };

        var logout = new Button
        {
            Text = "登出",
            X = Pos.X(_accountLabel),
            Y = Pos.Bottom(this) - 1,
        };
        logout.MouseClick += (sender, args) =>
        {
            if (_queryLogout) return;
            _queryLogout = true;
            string account = _databaseServer.CurrentProfile?.Account ?? string.Empty;
            if (string.IsNullOrEmpty(account)) return;
            int? choice = UIUtil.QueryAtMainLoop(this, "登出",
                $"确认登出账号: \"{account}\"吗", ["确认登出", "取消登出"]);
            if (choice == 0)
            {
                _databaseServer.CurrentProfile = null;
            }

            _queryLogout = false;
            SetNeedsDraw();
            args.Handled = true;
        };
        Add(accountLabel, _accountLabel, tokenLabel, _tokenLabel, logout);
        SubViewLayout += (_, _) => UpdateData();
    }

    private void UpdateData()
    {
        _accountLabel.Text = _databaseServer.CurrentProfile?.Account ?? "无";
        _tokenLabel.Text = (_databaseServer.CurrentProfile?.Token ?? 0).ToString();
    }
}