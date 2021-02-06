using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Networking
{
    public class VoiceMetadataPacket : INetSerializable, IPoolablePacket, IVoipPacket
    {
        public VoipPacketType PacketType => VoipPacketType.InfoRequest;

        private byte _packetVersion;
        /// <summary>
        /// Version of the <see cref="VoiceMetadataPacket"/>.
        /// </summary>
        public byte PacketVersion => _packetVersion;
        /// <summary>
        /// SampleRate (in Hz) used by the voice sender.
        /// </summary>
        public int SampleRate;
        /// <summary>
        /// Number of channels in the voice sender's encoded packets.
        /// </summary>
        public byte Channels;
        /// <summary>
        /// Codec used by the sender.
        /// If empty string, sender does not support any of the receiver's codecs.
        /// </summary>
        public string Codec = string.Empty;
        protected static PacketPool<VoiceMetadataPacket> Pool
        {
            get
            {
                return ThreadStaticPacketPool<VoiceMetadataPacket>.pool;
            }
        }
        public static VoiceMetadataPacket Obtain() => Pool.Obtain();

        public static VoiceMetadataPacket Create(int sampleRate, byte channels, string codec)
        {
            VoiceMetadataPacket packet = Pool.Obtain();
            packet._packetVersion = 1;
            packet.SampleRate = sampleRate;
            packet.Channels = channels;
            packet.Codec = codec;
            return packet;
        }

        public void Deserialize(NetDataReader reader)
        {
            _packetVersion = reader.GetByte();
            SampleRate = reader.GetInt();
            Channels = reader.GetByte();
            Codec = Encoding.UTF8.GetString(reader.GetBytesWithLength());
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(_packetVersion);
            writer.Put(SampleRate);
            writer.Put(Channels);
            writer.PutBytesWithLength(Encoding.UTF8.GetBytes(Codec));
        }

        public void Release()
        {
            Pool.Release(this);
        }
    }
}
