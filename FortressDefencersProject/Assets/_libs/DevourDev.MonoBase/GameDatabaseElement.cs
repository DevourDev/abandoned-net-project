using DevourDev.Base;
using UnityEngine;

namespace DevourDev.MonoBase
{
    public abstract class GameDatabaseElement : ScriptableObject, IUnique
    {
    
        [SerializeField] private int _databaseElementId;

        
        public int UniqueID { get => _databaseElementId; set => _databaseElementId = value; }
    }
}