namespace DevourDev.Base
{
    public class RelevantData<T>
    {
        private readonly T _key;
        private RelevantData _relevantData;


        public RelevantData(T key, bool relevantState = false)
        {
            _key = key;
            _relevantData = new(relevantState);
        }


        public T Key => _key;
        public bool IsActual => _relevantData.IsActual;


        public void SetActual() => _relevantData.SetActual();
        public void SetOutdated() => _relevantData.SetOutdated();


        public void MarkAsAlwaysActual() => _relevantData.MarkAsAlwaysActual();
    }

    public  class RelevantData
    {
        private bool _isActual;
        private bool _alwaysActual;


        public RelevantData(bool relevantState = false)
        {
            _isActual = relevantState;
        }


        public bool IsActual => AlwaysActual || _isActual;

        protected bool AlwaysActual => _alwaysActual;


        public void SetActual()
        {
            if (AlwaysActual)
                return;

            _isActual = true;
        }
        public void SetOutdated()
        {
            if (AlwaysActual)
                return;

            _isActual = false;
        }


        public void MarkAsAlwaysActual()
        {
            _isActual = true;
            _alwaysActual = true;
        }
    }
}