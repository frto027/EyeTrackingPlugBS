using System;
using BeatLeader;
using Zenject;

namespace EyeTrackingPlug;

public class BeatLeaderProxyInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.Bind<IBeatLeaderProxy>().To<BeatLeaderProxy>().AsSingle();
    }
}

public interface IBeatLeaderProxy
{
    public event Action OnFinalizeReplay;
    public bool TryWriteCustomDataStatic(string key, byte[] data);
}

public class BeatLeaderProxy : IBeatLeaderProxy
{
    public event Action OnFinalizeReplay = () => { };
    
    public BeatLeaderProxy(ReplayRecorder replayRecorder)
    {
        replayRecorder.OnFinalizeReplay += () => OnFinalizeReplay?.Invoke();
    }
    
    public bool TryWriteCustomDataStatic(string key, byte[] data)
    {
        return ReplayRecorder.TryWriteCustomDataStatic(key, data);
    }
}