using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DevourDev.MonoBase.Inputs.SelectionSystem
{
    public abstract class Selectable : MonoBehaviour
    {
        [SerializeField] private bool _selected;
        [SerializeField] private UnityEvent _onSelectedEvent;
        [SerializeField] private UnityEvent _onDeselectedEvent;

        [SerializeField] private bool _canBeSingleSelected = true;
        [SerializeField] private bool _canBeMultiSelected = true;

        private SelectionHandler _handler;


        public bool IsSelected { get => _selected; protected set => _selected = value; }
        public bool CanBeSingleSelected { get => _canBeSingleSelected; protected set => _canBeSingleSelected = value; }
        public bool CanBeMultiSelected { get => _canBeMultiSelected; protected set => _canBeMultiSelected = value; }

        protected SelectionHandler Handler => _handler;
        protected UnityEvent OnSelectedEvent => _onSelectedEvent;
        protected UnityEvent OnDeselectedEvent => _onDeselectedEvent;


        protected void Init()
        {
            _handler = SelectionHandler.Instance;
            _handler.RegistrateSelectableObject(this);
        }

        public void Select()
        {
            if (IsSelected)
                return;

            OnSelectedEvent?.Invoke();
            _selected = true;
        }
        public void Deselect()
        {
            if (!IsSelected)
                return;

            OnDeselectedEvent?.Invoke();
            _selected = false;
        }
    }
}
