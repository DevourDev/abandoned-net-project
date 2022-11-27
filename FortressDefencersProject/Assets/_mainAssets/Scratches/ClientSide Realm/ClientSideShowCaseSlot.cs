using FD.ClientSide.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FD.ClientSide.UiHandlers
{
    public class ClientSideShowCaseSlot : MonoBehaviour
    {
        public event System.Action<ClientSideShowCaseSlot> OnClick;

        [SerializeField] private TextMeshProUGUI _unitNameText;
        [SerializeField] private TextMeshProUGUI _unitCostText;
        [SerializeField] private Image _unitIcon;
        [SerializeField] private Button _buyUnitButton;

        [SerializeField] private Color _emptyColor = new(0.2f, 0.2f, 0.2f, 1f);

        //[SerializeField] private TextMeshProUGUI _unitCostText;

        private ClientSideGameManager _cgm;

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

        public int SlotID { get; private set; }
        public int UnitID { get; private set; }



        private void OnDestroy()
        {
            _buyUnitButton.onClick.RemoveListener(Click);
        }
        private void Awake()
        {
            SlotID = -1;
            UnitID = -2; //to reset name/cost if receiving null (id == -1) unit
        }

        private void Start()
        {
            _buyUnitButton.onClick.AddListener(Click);
        }

        public void Init(int slotID)
        {
            SlotID = slotID;
        }

        private void Click()
        {
            OnClick?.Invoke(this);
        }

        public void SetEmpty()
        {
            UnitID = -1;
            _unitNameText.text = string.Empty;
            _unitCostText.text = string.Empty;
            _unitIcon.sprite = null;
            _unitIcon.color = _emptyColor;
            _buyUnitButton.interactable = false;
        }

        public void SetUnit(int unitID)
        {
            if (UnitID == unitID)
                return;

            if (unitID == -1)
            {
                SetEmpty();
                return;
            }

            if (UnitID == -1) // sign, that this slot is empty (so we have to fix its color and interactiveness)
            {
                _buyUnitButton.interactable = true;
                _unitIcon.color = Color.white;
            }


            UnitID = unitID;
            var u = CGM.GameRules.UnitsDatabase.GetElement(UnitID);

            _unitNameText.text = u.UnitName.GetAnyNameOrDefault("NameNotInited");
            _unitCostText.text = u.CoinsCost.ToString();
            _unitIcon.sprite = u.PreviewImage;

        }
    }
}
