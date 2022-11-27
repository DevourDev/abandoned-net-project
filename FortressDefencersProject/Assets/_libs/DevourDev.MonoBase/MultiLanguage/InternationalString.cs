using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevourDev.MonoBase.Multilanguage
{
    [Serializable]
    public class InternationalString
    {
        [SerializeField] private List<InternationalStringTranslation> _translations;

        private Dictionary<LanguageObject, InternationalStringTranslation> _dictionary;


        public List<InternationalStringTranslation> Translations { get => _translations; set => _translations = value; }

        public Dictionary<LanguageObject, InternationalStringTranslation> Dictionary
        {
            get
            {
                bool uninited = _dictionary == null;
                bool outdated = uninited || _dictionary.Count != _translations.Count;

                if (outdated)
                {
                    if (uninited)
                    {
                        _dictionary = new(_translations.Count);
                    }
                    else if (outdated)
                    {
                        _dictionary.Clear();
                    }

                    foreach (var t in _translations)
                    {
                        _dictionary.Add(t.Language, t);
                    }
                }

                return _dictionary;
            }

        }

        public bool TryTranslate(LanguageObject language, out string value)
        {
            if (Dictionary.TryGetValue(language, out var v))
            {
                value = v.Text;
                return true;
            }

            value = null;
            return false;
        }

        public string GetAnyNameOrDefault(string defaultValue = " ")
        {
            if (Translations == null || Translations.Count == 0)
                return " ";

            if (Translations[0] != null && Translations[0].Text != null)
                return Translations[0].Text;
            else
                return " ";
        }

        public string TranslateToOrAny(LanguageObject language, string noTranslateText = " ")
        {
            if (TryTranslate(language, out var v))
                return v;

            return GetAnyNameOrDefault(noTranslateText);

        }
    }
}
