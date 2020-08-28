using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace MarchingCode.MarchingCubes.Assets.Codebase
{
    public class World : MonoBehaviour
    {
        public MarchingChunk marchingChunkPrefab;
        public Transform viewer;

        public const float maxViewDst = 60;
        public static Vector2 viewerPosition;

        [Header("Terrain Structure")]
        public int chunkSize;
        public int layers;
        [Range(0f, 1f)]
        public float isoLevel; // surface level
        public float scale;
        public float lacunarity;

        [Range(0f, 1f)]
        public float persistence;

        private int numChunksVisibleWithinRange;
        private Dictionary<Vector2, MarchingChunk> chunkDictionary = new Dictionary<Vector2, MarchingChunk>();
        private List<MarchingChunk> terrainChunksMarkedVisible = new List<MarchingChunk>();



        private void Start()
        {
            numChunksVisibleWithinRange = Mathf.RoundToInt(maxViewDst / chunkSize);
        }

        private void Update()
        {
            viewerPosition.x = viewer.position.x;
            viewerPosition.y = viewer.position.z;

            UpdateVisibleChunks();
        }

        /// <summary>
        /// Needs more work!
        /// </summary>
        private void UpdateVisibleChunks()
        {
            terrainChunksMarkedVisible.Clear();

            int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);
            Vector2 inspectedChunkCoord;
            Vector2 offset;

            for (int yOffset = -numChunksVisibleWithinRange; yOffset <= numChunksVisibleWithinRange; yOffset++)
            {
                for (int xOffset = -numChunksVisibleWithinRange; xOffset <= numChunksVisibleWithinRange; xOffset++)
                {
                    inspectedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (chunkDictionary.ContainsKey(inspectedChunkCoord))
                    {
                        chunkDictionary[inspectedChunkCoord].UpdateTerrainChunk(viewerPosition, maxViewDst);

                        if (chunkDictionary[inspectedChunkCoord].IsVisible())
                        {
                            terrainChunksMarkedVisible.Add(chunkDictionary[inspectedChunkCoord]);
                        }
                    }
                    else
                    {
                        // Instantiate new chunk...
                        offset.x = inspectedChunkCoord.x;
                        offset.y = inspectedChunkCoord.y;

                        MarchingChunk chunkInstance = Instantiate(marchingChunkPrefab);
                        chunkInstance.InitializeChunk(inspectedChunkCoord, chunkSize + 2, transform);
                        chunkInstance.BuildMesh(isoLevel, chunkSize, scale, layers, lacunarity, persistence, offset);

                        chunkDictionary.Add(inspectedChunkCoord, chunkInstance);
                    }
                }
            }
        }
    }
}

