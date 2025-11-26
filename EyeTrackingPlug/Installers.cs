using System;
using EyeTrackingPlug.DataProvider;
using IPA.Loader;
using JetBrains.Annotations;
using Zenject;

namespace EyeTrackingPlug;

[UsedImplicitly]
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
