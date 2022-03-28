using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace Helper
{
    public static class MathHelper
    {
        #region Random

        private const int A = 16807;
        private const int M = 2147483647;
        private const int Q = 127773;
        private const int R = 2836;

        public static int GetRandomSeed()
        {
            return Random.Range(1, int.MaxValue);
        }

        public static float GetRandomBySeed(this ref int seed)
        {
            var hi = seed / Q;
            var lo = seed % Q;
            seed = A * lo - R * hi;
            if (seed <= 0) seed += M;
            return seed * 1.0f / M;
        }

        public static int GetRandomSeedBySeed(this ref int seed)
        {
            return (int)(int.MaxValue * seed.GetRandomBySeed());
        }

        public static int GetRandomRange(this ref int seed, int min, int max)
        {
            return (int)math.floor(seed.GetRandomBySeed() * (max - min) + min);
        }

        public static float GetRandomRange(this ref int seed, float min, float max)
        {
            return seed.GetRandomBySeed() * (max - min) + min;
        }

        #endregion

        #region float2

        public static float3 ToFloat3(this float2 v)
        {
            return new float3(v.x, v.y, 0);
        }

        #endregion

        #region float

        public static void CheckMax(this ref float f, float max)
        {
            if (f > max) f = max;
        }

        public static void CheckMin(this ref float f, float min)
        {
            if (f < min) f = min;
        }

        public static void GetMax(this ref float f, float other)
        {
            if (other > f) f = other;
        }

        public static void GetMin(this ref float f, float other)
        {
            if (other < f) f = other;
        }

        #endregion

        #region int

        public static void CheckMax(this ref int i, int max)
        {
            if (i > max) i = max;
        }

        public static void CheckMin(this ref int i, int min)
        {
            if (i < min) i = min;
        }

        public static void GetMax(this ref int i, int other)
        {
            if (other > i) i = other;
        }

        public static void GetMin(this ref int i, int other)
        {
            if (other < i) i = other;
        }

        #endregion
    }
}