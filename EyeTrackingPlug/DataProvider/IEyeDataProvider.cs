namespace EyeTrackingPlug.DataProvider;

public interface IEyeDataProvider
{
    bool GetData(out EyeTrackingData data);
}