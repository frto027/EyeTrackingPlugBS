using JetBrains.Annotations;

namespace EyeTrackingPlug.DataProvider;

public interface IEyeDataProvider
{
    [PublicAPI]
    bool GetData(out EyeTrackingData data);
}