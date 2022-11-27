using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevourDev.MonoBase.Inputs
{
    public static class InputChecks

    {
        public static bool CheckForAnyBindPressing(params KeyBind[] keyBinds)
        {
            foreach (var binds in keyBinds)
            {
                if (CheckForAllKeysPressing(binds.Keys))
                    return true;
            }

            return false;
        }
        public static bool CheckForAllBindsPressing(params KeyBind[] keyBinds)
        {
            foreach (var binds in keyBinds)
            {
                if (!CheckForAllKeysPressing(binds.Keys))
                    return false;
            }

            return true;
        }

        public static bool CheckForAnyKeyPressing(params KeyCode[] keys)
        {
            foreach (var k in keys)
            {
                if (Input.GetKey(k))
                    return true;
            }

            return false;
        }


        public static bool CheckForAnyKeyPressed(params KeyBind[] keyBinds)
        {
            foreach (var binds in keyBinds)
            {
                if (CheckForAnyKeyPressed(binds.Keys))
                    return true;
            }

            return false;
        }
        public static bool CheckForAnyKeyPressed(params KeyCode[] keys)
        {
            foreach (var k in keys)
            {
                if (Input.GetKeyDown(k))
                    return true;
            }

            return false;
        }

        public static bool CheckForAnyKeyReleased(params KeyBind[] keyBinds)
        {
            foreach (var binds in keyBinds)
            {
                if (CheckForAnyKeyReleased(binds.Keys))
                    return true;
            }

            return false;
        }
        public static bool CheckForAnyKeyReleased(params KeyCode[] keys)
        {
            foreach (var k in keys)
            {
                if (Input.GetKeyUp(k))
                    return true;
            }

            return false;
        }

        public static bool CheckForAllKeysPressing(params KeyCode[] keys)
        {
            foreach (var k in keys)
            {
                if (!Input.GetKey(k))
                    return false;
            }

            return true;
        }


        public static bool TryMouseCast<T>(Camera cam, LayerMask layerMask, out T item) where T : MonoBehaviour
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, cam.farClipPlane, layerMask))
            {
                return hit.collider.TryGetComponent(out item);
            }
            item = null;
            return false;
        }
    }
}
