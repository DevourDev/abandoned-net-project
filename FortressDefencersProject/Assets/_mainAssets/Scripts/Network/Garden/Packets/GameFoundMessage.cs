using FD.Global;

namespace FD.Networking.Garden.Packets
{
    public class GameFoundMessage : IPacketContent
    {
        public int UniqueID => 23;
        public GameMode FoundGameMode;

        public void Decode(Decoder d, bool readId = true)
        {
            if (readId)
                d.ReadInt();
            FoundGameMode = (GameMode)d.ReadInt();
        }

        public void Encode(Encoder e)
        {
            e.Write(UniqueID);
            e.Write((int)FoundGameMode);
        }
    }
}
