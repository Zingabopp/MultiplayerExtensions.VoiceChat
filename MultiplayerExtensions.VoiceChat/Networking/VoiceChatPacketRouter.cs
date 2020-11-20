using MultiplayerExtensions.VoiceChat.Codecs;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace MultiplayerExtensions.VoiceChat.Networking
{
    public sealed class VoiceChatPacketRouter : IDisposable
    {
        private bool _isConnected;

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                VoipSender.Enabled = value;
                if (value == false)
                {
                    VoipSender.StopRecording();
                }
                else
                {
                    VoipSender.StartRecording();
                }
                _isConnected = value;
            }
        }

        private readonly DiContainer _container;
        private IMultiplayerSessionManager SessionManager;
        //private IConnectionManager ConnectionManager;
        //private VoipReceiver VoipReceiver;
        private ICodecFactory CodecFactory;
        private VoipSender VoipSender;
        private readonly ConcurrentDictionary<string, VoipReceiver> PlayerReceivers = new ConcurrentDictionary<string, VoipReceiver>();

        private readonly NetworkPacketSerializer<byte, IConnectedPlayer> _mainSerializer = new NetworkPacketSerializer<byte, IConnectedPlayer>();
        private readonly NetworkPacketSerializer<byte, IConnectedPlayer> _voipDataSerializer = new NetworkPacketSerializer<byte, IConnectedPlayer>();
        private readonly NetworkPacketSerializer<byte, IConnectedPlayer> _voipMetadataSerializer = new NetworkPacketSerializer<byte, IConnectedPlayer>();
        private VoipReceiver dummyReceiver;
        public VoiceChatPacketRouter(IMultiplayerSessionManager sessionManager, VoipSender voipSender, ICodecFactory codecFactory, DiContainer container)
        {
            _container = container;
            SessionManager = sessionManager;
            //ConnectionManager = connectionManager;
            //VoipReceiver = voipReceiver;
            CodecFactory = codecFactory;
            VoipSender = voipSender;
            dummyReceiver = container.InstantiateComponentOnNewGameObject<VoipReceiver>();
            dummyReceiver.Initialize(codecFactory, codecFactory.DefaultSettings);
            AddEvents();
            sessionManager.RegisterSerializer((MultiplayerSessionManager.MessageType)128, _mainSerializer);
            _mainSerializer.RegisterSubSerializer((byte)VoipPacketType.VoiceData, _voipDataSerializer);
            _mainSerializer.RegisterSubSerializer((byte)VoipPacketType.InfoRequest, _voipMetadataSerializer);
            _mainSerializer.RegisterSubSerializer((byte)VoipPacketType.VoiceMetadata, _voipMetadataSerializer);
            _voipDataSerializer.RegisterCallback((byte)VoipPacketType.VoiceData, HandleVoipDataPacket, VoipDataPacket.Obtain);
            Plugin.Log?.Debug($"VoiceChatPacketRouter Constructed.");
            foreach (IConnectedPlayer? player in sessionManager.connectedPlayers)
            {
                if (!player.isMe)
                    CreatePlayerVoipReceiver(player.userId);
            }
            //if (sessionManager.isConnected)
                IsConnected = true;
        }

        private void SessionManager_connectedEvent()
        {
            Plugin.Log?.Info($"SessionManager Connected");
            IsConnected = true;
        }
        private void SessionManager_Disconnected(DisconnectedReason obj)
        {
            Plugin.Log?.Info($"SessionManager Disconnected");
            IsConnected = false;
            PlayerReceivers.Clear();
        }

        private void AddEvents()
        {
            VoipSender.OnAudioGenerated += VoipSender_OnAudioGenerated;
            SessionManager.connectedEvent += SessionManager_connectedEvent;
            SessionManager.disconnectedEvent += SessionManager_Disconnected;
            SessionManager.playerConnectedEvent += OnPlayerConnected;
            SessionManager.playerDisconnectedEvent += OnPlayerDisconnected;
        }


        private void RemoveEvents()
        {
            VoipSender.OnAudioGenerated -= VoipSender_OnAudioGenerated;
            SessionManager.connectedEvent -= SessionManager_connectedEvent;
            SessionManager.disconnectedEvent -= SessionManager_Disconnected;
            SessionManager.playerConnectedEvent -= OnPlayerConnected;
            SessionManager.playerDisconnectedEvent -= OnPlayerDisconnected;
        }

        private void OnPlayerDisconnected(IConnectedPlayer player)
        {
            if (PlayerReceivers.TryRemove(player.userId, out VoipReceiver receiver) && receiver != null)
            {
                GameObject.Destroy(receiver);
            }
        }

        private void OnPlayerConnected(IConnectedPlayer player)
        {
            string userId = player.userId;

            if (IPA.Utilities.UnityGame.OnMainThread)
                GetVoipReceiverForId(userId);
            else
            {
                Plugin.Log?.Debug($"Invoke required for OnPlayerConnected");
                HMMainThreadDispatcher.instance.Enqueue(() => GetVoipReceiverForId(userId));
            }
        }

        private VoipReceiver GetVoipReceiverForId(string userId)
        {
            VoipReceiver receiver = PlayerReceivers.GetOrAdd(userId, CreatePlayerVoipReceiver);
            BindReceiver(receiver);
            return receiver;
        }

        private VoipReceiver CreatePlayerVoipReceiver(string userId)
        {
            Plugin.Log?.Info($"CreatePlayerVoipReceiver: {userId}");
            VoipReceiver voipReceiver = _container.InstantiateComponentOnNewGameObject<VoipReceiver>($"VoipReceiver_{userId}");
            voipReceiver.Initialize(CodecFactory, CodecFactory.DefaultSettings);
            return voipReceiver;
        }

        /// <summary>
        /// Bind the VoipReceiver to a player GameObject (for eventual spacial audio and 'IsTalking' icon over head).
        /// </summary>
        /// <param name=""></param>
        private void BindReceiver(VoipReceiver receiver)
        {
            // Set parent transform?
        }

        private void VoipSender_OnAudioGenerated(object sender, VoipDataPacket e)
        {
            Plugin.Log?.Debug($"VoipSender_OnAudioGenerated. {e.Data?.Length.ToString() ?? "NULL"} | {e.DataLength}");
            dummyReceiver.HandleAudioDataReceived(sender, e);
            Send(e);
        }

        private void HandleVoipDataPacket(VoipDataPacket packet, IConnectedPlayer player)
        {
            try
            {
                Plugin.Log?.Debug($"Received a packet {player.userName} ({player.userId}). '{packet.Data?.Length}' | {packet.DataLength}");
                if (PlayerReceivers.TryGetValue(player.userId, out VoipReceiver receiver))
                {
                    if (receiver != null)
                    {
                        receiver.HandleAudioDataReceived(this, packet);
                    }
                    else
                    {
                        Plugin.Log?.Error($"VoipReceiver is null");
                    }
                }
                else
                    Plugin.Log?.Debug($"Received a Voip packet from {player.userId} ({player.userName}), but they weren't in the receiver dictionary.");
                packet.Release();
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error handling VoipDataPacket: {ex.Message}");
                Plugin.Log?.Debug(ex);
            }
        }

        public void Send<T>(T packet) where T : IVoipPacket
        {
            // packet is released by ConnectedPlayerManager
            SessionManager.Send(packet);
        }

        public void Dispose()
        {
            Plugin.Log?.Debug($"VoiceChatPacketRouter Disposed.");
            RemoveEvents();
            SessionManager = null!;
            //ConnectionManager = null!;
            VoipSender = null!;
            PlayerReceivers.Clear();
        }
    }

    public enum VoipPacketType : byte
    {
        /// <summary>
        /// Packet contains voice data.
        /// </summary>
        VoiceData = 0,
        /// <summary>
        /// Information about voice data is requested (channel count, sample rate, etc).
        /// </summary>
        InfoRequest = 1,
        /// <summary>
        /// Information about voice data being transmitted (channel count, sample rate, etc).
        /// </summary>
        VoiceMetadata = 2
    }
}
