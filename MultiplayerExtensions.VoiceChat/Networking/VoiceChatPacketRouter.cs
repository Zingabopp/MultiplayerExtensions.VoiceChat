using System;
using System.Collections.Concurrent;
using UnityEngine;
using Zenject;

namespace MultiplayerExtensions.VoiceChat.Networking
{
    public class VoiceChatPacketRouter : IDisposable
    {
        private DiContainer _container;
        private IMultiplayerSessionManager SessionManager;
        //private IConnectionManager ConnectionManager;
        //private VoipReceiver VoipReceiver;
        private VoipSender VoipSender;
        private readonly ConcurrentDictionary<string, VoipReceiver> PlayerReceivers = new ConcurrentDictionary<string, VoipReceiver>();

        private readonly NetworkPacketSerializer<byte, IConnectedPlayer> _mainSerializer = new NetworkPacketSerializer<byte, IConnectedPlayer>();
        private readonly NetworkPacketSerializer<byte, IConnectedPlayer> _voipDataSerializer = new NetworkPacketSerializer<byte, IConnectedPlayer>();
        private readonly NetworkPacketSerializer<byte, IConnectedPlayer> _voipMetadataSerializer = new NetworkPacketSerializer<byte, IConnectedPlayer>();
        public VoiceChatPacketRouter(IMultiplayerSessionManager sessionManager, VoipSender voipSender, DiContainer container)
        {
            _container = container;
            SessionManager = sessionManager;
            //ConnectionManager = connectionManager;
            //VoipReceiver = voipReceiver;
            VoipSender = voipSender;
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
        }

        private void SessionManager_connectedEvent()
        {
            Plugin.Log?.Info($"SessionManager Connected");
        }

        private void AddEvents()
        {
            VoipSender.OnAudioGenerated += VoipSender_OnAudioGenerated;
            SessionManager.connectedEvent += SessionManager_connectedEvent;
            SessionManager.playerConnectedEvent += OnPlayerConnected;
            SessionManager.playerDisconnectedEvent += OnPlayerDisconnected;
        }

        private void RemoveEvents()
        {
            VoipSender.OnAudioGenerated -= VoipSender_OnAudioGenerated;
            SessionManager.connectedEvent -= SessionManager_connectedEvent;
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
            CreatePlayerVoipReceiver(userId);
        }

        private VoipReceiver CreatePlayerVoipReceiver(string userId)
        {
            VoipReceiver receiver = PlayerReceivers.GetOrAdd(userId, CreatePlayerVoipReceiver);
            BindReceiver(receiver);
            return _container.InstantiateComponentOnNewGameObject<VoipReceiver>($"VoipReceiver_{userId}");
        }

        /// <summary>
        /// Bind the VoipReceiver to a player GameObject (for eventual spacial audio and 'IsTalking' icon over head.
        /// </summary>
        /// <param name=""></param>
        private void BindReceiver(VoipReceiver receiver)
        {
            // Set parent transform?
        }

        private void VoipSender_OnAudioGenerated(object sender, VoipDataPacket e)
        {
            Plugin.Log?.Debug($"VoipSender_OnAudioGenerated.");
            Send(e);
        }

        private void HandleVoipDataPacket(VoipDataPacket packet, IConnectedPlayer player)
        {
            Plugin.Log?.Debug($"Received a packet from someone else.");
            if (PlayerReceivers.TryGetValue(player.userId, out VoipReceiver receiver))
            {
                if (receiver != null)
                {
                    try
                    {
                        receiver.HandleAudioDataReceived(this, packet);
                    }
                    catch (Exception ex)
                    {
                        Plugin.Log?.Error($"Error handling VoipDataPacket: {ex.Message}");
                        Plugin.Log?.Debug(ex);
                    }
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

        public void Send<T>(T packet) where T : IVoipPacket
        {
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
