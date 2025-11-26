using System;
using JetBrains.Annotations;
using Zenject;

namespace EyeTrackingPlug.DataProvider;

public class ReplayOrRealtimeEyeDataProvider: IEyeDataProvider
{
    
    private IEyeDataProvider _eyeDataProvider;

    internal BeatLeaderReplayDataProvider? BLReplayProvider = null;

    public ReplayOrRealtimeEyeDataProvider(EyeDataManager manager)
    {
        _eyeDataProvider = manager.RealtimeProvider;
        manager.OnRealtimeProviderChanged += p => _eyeDataProvider = p; 
    }
    
    [PublicAPI]
    public bool IsReplayData => BLReplayProvider != null && BLReplayProvider.HasData();
    public bool GetData(out EyeTrackingData data)
    {
        if(BLReplayProvider != null && BLReplayProvider.HasData())
            return BLReplayProvider.GetData(out data);
        return _eyeDataProvider.GetData(out data);
    }
}
