using System;

namespace DevourDev.Base
{
    public interface ITicker
    {
        public event EventHandler<int> OnTick;


        public int CurrentTickrate { get; set; }


        public void StartTicking();
        public void PauseTicking();

    }
}
