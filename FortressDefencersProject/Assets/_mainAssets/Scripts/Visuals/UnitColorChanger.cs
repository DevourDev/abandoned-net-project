using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Visuals
{
    public class UnitColorChanger : MonoBehaviour
    {
        [SerializeField] private Renderer[] _renderers;

        [SerializeField] private Material _mat;
        [SerializeField] private bool _useColor;
        [SerializeField] private Color _color;


        public void Set(Color c)
        {
            foreach (var r in _renderers)
            {
                r.material.color = c;
            }
        }
        public void Set(Material m)
        {
            foreach (var r in _renderers)
            {
                r.material = m;
            }
        }
    }
}
