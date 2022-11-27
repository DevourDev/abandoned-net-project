namespace FD.Networking.Garden.Packets
{
    public class FoundGameStateMessage : IPacketContent
    {
        public int UniqueID => 24;
        public bool[] PlayersAcceptions;
        public float TimeToAcceptLeft;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            PlayersAcceptions = d.ReadBools();
            TimeToAcceptLeft = d.ReadSingle();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write(PlayersAcceptions);
            e.Write(TimeToAcceptLeft);
        }
    }
}
