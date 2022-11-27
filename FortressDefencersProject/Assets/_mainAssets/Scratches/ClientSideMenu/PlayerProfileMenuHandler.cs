using FD.Networking.Garden.Packets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FD.ClientSide.UiHandlers
{
    public class PlayerProfileMenuHandler : UiHandlerBase
    {
        [SerializeField] private TextMeshProUGUI _nickNameText;
        [SerializeField] private TextMeshProUGUI _currentMmrText;


        public void SetName(string v) => _nickNameText.text = v;

        public void SetMmr(int v)
        {
            _currentMmrText.text = $"MM pts: {v}";
        }

        private void OnDisable()
        {

        }

        private async void OnEnable()
        {
            //request actual MMR value (todo: create packet from client to garden)
            GetMyProfileDataRequest req = new();
            var rawRes = await CM.GardenConnection.Channel.RequestingConnection.RequestAsync(req);

            if (rawRes is GetMyProfileDataResponse res && res.Result)
            {
                _nickNameText.text = res.NickName;
                _currentMmrText.text = $"MM pts: {res.Mmr}";
            }



        }


    }
}
