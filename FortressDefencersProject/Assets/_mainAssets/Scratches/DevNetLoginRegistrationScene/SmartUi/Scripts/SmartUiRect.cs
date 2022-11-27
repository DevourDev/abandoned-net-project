using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevourDev
{
    [System.Obsolete("WIP")]
    public class SmartUiRect : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private RectTransform _toTranslate;
        [SerializeField] private Vector2 _newCenterPos;
        [SerializeField] private Vector2 _newSize;
        [SerializeField] private float _transDur;

        [SerializeField] private Vector3[] _localCorners;
        [SerializeField] private Vector3[] _worldCorners;

        private List<RectTransform> _placedElements;

        private void Update()
        {
            //_rectTransform.GetLocalCorners(_localCorners);
            //_rectTransform.GetWorldCorners(_worldCorners);


            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(TranslateUiElement(_toTranslate, _newCenterPos, _newSize, _transDur));
            }


        }


        public void PlaceElement(RectTransform el)
        {

        }

        /// <summary>
        /// lower corner == lower left corner
        /// </summary>
        /// <param name="rectSize"></param>
        /// <returns></returns>
        private Vector2 FindLowerCornerPosition(Vector2 rectSize)
        {
            return Vector2.one;
        }


        private IEnumerator TranslateUiElement(RectTransform r, Vector2 newCenterPos, Vector2 newSize, float translationDuration)
        {
            Vector2 oldCenterPos = r.position;
            Vector2 oldSize = r.sizeDelta;

            Vector2 deltaPos = newCenterPos - oldCenterPos;
            Vector2 deltaSize = newSize - oldSize;
            float dur = 0;
            while (true)
            {
                float deltaT = Time.deltaTime;
                Vector3 posStep = deltaPos * deltaT / translationDuration;
                Vector2 sizeStep = deltaSize * deltaT / translationDuration;
                r.position += posStep;
                r.sizeDelta += sizeStep;
                dur += deltaT;

                if (dur >= translationDuration)
                {
                    r.position = newCenterPos;
                    r.sizeDelta = newSize;
                    break;
                }

                yield return null;
            }
        }

    }
}
