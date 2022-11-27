using System;

namespace DevourDev.Base
{
    public class IntegerWalletBase : IWallet<int>
    {
        private event EventHandler<int> BalanceChangedEvent;

        private readonly object _lockObject = new();


        public IntegerWalletBase(int maxValue, int minValue = 0, int startValue = 0)
        {
            MaxBalance = maxValue;
            MinBalance = minValue;
            Balance = startValue;
        }


        public event EventHandler<int> OnBalanceChanged
        {
            add
            {
                lock (_lockObject)
                {
                    BalanceChangedEvent += value;
                }
            }

            remove
            {
                lock (_lockObject)
                {
                    BalanceChangedEvent -= value;
                }
            }
        }

        public int Balance { get; set; }

        public int MinBalance { get; set; }
        public int MaxBalance { get; set; }


        public bool ChangeBalanceToValue(int endValue)
        {
            if (endValue > MaxBalance || endValue < MinBalance)
                return false;

            int delta = endValue - Balance;

            ChangeBalance(delta);
            return true;
        }

        public void ChangeBalanceToValueOrClosestValid(int desiredValue)
        {
            int safeValue;
            if (desiredValue > MaxBalance)
            {
                safeValue = MaxBalance;
            }
            else if (desiredValue < MinBalance)
            {
                safeValue = MinBalance;
            }
            else
            {
                safeValue = desiredValue;
            }

            ChangeBalance(safeValue, desiredValue);
        }

        public bool CanEarn(int value)
        {
            return MaxBalance - Balance >= value;
        }

        public bool CanSpend(int value)
        {
            return Balance - MinBalance >= value;
        }

        public void Earn(int value)
        {
            if (value < 0)
                throw new OverflowException("Trying to add negative amount");

            ChangeBalance(value);
        }

        public void Spend(int value)
        {
            if (value < 0)
                throw new OverflowException("Trying to remove negative amount");

            ChangeBalance(-value);
        }

        private void ChangeBalance(int value)
        {
            Balance += value;
            BalanceChangedEvent?.Invoke(this, value);
        }
        private void ChangeBalance(int safeValue, int realValue)
        {
            if (realValue == 0) // do i want it?..
                return;

            Balance += safeValue;
            BalanceChangedEvent?.Invoke(this, realValue);
        }
    }
}
