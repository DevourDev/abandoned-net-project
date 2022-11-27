using System;

namespace FD.Networking.Gates.Packets
{
    public class WC_GatesPacketsResolver : Networking.Packets.FD_PacketsResolverBase
    {
        public override IPacketContent GetSpecialPacket(int packetID)
            => packetID switch
            {
                10 => new LogInRequest(),
                11 => new LogInResponse(),
                20 => new SignUpRequest(),
                21 => new SignUpResponse(),

                _ => throw new NotImplementedException("unknown packet ID"),
            };
    }
}
