using LiteNetLib.Utils;
using System;

namespace MultiplayerExtensions.VoiceChat.Networking
{
    public class VoipDataPacket : INetSerializable, IPoolablePacket, IVoipPacket
    {
        protected readonly System.Buffers.ArrayPool<byte> ByteAryPool = System.Buffers.ArrayPool<byte>.Shared;
        protected byte _packetVersion;
        public byte PacketVersion { get => _packetVersion; }
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
            packet._packetVersion = 1;
            packet.Index = index;
            packet.Data = data;
            packet.DataLength = dataLength;
            return packet;
        }

        public VoipPacketType PacketType => VoipPacketType.VoiceData;

        public void Deserialize(NetDataReader reader)
        {
            try
            {
                _packetVersion = reader.GetByte();
                Index = reader.GetInt();
                DataLength = reader.GetInt();
                if (DataLength > 1024)
                {
                    Plugin.Log?.Warn($"DataLength of '{DataLength}' is higher than expected, dropping packet.");
                    DataLength = 0;
                    Data = Array.Empty<byte>();
                }
                else if (DataLength > 0 && DataLength <= reader.AvailableBytes)
                {
                    ArrayRented = true;
                    Data = ByteAryPool.Rent(DataLength);
                    reader.GetBytes(Data, 0, DataLength);
                }
                else
                {
                    Plugin.Log?.Warn($"Unable to parse packet. DataLength: {DataLength} | Available bytes: {reader.AvailableBytes}");
                    DataLength = 0;
                    Data = Array.Empty<byte>();
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Debug(ex);
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            //writer.Put(PlayerId);
            writer.Put(_packetVersion);
            writer.Put(Index);
            if (Data != null)
            {
                writer.PutBytesWithLength(Data, 0, DataLength);
#if DEBUG
                Plugin.Log?.Debug($"Sending: {Index}|{DataLength}|{Data[0]}:{Data[DataLength - 1]}");
#endif
            }
            else
            {
                writer.PutBytesWithLength(Array.Empty<byte>(), 0, 0);
                Plugin.Log?.Warn($"Trying to serialize a 'VoipPacket' with null data.");
            }
        }

        public void Release()
        {
            if (ArrayRented)
            {
                ByteAryPool.Return(Data);
                Data = null;
            }
            Pool.Release(this);
        }
    }
}
