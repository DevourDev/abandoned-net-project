using FD.Global;
using FD.Global.Sides;
using FD.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevourDev.Base;
using DevourDev.MonoBase;
using DevourDev.Base.HeavyRandom;
using DevourDev.Base.SystemExtentions;

namespace FD.Test
{
    public class PseudoGameSide_test : MonoBehaviour
    {
        [SerializeField] private long _playerAccountID;
        [SerializeField] private int _playerID;
        [SerializeField] private string _nickName;

        [SerializeField] private KeyCode _randomUnitSpawningKey;
        [SerializeField] private UnitObject[] _randomUnits;

        [SerializeField] private int _currentCoins;
        [SerializeField] private bool _addTenCoins;


        public GameSideDefault Side { get; set; }

        private GameManager GM => GameManager.Instance;

        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            if (_addTenCoins)
            {
                Side.Resources.CoinsWallet.Earn(10);
            }
        }

        private void Start()
        {
            Side = GM.AddGameSide<GameSideLocal>();
        }

        private void Update()
        {
            _currentCoins = Side.Resources.CoinsWallet.Balance;

            if (Input.GetKeyDown(_randomUnitSpawningKey))
            {
                if (MonoSimples.TryGetWorldPositionOnMouse(Camera.main, out var floorPos, GM.GameRules.GroundLayer))
                {
                    var spawnResult = GM.SpawnUnit_for_pseudo(_randomUnits.RandElement(), Side, floorPos);

                    if (spawnResult.TryGetUnit(out var u))
                    {
                        Debug.Log($"{u.name} spawned!");
                    }
                    else
                    {
                        Debug.Log($"Unit was not spawned. Error: {spawnResult.FailureReason}");
                    }
                }

            }
        }

    }
}
