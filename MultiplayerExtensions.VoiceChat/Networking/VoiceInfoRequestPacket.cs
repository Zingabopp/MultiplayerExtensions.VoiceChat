using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Networking
{
    public class VoiceInfoRequestPacket : INetSerializable, IPoolablePacket, IVoipPacket
    {
        public VoipPacketType PacketType => VoipPacketType.InfoRequest;

        private byte _packetVersion;
        /// <summary>
        /// Version of the <see cref="VoiceInfoRequestPacket"/>.
        /// </summary>
        public byte PacketVersion => _packetVersion;
        /// <summary>
        /// Codec preferred by the voice receiver.
        /// </summary>
        public string PreferredCodec = string.Empty;
        /// <summary>
        /// Codecs supported by the voice receiver.
        /// </summary>
        public string[] SupportedCodecs = Array.Empty<string>();
        protected static PacketPool<VoiceInfoRequestPacket> Pool
        {
            get
            {
                return ThreadStaticPacketPool<VoiceInfoRequestPacket>.pool;
            }
        }
        public static VoiceInfoRequestPacket Obtain() => Pool.Obtain();

        public static VoiceInfoRequestPacket Create(string preferredCodec, IEnumerable<string> supportedCodecs)
        {
            VoiceInfoRequestPacket packet = Pool.Obtain();
            packet._packetVersion = 1;
            packet.PreferredCodec = preferredCodec;
            packet.SupportedCodecs = supportedCodecs.ToArray();
            return packet;
        }

        public void Deserialize(NetDataReader reader)
        {
            _packetVersion = reader.GetByte();
            PreferredCodec = reader.GetString();
            SupportedCodecs = reader.GetStringArray();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(_packetVersion);
            writer.Put(PreferredCodec);
            writer.PutArray(SupportedCodecs);
        }

        public void Release()
        {
            Pool.Release(this);
        }
    }
}
