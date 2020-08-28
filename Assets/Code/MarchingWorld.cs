using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MarchingCode.MarchingCubes.Assets.Codebase
{
    /// <summary>
    /// Responsive for managing chunks
    /// </summary>
    public class MarchingWorld : MonoBehaviour
    {
        public List<MarchingChunk> marchingChunks = new List<MarchingChunk>();
        public int height;
        public int width;


        private void Start()
        {
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                }
            }
        }
    }
}
