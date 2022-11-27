using DevourDev.MonoBase;
using UnityEngine;

namespace FD.ClientSide.Global
{
    public class ClientSideInputsHandler : MonoBehaviour
    {
        private FD.Global.Rules.GameRulesObject _grm;
        private ClientSideGameManager _gm;
        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
            _gm = ClientSideGameManager.Instance;
          //  _grm = GameManager.Instance.GameRules;
        }

        private void Update()
        {
            if (MonoSimples.TryGetWorldPositionOnMouse(_cam, out var spawnPos, _grm.GroundLayer, 0))
            {
                //var tryPlaceResponse = await _gm.RequestUnitPlacingAsync(spawnPos);
                //if (tryPlaceResponse.Result)
                //{

                //    Debug.Log("Here you should add unit in ClientSideGameManager");

                //    Debug.Log("But we'll send TEST REQUEST =)");

                //}
                //else
                //{

                //    Debug.Log("Holy fuck, you've got GNOMED...");
                //}
            }
        }
    }
}