using System;
using System.Net;

namespace FD.Networking.Garden.Packets
{
    public class RealmInviteMessage : IPacketContent
    {
        public int UniqueID => 35;
        public IPEndPoint RealmIPEP;
        public byte[] RealmSessionKey;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            RealmIPEP = d.ReadIPEndPoint();
            RealmSessionKey = d.ReadBytes();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(RealmIPEP);
            //sessionKeys sending separately to each client (or not))))
            e.Write(RealmSessionKey);
        }
    }
    public class GetMyProfileDataRequest : IPacketContent
    {
        public int UniqueID => 100_20;

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
    public class GetMyProfileDataResponse : IPacketContent
    {
        public int UniqueID => 100_21;
        public bool Result;
        public long AccID;
        public string NickName;
        public int MatchesCount;
        public int Wins;
        public int Mmr;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();

            Result = d.ReadBool();
            if (!Result)
                return;

            AccID = d.ReadInt64();
            NickName = d.ReadString();
            MatchesCount = d.ReadInt();
            Wins = d.ReadInt();
            Mmr = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);

            if (!Result)
                return;

            e.Write(AccID);
            e.Write(NickName);
            e.Write(MatchesCount);
            e.Write(Wins);
            e.Write(Mmr);
        }
    }
    public class GetProfileDataRequest : IPacketContent
    {
        public int UniqueID => 100_30;
        public long ID;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            ID = d.ReadInt64();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(ID);
        }
    }
    public class QuitFindGameQueueRequest : IPacketContent
    {
        public int UniqueID => 60;

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
    public class QuitFindGameQueueResponse : IPacketContent
    {
        public int UniqueID => 61;
        public bool Result;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();

            Result = d.ReadBool();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
        }
    }
    public class GetProfileDataResponse : IPacketContent
    {
        public int UniqueID => 100_31;
        public bool Result;
        public string NickName;
        public int MatchesCount;
        public int Wins;
        public int Mmr;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();

            Result = d.ReadBool();
            NickName = d.ReadString();
            MatchesCount = d.ReadInt();
            Wins = d.ReadInt();
            Mmr = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(NickName);
            e.Write(MatchesCount);
            e.Write(Wins);
            e.Write(Mmr);
        }
    }

    public class SetMyProfileDataRequest : IPacketContent
    {
        public int UniqueID => 100_40;
        public bool NickNameChanged;
        public string NickName;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();

            NickNameChanged = d.ReadBool();

            if (NickNameChanged)
                NickName = d.ReadString();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(NickNameChanged);

            if (NickNameChanged)
                e.Write(NickName);
        }
    }

    public class SetMyProfileDataResponse : IPacketContent
    {
        public int UniqueID => 100_40;
        public bool Result;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();

            Result = d.ReadBool();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
        }
    }

    public class OnlinePlayersStatsRequest : IPacketContent
    {
        public int UniqueID => 70;


        public OnlinePlayersStatsRequest()
        {
        }


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

    public class OnlinePlayersStatsResponse : IPacketContent
    {
        public int UniqueID => 71;
        public bool Result;
        public int OnlinersCount;
        public int SearchersCount;


        public OnlinePlayersStatsResponse()
        {

        }


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();

            Result = d.ReadBool();

            if (!Result)
                return;

            OnlinersCount = d.ReadInt();
            SearchersCount = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);

            if (!Result)
                return;

            e.Write(OnlinersCount);
            e.Write(SearchersCount);
        }
    }

    public class FD_GardenPacketsResolver : Networking.Packets.FD_PacketsResolverBase
    {
        public override IPacketContent GetSpecialPacket(int packetID)
            => packetID switch
            {

                //TEST
                228 => new TestGardenPacketMessage(),

                320 => new TestGardenEchoRequest(),
                321 => new TestGardenEchoResponse(),


                10 => new ConnectToGardenRequest(),
                11 => new ConnectToGardenResponse(),

                //TODO: add LeaveFindGameQueueRequest & Response
                20 => new EnterFindGameQueueRequest(),
                21 => new EnterFindGameQueueResponse(),

                23 => new GameFoundMessage(),
                24 => new FoundGameStateMessage(),

                30 => new AcceptGameRequest(),
                31 => new AcceptGameResponse(),

                33 => new RealmStateUpdatedMessage(),
                34 => new LobbyDestroyedMessage(),
                35 => new RealmInviteMessage(),

                40 => new GetEnterRealmDataRequest(),
                41 => new GetEnterRealmDataResponse(),

                50 => new EstablishRealmGardenConnectionRequest(),
                51 => new EstablishRealmGardenConnectionResponse(),

                60 => new QuitFindGameQueueRequest(),
                61 => new QuitFindGameQueueResponse(),

                70 => new OnlinePlayersStatsRequest(),
                71 => new OnlinePlayersStatsResponse(),


                10_000 => new DisconnectRequest(),
                10_001 => new DisconnectResponse(),

                100_003 => new SessionKeyExpiringSoonMessage(), // not handling
                100_010 => new UpdateSessionKeyRequest(),       // not handling
                100_011 => new UpdateSessionKeyResponse(),      // not handling


                //todo: implement client&server-side handles for:


                100_20 => new GetMyProfileDataRequest(),
                100_21 => new GetMyProfileDataResponse(),

                100_30 => new GetProfileDataRequest(),
                100_31 => new GetProfileDataResponse(),

                100_40 => new SetMyProfileDataRequest(),
                100_41 => new SetMyProfileDataResponse(),


                _ => throw new NotImplementedException($"unknown packet ID: {packetID}"),
            };
    }
}
