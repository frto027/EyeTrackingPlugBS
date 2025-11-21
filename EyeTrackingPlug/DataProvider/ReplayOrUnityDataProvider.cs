using System;
using JetBrains.Annotations;
using Zenject;

namespace EyeTrackingPlug.DataProvider;

public class ReplayOrUnityDataProvider: IEyeDataProvider, IInitializable, IDisposable
{
    public static IEyeDataProvider? Instance { get; private set; } = null!;
    
    [Inject]
    UnityEyeDataProvider _eyeDataProvider = null!;

    public BeatLeaderReplayDataProvider? replayProvider = null;
    
    public bool GetData(out EyeTrackingData data)
    {
        if(replayProvider != null && replayProvider.HasData())
            return replayProvider.GetData(out data);
        return _eyeDataProvider.GetData(out data);
    }

    public void Initialize()
    {
        Instance = this;
    }

    public void Dispose()
    {
        Instance = null!;
    }

}