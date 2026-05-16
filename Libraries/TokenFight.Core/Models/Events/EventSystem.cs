using TokenFight.Core.Consoles.Display;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Events;
using TokenFight.Core.Interfaces.FStream;

namespace TokenFight.Core.Models.Events;

/// <summary> 游戏事件系统 </summary>
public class EventSystem(ILocalLog localLog) : IEventSystem
{
    public void Init()
    {
        foreach (EventType eventType in Enum.GetValues<EventType>())
        {
            EventCallbacks[eventType] = _ => { };
        }
    }

    public void Reset()
    {
        // foreach (EventType type in EventCallbacks.Keys.ToArray())
        // {
        //     EventCallbacks.Remove(type);
        //     EventCallbacks[type] = _ => { };
        // }
    }

    public void Trigger(EventType eventType, IContext context)
    {
        try
        {
            localLog.Detail($"[游戏事件 {eventType.ToString()}] " + "");
            // $"\n\t> sender: {IGameOutput.MakeString(sender)}" +
            // $"\n\t> EventData: {IGameOutput.MakeString(data)}");
        }
        catch (Exception e)
        {
            localLog.LogError($"记录事件系统战斗详情日志时出现错误: {e.Message}\n\t> stack: {e.StackTrace}");
            throw;
        }
        try
        {
            EventCallbacks[eventType].Invoke(context);
        }
        catch (Exception e)
        {
            string msg = $"触发游戏事件{eventType.ToString()}时发生错误: {e.Message}\n\t> 堆栈: {e.StackTrace}";
            var printer = new ConsolePrinter();
            printer.Add(msg, ConsoleColor.Red).Display();
            localLog.LogError(msg);
        }
    }

    /// <summary> 事件回调函数 </summary>
    protected readonly Dictionary<EventType, EventCallback> EventCallbacks = new();

    public bool Subscribe(EventType eventType, EventCallback callback)
    {
        if (eventType == EventType.Null) return false;
        EventCallbacks[eventType] += callback;
        return true;
    }

    public bool Unsubscribe(EventType eventType, EventCallback callback)
    {
        if (eventType == EventType.Null) return false;
        if (EventCallbacks.ContainsKey(eventType))
        {
            try
            {
                EventCallbacks[eventType] -= callback;
                return true;
            }
            catch (Exception e)
            {
                localLog.LogError($"注销游戏事件时发生错误: {e.Message}\n\t> 堆栈: {e.StackTrace}");
            }
        }

        return false;
    }
}