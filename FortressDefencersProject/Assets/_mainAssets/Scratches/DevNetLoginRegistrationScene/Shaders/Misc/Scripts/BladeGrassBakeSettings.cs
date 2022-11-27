using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevourDev.Shaders
{
    [CreateAssetMenu(fileName = "BladeGrassMeshSettings", menuName = "NedMakesGames/BladeGrassMeshSettings")]
    public class BladeGrassBakeSettings : ScriptableObject
    {
        [Tooltip("The source mesh to build off of")]
        public Mesh SourceMesh;
        [Tooltip("The submesh index of the source mesh to use")]
        public int SourceSubMeshIndex = 0;
        [Tooltip("A scale to apply to the source mesh before generating pyramids")]
        public Vector3 Scale = Vector3.one;
        [Tooltip("A rotation to apply to the source mesh before generating pyramids. Euler angles, in degrees")]
        public Vector3 Rotation;
        [Tooltip("An offset to the random function used in the compute shader")]
        public Vector3 RandomOffset = new Vector3(.07f, .13f, .19f);
        [Tooltip("The number of segments per blade. Will be clamped by the max value in the compute shader")]
        public int NumBladeSegments = 3;
        [Tooltip("The curveature shape of a grass blade")]
        public float Curvature = 3;
        [Tooltip("The maximum bend angle of a grass blade. In degrees")]
        public float MaxBendAngle = 30;
        [Tooltip("Grass blade height")]
        public float Height = 1;
        [Tooltip("Grass blade height variance")]
        public float HeightVariance = .1f;
        [Tooltip("Grass blade width")]
        public float Width = .5f;
        [Tooltip("Grass blade width variance")]
        public float WidthVariance = .05f;
    }
}
