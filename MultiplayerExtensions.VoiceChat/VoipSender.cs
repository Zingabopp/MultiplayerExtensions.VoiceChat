using MultiplayerExtensions.VoiceChat.Codecs;
using MultiplayerExtensions.VoiceChat.Configuration;
using MultiplayerExtensions.VoiceChat.Networking;
using MultiplayerExtensions.VoiceChat.Utilities;
using MultiplayerExtensions.VoiceChat.Utilities.Input;
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
            get { return _enabled && _voiceSettings.EnableVoiceChat; }
            internal set { _enabled = value; }
        }

        private IEncoder _encoder;
        private ICodecFactory _codecFactory;
        private IInputController _inputController;
        private IVoiceSettings _voiceSettings;

        private AudioClip? recording;
        private float[]? recordingBuffer;
        private float[]? resampleBuffer;
        private string? _usedMicrophone;
        private int lastPos = 0;
        private int index;
        public int inputFreq;
        public event EventHandler<VoipDataPacket>? OnAudioGenerated;
        protected readonly System.Buffers.ArrayPool<byte> ByteAryPool = System.Buffers.ArrayPool<byte>.Shared;
        private bool _recordingActive;
        private bool _isListening;

        public bool IsListening
        {
            get
            {
                return _isListening && _voiceSettings.MicEnabled;
            }
            set
            {
                if (value == _isListening)
                    return;
#if DEBUG
                Plugin.Log?.Debug($"IsListening changed: {value}");
#endif
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
                if (value == null || value.Length == 0)
                    value = null;
                if (_usedMicrophone == value)
                    return;
                if(value != null)
                {
                    string device = Microphone.devices.Where(d => d.Equals(value, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (device != null)
                    {
                        _usedMicrophone = device;
                        if (_recordingActive)
                        {
                            StopRecording();
                            StartRecording();
                        }
                    }
                    else
                        Plugin.Log?.Warn($"'{value}' does not appear to be a valid microphone.");
                }

            }
        }

        public VoipSender(ICodecFactory codecFactory, IEncoder encoder, IVoiceSettings voiceSettings, IInputController inputController)
        {
            Plugin.Log?.Info("Created VoipSender");
            _codecFactory = codecFactory;
            _encoder = encoder;
            _inputController = inputController;
            _voiceSettings = voiceSettings;
            _voiceSettings.VoiceSettingChanged += OnVoiceSettingChanged;
            SelectedMicrophone = _voiceSettings.VoiceChatMicrophone;
        }

        private void OnVoiceSettingChanged(object sender, string e)
        {
            if (e == nameof(IVoiceSettings.VoiceChatMicrophone))
                SelectedMicrophone = _voiceSettings.VoiceChatMicrophone;
        }

        public void StartRecording()
        {
            Plugin.Log?.Info("StartRecording()");
            if (Microphone.devices.Length == 0)
                return;
            if ((_usedMicrophone?.Length ?? -1) == 0)
                _usedMicrophone = null;
            inputFreq = AudioUtils.GetFreqForMic(_usedMicrophone);
            if (_encoder == null || _encoder.SampleRate != inputFreq)
                _encoder = _codecFactory.CreateEncoder(inputFreq, 1);
            int sizeRequired = (inputFreq * 1 * _encoder.FrameDuration) / 1000;
            recordingBuffer = new float[sizeRequired];
            if (sizeRequired != _encoder.FrameSize)
                Plugin.Log?.Warn($"{sizeRequired} != {_encoder.FrameSize}");
            if (inputFreq != _encoder.SampleRate)
            {
                Plugin.Log?.Debug($"Upscaling input frequency {inputFreq}Hz to {_encoder.SampleRate}Hz");
                resampleBuffer = new float[_encoder.FrameSize];
            }
            else
                resampleBuffer = null;
            if (recording != null)
                GameObject.Destroy(recording);
            recording = Microphone.Start(_usedMicrophone, true, 20, AudioUtils.GetFreqForMic(_usedMicrophone));
            Plugin.Log?.Debug($"Used microphone: {(_usedMicrophone ?? "DEFAULT")} | AudioClip channels: {recording.channels}");
            Plugin.Log?.Debug($"Used mic sample rate: {inputFreq}Hz | Encoding SampleRate: {_encoder.SampleRate}Hz");
            Plugin.Log?.Debug($"Used buffer size for recording: " + sizeRequired + " floats");
            _recordingActive = true;
        }

        public void SetEncoder(IEncoder encoder)
        {
            StopRecording();
            _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
            StartRecording();
        }

        public void StopRecording()
        {
            Microphone.End(_usedMicrophone);
            _recordingActive = false;
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
            //if (!_inputController.UsePushToTalk)
            //{
            //    // Test if talk should be enabled
            //}
            //else

            if (!_inputController.UsePushToTalk || _inputController.TalkEnabled) // For now just always enable talk if PTT disabled
            {
                if (!IsListening && !_inputController.UsePushToTalk)
                    _inputController.TriggerFeedback();
                IsListening = true;
            }
            else if (Input.GetKey(KeyCode.K))
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
                        float[] buffer = recordingBuffer;
                        // Upsample if needed.
                        if (resampleBuffer != null)
                        {
                            AudioUtils.Resample(recordingBuffer, resampleBuffer, inputFreq, _encoder.SampleRate);
                            buffer = resampleBuffer;
                        }
                        byte[] data = ByteAryPool.Rent(1276);
                        //AudioUtils.Convert(recordingBuffer, pcmBytes);
                        int dataLength = _encoder.Encode(buffer, 0, _encoder.FrameSize, data, 0, 1276);
                        if (dataLength == 0)
                            Plugin.Log?.Warn($"Why is DataLength 0?");
                        VoipDataPacket frag = VoipDataPacket.Create(index, data, dataLength);

                        OnAudioGenerated?.Invoke(this, frag);
                        ByteAryPool.Return(data);
                    }
                }
                length -= recordingBuffer.Length;
                lastPos += recordingBuffer.Length;
            }
        }

    }
}
