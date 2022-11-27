using DevourDev.MonoBase.Ai.ExExAct;
using UnityEngine;

namespace FD.Units.Ai.Enquirers
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Enquirers/Predefined")]
    public class PredefinedEnquirer : UnitEnquirer
    {
        [SerializeField] private bool _result;


        protected override void Examine(UnitAi ai, out bool result, out EnquirerData ed)
        {
            ed = ai.GetOrCreateEnquirerData<Data>(this);
            result = _result;
        }


        public class Data : EnquirerData
        {

        }
    }
}
