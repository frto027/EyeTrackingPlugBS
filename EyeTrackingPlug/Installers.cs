using System;
using EyeTrackingPlug.DataProvider;
using IPA.Loader;
using Zenject;

namespace EyeTrackingPlug;

public class AppInstaller : Installer<AppInstaller>
{
    public override void InstallBindings()
    {
        // There are two data sources.
        
        // the first is UnityEyeDataProvider, which reads eye tracking data from unity directly.
        // use this if you want to interact with game UI.
        Container.BindInterfacesAndSelfTo<UnityEyeDataProvider>().AsSingle();
        
        // the seconed is RecordOrUnityDataProvider, which reads eye tracking data not only fron unity, but also the replay file of beatleader if avaliable.
        // use this if you want display some eye related information in game.
        Container.BindInterfacesAndSelfTo<ReplayOrUnityDataProvider>().AsSingle();
    }
}

public class SinglePlayerInstaller : Installer<SinglePlayerInstaller>
{
    public override void InstallBindings()
    {
        if (Plugin.BeatLeaderEnabled)
        {
            BeatLeaderReplayDataProvider.StaticInit();
            
            Container
                .BindInterfacesAndSelfTo<BeatLeaderReplayDataProvider>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<BeatLeaderRecorder>()
                .AsSingle();
        }

    }
}
