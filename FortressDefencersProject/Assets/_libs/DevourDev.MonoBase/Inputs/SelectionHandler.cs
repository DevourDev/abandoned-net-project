using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevourDev.MonoBase.Inputs.SelectionSystem
{
    public class ActiveSelection
    {
        public Vector2 StartPos;
        public Vector2 EndPos;
        public Rect Rect;


    }
    [Serializable]
    public struct SelectionSettings
    { 
        //TODO: transf KeyCodes to KeyBinds
        public KeyCode[] SelectionKeys;
        public KeyCode[] MultiSelectionKeys;
        public KeyCode[] DeselectAllKeys;

        public float MinRangeForBoxSelection;
        public RectTransform BoxSelection;
        public Camera Cam;
        public LayerMask _selectablesLayerMask;

    }
    public enum SelectionState
    {
        None = 0,
        Clicked = 1,
        BoxSelecting = 2,
        Released = 3,
    }

    public class SelectionHandler : MonoBehaviour
    {
        public static SelectionHandler Instance { get; private set; }
        private bool _isSingletonValid;


        [SerializeField]
        private SelectionSettings _settings = new()
        {
            SelectionKeys = new KeyCode[] { KeyCode.Mouse0 },
            MultiSelectionKeys = new KeyCode[] { KeyCode.LeftShift },
            DeselectAllKeys = new KeyCode[] { KeyCode.Escape },
            MinRangeForBoxSelection = 3f,
        };
        [SerializeField] private HashSet<Selectable> _selectables;
        [SerializeField] private List<Selectable> _selecteds;

        private SelectionState _state;
        private ActiveSelection _activeSelection;
        private bool _isActive;

        public bool IsMultiSelecting => InputChecks.CheckForAnyKeyPressing(_settings.MultiSelectionKeys);


        private void Awake()
        {
            InitSingleton();
            if (!_isSingletonValid)
                return;

            _selectables = new HashSet<Selectable>();
            _selecteds = new List<Selectable>();
            _activeSelection = new ActiveSelection();
        }

        private void Start()
        {
            if (!_isSingletonValid)
                return;
            _isActive = true;
        }

        private void Update()
        {
            if (!_isActive)
                return;

            CheckForSelectionStart();
            CheckForDeselectAll();

            if (_state != SelectionState.Clicked && _state != SelectionState.BoxSelecting)
                return;

            CheckForBoxSelecting();
            HandleBoxSelection();

            CheckForSelectionEnd();
        }

        private void CheckForDeselectAll()
        {
            if (!InputChecks.CheckForAnyKeyPressed(_settings.DeselectAllKeys))
                return;

            SetVisualSelectionBoxActiveState(false);
            _state = SelectionState.Released;
            DeselectAll();
        }

        private void CheckForSelectionStart()
        {
            if (!InputChecks.CheckForAnyKeyPressed(_settings.SelectionKeys))
                return;

            if (!IsMultiSelecting)
                DeselectAll();

            _activeSelection.StartPos = Input.mousePosition;

            if (InputChecks.TryMouseCast(_settings.Cam, _settings._selectablesLayerMask, out Selectable s))
            {
                if (IsMultiSelecting)
                {
                    if (s.IsSelected)
                    {
                        TryDeselectObject(s);
                    }
                    else
                    {
                        TrySelectObject(s);
                    }
                }
                else
                {
                    DeselectAll();
                    TrySelectObject(s);
                }
            }

            _state = SelectionState.Clicked;

        }

        private void CheckForBoxSelecting()
        {
            _activeSelection.EndPos = Input.mousePosition;

            if (_state == SelectionState.BoxSelecting)
                return;

            if (Vector2.Distance(_activeSelection.StartPos, _activeSelection.EndPos) >= _settings.MinRangeForBoxSelection)
            {
                _state = SelectionState.BoxSelecting;
                SetVisualSelectionBoxActiveState(true);
            }
        }

        private void HandleBoxSelection()
        {
            if (_state != SelectionState.BoxSelecting)
                return;
            BuildSelectionBox();
            UpdateVisualSelectionBox();
            SelectUnitsInBox();
        }

        private void CheckForSelectionEnd()
        {
            if (!InputChecks.CheckForAnyKeyReleased(_settings.SelectionKeys))
                return;

            //switch (_state)
            //{
            //    case SelectionState.Clicked:
            //        if (InputSimples.TryMouseCast(_settings.Cam, _settings._selectablesLayerMask, out Selectable s))
            //        {
            //            if (IsMultiSelecting)
            //            {
            //                if (s.IsSelected)
            //                {
            //                    TryDeselectObject(s);
            //                }
            //                else
            //                {
            //                    TrySelectObject(s);
            //                }
            //            }
            //            else
            //            {
            //                DeselectAll();
            //                TrySelectObject(s);
            //            }
            //        }
            //        break;
            //    default:
            //        break;
            //}

            _state = SelectionState.Released;
            SetVisualSelectionBoxActiveState(false);
        }


        private void UpdateVisualSelectionBox()
        {
            var r = _activeSelection.Rect;
            var box = _settings.BoxSelection;

            box.position = r.center;
            box.sizeDelta = new Vector2(r.width, r.height);
        }

        private void SelectUnitsInBox()
        {
            var r = _activeSelection.Rect;

            if (!IsMultiSelecting)
            {
                DeselectAll();
            }

            foreach (var s in _selectables)
            {
                if (r.Contains(OnScreenPos(s)))
                {
                    TrySelectObject(s);
                }
            }

            Vector2 OnScreenPos(Selectable s)
            {
                return _settings.Cam.WorldToScreenPoint(s.transform.position);
            }
        }

        private void BuildSelectionBox()
        {
            Vector2 minCorner = new Vector2(Math.Min(_activeSelection.StartPos.x, _activeSelection.EndPos.x),
                Math.Min(_activeSelection.StartPos.y, _activeSelection.EndPos.y));
            Vector2 size = new Vector2(Math.Abs(_activeSelection.StartPos.x - _activeSelection.EndPos.x),
                Math.Abs(_activeSelection.StartPos.y - _activeSelection.EndPos.y));

            _activeSelection.Rect = new Rect(minCorner, size);
        }


        private void SetVisualSelectionBoxActiveState(bool state)
        {
            _settings.BoxSelection.gameObject.SetActive(state);
        }
        private void DeselectAll()
        {
            foreach (var s in _selecteds)
            {
                DeselectObject(s, true);
            }

            _selecteds.Clear();
        }

        public void RegistrateSelectableObject(Selectable s)
        {
            _selectables.Add(s);
        }
        public void UnregistrateSelectableObject(Selectable s)
        {
            _selectables.Remove(s);
        }


        public bool TrySelectObject(Selectable s)
        {
            if (s.IsSelected || !s.CanBeSingleSelected || (IsMultiSelecting && !s.CanBeMultiSelected))
                return false;

            SelectObject(s);
            return true;
        }
        public bool TryDeselectObject(Selectable s)
        {
            if (!s.IsSelected)
                return false;

            DeselectObject(s);
            return true;
        }

        public void SelectObject(Selectable s)
        {
            _selecteds.Add(s);
            s.Select();
        }
        public void DeselectObject(Selectable s, bool keepInList = false)
        {
            s.Deselect();

            if (!keepInList)
                _selecteds.Remove(s);
        }

        public void Pause() => _isActive = false;
        public void Resume() => _isActive = true;

        private void InitSingleton()
        {

            if (Instance != null)
            {
                if (Instance != this)
                {
                    _isSingletonValid = false;

                    Destroy(gameObject);
                }
                else
                {

                    _isSingletonValid = true;
                }
            }
            else
            {

                Instance = this;
                _isSingletonValid = true;
            }
        }
    }
}
