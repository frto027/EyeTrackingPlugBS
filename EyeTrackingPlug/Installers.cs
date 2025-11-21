using System;
using EyeTrackingPlug.DataProvider;
using IPA.Loader;
using Zenject;

namespace EyeTrackingPlug;

public class AppInstaller : Installer<AppInstaller>
{
    public override void InstallBindings()
    {
        // There is two data sources
        
        // the first is UnityEyeDataProvider, which reads eye tracking data from unity directly.
        // use this if you want to interact with game UI.
        Container.Bind(typeof(UnityEyeDataProvider), typeof(IInitializable), typeof(IDisposable))
            .To(typeof(UnityEyeDataProvider)).AsSingle();
        
        // the seconed is RecordOrUnityDataProvider, which reads eye tracking data not only fron unity, but also the replay file of beatleader.
        Container.Bind(typeof(ReplayOrUnityDataProvider), typeof(IInitializable), typeof(IDisposable))
            .To<ReplayOrUnityDataProvider>().AsSingle();
    }
}

public class SinglePlayerInstaller : Installer<SinglePlayerInstaller>
{
    public override void InstallBindings()
    {
        if (PluginManager.IsEnabled(PluginManager.GetPluginFromId("BeatLeader")))
        {
            Plugin.Log.Info("Beatleader detected, installing BeatLeaderReplayDataProvider.");
            Container.Bind(typeof(BeatLeaderReplayDataProvider), typeof(IInitializable), typeof(IDisposable))
                .To<BeatLeaderReplayDataProvider>().AsSingle();
        }
        else
        {
            Plugin.Log.Info("Beatleader not detected, don't install BeatLeaderReplayDataProvider.");
        }
        
    }
}