//using Concentus.Structs;
using MultiplayerExtensions.VoiceChat.Codecs;
using MultiplayerExtensions.VoiceChat.Codecs.Opus;
using MultiplayerExtensions.VoiceChat.Networking;
using MultiplayerExtensions.VoiceChat.Utilities;
using System;
using System.IO;
using UnityEngine;
using Zenject;

namespace MultiplayerExtensions.VoiceChat
{
    public class VoipReceiver : MonoBehaviour
    {
        private const int _voipDelay = 1;
        private IDecoder Decoder = null!;
        protected AudioSource? voipSource;
        private readonly FifoFloatStream _voipFragQueue = new FifoFloatStream();
        private bool _voipPlaying;
        private float[]? _voipBuffer;
        private int _lastVoipFragIndex;
        private int _silentFrames;
        private int _voipDelayCounter;
        protected readonly System.Buffers.ArrayPool<float> FloatAryPool = System.Buffers.ArrayPool<float>.Shared;

        public void Initialize(ICodecFactory codecFactory, ICodecSettings codecSettings)
        {
            enabled = false;
            _voipFragQueue.Flush();
            _voipDelayCounter = 0;
            Decoder = codecFactory.CreateDecoder(codecSettings);
            Plugin.Log?.Debug($"{name} initialized at {codecSettings.Channels}x{codecSettings.SampleRate}Hz");
            enabled = true;
        }

        public bool DecoderMatches(string decoderId, ICodecSettings codecSettings)
        {
            if (Decoder == null)
                return false;
            return Decoder.CodecId == decoderId && Decoder.SettingsMatch(codecSettings);
        }

        protected void Start()
        {
            Plugin.Log?.Critical($"VoipReceiver Started");
            voipSource = gameObject.AddComponent<AudioSource>();
            voipSource.clip = null;
            voipSource.spatialize = false;
            //voipSource.bypassEffects = true;
            //voipSource.bypassListenerEffects = true;
            //voipSource.bypassReverbZones = true;
            voipSource.loop = true;
            voipSource.volume = 1f;
            voipSource.Play();
            //thing.Decode(null, 0, 0,)
            //if (voipSender != null)
            //    voipSender.OnAudioGenerated += HandleAudioDataReceived;
            //else
            //    Plugin.Log?.Error("No VoipSender available.");
        }

        public void HandleAudioDataReceived(object sender, VoipDataPacket e)
        {
            if (!enabled)
                return;
            if (e.Data != null && e.DataLength > 0)
            {
                //if (e.Data.Length > e.DataLength)
                //    Plugin.Log?.Debug($"Data length is {e.Data.Length}, expected length is {e.DataLength}");
                if (e.Data.Length < e.DataLength)
                    Plugin.Log?.Warn($"Data length of '{e.Data.Length}' is less than the expected length of '{e.DataLength}'");
                float[] floatData = FloatAryPool.Rent(4096);
                int length = Decoder.Decode(e.Data, 0, e.DataLength, floatData, 0);
                //Plugin.Log?.Debug($"Playing fragment, length {length}x{Decoder.Channels}");
                PlayVoIPFragment(floatData, length * Decoder.Channels, e.Index);
                FloatAryPool.Return(floatData);
            }
            else
                Plugin.Log?.Warn($"HandleAudioDataReceived {(e.Data == null ? "Data was null" : $"DataLength: {e.DataLength}")}");
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                if (Decoder is OpusDecoder opus)
                {
                    if (opus.Gain < 100)
                    {
                        opus.Gain += 5;
                        Plugin.Log?.Info($"Decoder gain = {opus.Gain}");
                    }
                    else
                        Plugin.Log?.Warn($"Max gain reached ({opus.Gain})");
                }
                else
                    Plugin.Log?.Warn("No gain support");
            }
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if (Decoder is OpusDecoder opus && opus.Gain > -50)
                {
                    if (opus.Gain > -50)
                    {
                        opus.Gain -= 5;
                        Plugin.Log?.Info($"Decoder gain = {opus.Gain}");
                    }
                    else
                        Plugin.Log?.Warn($"Min gain reached ({opus.Gain})");
                }
            }
            if (voipSource != null)
            {
                if (_voipFragQueue.Length <= 0)
                {
                    _voipPlaying = false;
                }

                if (_voipPlaying)
                {
                    _silentFrames = 0;
                }
            }
            else
            {
                _silentFrames = 999;
            }
        }

        protected void OnAudioFilterRead(float[] data, int channels)
        {
            if (!enabled)
                Plugin.Log?.Warn($"Disabled, but still handling audio.");
            if (!_voipPlaying)
                return;
            //Plugin.Log?.Debug($"OnAudioFilterRead: {data.Length} | {channels} channels");
            int sampleRate = AudioSettings.outputSampleRate;
            int dataLen = data.Length / channels;
            if (_voipBuffer == null)
                _voipBuffer = new float[dataLen];

            int bufferSize = Mathf.CeilToInt(dataLen / (AudioSettings.outputSampleRate / 48000));
            if (_voipBuffer.Length < Mathf.CeilToInt(dataLen / (sampleRate / 48000)))
            {
                _voipBuffer = new float[bufferSize];// new float[];
                Plugin.Log?.Debug($"Created new VoIP player buffer ({bufferSize})! Size: {dataLen}, Channels: {channels}, Resampling rate: {sampleRate / 48000}x");
            }
            int read = _voipFragQueue.Read(_voipBuffer, 0, bufferSize);
            AudioUtils.Resample(_voipBuffer, data, read, data.Length, 48000, sampleRate, channels);
        }

        public void PlayVoIPFragment(float[] data, int dataLength, int fragIndex)
        {
            if (voipSource != null)// && !InGameOnlineController.Instance.mutedPlayers.Contains(playerInfo.playerId))
            {
                if ((_lastVoipFragIndex + 1) != fragIndex || _silentFrames > 15)
                {
#if DEBUG
                    Plugin.Log?.Info($"Starting from scratch! ((_lastVoipFragIndex + 1) != fragIndex): {(_lastVoipFragIndex + 1) != fragIndex}, (_silentFrames > 20): {_silentFrames > 20}, _lastVoipFragIndex: {_lastVoipFragIndex}, fragIndex: {fragIndex}");
#endif
                    _voipFragQueue.Flush();
                    _voipDelayCounter = 0;
                }
                else
                {
#if DEBUG && VERBOSE
                    Plugin.Log?.Info($"New data ({data.Length}) at pos {_voipFrames} while playing at {voipSource.timeSamples}, Overlap: {voipSource.timeSamples > _voipFrames && !(voipSource.timeSamples > _voipClip.samples/2 && _voipFrames < _voipClip.samples / 2)}, Delay: {_voipFrames - voipSource.timeSamples}, Speed: {voipSource.timeSamples - lastPlayPos}, Frames: {Time.frameCount - lastFrame}");

                    lastPlayPos = voipSource.timeSamples;
                    lastFrame = Time.frameCount;
#endif
                    _voipDelayCounter++;

                    if (!_voipPlaying && _voipDelayCounter >= _voipDelay)
                    {
                        _voipPlaying = true;
                    }
                }

                _lastVoipFragIndex = fragIndex;
                _silentFrames = 0;
                //if (Config.Instance.VoiceChatSettings.VoiceChatVolume > 1)
                //{
                //    AudioUtils.ApplyGain(data, Config.Instance.VoiceChatSettings.VoiceChatVolume);
                //}
                _voipFragQueue.Write(data, 0, dataLength);
            }
        }
        protected void OnDestroy()
        {
            Plugin.Log?.Debug($"VoipReceiver destroyed.");
            Destroyed?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? Destroyed;
    }
}
