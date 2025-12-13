using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TokenFight.Core.Databases.Helpers;
using TokenFight.Core.Databases.Interfaces;
using TokenFight.Core.Databases.Models.Profiles;

namespace TokenFight.UI.Core.Profile;

public sealed class ProfileView: View
{

    public ProfileView(IDatabaseServer databaseServer)
    {
        var profileInfo = new ProfileInfoView(databaseServer);
        var itemsTab = new ItemsTabView(databaseServer);

        Width = Dim.Fill();
        Height = Dim.Fill();

        itemsTab.X = Pos.Right(profileInfo);

        Add(profileInfo, itemsTab);
    }
}

internal sealed class ProfileInfoView: FrameView
{
    private readonly IDatabaseServer _databaseServer;
    private bool _queryLogout;
    private readonly Label _accountLabel;
    private readonly Label _tokenLabel;

    public ProfileInfoView(IDatabaseServer databaseServer)
    {
        _databaseServer = databaseServer;
        Title = "账号信息";
        Width = Dim.Auto(minimumContentDim: 45);
        Height = Dim.Auto(minimumContentDim: 4, maximumContentDim: 6);

        var accountLabel = new Label { Text = "账号: " };
        _accountLabel = new Label
        {
            X = Pos.Right(accountLabel),
            Y = Pos.Y(accountLabel)
        };
        var tokenLabel = new Label
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
            Y = Pos.Bottom(_accountLabel) + 1
        };
        logout.Accepting += (sender, args) =>
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

internal sealed class ItemsTabView: FrameView
{
    private static IDatabaseServer? _databaseServer;

    public ItemsTabView(IDatabaseServer databaseServer)
    {
        _databaseServer ??= databaseServer;
        Title = "背包概览";
        Width = Dim.Auto(minimumContentDim: 45);
        // Height = Dim.Auto(minimumContentDim: 6, maximumContentDim: 10);
        Height = Dim.Fill();

        var tabView = new TabView
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        tabView.AddTab(new WeaponTab(), true);
        tabView.AddTab(new RelicsTab(), false);
        tabView.AddTab(new ActorTab(), false);
        tabView.AddTab(new ResourceTab(), false);
        tabView.AddTab(new GiftTab(), false);

        Add(tabView);
    }

    private abstract class ItemTableTab: Tab
    {
        private readonly TableView _tableView = new()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        public ItemType ItemType { get; protected set; }

        protected Item[] Items => ItemHelper.GetVerifyItems(
            _databaseServer?.CurrentProfile?.Inventory.GetItemsByType(ItemType).Values.ToArray() ?? [], ItemType);

        protected ItemTableTab(string displayText, ItemType itemType)
        {
            DisplayText = displayText;
            ItemType = itemType;
            Width = Dim.Fill();
            Height = Dim.Fill();

            View = _tableView;
            SubViewLayout += (_, _) => UpdateData();
        }

        public abstract Dictionary<string, Func<Item, object>> GetColumns();

        private void UpdateData()
        {
            _tableView.Table = new EnumerableTableSource<Item>(Items, GetColumns());
        }
    }

    private sealed class WeaponTab(): ItemTableTab("武器", ItemType.Weapon)
    {
        public override Dictionary<string, Func<Item, object>> GetColumns()
        {
            return new Dictionary<string, Func<Item, object>>
            {
                { "序号", t => Items.IndexOf(t) },
                { "名称", t => t.Properties!.Name! },
                { "描述", t => t.Properties!.Desc! },
                { "叠影层数", t => t.Properties!.WeaponLayers!.ToString()! },
                { "等级", t => t.Properties!.WeaponLevel!.ToString()! }
            };
        }
    }

    private sealed class RelicsTab(): ItemTableTab("遗器", ItemType.Relics)
    {
        public override Dictionary<string, Func<Item, object>> GetColumns()
        {
            return new Dictionary<string, Func<Item, object>>
            {
                { "序号", t => Items.IndexOf(t) },
                { "名称", t => t.Properties!.Name! },
                { "描述", t => t.Properties!.Desc! },
                { "等级", t => t.Properties!.RelicsLevel!.ToString()! },
                { "主词条", t => t.Properties!.MainEntry!.ToString()! },
                { "副词条", t => string.Join(", ", t.Properties!.SubEntries!.Select(x => x.ToString())) }
            };
        }
    }

    private sealed class ActorTab(): ItemTableTab("角色", ItemType.Actor)
    {
        public override Dictionary<string, Func<Item, object>> GetColumns()
        {
            return new Dictionary<string, Func<Item, object>>
            {
                { "序号", t => Items.IndexOf(t) },
                { "名称", t => t.Properties!.Name! },
                { "描述", t => t.Properties!.Desc! },
                { "等级", t => t.Properties!.ActorLevel!.ToString()! },
                { "技能养成", t => string.Join(", ", t.Properties!.SkillLevel!.Select(x => $"{x.Key}: {x.Value}")) }
            };
        }
    }

    private sealed class ResourceTab(): ItemTableTab("资源", ItemType.Resource)
    {
        public override Dictionary<string, Func<Item, object>> GetColumns()
        {
            return new Dictionary<string, Func<Item, object>>
            {
                { "序号", t => Items.IndexOf(t) },
                { "名称", t => t.Properties!.Name! },
                { "描述", t => t.Properties!.Desc! },
                { "数量", t => $"{t.Properties!.CurrentResources}/{t.Properties!.MaxResources}" }
            };
        }
    }

    private sealed class GiftTab(): ItemTableTab("礼物", ItemType.Gift)
    {
        public override Dictionary<string, Func<Item, object>> GetColumns()
        {
            return new Dictionary<string, Func<Item, object>>
            {
                { "序号", t => Items.IndexOf(t) },
                { "名称", t => t.Properties!.Name! },
                { "描述", t => t.Properties!.Desc! },
                { "已兑换次数", t => _databaseServer!.CurrentProfile!.GiftInfos!.GetValueOrDefault(t.Properties!.GiftCode, 0) },
                { "最大兑换次数", t => t.Properties!.GiftMaxExchangeTimes! }
            };
        }
    }
}