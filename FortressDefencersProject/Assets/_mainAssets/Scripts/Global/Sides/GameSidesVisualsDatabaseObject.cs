using UnityEngine;

namespace FD.Global.Sides
{
    [CreateAssetMenu(menuName = "FD/Global/Sides/New Player Visuals Database Object")]
    public class GameSidesVisualsDatabaseObject : DevourDev.MonoBase.GameDatabase<GameSideVisualsObject>
    {
        private void OnValidate()
        {
            ManageElementsOnValidate();
        }

    }
}