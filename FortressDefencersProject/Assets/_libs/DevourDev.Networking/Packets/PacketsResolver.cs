namespace DevourDev.Networking.Packets
{
    public abstract class PacketsResolver<PacketContent, Encoder, Decoder>
        where Encoder : DevourEncoding.DevourEncoderBase
        where Decoder : DevourEncoding.DevourDecoderBase
        where PacketContent : IPacketContentBase<Encoder, Decoder>
    {
        public PacketContent GetPacket(int packetID)
            => packetID switch
            {
                < 1_000_000 => GetSpecialPacket(packetID),
                >= 1_000_000 => GetBasePacket(packetID),
            };

        public abstract PacketContent GetSpecialPacket(int packetID);

        public abstract PacketContent GetBasePacket(int packetID);

    }
}
