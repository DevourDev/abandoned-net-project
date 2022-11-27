using UnityEngine;

namespace FD.Units.Ai.Actions
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/Actions/Abilities/Set Spawn Point as Area Target")]
    public class SetSpawnPointAsAreaTargetAction : UnitAction
    {
        [SerializeField] private Abilities.UnitAbilityObject _abilityObject;


        public override void Act(UnitAi ai)
        {
            ai.ServerSideUnit.AbilitiesCollection.Collection[_abilityObject.UniqueID].Target.Set(ai.ServerSideUnit.SpawnPoint);   
        }
    }
}
