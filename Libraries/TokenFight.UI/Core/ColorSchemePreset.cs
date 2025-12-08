using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace TokenFight.UI.Core;

public static class ColorSchemePreset
{
    /// <summary>
    /// 黑底白字
    /// </summary>
    public static readonly ColorScheme BlackBackgroundScheme = new()
    {
        Normal = new Attribute(Color.White, Color.Black),     // 黑底白字
        HotNormal = new Attribute(Color.BrightYellow, Color.Black),
        Focus = new Attribute(Color.Black, Color.Gray),
        HotFocus = new Attribute(Color.BrightRed, Color.Gray),
        Disabled = new Attribute(Color.Gray, Color.Black)
    };
    
    /// <summary>
    /// 白底黑字
    /// </summary>
    public static readonly ColorScheme WhiteBackgroundScheme = new()
    {
        Normal = new Attribute(Color.Black, Color.White),     // 白底黑字
        HotNormal = new Attribute(Color.BrightBlue, Color.White),
        Focus = new Attribute(Color.White, Color.Gray),
        HotFocus = new Attribute(Color.BrightRed, Color.Gray),
        Disabled = new Attribute(Color.Gray, Color.White)
    };
}