namespace DevourEncoding.Interfaces
{
    public interface IEncodable<Encoder> where Encoder : DevourEncoderBase
    {
        public void Encode(Encoder e);
    }
}
