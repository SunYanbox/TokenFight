using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TokenFight.Core.Databases.Interfaces;

namespace TokenFight.UI.Core.Profile;

public class LoginView: View
{
    private IDatabaseServer _databaseServer;
    private TextField _accountTextField;
    private TextField _passwordTextField;
    private Button _loginButton;
    private Button _registerButton;
    private TableView _accountTableView;

    public LoginView(IDatabaseServer databaseServer)
    {
        _databaseServer = databaseServer;
        CanFocus = true;

        Width = Dim.Fill();
        Height = Dim.Fill();

        var usernameLabel = new Label { Text = "账号:" };

        _accountTextField = new TextField
        {
            Text = "admin",
            X = Pos.Right(usernameLabel) + 1,
            Width = Dim.Absolute(20),
            CanFocus = true
        };

        var passwordLabel = new Label
        {
            Text = "密码:", X = Pos.Left(usernameLabel), Y = Pos.Bottom(usernameLabel) + 1
        };

        _passwordTextField = new TextField
        {
            Text = "123456",
            Secret = true,
            X = Pos.Left(_accountTextField),
            Y = Pos.Top(passwordLabel),
            Width = Dim.Absolute(20),
            CanFocus = true
        };

        // Create login button
        _loginButton = new Button
        {
            Text = "登录",
            Y = Pos.Bottom(passwordLabel) + 1,

            // center the login button horizontally
            X = 0,
            IsDefault = true
        };

        // Create login button
        _registerButton = new Button
        {
            Text = "注册",
            Y = Pos.Bottom(passwordLabel) + 1,

            // center the login button horizontally
            X = Pos.Right(_loginButton),
            IsDefault = true
        };

        _accountTableView = new TableView
        {
            NullSymbol = "无",
            Y = Pos.Bottom(_registerButton) + 1,
            X = 0,
            Width = Dim.Absolute(50),
            Height = Dim.Auto(minimumContentDim: 20)
        };

        // When login button is clicked display a message popup
        _loginButton.Accepting += (s, args) =>
        {
            object oLock = new();
            App!.Invoke(() =>
            {
                lock (oLock)
                {
                    if (_databaseServer.TryLogin(_accountTextField.Text, _passwordTextField.Text))
                    {
                        UIUtil.QueryAtMainLoop(this, "登录", $"登录成功: {_accountTextField.Text}", "确认");
                    }
                    else
                    {
                        OutputLoginOrRegisterError("登录");
                    }
                }
            });
            SetTableView();
            args.Handled = true;
        };

        _registerButton.Accepting += (s, args) =>
        {
            if (_databaseServer.Register(_accountTextField.Text, _passwordTextField.Text))
            {
                _databaseServer.Save(_databaseServer.ProfileTables[_accountTextField.Text]);
                UIUtil.QueryAtMainLoop(this, "注册", $"注册成功: {_accountTextField.Text}", "确认");
            }
            else
            {
                OutputLoginOrRegisterError("注册");
            }

            SetTableView();
            args.Handled = true;
        };

        // Add the views to the Window
        Add(usernameLabel, _accountTextField, passwordLabel,
            _passwordTextField, _loginButton,
            _registerButton, _accountTableView);
        SetTableView();
    }

    private void OutputLoginOrRegisterError(string title)
    {
        string errorMessage = $"{title}失败:";
        bool accountExists = _databaseServer.ProfileTables.ContainsKey(_accountTextField.Text);
        bool invalidLength = _accountTextField.Text.Length < 4 || _accountTextField.Text.Length > 16 ||
                             _passwordTextField.Text.Length < 4 || _passwordTextField.Text.Length > 16;
        bool invalidAccountChars = !_accountTextField.Text.All(char.IsLetterOrDigit);
        bool invalidPasswordChars = !_passwordTextField.Text.All(c => c >= 32 && c <= 126);

        if (accountExists && title != "登录")
            errorMessage += "\n- 账号已存在";
        if (invalidLength)
            errorMessage += "\n- 账号或密码长度不在[4, 16]之间";
        if (invalidAccountChars)
            errorMessage += "\n- 账号包含非字母数字字符";
        if (invalidPasswordChars)
            errorMessage += "\n- 密码包含非ASCII可打印字符";
        UIUtil.ErrorQueryAtMainLoop(this, title, errorMessage, "确认");
    }

    private void SetTableView()
    {
        string[] accounts = _databaseServer.ProfileTables.Keys.ToArray();

        _accountTableView.Table = new EnumerableTableSource<string>(accounts,
            new Dictionary<string, Func<string, object>>
            {
                { "序号", t => accounts.IndexOf(t) },
                { "账号", t => t },
                { "Token", t => _databaseServer.ProfileTables.GetValueOrDefault(t)?.Token ?? 0 }
            });
    }
}