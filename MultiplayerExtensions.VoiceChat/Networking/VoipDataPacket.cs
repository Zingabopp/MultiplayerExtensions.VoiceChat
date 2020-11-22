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
        public int Checksum;
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
            packet.Checksum = 0;
            packet.Data = data;
            packet.DataLength = dataLength;
            return packet;
        }

        public VoipPacketType PacketType => VoipPacketType.VoiceData;

        public void Deserialize(NetDataReader reader)
        {
            ArrayRented = true;
            //PlayerId = reader.GetString();
            _packetVersion = reader.GetByte();
            Index = reader.GetInt();
            Checksum = reader.GetInt();
            DataLength = reader.GetInt();
            if (DataLength > 0)
            {
                Data = ByteAryPool.Rent(DataLength);
                reader.GetBytes(Data, 0, DataLength);
                int checksum = GetChecksum(Data, DataLength);
                if (checksum != Checksum)
                    Plugin.Log?.Warn($"Expected checksum didn't match.");
#if DEBUG
                Plugin.Log?.Debug($"Receiving: {Index}|{DataLength}|{Checksum}:{checksum}|{Data[0]}:{Data[DataLength - 1]}");
#endif
            }
            else
                Data = Array.Empty<byte>();
        }

        public void Serialize(NetDataWriter writer)
        {
            //writer.Put(PlayerId);
            writer.Put(_packetVersion);
            writer.Put(Index);
            int checksum = GetChecksum(Data, DataLength);
            writer.Put(checksum);
            if (Data != null)
            {
                writer.PutBytesWithLength(Data, 0, DataLength);
#if DEBUG
                Plugin.Log?.Debug($"Sending: {Index}|{DataLength}|{checksum}|{Data[0]}:{Data[DataLength - 1]}");
#endif
            }
            else
            {
                writer.PutBytesWithLength(Array.Empty<byte>(), 0, 0);
                Plugin.Log?.Warn($"Trying to serialize a 'VoipPacket' with null data.");
            }
        }

        private static int GetChecksum(byte[]? data, int dataLength)
        {
            if (data == null)
                return 0;
            int checksum = 0;
            for (int i = 0; i < dataLength; i++)
            {
                unchecked
                {
                    checksum += data[i];
                }
            }
            return checksum;
        }

        public void Release()
        {
            if (ArrayRented)
                ByteAryPool.Return(Data);
            Pool.Release(this);
        }
    }
}
