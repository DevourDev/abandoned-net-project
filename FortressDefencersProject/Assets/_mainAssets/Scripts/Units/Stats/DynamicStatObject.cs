using DevourDev.Base;
using DevourDev.MonoBase.Multilanguage;
using UnityEngine;

namespace FD.Units.Stats
{
    [CreateAssetMenu(menuName = "FD/Units/Stats/Dynamics/New Stat Object")]
    public class DynamicStatObject : DevourDev.MonoBase.GameDatabaseElement
    {
        [SerializeField] private InternationalString _statName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Color _color;


        public InternationalString StatName => _statName;
        public Sprite Icon => _icon;
        public Color Color => _color;
    }
}
