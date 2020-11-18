using LiteNetLib;
using LiteNetLib.Utils;
using MultiplayerExtensions.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Networking
{
    public class VoiceChatPacketRouter : IDisposable
    {
        private IMultiplayerSessionManager SessionManager;
        //private IConnectionManager ConnectionManager;
        private VoipReceiver VoipReceiver;
        private VoipSender VoipSender;

        private readonly NetworkPacketSerializer<byte, IConnectedPlayer> _mainSerializer = new NetworkPacketSerializer<byte, IConnectedPlayer>();
        private readonly NetworkPacketSerializer<byte, IConnectedPlayer> _voipDataSerializer = new NetworkPacketSerializer<byte, IConnectedPlayer>();
        private readonly NetworkPacketSerializer<byte, IConnectedPlayer> _voipMetadataSerializer = new NetworkPacketSerializer<byte, IConnectedPlayer>();
        public VoiceChatPacketRouter(IMultiplayerSessionManager sessionManager, VoipReceiver voipReceiver, VoipSender voipSender)
        {
            SessionManager = sessionManager;
            //ConnectionManager = connectionManager;
            VoipReceiver = voipReceiver;
            VoipSender = voipSender;
            AddEvents();
            sessionManager.RegisterSerializer((MultiplayerSessionManager.MessageType)128, _mainSerializer);
            _mainSerializer.RegisterSubSerializer((byte)VoipPacketType.VoiceData, _voipDataSerializer);
            _mainSerializer.RegisterSubSerializer((byte)VoipPacketType.InfoRequest, _voipMetadataSerializer);
            _mainSerializer.RegisterSubSerializer((byte)VoipPacketType.VoiceMetadata, _voipMetadataSerializer);
            _voipDataSerializer.RegisterCallback((byte)VoipPacketType.VoiceData, HandleVoipDataPacket, VoipDataPacket.Obtain);
            Plugin.Log?.Debug($"VoiceChatPacketRouter Constructed.");
        }

        private void SessionManager_connectedEvent()
        {
            Plugin.Log?.Info($"SessionManager Connected");
        }

        private void AddEvents()
        {
            VoipSender.OnAudioGenerated += VoipSender_OnAudioGenerated;
            SessionManager.connectedEvent += SessionManager_connectedEvent;
        }
        private void RemoveEvents()
        {
            VoipSender.OnAudioGenerated -= VoipSender_OnAudioGenerated;
            SessionManager.connectedEvent -= SessionManager_connectedEvent;
        }

        private void VoipSender_OnAudioGenerated(object sender, VoipDataPacket e)
        {
            Plugin.Log?.Debug($"VoipSender_OnAudioGenerated.");
            Send(e);
        }

        private void HandleVoipDataPacket(VoipDataPacket packet, IConnectedPlayer player)
        {
            if (player.isMe)
            {
                Plugin.Log?.Debug($"Received a packet from myself.");
            }
            else
                Plugin.Log?.Debug($"Received a packet from someone else.");
            if (VoipReceiver == null)
            {
                Plugin.Log?.Error($"VoipReceiver is null");
                return;
            }
            try
            {
                VoipReceiver.HandleAudioDataReceived(this, packet);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"Error handling VoipDataPacket: {ex.Message}");
                Plugin.Log?.Debug(ex);
            }
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
            VoipReceiver = null!;
            VoipSender = null!;
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
