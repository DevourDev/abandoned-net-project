using DevourDev.Base;
using DevourDev.MonoBase;
using DevourDev.Networking;
using FD.Units;
using FD.Units.Stats;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FD.Visuals
{
    public class DynamicStatSlider : MonoBehaviour
    {
        [SerializeField] private UnitOnSceneBase _serverSideUnit;
        [SerializeField] private FD.ClientSide.Units.UnitOnSceneClientSide _clientSideUnit;
        [SerializeField] private DynamicStatObject _dynamicStatObject;

        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _inSliderText;
        [SerializeField] private TextMeshProUGUI _regenerationValueText;
        [SerializeField] private Image _fillImg;
        [SerializeField] private Image _bgImg;

        private float _previousMinValue;
        private float _previousMaxValue;

        //TODO: switch to Awake
        private void Start()
        {
            var networkManager = FD.Networking.NetworkManager.Instance;
            if (networkManager == null)
                throw new NullReferenceException(nameof(networkManager) + " is null.");

            DynamicStat ds;

            if (networkManager.Mode == NetworkMode.Client || networkManager.Mode == NetworkMode.Host)
            {
                if (!_clientSideUnit.DynamicStatsCollection.TryGetDynamicStat(_dynamicStatObject, out ds))
                {
                    Debug.LogError($"No {_dynamicStatObject.StatName.GetAnyNameOrDefault()} in Unit {_serverSideUnit.name} detected.");
                    return;
                }
            }
            else if (networkManager.Mode == NetworkMode.Server)
            {
                if (!_serverSideUnit.DynamicStatsCollection.TryGetDynamicStat(_dynamicStatObject, out ds))
                {
                    Debug.LogError($"No {_dynamicStatObject.StatName.GetAnyNameOrDefault()} in Unit {_serverSideUnit.name} detected.");
                    return;
                }
            }
            else
            {
                Debug.LogError($"Unexpected NetworkMode: {networkManager.Mode}");
                ds = null;
                return;
            }
            //SYNC FROM START (sliders are at default values intil first event's invokation)

            InitVisuals(ds);
            ds.OnCurrentValueChanged += OnCurrentValueChanged;
           
        }


        private void InitVisuals(DynamicStat ds)
        {
            _slider.minValue = ds.Min;
            _slider.maxValue = ds.Max;
            _slider.value = ds.Current;


            _inSliderText.text = $"{_dynamicStatObject.StatName.GetAnyNameOrDefault()}: {ds.Current}/{ds.Max}";

            _regenerationValueText.text = ds.Regen.ToString();

            _fillImg.color = _dynamicStatObject.Color;
            _bgImg.color = _dynamicStatObject.Color - new Color(0.4f, 0.4f, 0.4f, 0f);
            _inSliderText.color = MonoSimples.NegateColor(_dynamicStatObject.Color);
        }
        private void OnCurrentValueChanged(DynamicStat ds, float before, float softedDelta, float realDelta)
        {
            //нужно добавить всяких визуальных фишек типа "фейдинга" шкалы или, хз...
            _slider.value = ds.Current;
            _inSliderText.text = $"{_dynamicStatObject.StatName.GetAnyNameOrDefault()}: {ds.Current}/{ds.Max}";

            if (_previousMinValue != ds.Min)
            {
                _slider.minValue = ds.Min;
                _previousMinValue = ds.Min;
            }

            if (_previousMaxValue != ds.Max)
            {
                _slider.maxValue = ds.Max;
                _previousMaxValue = ds.Max;
            }
        }


    }
}
