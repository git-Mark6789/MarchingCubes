using UnityEngine;
using System.Collections;


namespace MarchingCode.MarchingCubes.Assets.Codebase
{
    /// <summary>
    /// Provides a 3D scalar region full of points.
    /// </summary>
    public class ScalarField
    {
        protected float[] points;

        protected int width;
        protected int height;
        protected int depth;


        public ScalarField(int width, int height, int depth)
        {
            points = new float[width * height * depth];

            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public float SamplePoint(int x, int y, int z)
        {
            return points[x + width * (y + depth * z)];
        }

        public float SamplePoint(int index)
        {
            return points[index];
        }
    }
}

