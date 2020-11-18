using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat
{
    public class VoipPacket : INetSerializable, IPoolablePacket
    {
        public string? PlayerId;
        public byte[]? Data;
        public int DataLength;
        public int Index;

        protected static PacketPool<VoipPacket> pool
        {
            get
            {
                return ThreadStaticPacketPool<VoipPacket>.pool;
            }
        }

        public static VoipPacket Create(string playerId, int index, byte[] data, int dataLength)
        {
            VoipPacket packet = pool.Obtain();
            packet.PlayerId = playerId;
            packet.Index = index;
            packet.Data = data;
            packet.DataLength = dataLength;
            return packet;
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetString();
            Index = reader.GetInt();
            Data = reader.GetBytesWithLength();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PlayerId);
            writer.Put(Index);
            if (Data != null)
                writer.PutBytesWithLength(Data, 0, DataLength);
            else
                Plugin.Log?.Warn($"Trying to serialize a 'VoipPacket' with null data.");
        }

        public void Release()
        {
            Data = null;
            pool.Release(this);
        }
    }
}
