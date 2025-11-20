using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.XR;
using Zenject;

namespace EyeTrackingPlug;

public struct RecordItem
{
    public float songTime;
    public Vector3 positionL, positionR;
    public Quaternion rotationL, rotationR;

    public void Encode(BinaryWriter binaryWriter)
    {
        var v3 = (Vector3 v) =>
        {
            binaryWriter.Write(v.x);
            binaryWriter.Write(v.y);
            binaryWriter.Write(v.z);
        };
        var q4 = (Quaternion q) =>
        {
            binaryWriter.Write(q.x);
            binaryWriter.Write(q.y);
            binaryWriter.Write(q.z);
            binaryWriter.Write(q.w);
        };
        binaryWriter.Write(songTime);
        v3(positionL);
        q4(rotationL);
        v3(positionR);
        q4(rotationR);
    }
}

public class RecorderInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<EyeTrackingRecorder>().AsSingle();
    }
}

public class EyeTrackingRecorder : ITickable, IDisposable
{
    public bool recording = false;
    
    private List<InputDevice> _devices = new List<InputDevice>();

    public List<RecordItem> records = new List<RecordItem>();
    
    IBeatLeaderProxy _beatLeaderProxy;
    
    AudioTimeSyncController _audioTimeSyncController;

    void flushDev()
    {
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking, _devices);
    }
    
    EyeTrackingRecorder(IBeatLeaderProxy iProxy, AudioTimeSyncController audioTimeSyncController)
    {
        
        // InputDevices.deviceConfigChanged += (d) => flushDev();
        // InputDevices.deviceConnected += (d) => flushDev();
        // InputDevices.deviceDisconnected += (d) => flushDev();
        
        flushDev();
        records.Clear();
        
        _beatLeaderProxy = iProxy;

        _beatLeaderProxy.OnFinalizeReplay += FinalizeReplay;
        _audioTimeSyncController =  audioTimeSyncController;
        
        
    }

    public void Dispose()
    {
        _beatLeaderProxy.OnFinalizeReplay -= FinalizeReplay;
        
        // InputDevices.deviceConfigChanged -= (d) => flushDev();
        // InputDevices.deviceConnected -= (d) => flushDev();
        // InputDevices.deviceDisconnected -= (d) => flushDev();
    }

    void FinalizeReplay()
    {
        if (!recording)
            return;
        if(records.Count == 0)
            return;
        // I don't want record any data for now...
        Plugin.Log.Notice($"{EncodeData().Length} bytes will be recorded. We don't record them for now.");
        //_beatLeaderProxy.TryWriteCustomDataStatic("EyeTrackingR", EncodeData());
        records.Clear();
    }

    private byte[] EncodeData()
    {
        using var memoryStream = new MemoryStream();
        using var binaryWriter = new BinaryWriter(memoryStream, Encoding.ASCII, true);
        binaryWriter.Write(1);//version
        binaryWriter.Write(records.Count);//count
        foreach (var record in records)
        {
            record.Encode(binaryWriter);
        }
        
        //etAgent
        var bytes = Encoding.UTF8.GetBytes(Plugin.EtAgent);
        binaryWriter.Write(bytes.Length);
        binaryWriter.Write(bytes);
        
        return memoryStream.ToArray();
    }

    public void Tick()
    {
        if(!recording)
            return;
        if (_devices.Count == 0)
            return;
        InputDevice device = _devices[0];
        if(!device.TryGetFeatureValue(CommonUsages.eyesData, out Eyes eyes))
            return;
        if(!eyes.TryGetLeftEyePosition(out Vector3 leftEyePosition))
            return;
        if (!eyes.TryGetLeftEyeRotation(out Quaternion leftEyeRotation))
            return;
        if (!eyes.TryGetRightEyePosition(out Vector3 rightEyePosition)) 
            return;
        if (!eyes.TryGetRightEyeRotation(out Quaternion rightEyeRotation))
            return;

        if (_audioTimeSyncController.state == AudioTimeSyncController.State.Playing)
        {
            records.Add(new RecordItem()
            {
                positionL = leftEyePosition,
                positionR = rightEyePosition,
                rotationL = leftEyeRotation,
                rotationR = rightEyeRotation
            });
        }
    }
}