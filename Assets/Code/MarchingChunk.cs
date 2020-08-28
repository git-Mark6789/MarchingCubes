using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MarchingCode.MarchingCubes.Assets.Codebase
{
    /// <summary>
    /// A single chunk with a mesh
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MarchingChunk : MonoBehaviour
    {
        public MeshFilter _meshFilter;
        private float isoLevel;

        // Chunk system:
        private Vector2 position;
        private Bounds bounds;

        private ScalarField scalarField;
        

        public void BuildMesh(float isoLevel, int chunkSize, float scale, int layers, float lacunarity, float persistence, Vector2 offset)
        {
            this.isoLevel = isoLevel;

            numTriangles = 0;

            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            scalarField = new PerlinScalarField(chunkSize, chunkSize, chunkSize, scale, layers, lacunarity, persistence, offset);


            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        // If we're at any edge, we can't march
                        if (x == (chunkSize - 1) || y == (chunkSize - 1) || z == (chunkSize - 1)) continue;


                        // Get the 8 points and make a cube
                        float[] cube =
                        {
                        scalarField.SamplePoint(x, y, z + 1),
                        scalarField.SamplePoint(x + 1, y, z + 1),
                        scalarField.SamplePoint(x + 1, y, z),
                        scalarField.SamplePoint(x, y, z),

                        scalarField.SamplePoint(x, y + 1, z + 1),
                        scalarField.SamplePoint(x + 1, y + 1, z + 1),
                        scalarField.SamplePoint(x + 1, y + 1, z),
                        scalarField.SamplePoint(x, y + 1, z)
                    };

                        March(cube, vertices, indices, new Vector3(x, y, z));
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.vertices = vertices.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        /// <summary>
        /// Initialize at position
        /// </summary>
        public void InitializeChunk(Vector2 coord, int size, Transform parent)
        {
            _meshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));

            transform.parent = parent;
            transform.localPosition = new Vector3(coord.x * size, 0f, coord.y * size);
            bounds = new Bounds(transform.position, Vector2.one * size);
        }

        public void UpdateTerrainChunk(Vector2 viewerPosition, float maxViewDst)
        {
            float viewerDstFromNearestEdge = bounds.SqrDistance(viewerPosition);
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }


        int numTriangles;
        /// <summary>
        /// March over a cube of values, origin should be left bottom back
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        public void March(float[] cube, List<Vector3> vertices, List<int> indices, Vector3 cubePos)
        {
            int cubeIndex = 0;
            if (cube[0] < isoLevel) cubeIndex |= 1;
            if (cube[1] < isoLevel) cubeIndex |= 2;
            if (cube[2] < isoLevel) cubeIndex |= 4;
            if (cube[3] < isoLevel) cubeIndex |= 8;
            if (cube[4] < isoLevel) cubeIndex |= 16;
            if (cube[5] < isoLevel) cubeIndex |= 32;
            if (cube[6] < isoLevel) cubeIndex |= 64;
            if (cube[7] < isoLevel) cubeIndex |= 128;

            int[] triangulationIndices = Table.Triangulation[cubeIndex];


            Vector3 adjacentVertexA, adjacentVertexB, midpointVertex;
            int indexA, indexB;

            // For each edge find it's adjacent vertices and calculate the midpoint vertex
            foreach (int edgeIndex in triangulationIndices)
            {
                if (edgeIndex == -1) break;

                indexA = Table.EdgeCornerVertices[edgeIndex, 0];
                indexB = Table.EdgeCornerVertices[edgeIndex, 1];

                adjacentVertexA.x = Table.CubeVertices[indexA, 0];
                adjacentVertexA.y = Table.CubeVertices[indexA, 1];
                adjacentVertexA.z = Table.CubeVertices[indexA, 2];

                adjacentVertexB.x = Table.CubeVertices[indexB, 0];
                adjacentVertexB.y = Table.CubeVertices[indexB, 1];
                adjacentVertexB.z = Table.CubeVertices[indexB, 2];

                float weightA = cube[indexA]; // Example A is 0.5, minimum value
                float weightB = cube[indexB]; // Example B is 1, maximum value

                // Range is 0.5...1

                midpointVertex =
                    MarchingCubesUtility.InterpolateVertex(isoLevel, adjacentVertexA, adjacentVertexB,
                    weightA, weightB);

                midpointVertex += cubePos;
                // Add vertex to list
                vertices.Add(midpointVertex);
                indices.Add(numTriangles);

                numTriangles++;
            }
        }
    }
}
