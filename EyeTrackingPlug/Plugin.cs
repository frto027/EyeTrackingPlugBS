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
        
        OpenXRFeatureManager.FeatureManager.instance.afterOpenXRUnloaded +=
            ()=>OpenXRSettings.Instance.features.First((f => f is EyeGazeInteraction)).enabled = true;
        
        EyeDataManager.Instance = new EyeDataManager();
        
        zenjector.UseLogger(ipaLogger);
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
