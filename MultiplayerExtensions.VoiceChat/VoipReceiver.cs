using Concentus.Structs;
using MultiplayerExtensions.VoiceChat.Networking;
using MultiplayerExtensions.VoiceChat.Utilities;
using System.IO;
using UnityEngine;
using Zenject;

namespace MultiplayerExtensions.VoiceChat
{
    public class VoipReceiver : MonoBehaviour
    {
        private const int _voipDelay = 1;
        [Inject]
        protected VoipSender? voipSender;
        public OpusDecoder Decoder = null!;
        protected AudioSource? voipSource;
        private FifoFloatStream _voipFragQueue = new FifoFloatStream();
        private bool _voipPlaying;
        private float[]? _voipBuffer;
        private int _lastVoipFragIndex;
        private int _silentFrames;
        private int _voipDelayCounter;
        System.Buffers.ArrayPool<float> floatAryPool = System.Buffers.ArrayPool<float>.Shared;

        private void Start()
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
            Decoder = new OpusDecoder(48000, 2);
            //if (voipSender != null)
            //    voipSender.OnAudioGenerated += HandleAudioDataReceived;
            //else
            //    Plugin.Log?.Error("No VoipSender available.");
        }

        public void HandleAudioDataReceived(object sender, VoipDataPacket e)
        {
            if (e.Data != null && e.DataLength > 0)
            {

                float[] floatData = new float[480 * 2];// floatAryPool.Rent(480 * 2);
                int length = Decoder.Decode(e.Data, 0, e.DataLength, floatData, 0, 5760);
                Plugin.Log?.Debug($"Playing fragment, length {length}");
                PlayVoIPFragment(floatData, length * Decoder.NumChannels, e.Index);
                //floatAryPool.Return(floatData);
            }
        }

        public void Tick()
        {
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
        void OnAudioFilterRead(float[] data, int channels)
        {
            if (!_voipPlaying)
                return;
            //Plugin.Log?.Debug($"OnAudioFilterRead: {data.Length} | {channels} channels");
            int sampleRate = AudioSettings.outputSampleRate;
            int dataLen = data.Length / channels;
            if(_voipBuffer == null)
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
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"VoipReceiver destroyed.");
        }
    }
}
