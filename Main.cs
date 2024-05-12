using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Timers;
using CrashPlayerSharpApi;
using System.Text.Json.Serialization;



namespace CrashPlayerAutocrash;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("Autocrash")] public List<string> players { get; set; } = new List<string>(["Nick Fox", "STEAM_1:1:0", "7123456789"]);
}

public class CrashPlayerCmd : BasePlugin, IPluginConfig<PluginConfig>
{
    public PluginConfig Config { get; set; }
    public override string ModuleName => "CrashPlayer CSharp Autocrash [Module]";
    public override string ModuleVersion => "0.1";

    private ICrashPlayerApi? cpc;
    private readonly PluginCapability<ICrashPlayerApi> pluginCPC = new("crashplayer:nfcore");


    public void OnConfigParsed(PluginConfig config)
    {
        Config = config;
    }



    public override void OnAllPluginsLoaded(bool hotReload)
    {
        cpc = pluginCPC.Get();
        RegisterListener<Listeners.OnClientAuthorized>(OnClientAuthorized);
    }

    public void OnClientAuthorized(int playerSlot, SteamID steamId)
    {
        var player = Utilities.GetPlayerFromSlot(playerSlot);
        if (player != null)
        {
            Config.players.ForEach(s =>
            {
                if (s.Equals(player.SteamID.ToString()) || s.Equals(player.PlayerName) || s.Equals(player.IpAddress))
                {
                    var timer = new CounterStrikeSharp.API.Modules.Timers.Timer(4.0f, () => {
                        Console.WriteLine($"Крашим игрока {player.PlayerName}");
                        cpc.CPC_CrashPlayer(player);
                    });
                    return;
                }
            });
        }
    }
}