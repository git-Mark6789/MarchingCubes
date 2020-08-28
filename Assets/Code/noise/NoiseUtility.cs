using UnityEngine;
using System.Collections;



namespace MarchingCode.MarchingCubes.Assets.Codebase
{
    /// <summary>
    /// Helper functions
    /// </summary>
    public static class NoiseUtility
    {
        /// <summary>
        /// Simple 3D perlin noise constructed from a cube structure of 2D samples
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static float Perlin3D(float x, float y, float z)
        {
            float ab = Mathf.PerlinNoise(x, y);
            float bc = Mathf.PerlinNoise(y, z);
            float ac = Mathf.PerlinNoise(x, z);

            float ba = Mathf.PerlinNoise(y, x);
            float cb = Mathf.PerlinNoise(z, y);
            float ca = Mathf.PerlinNoise(z, x);

            float abc = ab + bc + ac + ba + cb + ca;
            return abc / 6f;
        }
    }
}
