using DevourDev.Base;
using System;
using UnityEngine;

namespace DevourDev.MonoBase.Multilanguage
{
    [CreateAssetMenu(fileName = "new language object", menuName = "DevourDev/MultiLanguage/New Language Object")]
    public class LanguageObject : ScriptableObject, IUnique
    {
        [SerializeField] private string _name;
        [HideInInspector, SerializeField]
        private int _unitID;

        public string Name => _name;
        public int UniqueID { get => _unitID; set => _unitID = value; }

        public override bool Equals(object obj)
        {
            return obj is LanguageObject @object &&
                   base.Equals(obj) &&
                   _unitID == @object._unitID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), _unitID);
        }
    }
}
