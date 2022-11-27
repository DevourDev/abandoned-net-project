namespace FD.Networking.Realm.GamePackets
{
    public class BuyUnitRequest : IPacketContent
    {
        public int UniqueID => 1000;
        public int ShowCaseSlotID;


        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            ShowCaseSlotID = d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(ShowCaseSlotID);
        }
    }

    public class BuyUnitResponse : IPacketContent
    {
        public int UniqueID => 1001;
        public bool Result;
        public FailReasons FailReason;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            Result = d.ReadBool();
            if (!Result)
                FailReason = (FailReasons)d.ReadByte();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(Result);
            if (!Result)
                e.Write((byte)FailReason);
        }

        public enum FailReasons : byte
        {
            None,
            NotEnoughCoins,
            SlotIsEmpty,
            UnitBlocked,
            UnitInHand,
            PlayerLost,
            Other
        }
    }
}
