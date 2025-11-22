using System;
using EyeTrackingPlug.DataProvider;
using IPA.Loader;
using Zenject;

namespace EyeTrackingPlug;

public class AppInstaller : Installer<AppInstaller>
{
    public override void InstallBindings()
    {
        Container
            .Bind(typeof(UnityEyeDataProvider), typeof(IInitializable), typeof(IDisposable))
            .To<UnityEyeDataProvider>()
            .AsSingle();
        // There are two data sources.

        // the first is IEyeDataProvider, which reads eye tracking data from unity directly.
        // use this if you want to interact with game UI.
        // This provider maybe changed in the future if we have more eye data sources.
        Container
            .Bind<IEyeDataProvider>()
            .To<UnityEyeDataProvider>()
            .FromResolve();
        
        //Container.Bind<IEyeDataProvider>().To<UnityEyeDataProvider>().FromResolve();
        // the seconed is ReplayOrRawEyeDataProvider, which reads eye tracking data not only fron unity, but also the replay file of beatleader if avaliable.
        // use this if you want display some eye related information in game.
        Container
            .Bind(typeof(ReplayOrRawEyeDataProvider))
            .To<ReplayOrRawEyeDataProvider>()
            .AsSingle();
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
                .Bind(typeof(BeatLeaderReplayDataProvider), typeof(IInitializable), typeof(IDisposable))
                .To<BeatLeaderReplayDataProvider>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<BeatLeaderRecorder>()
                .AsSingle();
        }

    }
}
