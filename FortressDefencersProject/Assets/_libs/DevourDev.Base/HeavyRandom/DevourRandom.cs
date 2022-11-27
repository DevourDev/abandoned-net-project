using System;
using System.Collections.Generic;
using System.Linq;

namespace DevourDev.Base.HeavyRandom
{
    public class DevourRandom
    {
        private readonly Random _rnd;

        public DevourRandom()
        {
            _rnd = new Random();
        }

        public Random SysRandom => _rnd;


        public int RandInt(bool positive = true)
        {
            int minValue = positive ? 0 : int.MinValue;
            return RandInt(minValue, int.MaxValue);
        }
        public int RandInt(int maxValue, bool positive = true)
        {
            int minValue = positive ? 0 : int.MinValue;
            if (minValue >= maxValue)
                return maxValue;
            return RandInt(minValue, maxValue);
        }
        public int RandInt(int minValue, int maxValue)
        {
            return _rnd.Next(minValue, maxValue);
        }

        public float RandFloat()
        {
            double mantissa = _rnd.NextDouble() * 2.0 - 1.0;
            double exponent = Math.Pow(2.0, _rnd.Next(-126, 128));
            return (float)(mantissa * exponent);
        }
        public float RandFloat(float maxValue, bool positive = true)
        {
            return RandFloat(positive ? 0 : float.MinValue, maxValue);
        }
        public float RandFloat(float minValue, float maxValue)
        {
            double rDouble = _rnd.NextDouble() * (maxValue - minValue) + minValue;
            return (float)rDouble;
        }

        public double RandDouble()
        {
            return RandDouble(double.MinValue, double.MaxValue);
        }
        public double RandDouble(double minValue, double maxValue)
        {
            return _rnd.NextDouble() * (maxValue - minValue) + minValue;
        }

        public decimal RandDecimal(decimal minValue, decimal maxValue)
        {
            decimal rDecimal = (decimal)_rnd.NextDouble() * (maxValue - minValue) + minValue;
            return rDecimal;
        }


        public bool FlipCoin()
        {
            return _rnd.Next(0, 2) == 0;
        }


        public T RandElement<T>(IEnumerable<T> collection)
        {
            int randindex = _rnd.Next(0, collection.Count());
            return collection.ElementAt(randindex);
        }
        public T RandElementExcept<T>(IEnumerable<T> collection, params int[] exceptingIndexes)
        {
            int[] availableIndexes = new int[collection.Count() - exceptingIndexes.Length];

            Array.Sort(exceptingIndexes);
            List<int> exList = exceptingIndexes.ToList();
            int colAvailableIndex = 0;

            for (int i = 0; i < collection.Count(); i++)
            {
                if (!CheckForExcIndex(i))
                {
                    colAvailableIndex = i;
                    break;
                }
            }

            for (int i = 0; i < availableIndexes.Length; i++)
            {
                availableIndexes[i] = colAvailableIndex;
                colAvailableIndex++;
                while (!CheckForExcIndex(colAvailableIndex))
                {
                    colAvailableIndex++;
                }
            }

            return collection.ElementAt(RandElement(availableIndexes));

            bool CheckForExcIndex(int index)
            {
                if (exList.Contains(index))
                {
                    exList.Remove(index);
                    return true;
                }
                return false;
            }
        }
    }
}
