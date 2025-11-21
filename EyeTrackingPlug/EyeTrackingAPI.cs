using EyeTrackingPlug.DataProvider;
using JetBrains.Annotations;

namespace EyeTrackingPlug;

[PublicAPI]
public class EyeTrackingAPI
{
    [PublicAPI]
    public static IEyeDataProvider? ReplayableDataProvider => ReplayOrUnityDataProvider.Instance ?? UnityEyeDataProvider.Instance;
    
    [PublicAPI]
    public static IEyeDataProvider? RawDataProvider => UnityEyeDataProvider.Instance;
    
    // Indicate if current data from ReplayableDataProvider is provided a replay file
    [PublicAPI]
    public static bool IsReplayData {
        get
        {
            if(ReplayOrUnityDataProvider.Instance == null)
                return false;
            var provider = ((ReplayOrUnityDataProvider)ReplayOrUnityDataProvider.Instance).replayProvider;
            return provider != null && provider.HasData();
        }
    }

}