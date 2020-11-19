using Concentus.Structs;
using MultiplayerExtensions.VoiceChat.Networking;
using MultiplayerExtensions.VoiceChat.Utilities;
using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace MultiplayerExtensions.VoiceChat
{
    public class VoipSender : ITickable
    {
        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            internal set { _enabled = value; }
        }

        private OpusEncoder? _encoder;

        private AudioClip? recording;
        private float[]? recordingBuffer;
        //private float[]? resampleBuffer;
        private string? _usedMicrophone;
        private int lastPos = 0;
        private int index;
        public int inputFreq;
        public event EventHandler<VoipDataPacket>? OnAudioGenerated;
        protected readonly System.Buffers.ArrayPool<byte> ByteAryPool = System.Buffers.ArrayPool<byte>.Shared;

        private bool _isListening;

        public bool IsListening
        {
            get
            {
                return _isListening;
            }
            set
            {
                if (value == _isListening)
                    return;
                Plugin.Log?.Debug($"IsListening changed: {value}");
                if (!_isListening && value && recordingBuffer != null)
                {
                    index += 3;
                    lastPos = Math.Max(Microphone.GetPosition(_usedMicrophone) - recordingBuffer.Length, 0);
                }
                _isListening = value;
            }
        }

        public string? SelectedMicrophone
        {
            get => _usedMicrophone;
            set
            {
                if (_usedMicrophone == value)
                    return;
                if (value == null || value.Length == 0)
                    _usedMicrophone = null;
                else
                {
                    string device = Microphone.devices.Where(d => d.Equals(value, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (device == null)
                        Plugin.Log?.Warn($"'{value}' does not appear to be a valid microphone.");
                    else
                        _usedMicrophone = device;
                }
                
            }
        }

        public VoipSender()
        {
            Plugin.Log?.Info("Created VoipSender");
            //StartRecording();
        }

        public void StartRecording()
        {
            if (Microphone.devices.Length == 0)
                return;
            if ((_usedMicrophone?.Length ?? -1) == 0)
                _usedMicrophone = null;
            _encoder = GetEncoder(_usedMicrophone, 48000);
            float ratio = inputFreq / (float)(_encoder.SampleRate); 
            int sizeRequired = 960;
            recordingBuffer = new float[sizeRequired];
            if (recording != null)
                GameObject.Destroy(recording);
            recording = Microphone.Start(_usedMicrophone, true, 20, AudioUtils.GetFreqForMic(_usedMicrophone)); 
            Plugin.Log?.Debug("Used microphone: " + (_usedMicrophone ?? "DEFAULT"));
            Plugin.Log?.Debug("Used mic sample rate: " + inputFreq + "Hz");
            Plugin.Log?.Debug("Used buffer size for recording: " + sizeRequired + " floats");
        }

        protected OpusEncoder GetEncoder(string? deviceName, int sampleRate)
        {
            if (_encoder != null)
            {
                return _encoder;
            }
            inputFreq = AudioUtils.GetFreqForMic(deviceName);
            _encoder = new OpusEncoder(sampleRate, 2, Concentus.Enums.OpusApplication.OPUS_APPLICATION_VOIP)
            {
                Bitrate = 128000,
            };
            return _encoder;
        }
        public void StopRecording()
        {
            Microphone.End(_usedMicrophone);
            if (recording != null)
                GameObject.Destroy(recording);
            recording = null;
        }

        public void Tick()
        {
            if (!_enabled)
                return;
            if (recording == null)
                return;
            if (Input.GetKey(KeyCode.K))
                IsListening = true;
            else
                IsListening = false;
            int now = Microphone.GetPosition(_usedMicrophone);
            int length = now - lastPos;
            if (now < lastPos)
            {
                lastPos = 0;
                length = now;
            }
            while (recordingBuffer != null && length >= recordingBuffer.Length)
            {
                if (_isListening && _encoder != null && recording.GetData(recordingBuffer, lastPos))
                {
                    //Send..
                    index++;
                    if (OnAudioGenerated != null)
                    {
                        ////Downsample if needed.
                        //if (recordingBuffer != resampleBuffer)
                        //{
                        //    AudioUtils.Resample(recordingBuffer, resampleBuffer, inputFreq, AudioUtils.GetFrequency(encoder.mode));
                        //}
                        byte[] pcmBytes = ByteAryPool.Rent(1275 * 2);
                        //AudioUtils.Convert(recordingBuffer, pcmBytes);
                        int dataLength = _encoder.Encode(recordingBuffer, 0, 480, pcmBytes, 0, 1275);
                        if (dataLength == 0)
                            Plugin.Log?.Warn($"Why is DataLength 0?");
                        VoipDataPacket frag = VoipDataPacket.Create("test", index, pcmBytes, dataLength);

                        OnAudioGenerated?.Invoke(this, frag);
                        ByteAryPool.Return(pcmBytes);
                    }
                }
                length -= recordingBuffer.Length;
                lastPos += recordingBuffer.Length;
            }
        }
    }
}
