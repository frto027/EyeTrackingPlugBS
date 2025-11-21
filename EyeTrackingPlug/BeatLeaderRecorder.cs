using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BeatLeader;
using BeatLeader.Models.Replay;
using EyeTrackingPlug.DataProvider;
using IPA.Loader;
using SiraUtil.Zenject;
using Zenject;

namespace EyeTrackingPlug.BeatLeaderRecorder;

public class BeatLeaderRecorderInstaller : Installer
{
    public static void PluginInit(Zenjector zenjector)
    {
        if (PluginManager.IsEnabled(PluginManager.GetPluginFromId("BeatLeader")))
            zenjector.Install<BeatLeaderRecorderInstaller>(Location.Singleplayer);
    }
    public override void InstallBindings()
    {
        Container.Bind<BeatLeaderRecorder>().AsSingle();
    }

}

struct RecordedEyeTrackingData
{
    public float SongTime;
    public EyeTrackingData EyeTrackingData;
}

public class BeatLeaderRecorder : ITickable, IInitializable, IDisposable
{
    [Inject]
    private readonly ReplayRecorder? _recorder = null!;

    [Inject]
    private readonly UnityEyeDataProvider? _eyeDataProvider = null!;
    
    [Inject]
    private readonly AudioTimeSyncController _audioTimeSyncController = null!;
    
    private bool _recordEnabled = false;
    
    private readonly List<RecordedEyeTrackingData> _records = new List<RecordedEyeTrackingData>();

    public void Initialize()
    {
        // The beatleader will not install the recorder if beatleader or scoresaber is in replay mode.
        _recordEnabled = _recorder != null;
        
        
        _recorder?.OnFinalizeReplay += OnFinalizeReplay;
        
        Plugin.Log.Debug($"Initializing Eye DataProvider, _recordEnabled: {_recordEnabled}");

    }
    public void Dispose()
    {
        Plugin.Log.Debug("Disposing Eye DataProvider");
        _recorder?.OnFinalizeReplay -= OnFinalizeReplay;
    }

    public void Tick()
    {
        if(!_recordEnabled)
            return;
        if (_eyeDataProvider!.GetData(out EyeTrackingData eyeTrackingData))
        {
            _records.Add(new  RecordedEyeTrackingData()
            {
                SongTime = _audioTimeSyncController.songTime,
                EyeTrackingData = eyeTrackingData
            });
        }
    }

    private void OnFinalizeReplay()
    {
        if(!_recordEnabled)
            return;
        using var memoryStream = new MemoryStream();
        using var binaryWriter = new BinaryWriter(memoryStream, Encoding.ASCII, true);

        var i32 = (Int32 i) => binaryWriter.Write(i);
        var f32 = (float f) => binaryWriter.Write(f);
        var v3 = (UnityEngine.Vector3 v) => {f32(v.x);f32(v.y);f32(v.z);};
        var q4 = (UnityEngine.Quaternion q) => {f32(q.x);f32(q.y);f32(q.z);f32(q.w);};

        i32(1);//version
        i32(_records.Count);//count
        
        foreach (var record in _records)
        {
            f32(record.SongTime);
            v3(record.EyeTrackingData.LeftPosition);
            q4(record.EyeTrackingData.LeftRotation);
            v3(record.EyeTrackingData.RightPosition);
            q4(record.EyeTrackingData.RightRotation);
        }
        
        //etAgent
        var bytes = Encoding.UTF8.GetBytes(Plugin.EtAgent);
        i32(bytes.Length);
        binaryWriter.Write(bytes);
        
        var customDataBytes = memoryStream.ToArray();
        
        Plugin.Log.Notice($"Recorded {customDataBytes.Length} bytes(not recorded for debug purpose)");
        //recorder.TryWriteCustomData("EyeTrackingR", memoryStream.ToArray());
    }
}