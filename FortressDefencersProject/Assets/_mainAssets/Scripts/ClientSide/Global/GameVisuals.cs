using FD.Global.Sides;
using UnityEngine;

namespace FD.ClientSide.Global
{
    [CreateAssetMenu(menuName = "FD/Client-Side/Game Visuals/Game Visuals Object")]
    public class GameVisuals : ScriptableObject
    {
        [SerializeField] private GameSidesVisualsDatabaseObject _gameSidesVisuals;
        [SerializeField] private TeamsDatabaseObject _teams;



        public GameSidesVisualsDatabaseObject GameSides => _gameSidesVisuals;
        public TeamsDatabaseObject Teams => _teams;
    }
}