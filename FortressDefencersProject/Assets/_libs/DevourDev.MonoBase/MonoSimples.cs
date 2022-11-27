using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevourDev.MonoBase
{
    public static class MonoSimples
    {
        public static (T closest, float distance) GetClosestTo<T>(Vector3 origin, List<T> collection) where T : MonoBehaviour
        {
            if (collection.Count < 1)
            {
                throw new System.Exception("Zero-lengthed collection can not be processed");
            }

            var closest = collection[0];
            var closestD = CalcD(closest);

            for (int i = 1; i < collection.Count; i++)
            {
                var d = CalcD(collection[i]);
                if(d < closestD)
                {
                    closest = collection[i];
                    closestD = d;
                }
            }

            return (closest, closestD);


            float CalcD(T element)
            {
                return Vector3.Distance(origin, element.transform.position);
            }
        }
        public static Color NegateColor(Color c)
        {
            Color nc = new(1f - c.r, 1f - c.g, 1f - c.b);
            return nc;
        }

        public static bool TryCameraCast<T>(Vector2 mousePos, float depth, out T foundObject, Camera cam = null) where T : Component
        {
            if (cam == null)
                cam = Camera.main;

            Vector3 pos = mousePos;
            pos.z = depth;

            var ray = cam.ScreenPointToRay(pos);
            var hits = Physics.RaycastAll(ray, depth);

            if (hits.Length < 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.collider.TryGetComponent<T>(out var c))
                    {
                        foundObject = hit.collider.GetComponent<T>();
                        return true;
                    }
                }
            }

            foundObject = default;
            return false;
        }
        public static bool TryCameraCast<T>(Vector2 mousePos, float depth, out T foundObject, LayerMask layerMask, Camera cam = null) where T : Component
        {
            if (cam == null)
                cam = Camera.main;

            Vector3 pos = mousePos;
            pos.z = depth;

            var ray = cam.ScreenPointToRay(pos);
            var hits = Physics.RaycastAll(ray, depth, layerMask);

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.collider.TryGetComponent<T>(out var c))
                    {
                        foundObject = hit.collider.GetComponent<T>();
                        return true;
                    }
                }
            }

            foundObject = default;
            return false;
        }

        public static Vector3 WorldPositionToLocal(Vector3 parentPos, Vector3 worldPos)
        {
            Vector3 localPos = worldPos + parentPos;
            return localPos;
        }
        public static bool TryGetWorldPositionOnMouse(Camera cam, Vector2 mousePos, out Vector3 floorPos, LayerMask floorLayer)
        {
            floorPos = Vector3.zero;
            var ray = cam.ScreenPointToRay(mousePos);
            bool floorFound = Physics.Raycast(ray, out var hit, cam.farClipPlane, floorLayer);
            if (floorFound)
            {
                floorPos = hit.point;
            }

            return floorFound;
        }
        public static bool TryGetWorldPositionOnMouse(Camera cam, out Vector3 floorPos)
        {
            floorPos = Vector3.zero;
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            bool floorFound = Physics.Raycast(ray, out var hit, cam.farClipPlane);
            if (floorFound)
            {
                floorPos = hit.point;
            }

            return floorFound;
        }
        public static bool TryGetWorldPositionOnMouse(Camera cam, out Vector3 floorPos, LayerMask layerMask)
        {
            floorPos = Vector3.zero;
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            bool floorFound = Physics.Raycast(ray, out var hit, cam.farClipPlane, layerMask);
            if (floorFound)
            {
                floorPos = hit.point;
            }

            return floorFound;
        }
        public static bool TryGetWorldPositionOnMouse(Camera cam, out Vector3 floorPos, LayerMask layerMask, short forcedY)
        {
            floorPos = Vector3.zero;
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            bool floorFound = Physics.Raycast(ray, out var hit, cam.farClipPlane, layerMask);
            if (floorFound)
            {
                floorPos = hit.point;
                floorPos.y = forcedY;
            }

            return floorFound;
        }

        public static Vector3 NumericsToUnity(System.Numerics.Vector3 nv)
        {
            Vector3 uv;
            uv.x = nv.X;
            uv.y = nv.Y;
            uv.z = nv.Z;
            return uv;
        }

        public static Vector2 NumericsToUnity(System.Numerics.Vector2 nv)
        {
            Vector2 uv;
            uv.x = nv.X;
            uv.y = nv.Y;
            return uv;
        }

        public static Vector3 FlipVector(Vector2 v)
        {
            return new Vector3(v.x, 0, v.y);
        }
        public static Vector3 FlipVector(Vector3 v)
        {
            return new Vector3(v.x, v.z, v.y);
        }

        public static bool CheckGameObjectTouchesNothing(GameObject go, Vector3 pos, Quaternion rot, LayerMask lm)
        {
            var col = go.GetComponent<Collider>();
            Collider[] overlappedCols = col switch
            {
                BoxCollider boxC => Physics.OverlapBox(pos, boxC.bounds.extents, rot, lm),
                CapsuleCollider capC => Physics.OverlapCapsule(pos, pos + new Vector3(0, capC.height, 0), capC.radius, lm),
                _ => Physics.OverlapBox(pos, col.bounds.extents, rot, lm),
            };

            return overlappedCols.Length == 0;
        }

        public static bool CheckGameObjectTouchesNothing(GameObject go, Vector3 pos, Quaternion rot, LayerMask lm, out Collider[] overlappedCols)
        {
            var col = go.GetComponent<Collider>();
            if (col == null)
                Debug.LogError("No Collider detected!");

            overlappedCols = col switch
            {
                BoxCollider boxC => Physics.OverlapBox(pos, boxC.bounds.extents, rot, lm),
                CapsuleCollider capC => Physics.OverlapCapsule(pos, pos + new Vector3(0, capC.height, 0), capC.radius, lm),
                _ => Physics.OverlapBox(pos, col.bounds.extents, rot, lm),
            };

            return overlappedCols.Length == 0;
        }

    }
}