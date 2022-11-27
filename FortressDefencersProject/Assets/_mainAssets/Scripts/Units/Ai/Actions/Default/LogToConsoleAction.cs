using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Debug/Log to Console")]
    public class LogToConsoleAction : UnitAction
    {
        [SerializeField] private string _log;


        public override void Act(UnitAi ai)
        {

            //Debug.Log(_log);
        }
    }


}
