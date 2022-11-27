using UnityEngine;

namespace DevourDev.MonoBase.Inputs
{
    [System.Serializable]
    public struct KeyBind
    {
        public KeyCode[] Keys;

        public KeyBind(params KeyCode[] keys)
        {
            Keys = keys;
        }
    }
}
