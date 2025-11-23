using System.Linq;
using EyeTrackingPlug.DataProvider;
using IPA;
using IPA.Loader;
using JetBrains.Annotations;
using SiraUtil.Zenject;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;
using IpaLogger = IPA.Logging.Logger;

namespace EyeTrackingPlug;

[Plugin(RuntimeOptions.SingleStartInit)]
[UsedImplicitly]
internal class Plugin
{
    internal static IpaLogger Log { get; private set; } = null!;

    public static string EtAgent = "";
    public static bool BeatLeaderEnabled { get; private set; } = false;
    
    [Init]
    public Plugin(Zenjector zenjector, IpaLogger ipaLogger, PluginMetadata pluginMetadata)
    {
        Log = ipaLogger;
        
        BeatLeaderEnabled = PluginManager.IsEnabled(PluginManager.GetPluginFromId("BeatLeader"));
        UnityEyeDataProvider.PluginInit();
        // Do not restart OpenXR immediately. I don't want to be too aggressive, even with the default 5-second delay before restarting.
        // If other mods also require a restart, then why not do them together later?
        
        
        zenjector.UseLogger(ipaLogger);
        
        zenjector.Install<AppInstaller>(Location.App);
        zenjector.Install<SinglePlayerInstaller>(Location.Singleplayer);
        
        EtAgent = $"{pluginMetadata.Name}/{pluginMetadata.HVersion} ({OpenXRRuntime.LibraryName},{OpenXRRuntime.name}/{OpenXRRuntime.version}/{OpenXRRuntime.apiVersion}/{OpenXRRuntime.pluginVersion})";
        
        Log.Info($"{pluginMetadata.Name} {pluginMetadata.HVersion} initialized, etAgent: {EtAgent}");
    }

    
    [OnStart]
    public void OnApplicationStart()
    {
    }
    [OnExit]
    public void OnApplicationExit()
    {
        
    }
}
