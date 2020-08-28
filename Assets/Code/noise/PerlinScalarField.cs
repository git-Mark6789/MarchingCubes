using UnityEngine;
using System.Collections;



namespace MarchingCode.MarchingCubes.Assets.Codebase
{
    public class PerlinScalarField : ScalarField
    {

        public PerlinScalarField(int width, int height, int depth, float scale, int layers, float lacunarity, float persistence,
            Vector2 offset, bool spherical = true) : base(width, height, depth)
        {

            float sampleX, sampleY, sampleZ;

            Vector3 p;

            // Stuff for spherical regions...
            Vector3 centerPoint = new Vector3(width / 2, height / 2, depth / 2);
            float maxDist = Vector3.Distance(Vector3.zero, centerPoint);


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        p.x = x;
                        p.y = y;
                        p.z = z;

                        float noiseVal = 0f;
                        float amplitude = 1;
                        float frequency = 1;

                        // Additive layering of noise
                        for (int o = 0; o < layers; o++)
                        {
                            sampleX = offset.x + (x / scale * frequency);
                            sampleY = (y / scale * frequency);
                            sampleZ = offset.y + (z / scale * frequency);

                            noiseVal += NoiseUtility.Perlin3D(sampleX, sampleY, sampleZ) * amplitude;

                            frequency *= lacunarity;
                            amplitude *= persistence;
                        }

                        if (spherical)
                        {
                            noiseVal *= (maxDist - Vector3.Distance(p, centerPoint)) / maxDist;
                        }

                        points[x + width * (y + depth * z)] = noiseVal;

                        // If we're at an edge of the region, set point value to 0 to define a "boundary" along the outside
                        if (x == (height - 1) || y == (height - 1) || z == (height - 1) || x == 0 || y == 0 || z == 0)
                            points[x + width * (y + depth * z)] = 0;
                    }
                }
            }
        }
    }
}