using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FD.Networking.Realm.GamePackets
{
    public class BigPacketRequest : IPacketContent
    {
        public int UniqueID => 980;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
        }
    }
    public class BigPacketResponse : IPacketContent
    {
        public int UniqueID => 981;
        public byte[] BigDataArray;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            BigDataArray = d.ReadBytes();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(BigDataArray);
        }
    }

    public class FD_RealmGamePacketsResolver : Networking.Packets.FD_PacketsResolverBase
    {

        public override IPacketContent GetSpecialPacket(int packetID)
            => packetID switch
            {
                510 => new ConnectToRealmRequest(),
                511 => new ConnectToRealmResponse(),

                520 => new FullGameStateRequest(),
                521 => new FullGameStateResponse(),

                980 => new BigPacketRequest(),
                981 => new BigPacketResponse(),

                990 => new TestRequest(),
                991 => new TestResponse(),


                1000 => new BuyUnitRequest(),
                1001 => new BuyUnitResponse(),

                1010 => new PlaceUnitRequest(),
                1011 => new PlaceUnitResponse(),

                1020 => new RefreshShowCaseRequest(),
                1021 => new RefreshShowCaseResponse(),

                100_000 => new UnitUpdatedEvent(),


                200_000 => new NetworkUnitTarget(),

                300_000 => new TransformPacket(),
                300_001 => new UpdatedDynamicStatPacket(),


                210_000 => new DynamicStatAllValuesPacket(),
                210_001 => new AbilityStagePacket(),
                210_002 => new PlayerNewResourcesPacket(),
                210_003 => new FullPlayerResourcesPacket(),
                210_004 => new UnitStatePacket(),


                300_101 => new UnitDiedEvent(),

                500_003 => new SharedGameStateMessage(),
                500_004 => new PersonalGameStateMessage(),

                500_103 => new GameOverMessage(), //when game is finally end (and winner is declared)
                500_104 => new YouLostMessage(), //when one player (side) lost his Fortress (or lost game other way)
                500_105 => new YouWinMessage(),



                _ => throw new NotImplementedException("unknown packet ID"),
            };
    }
    public class YouWinMessage : IPacketContent
    {
        public int UniqueID => 500_105;
        public WinReason Reason;
        /// <summary>
        /// 0 if not ranked
        /// </summary>
        public int MmrChange;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Reason = (WinReason)d.ReadByte();
            MmrChange = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write((byte)Reason);
            e.Write(MmrChange);
        }

        public enum WinReason : byte
        {
            None,
            EnemyFortressDestroyed,
            Other,
        }
    }
    public class YouLostMessage : IPacketContent
    {
        public int UniqueID => 500_104;
        public LostReason Reason;
        /// <summary>
        /// 0 if not ranked
        /// </summary>
        public int MmrChange;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Reason = (LostReason)d.ReadByte();
            MmrChange = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write((byte)Reason);
            e.Write(MmrChange);
        }

        public enum LostReason : byte
        {
            None,
            FortressDestroyed,
            Other,
        }
    }
    public class GameOverMessage : IPacketContent
    {
        public int UniqueID => 500_103;
        /// <summary>
        /// -1 if DRAW
        /// </summary>
        public int WinnerTeamID;
        /// <summary>
        /// side ID,
        /// -1 if DRAW
        /// </summary>
        public int WinnerPlayerID;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            WinnerTeamID = d.ReadInt();
            WinnerPlayerID = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(WinnerTeamID);
            e.Write(WinnerPlayerID);
        }
    }
}
