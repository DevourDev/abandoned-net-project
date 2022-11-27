using DevourDev.Base;
using DevourEncoding.Interfaces;

namespace DevourDev.Networking.Packets
{
    public interface IPacketContentBase<Encoder, Decoder> : IEncodable<Encoder>, IDecodable<Decoder>, IUniqueReadonly
         where Encoder : DevourEncoding.DevourEncoderBase
        where Decoder : DevourEncoding.DevourDecoderBase
    {

    }
}
