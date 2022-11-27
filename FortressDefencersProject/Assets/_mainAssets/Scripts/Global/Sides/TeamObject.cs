using DevourDev.MonoBase.Multilanguage;
using UnityEngine;

namespace FD.Global.Sides
{
    [CreateAssetMenu(menuName = "FD/Global/Sides/New Team Object")]
    public class TeamObject : DevourDev.MonoBase.GameDatabaseElement
    {
        [SerializeField] private InternationalString _teamName;
        [SerializeField] private InternationalString _description;
        [SerializeField] private Color _color;


        public InternationalString TeamName => _teamName;
        public InternationalString Description => _description;
        public Color Color => _color;
    }

}