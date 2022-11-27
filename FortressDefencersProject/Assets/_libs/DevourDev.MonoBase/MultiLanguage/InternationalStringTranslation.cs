using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevourDev.MonoBase.Multilanguage
{
    [Serializable]
    public class InternationalStringTranslation
    {
        [SerializeField] private LanguageObject _language;
        [SerializeField, TextArea(2, 20)] private string _text;

        public LanguageObject Language => _language;
        public string Text => _text;

        public override bool Equals(object obj)
        {
            return obj is InternationalStringTranslation translation &&
                   EqualityComparer<LanguageObject>.Default.Equals(_language, translation._language);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_language);
        }
    }
}
