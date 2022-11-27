using FD.Units.Abilities;
using UnityEngine;

namespace FD.Units.Ai.Actions
{
    public abstract class SetAbilityTargetActionBase : UnitAction
    {
        [SerializeField] private UnitAbilityObject[] _abilities;


        protected UnitAbilityObject[] Abilities => _abilities;

        protected void SetTarget(UnitAi ai, Vector3 t)
        {
            foreach (var ab in Abilities)
            {
                ai.ServerSideUnit.AbilitiesCollection.Collection[ab.UniqueID].Target.Set(t);
            }

        }
        protected void SetTarget(UnitAi ai, UnitOnSceneBase t)
        {
            foreach (var ab in Abilities)
            {
                ai.ServerSideUnit.AbilitiesCollection.Collection[ab.UniqueID].Target.Set(t);
            }
        }
        protected void SetTarget(UnitAi ai)
        {
            foreach (var ab in Abilities)
            {
                ai.ServerSideUnit.AbilitiesCollection.Collection[ab.UniqueID].Target.Set();
            }
        }
    }

}
