using UnityEngine;

namespace ProceduralGeneration.Utilities
{
    /// <summary>
    /// Provides extension methods for <see cref="System.Random"/> that mimic Unity-style
    /// random operations, allowing consistent usage across various procedural generation tasks.
    /// </summary>
    public static class RandomUtility
    {
        /// <summary>
        /// Returns a random float within [min, max].
        /// </summary>
        public static float Range(System.Random rng, float min, float max)
        {
            return (float)(rng.NextDouble() * (max - min) + min);
        }

        /// <summary>
        /// Returns a random integer within [minInclusive, maxExclusive).
        /// </summary>
        public static int RangeInt(System.Random rng, int minInclusive, int maxExclusive)
        {
            // Ensure at least one valid integer in range
            if (maxExclusive <= minInclusive)
                return minInclusive;

            return rng.Next(minInclusive, maxExclusive);
        }

        /// <summary>
        /// Returns a random Vector2 with each component in the specified range.
        /// </summary>
        public static Vector2 RangeVector2(System.Random rng, Vector2 min, Vector2 max)
        {
            float x = Range(rng, min.x, max.x);
            float y = Range(rng, min.y, max.y);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Returns a random Vector3 with each component in the specified range.
        /// </summary>
        public static Vector3 RangeVector3(System.Random rng, Vector3 min, Vector3 max)
        {
            float x = Range(rng, min.x, max.x);
            float y = Range(rng, min.y, max.y);
            float z = Range(rng, min.z, max.z);
            return new Vector3(x, y, z);
        }
    }
}