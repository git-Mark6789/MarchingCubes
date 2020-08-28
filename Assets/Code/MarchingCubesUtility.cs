using UnityEngine;
using System.Collections;



namespace MarchingCode.MarchingCubes.Assets.Codebase
{
    /// <summary>
    /// Utility functions for working with the marching cubes algorithm
    /// </summary>
    public static class MarchingCubesUtility
    {
        /// <summary>
        /// Interpolate a vertex along an edge
        /// </summary>
        /// <param name="isoLevel"> The surface value </param>
        /// <param name="p1"> Point 1 </param>
        /// <param name="p2"> Point 2 </param>
        /// <param name="valp1"> Value 1 </param>
        /// <param name="valp2"> Value 2 </param>
        /// <returns></returns>
        public static Vector3 InterpolateVertex(float isoLevel, Vector3 p1, Vector3 p2, float valp1, float valp2)
        {
            float mu;
            Vector3 p;

            float epsilon = 0.00001f; // tolerance

            // For values with minimal deviation from tolerance, don't bother with calculating stuff
            if (Mathf.Abs(isoLevel - valp1) < epsilon)
                return p1;
            if (Mathf.Abs(isoLevel - valp2) < epsilon)
                return p2;
            if (Mathf.Abs(valp1 - valp2) < epsilon)
                return p1;

            mu = (isoLevel - valp1) / (valp2 - valp1);

            p.x = p1.x + mu * (p2.x - p1.x);
            p.y = p1.y + mu * (p2.y - p1.y);
            p.z = p1.z + mu * (p2.z - p1.z);

            return p;
        }
    }
}
