namespace DevourEncoding.Interfaces
{
    public interface IDecodable<Decoder> where Decoder : DevourDecoderBase
    {
        public void Decode(Decoder d, bool readId = true);
    }
}
