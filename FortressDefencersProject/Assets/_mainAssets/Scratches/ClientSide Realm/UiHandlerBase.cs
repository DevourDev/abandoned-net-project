using FD.ClientSide.Global;
using FD.Networking.Client;
using UnityEngine;

namespace FD.ClientSide.UiHandlers
{
    public abstract class UiHandlerBase : MonoBehaviour
    {
        private ClientSideGameManager _cgm;
        private ClientManager _cm;


        protected ClientSideGameManager CGM
        {
            get
            {
                if (_cgm == null)
                {
                    _cgm = ClientSideGameManager.Instance;
                }
                return _cgm;
            }
        }

        protected ClientManager CM
        {
            get
            {
                if (_cm == null)
                {
                    _cm = ClientManager.Instance;
                }

                return _cm;
            }
        }
    }

}
