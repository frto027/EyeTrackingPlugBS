using System;
using EyeTrackingPlug.DataProvider;
using IPA.Loader;
using Zenject;

namespace EyeTrackingPlug;

public class AppInstaller : Installer<AppInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<EyeDataManager>().AsSingle();
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
                .BindInterfacesAndSelfTo<BeatLeaderRecorder.BeatLeaderRecorder>()
                .AsSingle();
        }

    }
}
