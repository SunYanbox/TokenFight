using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Timeout = Terminal.Gui.App.Timeout;

namespace TokenFight.UI.Core;

public static class UIUtil
{
    public static int? QueryAtMainLoop(View view, string title, string message, params string[] buttons)
    {
        int? result = null;
        try
        {
            IApplication? app = view.App;
            app?.Invoke(() =>
            {
                result = MessageBox.Query(app, title, message, buttons);
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return result;
    }
    
    
    public static int? ErrorQueryAtMainLoop(View view, string title, string errorMessage, params string[] buttons)
    {
        int? result = null;
        try
        {
            IApplication? app = view.App;
            app?.Invoke(() =>
            {
                result = MessageBox.ErrorQuery(app, title, errorMessage, buttons);
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return result;
    }
}