using DevourDev.Base;

namespace DevourDev.Database.Interfaces
{
    public interface IEntity : IUniqueLong
    {
        public byte[] Encode();
    }

    
}
