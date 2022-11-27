using System;

namespace DevourDev.Base
{
    public delegate void DynamicStatChanged(DynamicStat ds, float before, float softedDelta, float realDelta);
    public class DynamicStat
    {
        //todo: add Min < Max bounds check

        /// <summary>
        /// object #1 DynamicStat (this) float #2 - before, float #3 - delta
        /// </summary>
        public event DynamicStatChanged OnCurrentValueChanged;

        private float _min;
        private float _max;
        private float _regen;

        private float _current;


        public DynamicStat(float max, float regen, float min = 0)
        {
            _max = max;
            _regen = regen;
            _min = min;
            _current = _max;
        }

        public DynamicStat(float max, float regen, float min, float cur)
        {
            _max = max;
            _regen = regen;
            _min = min;
            _current = cur;
        }


        public float Min => _min;
        public float Max => _max;
        public float Regen => _regen;
        public float Current { get => _current; protected set => _current = value; }


        public void SetMin(float v)
        {
            _min = v;
        }
        public void SetMax(float v)
        {
            _max = v;
        }
        public void SetRegen(float v)
        {
            _regen = v;
        }

        /// <summary>
        /// Returns false if source value was > Max or < Min.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="invokeEvent"></param>
        /// <returns></returns>
        public bool SetCurrentToValueOrClosestValid(float v, bool invokeEvent = false)
        {
            bool result;
            if (v > Max)
            {
                v = Max;
                result = false;
            }
            else if (v < Min)
            {
                v = Min;
                result = false;
            }
            else
            {
                result = true;
            }

            if (invokeEvent)
            {
                if (!ChangeCurrentToValue(v))
                {
                    throw new Exception($"DynamicStat.cs SetCurrent({v}) !ChangeCurrentToValue(_min). Min is {_min}, Max is {_max}");
                }
            }
            else
            {
                Current = v;
            }

            return result;
        }


        private bool ChangeCurrentToValue(float endValue, float desiredDelta) // я забыл, что хотел
        {
            if (endValue > Max || endValue < Min)
                return false;

            float delta = endValue - Current;

            ChangeCurrent(delta, desiredDelta);
            return true;
        }
        private bool ChangeCurrentToValue(float endValue)
        {
            if (endValue > Max || endValue < Min)
                return false;

            float delta = endValue - Current;

            ChangeCurrent(delta, delta);
            return true;
        }

        public bool TrySpend(float value, out float newCurrent)
        {
            if (value < 0)
                throw new OverflowException("Trying to remove negative amount");

            var res = TrySpend(value);
            newCurrent = Current;
            return res;
        }
        public bool TrySpend(float value)
        {
            if (value < 0)
                throw new OverflowException("Trying to remove negative amount");

            if (CanSpend(value))
            {
                Spend(value);
                return true;
            }

            return false;
        }
        public void SpendValueOrAll(float value)
        {
            if (value < 0)
                throw new OverflowException("Trying to remove negative amount");

            if (CanSpend(value))
            {
                Spend(value);
            }
            else
            {
                if (!ChangeCurrentToValue(_min))
                {
                    throw new Exception($"DynamicStat.cs SpendValueOrAll({value}) else !ChangeCurrentToValue(_min). Min is {_min}");
                }
            }
        }

        public bool CanSpend(float value)
        {
            if (value < 0)
                throw new OverflowException("Trying to remove negative amount");

            return Current - value >= _min;
        }
        public bool CanSpend(float value, out float lack)
        {
            if (value < 0)
                throw new OverflowException("Trying to remove negative amount");

            lack = value - Current;
            return CanSpend(value);
        }

        public void Spend(float value)
        {
            if (value < 0)
                throw new OverflowException("Trying to remove negative amount");

            ChangeCurrent(-value, -value);
        }
        public void SafeRestore(float value, bool allowOverflow = false)
        {
            if (value < 0)
                throw new OverflowException("Trying to add negative amount");

            if (allowOverflow)
            {
                Restore(value);
            }
            else
            {
                Restore(Math.Clamp(value, 0, Max - Current), value);
            }
        }

        public void Restore(float safeV, float realV)
        {
            if (safeV < 0 || realV < 0)
                throw new OverflowException("Trying to add negative amount");

            ChangeCurrent(safeV, realV);
        }
        public void Restore(float value)
        {
            if (value < 0)
                throw new OverflowException("Trying to add negative amount");

            var before = Current;
            ChangeCurrent(value, value);
        }


        public virtual void Regenerate(int ticksPerSecond)
        {
            if (Current >= Max)
                return;

            SafeRestore(Regen / ticksPerSecond, false);
        }

        private void ChangeCurrent(float safeValue, float realValue)
        {
            var before = Current;
            Current += safeValue;
            OnCurrentValueChanged?.Invoke(this, Current, safeValue, realValue);
        }

    }

}
