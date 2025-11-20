using System.Linq;
using IPA;
using IPA.Loader;
using IPA.Utilities;
using SiraUtil.Zenject;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;
using IpaLogger = IPA.Logging.Logger;

namespace EyeTrackingPlug;

[Plugin(RuntimeOptions.SingleStartInit)]
internal class Plugin
{
    internal static IpaLogger Log { get; private set; } = null!;

    public static string EtAgent = "";
    
    [Init]
    public Plugin(Zenjector zenjector, IpaLogger ipaLogger, PluginMetadata pluginMetadata)
    {
        Log = ipaLogger;
        
        OpenXRRestarter.Instance.onAfterShutdown += EyeGazeEnabler;
        zenjector.UseLogger(ipaLogger);
        zenjector.Install<RecorderInstaller>(Location.GameCore);
        if (PluginManager.IsEnabled(PluginManager.GetPluginFromId("BeatLeader")))
            zenjector.Install<BeatLeaderProxyInstaller>(Location.GameCore);
        
        EtAgent = pluginMetadata.Name + "/" + pluginMetadata.HVersion;
        
        Log.Info($"{pluginMetadata.Name} {pluginMetadata.HVersion} initialized, etAgent: {EtAgent}");
    }

    private static void EyeGazeEnabler()
    {
        var profile = OpenXRSettings.Instance.features.First((f => f is EyeGazeInteraction));
        profile.enabled = true;
    }
    
    [OnStart]
    public void OnApplicationStart()
    {
        Log.Debug("OnApplicationStart");

        if (OpenXRSettings.Instance.features.First((f => f is EyeGazeInteraction)).enabled ||
            OpenXRRestarter.Instance.isRunning)
        {
            // Lucky. If other mods or something did/doing the OpenXR restart, we don't need do it.
        }
        else
        {
            OpenXRRestarter.Instance.PauseAndShutdownAndRestart();
        }
    }

    [OnExit]
    public void OnApplicationQuit()
    {
        Log.Debug("OnApplicationQuit");
        // OpenXRRestarter.Instance.onAfterShutdown -= EyeGazeEnabler;
    }
}
