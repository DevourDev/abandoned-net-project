using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevourDev.MonoBase
{
    public abstract class GameDatabase<T> : ScriptableObject
        where T : GameDatabaseElement
    {
        [SerializeField] private bool _resetIDs;
        [SerializeField] private bool _findAllAvailableElements;
        [SerializeField] private List<T> _unsortedElements;
        [SerializeField] private int _sortedElementsCount;


        [SerializeField] private List<T> _sortedElements = new();

        #region Dictionary for some reason
        //private Dictionary<int, T> _elementsDictionary;
        //private Dictionary<int, T> ElementsDictionary
        //{
        //    get
        //    {
        //        if (_elementsDictionary == null)
        //        {
        //            _elementsDictionary = new(_sortedElements.Count);

        //            foreach (var se in _sortedElements)
        //            {
        //                _elementsDictionary.Add(se.UniqueID, se);
        //            }
        //        }

        //        return _elementsDictionary;
        //    }
        //}
        //public T GetElement(int id) => ElementsDictionary[id];
        #endregion
        public T GetElement(int id) => _sortedElements[id];

        public bool TryGetElement(int id, out T element)
        {
            if(_sortedElements.Count <= id)
            {
                element = null;
                return false;
            }

            element = GetElement(id);
            return true;
        }

        protected void ManageElementsOnValidate()
        {
            if (_resetIDs)
            {
                _resetIDs = false;
                SetIDs();
            }

            if (_findAllAvailableElements)
            {
                _findAllAvailableElements = false;
                FindAvailableElements();
            }


            if (_unsortedElements == null)
                return;

            if (_unsortedElements.Count == 0)
                return;


            foreach (var ue in _unsortedElements)
            {
                if (ue == null)
                    continue;

                if (_sortedElements.Contains(ue))
                    continue;

                _sortedElements.Add(ue);
            }

            _unsortedElements.Clear();

            SetIDs();

            _sortedElementsCount = _sortedElements.Count;
        }

        private void FindAvailableElements()
        {
#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                var element = AssetDatabase.LoadAssetAtPath<T>(path);
                _unsortedElements.Add(element);
            }
#endif
        }

        protected void SetIDs()
        {
#if UNITY_EDITOR
            for (int i = 0; i < _sortedElements.Count; i++)
            {
                _sortedElements[i].UniqueID = i;
                EditorUtility.SetDirty(_sortedElements[i]);
            }

            _sortedElementsCount = _sortedElements.Count;
#endif
        }
    }
}