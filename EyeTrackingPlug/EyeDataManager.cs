using System;
using System.Diagnostics.CodeAnalysis;
using EyeTrackingPlug.DataProvider;
using JetBrains.Annotations;
using Zenject;
    
namespace EyeTrackingPlug;

[UsedImplicitly]
public class EyeDataManager : IInitializable
{
    public static EyeDataManager Instance = null!;
    /// <summary>
    /// This provider provides the realtime data from some hardware.
    /// </summary>
    public IEyeDataProvider RealtimeProvider => _realtimeProvider;
    public event Action<IEyeDataProvider>? OnRealtimeProviderChanged; 
    
    /// <summary>
    /// This provider provides the replay data when the game in replay mode.
    /// Or provides the realtime data in other cases.
    /// </summary>
    public ReplayOrRealtimeEyeDataProvider ReplayOrRealtimeEyeDataProvider => _replayOrRealtimeEyeDataProvider;

    private IEyeDataProvider _realtimeProvider = null!;
    private ReplayOrRealtimeEyeDataProvider _replayOrRealtimeEyeDataProvider = null!;
    

    public void Initialize()
    {
        Instance = this;
        ReplaceRealtimeEyeDataProvider(new UnityEyeDataProvider());
        _replayOrRealtimeEyeDataProvider = new ReplayOrRealtimeEyeDataProvider(this);
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public void ReplaceRealtimeEyeDataProvider(IEyeDataProvider newProvider)
    {
        (newProvider as IInitializable)?.Initialize();
        var old = _realtimeProvider;
        _realtimeProvider = newProvider;
        OnRealtimeProviderChanged?.Invoke(newProvider);
        (old as IDisposable)?.Dispose();
    }
}
