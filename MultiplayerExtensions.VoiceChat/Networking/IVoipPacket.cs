using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Networking
{
    public interface IVoipPacket : INetSerializable
    {
        VoipPacketType PacketType { get; }
        byte PacketVersion { get; }
    }
}
