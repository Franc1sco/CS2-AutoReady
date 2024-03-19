using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;

namespace CS2_AutoReady;


[MinimumApiVersion(179)]
public class CS2_AutoReady : BasePlugin
{
    public override string ModuleName => "CS2 Auto Ready";
    public override string ModuleAuthor => "Franc1sco Franug";
    public override string ModuleVersion => "0.0.1";

    internal static Dictionary<int, bool> g_bReady = new Dictionary<int, bool>();


    public override void Load(bool hotReload)
    {
        if (hotReload)
        {
            Utilities.GetPlayers().ForEach(player =>
            {
                g_bReady.Add((int)player.Index, false);
            });
        }
        RegisterEventHandler<EventPlayerConnectFull>((@event, info) =>
        {
            var player = @event.Userid;

            if (player.IsBot || !player.IsValid)
            {
                return HookResult.Continue;

            }
            else
            {
                g_bReady.Add((int)player.Index, false);
                return HookResult.Continue;
            }
        });

        RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
        {
            var player = @event.Userid;

            if (player.IsBot || !player.IsValid)
            {
                return HookResult.Continue;

            }
            else
            {
                if (g_bReady.ContainsKey((int)player.Index))
                {
                    g_bReady.Remove((int)player.Index);
                }
                return HookResult.Continue;
            }
        });

        RegisterEventHandler<EventPlayerSpawn>(eventPlayerSpawn);
    }

    private HookResult eventPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (!IsPlayerValid(player))
        {
            return HookResult.Continue;
        }

        Server.NextFrame(() =>
        {
            if (!IsPlayerValid(player) || g_bReady[(int)player.Index])
            {
                return;
            }

            NativeAPI.IssueClientCommand((int)player.Index - 1, "css_ready");
            g_bReady[(int)player.Index] = true;
        });

        return HookResult.Continue;
    }

    private bool IsPlayerValid(CCSPlayerController? player)
    {
        return (player != null && player.IsValid && !player.IsBot && !player.IsHLTV && player.PawnIsAlive);
    }
}

