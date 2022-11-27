using DevourDev.MonoBase;
using TMPro;
using UnityEngine;

namespace FD.Networking.Garden
{
    public class GardenServerVisualizator : MonoBehaviour
    {
        [SerializeField] private GardenServerManager _gsm;
        [SerializeField] private TextMeshProUGUI _onlinePlayersText;
        [SerializeField] private TextMeshProUGUI _lobbiesText;
        [SerializeField] private TextMeshProUGUI _lastRequestText;
        [SerializeField] private TextMeshProUGUI _logText;


        private void Start()
        {
            _gsm.OnRequestReceived += GSM_OnRequestReceived;
            _gsm.OnOnlinersCountChanged += GSM_OnOnlinersCountChanged;
            _gsm.OnLobbiesCountChanged += GSM_OnLobbiesCountChanged;
            _gsm.OnVisualLog += GSM_OnVisualLog;
        }

        private void GSM_OnVisualLog(string obj)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                _logText.text = obj;

                Debug.Log("::VISUAL_LOG:: " + obj);
            });
        }

        private void GSM_OnRequestReceived(IPacketContent ipc)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                string s = ipc.GetType().Name;
                _lastRequestText.text = s;
                Debug.Log("::GSM_ON_REQUEST_RECEIVED:: " + s);
            });
        }

        private void GSM_OnOnlinersCountChanged(int v)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                string s = v.ToString();
                _onlinePlayersText.text = s;
                Debug.Log("::GSM_ON_ONLINERS_COUNT_CHANGED:: " + s);
            }, nameof(GSM_OnOnlinersCountChanged));
        }

        private void GSM_OnLobbiesCountChanged(int v)
        {
            UnityMainThreadDispatcher.InvokeOnMainThread(() =>
            {
                string s =$"Lobbies count: {v}";
                _lobbiesText.text = s;
                Debug.Log("::GSM_ON_LOBBIES_COUNT_CHANGED:: " + s);
            }, nameof(GSM_OnLobbiesCountChanged));
        }


    }
}
