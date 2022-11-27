using System;

namespace DevourDev.Base
{
    public interface IWallet<CurrencyMeasure>
    {
        public event EventHandler<CurrencyMeasure> OnBalanceChanged;


        public CurrencyMeasure Balance { get; set; }

        public CurrencyMeasure MinBalance { get; set; }
        public CurrencyMeasure MaxBalance { get; set; }


        public bool CanSpend(CurrencyMeasure value);
        public void Spend(CurrencyMeasure value);

        public bool CanEarn(CurrencyMeasure value);
        public void Earn(CurrencyMeasure value);

    }
}
