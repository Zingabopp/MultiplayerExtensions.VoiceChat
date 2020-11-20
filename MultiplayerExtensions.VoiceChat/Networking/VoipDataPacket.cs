using LiteNetLib.Utils;
using System;

namespace MultiplayerExtensions.VoiceChat.Networking
{
    public class VoipDataPacket : INetSerializable, IPoolablePacket, IVoipPacket
    {
        protected readonly System.Buffers.ArrayPool<byte> ByteAryPool = System.Buffers.ArrayPool<byte>.Shared;
        //public string? PlayerId;
        public int DataLength;
        public byte[]? Data;
        public int Index;
        private bool ArrayRented;

        protected static PacketPool<VoipDataPacket> Pool
        {
            get
            {
                return ThreadStaticPacketPool<VoipDataPacket>.pool;
            }
        }
        public static VoipDataPacket Obtain() => Pool.Obtain();

        public static VoipDataPacket Create(int index, byte[] data, int dataLength)
        {
            VoipDataPacket packet = Pool.Obtain();
            packet.ArrayRented = false;
            //packet.PlayerId = playerId;
            packet.Index = index;
            packet.Data = data;
            packet.DataLength = dataLength;
            return packet;
        }

        public VoipPacketType PacketType => VoipPacketType.VoiceData;

        public void Deserialize(NetDataReader reader)
        {
            ArrayRented = true;
            //PlayerId = reader.GetString();
            Index = reader.GetInt();
            DataLength = reader.GetInt();
            if (DataLength > 0)
            {
                Data = ByteAryPool.Rent(DataLength);
                reader.GetBytes(Data, 0, DataLength);
            }
            else
                Data = Array.Empty<byte>();
        }

        public void Serialize(NetDataWriter writer)
        {
            //writer.Put(PlayerId);
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
            if (ArrayRented)
                ByteAryPool.Return(Data);
            Pool.Release(this);
        }
    }
}
