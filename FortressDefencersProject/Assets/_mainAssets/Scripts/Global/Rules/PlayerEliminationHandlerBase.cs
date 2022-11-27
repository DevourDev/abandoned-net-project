using UnityEngine;

namespace FD.Global.Rules
{
    public abstract class PlayerEliminationHandlerBase : ScriptableObject
    {
        public abstract void Eliminate(Sides.GameSideBase side);
    }
}
