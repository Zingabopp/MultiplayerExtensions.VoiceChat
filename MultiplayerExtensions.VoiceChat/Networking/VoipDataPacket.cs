using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Networking
{
    public class VoipDataPacket : INetSerializable, IPoolablePacket, IVoipPacket
    {
        public string? PlayerId;
        public byte[]? Data;
        public int DataLength;
        public int Index;

        protected static PacketPool<VoipDataPacket> Pool
        {
            get
            {
                return ThreadStaticPacketPool<VoipDataPacket>.pool;
            }
        }
        public static VoipDataPacket Obtain() => Pool.Obtain();

        public static VoipDataPacket Create(string playerId, int index, byte[] data, int dataLength)
        {
            VoipDataPacket packet = Pool.Obtain();
            packet.PlayerId = playerId;
            packet.Index = index;
            packet.Data = data;
            packet.DataLength = dataLength;
            return packet;
        }

        public VoipPacketType PacketType => VoipPacketType.VoiceData;

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetString();
            Index = reader.GetInt();
            DataLength = reader.GetInt();
            Data = reader.GetBytesWithLength();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PlayerId);
            writer.Put(Index);
            writer.Put(DataLength);
            if (Data != null)
                writer.PutBytesWithLength(Data, 0, DataLength);
            else
            {
                writer.PutBytesWithLength(Array.Empty<byte>(), 0, 0);
                Plugin.Log?.Warn($"Trying to serialize a 'VoipPacket' with null data.");
            }
        }

        public void Release()
        {
            Data = null;
            Pool.Release(this);
        }
    }
}
