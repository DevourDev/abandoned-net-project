namespace DevourDev.Base
{
    public interface IUnique
    {
        public int UniqueID { get; set; }
    }
    public interface IUniqueLong
    {
        public long UniqueID { get; set; }
    }

    public interface IUniqueReadonly
    {
        public int UniqueID { get; }

    }
    public interface IUniqueReadonlyLong
    {
        public long UniqueID { get; }

    }
}
