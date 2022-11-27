using DevourDev.MonoBase.Multilanguage;
using UnityEngine;

namespace FD.Global.Sides
{
    [CreateAssetMenu(menuName = "FD/Global/Sides/New Player Visuals Object")]
    public class GameSideVisualsObject : DevourDev.MonoBase.GameDatabaseElement
    {
        [SerializeField] private InternationalString _sideName;
        [SerializeField] private Color _color;


        public InternationalString SideName => _sideName;
        public Color Color => _color;

    }
}