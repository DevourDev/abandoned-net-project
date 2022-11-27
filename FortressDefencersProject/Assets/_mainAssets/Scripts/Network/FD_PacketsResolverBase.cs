using DevourDev.Networking.Packets;
using System;

namespace FD.Networking.Packets
{
    public abstract class FD_PacketsResolverBase : PacketsResolver<IPacketContent, Encoder, Decoder>
    {

        public override IPacketContent GetBasePacket(int packetID)
             => packetID switch
             {
                 1_000_010 => new HeartBeatRequest(),
                 1_000_011 => new HeartBeatResponse(),

                 _ => throw new NotImplementedException("unknown packet ID"),
             };
    }
}
