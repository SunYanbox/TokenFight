using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TokenFight.Core.Constants;
using TokenFight.Core.Databases.Models.Profiles;
using TokenFight.Core.Enums.Build;
using TokenFight.Core.Interfaces.Systems.Build;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Build;

namespace TokenFight.UI.Core.ProfileAbout;

public sealed class GachaView: TabView
{
    private static GameSystemRegistry? _gameSystemRegistry;

    public GachaView(GameSystemRegistry gameSystemRegistry)
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        _gameSystemRegistry ??= gameSystemRegistry;
        bool select = true;
        foreach (string key in gameSystemRegistry.DatabaseServer.GachaRewardTables.Keys)
        {
            AddTab(new GachaTab(key), select);
            select = false;
        }
    }

    private sealed class GachaTab: Tab
    {
        private static IGachaSystem? _gachaSystem;
        private readonly Button _gachaOnce;
        private readonly Button _gachaTen;
        private readonly Button _gachaN;

        public GachaTab(string id)
        {
            _gachaSystem ??= _gameSystemRegistry!.GachaSystem;
            GachaReward? gachaReward = _gameSystemRegistry!.DatabaseServer.GachaRewardTables.GetValueOrDefault(id);
            if (gachaReward == null) throw new Exception($"卡池不存在: {id}");
            DisplayText = $"卡池{gachaReward.Name}";

            #region Tab内部界面
            var frameView = new FrameView
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var label = new Label
            {
                Width = Dim.Fill(),
                Height = Dim.Auto(),
                Text = gachaReward.Name
            };
            frameView.Add(label);
            foreach (GachaRate value in Enum.GetValues<GachaRate>())
            {
                label = new Label
                {
                    Y = Pos.Bottom(label),
                    Width = Dim.Fill(),
                    Height = Dim.Auto(),
                    Text = $"{value} : {string.Join(", ", gachaReward.GetReward(value))}"
                };
                frameView.Add(label);
            }

            _gachaOnce = new Button
            {
                Y = Pos.Bottom(label),
                Text = "单抽"
            };
            _gachaTen = new Button
            {
                X = Pos.Right(_gachaOnce),
                Y = Pos.Y(_gachaOnce),
                Text = "十连抽"
            };
            label = new Label
            {
                X = Pos.Right(_gachaTen),
                Y = Pos.Y(_gachaOnce),
                Text = $"N连抽的抽数: "
            };
            var gachaCount = new TextField
            {
                Text = "20",
                Width = Dim.Auto(minimumContentDim: 5),
                CanFocus = true,
                X = Pos.Right(label),
                Y = Pos.Y(_gachaOnce)
            };
            _gachaN = new Button
            {
                X = Pos.Right(gachaCount),
                Y = Pos.Y(_gachaOnce),
                Text = "N连抽"
            };
            frameView.Add(_gachaOnce, _gachaTen, label, gachaCount, _gachaN);
            View = frameView;
            #endregion

            #region 按钮事件
            _gachaOnce.Accepting += (_, args) =>
            {
                Profile profile = _gameSystemRegistry.DatabaseServer.CurrentProfile!;
                args.Handled = true;
                if (profile.Token < GameConst.GachaTokenCost)
                {
                    UIUtil.QueryAtMainLoop(this, "抽卡", $"抽卡资源不足{GameConst.GachaTokenCost}: 当前只有{profile.Token}", "确认");
                    return;
                }
                GachaResult result = _gachaSystem.GachaOnce(profile, gachaReward);
                UIUtil.QueryAtMainLoop(this, "抽卡",
                    $"抽卡结果: {result.Rate} {result.ItemTpl} x{result.Count}", "确认");
            };
            _gachaTen.Accepting += (_, args) =>
            {
                Profile profile = _gameSystemRegistry.DatabaseServer.CurrentProfile!;
                args.Handled = true;
                if (profile.Token < GameConst.GachaTokenCost * 10)
                {
                    UIUtil.QueryAtMainLoop(this, "抽卡", $"抽卡资源不足{GameConst.GachaTokenCost * 10}: 当前只有{profile.Token}", "确认");
                    return;
                }
                IEnumerable<GachaResult> result = _gachaSystem.Gacha(profile, gachaReward, 10);
                UIUtil.QueryAtMainLoop(this, "抽卡",
                    $"抽卡结果: \n - {string.Join("\n - ",
                        result.Select(x => $"{x.Rate} {x.ItemTpl} x{x.Count}"))}", "确认");
            };
            _gachaN.Accepting += (_, args) =>
            {
                Profile profile = _gameSystemRegistry.DatabaseServer.CurrentProfile!;
                args.Handled = true;
                int n;
                try
                {
                    n = int.Parse(gachaCount.Text);
                }
                catch (Exception e)
                {
                    UIUtil.ErrorQueryAtMainLoop(this, "抽卡", $"输入错误: {e.Message}", "确认");
                    Console.WriteLine($"输入错误: {e.Message}");
                    gachaCount.Text = "20";
                    return;
                }

                if (profile.Token < GameConst.GachaTokenCost * n)
                {
                    UIUtil.QueryAtMainLoop(this, "抽卡", $"抽卡资源不足{GameConst.GachaTokenCost * n}: 当前只有{profile.Token}", "确认");
                    return;
                }
                IEnumerable<GachaResult> result = _gachaSystem.Gacha(profile, gachaReward, n);
                UIUtil.QueryAtMainLoop(this, "抽卡",
                    $"抽卡结果: \n - {string.Join("\n - ",
                        result.Select(x => $"{x.Rate} {x.ItemTpl} x{x.Count}"))}", "确认");
            };
            #endregion
        }
    }
}