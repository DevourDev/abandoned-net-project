using DevourDev.MonoExtentions;
using FD.ClientSide.Global;
using FD.Networking.Client;
using FD.Networking.Realm.GamePackets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FD.ClientSide.UiHandlers
{
    public class ClientSideShowCase : MonoBehaviour
    {
        [System.Serializable]
        private class KeyBindsSettings
        {
            public KeyCode Refresh = KeyCode.R;
            public KeyCode Slot0 = KeyCode.Alpha1;
            public KeyCode Slot1 = KeyCode.Alpha2;
            public KeyCode Slot2 = KeyCode.Alpha3;
            public KeyCode Slot3 = KeyCode.Alpha4;
            public KeyCode Slot4 = KeyCode.Alpha5;
        }
        /// <summary>
        /// You know what? We know each other for soooo long...
        /// I think our friendship lasts more than our lives...
        /// So can I have a proper name?
        /// </summary>
        public event System.Action<ClientSideShowCaseSlot> OnTryToBuyUnitWithUnitInHand;
        public event System.Action<ClientSideShowCaseSlot> OnNotEnoughCoinsToBuyUnit;
        public event System.Action<PlaceUnitResponse> OnUnableToPlaceUnit;

        [SerializeField] private ClientSideRealmHandler _realmHandler;
        [SerializeField] private ClientSideShowCaseSlot _slotPrefab;
        [SerializeField] private Transform[] _slotsPositions;

        [SerializeField] private KeyBindsSettings _binds;

        private ClientSideShowCaseSlot[] _slotsOnScreen;
        private ClientSideGameManager _cgm;
        private ClientManager _cm;

        private int _unitInHandID = -1;
        private ClientSideShowCaseSlot _lastActivatedSlot;
        //private kind_of_GameObject _unitInHand;


        public int UnitInHandID => _unitInHandID;

        private ClientSideGameManager CGM
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


        private void OnDestroy()
        {
            try
            {
                if (_cm != null)
                    _cm.OnRealmResponseReceived -= CM_OnRealmResponseReceived;
            }
            catch (System.Exception) { }
        }

        private void Start()
        {
            //_slotsOnScreen = new ClientSideShowCaseSlot[5]; //todo: this info should be received from GameRules (which should be updated or sended from Realm (or dont give a cow)
            CM.OnRealmResponseReceived += CM_OnRealmResponseReceived;
        }

        private void Update()
        {
            CheckInputs();
        }

        private void CheckInputs()
        {
            if (Input.GetKeyDown(_binds.Refresh))
                _realmHandler.SendRefreshShowCaseRequest();

            if (Input.GetKeyDown(_binds.Slot0))
                ClientSideShowCase_OnClick(_slotsOnScreen[0]);

            if (Input.GetKeyDown(_binds.Slot1))
                ClientSideShowCase_OnClick(_slotsOnScreen[1]);

            if (Input.GetKeyDown(_binds.Slot2))
                ClientSideShowCase_OnClick(_slotsOnScreen[2]);

            if (Input.GetKeyDown(_binds.Slot3))
                ClientSideShowCase_OnClick(_slotsOnScreen[3]);

            if (Input.GetKeyDown(_binds.Slot4))
                ClientSideShowCase_OnClick(_slotsOnScreen[4]);
        }

        private void CM_OnRealmResponseReceived(Networking.IPacketContent p)
        {
            switch (p)
            {
                case BuyUnitResponse buRes:
                    this.InvokeOnMainThread(() => HandleBuyUnitResponse(buRes)); //already handling in ClientSideShowCase.cs (not conflicting tho)
                    break;
                case PlaceUnitResponse puRes:
                    this.InvokeOnMainThread(() => HandlePlaceUnitResponse(puRes)); //already handling in ClientSideShowCase.cs (not conflicting tho)
                    break;
                default:
                    break;
            }
        }

        private void HandlePlaceUnitResponse(PlaceUnitResponse puRes)
        {
            if (!puRes.Result)
            {
                OnUnableToPlaceUnit?.Invoke(puRes);
                return;
            }

            ClearTmp();
        }

        private void HandleBuyUnitResponse(BuyUnitResponse buRes)
        {
            if (!buRes.Result)
            {
                if (_lastActivatedSlot == null)
                {
                    Debug.LogError($"Trying to handle declined {nameof(BuyUnitRequest)} (response), but {nameof(_lastActivatedSlot)} is null");
                    return;
                }

                _lastActivatedSlot.SetUnit(UnitInHandID);
                ClearTmp();
            }
        }

        private void ClearTmp()
        {
            _unitInHandID = -1;
            _lastActivatedSlot = null;
        }

        public void SetSlots(IList<int> unitsIDs) //todo: remove repeatings (check callback)
        {
            if (_slotsOnScreen == null || _slotsOnScreen.Length != unitsIDs.Count) //todo: replace with lists (mb better to ignore and leave array)
            {
                if (_slotsOnScreen != null)
                {
                    for (int i = 0; i < _slotsOnScreen.Length; i++)
                    {
                        if (_slotsOnScreen[i] != null)
                        {
                            _slotsOnScreen[i].OnClick -= ClientSideShowCase_OnClick;
                            Destroy(_slotsOnScreen[i].gameObject);
                        }
                    }
                }

                _slotsOnScreen = new ClientSideShowCaseSlot[unitsIDs.Count];
            }

            for (int i = 0; i < unitsIDs.Count; i++)
            {
                if (_slotsOnScreen[i] == null)
                {
                    _slotsOnScreen[i] = Instantiate(_slotPrefab, _slotsPositions[i]);
                    _slotsOnScreen[i].Init(i);
                    _slotsOnScreen[i].OnClick += ClientSideShowCase_OnClick;
                }

                _slotsOnScreen[i].SetUnit(unitsIDs[i]);

            }
        }

        private void ClientSideShowCase_OnClick(ClientSideShowCaseSlot slot)
        {
            if (UnitInHandID > -1)
            {
                OnTryToBuyUnitWithUnitInHand?.Invoke(slot);

                Debug.LogError("OnTryToBuyUnitWithUnitInHand");
                return;
            }

            if (!CGM.PersonalState.CoinsWallet.CanSpend(CGM.GameRules.UnitsDatabase.GetElement(slot.UnitID).CoinsCost))
            {
                OnNotEnoughCoinsToBuyUnit?.Invoke(slot);
                Debug.LogError("OnNotEnoughCoinsToBuyUnit");
                return;
            }

            _unitInHandID = slot.UnitID;
            _lastActivatedSlot = slot;
            slot.SetEmpty();
            _realmHandler.SendBuyUnitRequest(slot.SlotID);
        }
    }
}
