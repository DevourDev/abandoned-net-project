using System;

namespace FD.Networking.Database.Packets
{
    public class FD_DatabasePacketsResolver : Networking.Packets.FD_PacketsResolverBase
    {
        public override IPacketContent GetSpecialPacket(int packetID)
            => packetID switch
            {
                10 => new GetAccountDataRequest(),
                11 => new GetAccountDataResponse(),

                20 => new SetAccountDataRequest(),
                21 => new SetAccountDataResponse(),

                //30 => new DeleteAccountDataRequest(), // not implemented
                //31 => new DeleteAccountDataResponse(),

                10_010 => new HandleLogInRequest(),
                10_011 => new HandleLogInResponse(),

                10_020 => new HandleSignUpRequest(),
                10_021 => new HandleSignUpResponse(),

                10_100 => new RegistrateGameOverRequest(),
                10_101 => new RegistrateGameOverResponse(),




                _ => throw new NotImplementedException("unknown packet ID"),
            };
    }
}

