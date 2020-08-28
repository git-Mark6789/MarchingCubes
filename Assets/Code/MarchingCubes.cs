using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace MarchingCode.MarchingCubes.Assets.Codebase
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MarchingCubes : MonoBehaviour
    {
        public ScalarField ScalarPointField { get; set; }


        [Range(0, 1)]
        public float isoLevel;

        [Range(2, 200)]
        public int fieldSize;

        [Range(1, 12)]
        public int layers;
        public float scale;
        public float lacunarity;
        [Range(0, 1)]
        public float persistence;
        public bool spherical;

        private int numTriangles;
        private System.Random rand;


        private void Start()
        {
            ReloadMesh();
        }

        public void ReloadMesh()
        {
            rand = new System.Random(); // Initialize with new seed
            numTriangles = 0;

            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();
            Vector3 cubeOffset;
            Vector2 offset;

            offset.x = rand.Next(1, 100000);
            offset.y = rand.Next(1, 100000);

            // Quickly create a scalar field
            ScalarPointField = new PerlinScalarField(fieldSize, fieldSize, fieldSize, scale, layers, lacunarity, persistence, offset, spherical);


            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    for (int z = 0; z < fieldSize; z++)
                    {
                        // If we're at any edge, we can't march as we need all the surrounding points
                        if (x == (fieldSize - 1) || y == (fieldSize - 1) || z == (fieldSize - 1)) continue;


                        // Get the 8 points and make a cube
                        float[] cube =
                        {
                            ScalarPointField.SamplePoint(x, y, z + 1),
                            ScalarPointField.SamplePoint(x + 1, y, z + 1),
                            ScalarPointField.SamplePoint(x + 1, y, z),
                            ScalarPointField.SamplePoint(x, y, z),

                            ScalarPointField.SamplePoint(x, y + 1, z + 1),
                            ScalarPointField.SamplePoint(x + 1, y + 1, z + 1),
                            ScalarPointField.SamplePoint(x + 1, y + 1, z),
                            ScalarPointField.SamplePoint(x, y + 1, z)
                        };

                        cubeOffset.x = x;
                        cubeOffset.y = y;
                        cubeOffset.z = z;

                        March(cube, vertices, indices, cubeOffset);
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // 32-bit index format for large meshes

            mesh.vertices = vertices.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        /// <summary>
        /// March over a cube of values, origin should be left bottom back
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        public void March(float[] cube, List<Vector3> vertices, List<int> indices, Vector3 cubePos)
        {
            int cubeIndex = 0;
            if (cube[0] < isoLevel) cubeIndex |= 1; // Bitwise OR based on whether the value at cube[x] is inside, or outside of the surface 
            if (cube[1] < isoLevel) cubeIndex |= 2;
            if (cube[2] < isoLevel) cubeIndex |= 4;
            if (cube[3] < isoLevel) cubeIndex |= 8;
            if (cube[4] < isoLevel) cubeIndex |= 16;
            if (cube[5] < isoLevel) cubeIndex |= 32;
            if (cube[6] < isoLevel) cubeIndex |= 64;
            if (cube[7] < isoLevel) cubeIndex |= 128;

            int[] triangulationIndices = Table.Triangulation[cubeIndex]; // Get triangles from table based on the specific combination of bits in cubeIndex


            Vector3 adjacentVertexA, adjacentVertexB, midpointVertex;
            int indexA, indexB;

            // For each edge find it's adjacent vertices and calculate the midpoint vertex
            foreach (int edgeIndex in triangulationIndices)
            {
                if (edgeIndex == -1) break; // -1 signals we have passed the last index, terminate loop.

                // Find the two vertices forming this edge
                indexA = Table.EdgeCornerVertices[edgeIndex, 0];
                indexB = Table.EdgeCornerVertices[edgeIndex, 1];

                // Set both adjacent vertices
                adjacentVertexA.x = Table.CubeVertices[indexA, 0];
                adjacentVertexA.y = Table.CubeVertices[indexA, 1];
                adjacentVertexA.z = Table.CubeVertices[indexA, 2];

                adjacentVertexB.x = Table.CubeVertices[indexB, 0];
                adjacentVertexB.y = Table.CubeVertices[indexB, 1];
                adjacentVertexB.z = Table.CubeVertices[indexB, 2];

                // Store vertex weights (influence on midpoint position)
                float weightA = cube[indexA];
                float weightB = cube[indexB];

                // Find the midpoint vertex with linear interpolation based on the 2 vertices and their weights
                midpointVertex = MarchingCubesUtility.
                    InterpolateVertex(isoLevel, adjacentVertexA, adjacentVertexB, weightA, weightB);

                midpointVertex += cubePos; // Apply world space position

                // Add mesh data
                vertices.Add(midpointVertex);
                indices.Add(numTriangles);

                numTriangles++; // increment index
            }
        }
    }
}
