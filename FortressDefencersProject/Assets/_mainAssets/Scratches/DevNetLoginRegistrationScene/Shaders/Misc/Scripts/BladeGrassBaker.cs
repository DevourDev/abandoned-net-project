using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevourDev.Shaders
{
    public static class BladeGrassBaker
    {
        // The structure to send to the compute shader
        // This layout kind assures that the data is laid out sequentially
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct SourceVertex
        {
            public Vector3 position;
        }

        // The structure received from the compute shader
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct GeneratedVertex
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector2 uv;
            public Vector3 bladeAnchor;
            public Vector3 shadowCastNormal;
        }

        // The size of one entry in the various compute buffers
        private const int SOURCE_VERT_STRIDE = sizeof(float) * (3);
        private const int SOURCE_INDEX_STRIDE = sizeof(int);
        private const int GENERATED_VERT_STRIDE = sizeof(float) * (3 + 3 + 2 + 3 + 3);
        private const int GENERATED_INDEX_STRIDE = sizeof(int);

        // This function takes in a mesh and submesh and decomposes it into vertex and index arrays
        // A submesh is a subset of triangles in the mesh. This might happen, for instance, if a mesh
        // has a multiple materials.
        private static void DecomposeMesh(Mesh mesh, int subMeshIndex, out SourceVertex[] verts, out int[] indices)
        {
            var subMesh = mesh.GetSubMesh(subMeshIndex);

            Vector3[] allVertices = mesh.vertices;
            int[] allIndices = mesh.triangles;

            verts = new SourceVertex[subMesh.vertexCount];
            indices = new int[subMesh.indexCount];
            for (int i = 0; i < subMesh.vertexCount; i++)
            {
                // Find the index in the whole mesh index buffer
                int wholeMeshIndex = i + subMesh.firstVertex;
                verts[i] = new SourceVertex()
                {
                    position = allVertices[wholeMeshIndex],
                };
            }
            for (int i = 0; i < subMesh.indexCount; i++)
            {
                // We need to offset the indices in the mesh index buffer to match
                // the indices in our new vertex buffer. Subtract by subMesh.firstVertex
                // .baseVertex is an offset Unity may define which is a global
                // offset for all indices in this submesh
                indices[i] = allIndices[i + subMesh.indexStart] + subMesh.baseVertex - subMesh.firstVertex;
            }
        }

        // This function takes a vertex and index list and converts it into a Mesh object
        private static Mesh ComposeMesh(GeneratedVertex[] verts, int[] indices)
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] bladeAnchors = new Vector3[verts.Length];
            Vector3[] shadowCastNormals = new Vector3[verts.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                var v = verts[i];
                vertices[i] = v.position;
                normals[i] = v.normal;
                uvs[i] = v.uv;
                bladeAnchors[i] = v.bladeAnchor;
                shadowCastNormals[i] = v.shadowCastNormal;
            }
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs); // TEXCOORD0
            mesh.SetUVs(1, bladeAnchors); // TEXCOORD1
            mesh.SetUVs(2, shadowCastNormals); // TEXCOORD2
            mesh.SetIndices(indices, MeshTopology.Triangles, 0, true); // This sets the index list as triangles
            mesh.Optimize(); // Let Unity optimize the buffer orders
            return mesh;
        }

        public static bool Run(ComputeShader shader, BladeGrassBakeSettings settings, out Mesh generatedMesh)
        {
            Debug.Assert(settings.NumBladeSegments > 0);

            // Decompose the mesh into vertex/index buffers
            DecomposeMesh(settings.SourceMesh, settings.SourceSubMeshIndex, out var sourceVertices, out var sourceIndices);

            // The mesh topology is triangles, so there are three indices per triangle
            int numSourceTriangles = sourceIndices.Length / 3;
            int numGeneratedVerts = numSourceTriangles * (settings.NumBladeSegments * 2 + 1) * 2; // 2 verts per segment, plus the tip, doubled
            int numGeneratedIndices = numSourceTriangles * (settings.NumBladeSegments * 2 - 1) * 3 * 2;

            GeneratedVertex[] generatedVertices = new GeneratedVertex[numGeneratedVerts];
            int[] generatedIndices = new int[numGeneratedIndices];

            // A graphics buffer is a better version of the compute buffer
            GraphicsBuffer sourceVertBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, sourceVertices.Length, SOURCE_VERT_STRIDE);
            GraphicsBuffer sourceIndexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, sourceIndices.Length, SOURCE_INDEX_STRIDE);
            GraphicsBuffer genVertBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, generatedVertices.Length, GENERATED_VERT_STRIDE);
            GraphicsBuffer genIndexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, generatedIndices.Length, GENERATED_INDEX_STRIDE);

            // Cache the kernel ID
            int idGrassKernel = shader.FindKernel("Main");

            // Set buffers and variables
            shader.SetBuffer(idGrassKernel, "_SourceVertices", sourceVertBuffer);
            shader.SetBuffer(idGrassKernel, "_SourceIndices", sourceIndexBuffer);
            shader.SetBuffer(idGrassKernel, "_GeneratedVertices", genVertBuffer);
            shader.SetBuffer(idGrassKernel, "_GeneratedIndices", genIndexBuffer);
            // Convert the scale and rotation settings into a transformation matrix
            shader.SetMatrix("_Transform", Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(settings.Rotation), settings.Scale));
            shader.SetInt("_NumSourceTriangles", numSourceTriangles);
            shader.SetVector("_RandomSeed", settings.RandomOffset);
            shader.SetFloat("_MaxBendAngle", Mathf.Deg2Rad * settings.MaxBendAngle);
            shader.SetFloat("_BladeHeight", settings.Height);
            shader.SetFloat("_BladeHeightVariance", settings.HeightVariance);
            shader.SetFloat("_BladeWidth", settings.Width);
            shader.SetFloat("_BladeWidthVariance", settings.WidthVariance);
            shader.SetInt("_NumBladeSegments", settings.NumBladeSegments);
            shader.SetFloat("_BladeCurvature", Mathf.Max(0, settings.Curvature));

            // Set data in the buffers
            sourceVertBuffer.SetData(sourceVertices);
            sourceIndexBuffer.SetData(sourceIndices);

            // Find the needed dispatch size, so that each triangle will be run over
            shader.GetKernelThreadGroupSizes(idGrassKernel, out uint threadGroupSize, out _, out _);
            int dispatchSize = Mathf.CeilToInt((float)numSourceTriangles / threadGroupSize);
            // Dispatch the compute shader
            shader.Dispatch(idGrassKernel, dispatchSize, 1, 1);

            // Get the data from the compute shader
            // Unity will wait here until the compute shader is completed
            // Don't do this as runtime. Look into AsyncGPUReadback
            genVertBuffer.GetData(generatedVertices);
            genIndexBuffer.GetData(generatedIndices);

            // Compose the vertex/index buffers into a mesh
            generatedMesh = ComposeMesh(generatedVertices, generatedIndices);

            // Release the graphics buffers, disposing them
            sourceVertBuffer.Release();
            sourceIndexBuffer.Release();
            genVertBuffer.Release();
            genIndexBuffer.Release();

            return true; // No error
        }
    }
}
