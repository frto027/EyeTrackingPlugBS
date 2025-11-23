using System;
using EyeTrackingPlug.DataProvider;
using Zenject;
    
namespace EyeTrackingPlug;

public class EyeDataManager : IInitializable
{
    private IEyeDataProvider _realtimeProvider = null!;
    public IEyeDataProvider RealtimeProvider => _realtimeProvider;
    public event Action<IEyeDataProvider>? OnRealtimeProviderChanged; 
    
    private ReplayOrRealtimeEyeDataProvider _replayOrRealtimeEyeDataProvider = null!;
    public ReplayOrRealtimeEyeDataProvider ReplayOrRealtimeEyeDataProvider => _replayOrRealtimeEyeDataProvider;
    

    public void Initialize()
    {
        ReplaceRealtimeEyeDataProvider(new UnityEyeDataProvider());
        _replayOrRealtimeEyeDataProvider = new ReplayOrRealtimeEyeDataProvider(this);
    }

    public void ReplaceRealtimeEyeDataProvider(IEyeDataProvider obj)
    {
        (obj as IInitializable)?.Initialize();
        var old = _realtimeProvider;
        _realtimeProvider = obj;
        OnRealtimeProviderChanged?.Invoke(obj);
        (old as IDisposable)?.Dispose();
    }
}