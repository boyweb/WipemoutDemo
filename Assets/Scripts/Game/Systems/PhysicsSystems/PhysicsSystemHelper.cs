using Game.Components.PhysicsComponents;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Systems.PhysicsSystems
{
    public static class PhysicsSystemHelper
    {
        private const int FieldWidth = 4000;
        private const int FieldWidthHalf = FieldWidth / 2;
        private const int FieldHeight = 4000;
        private const int FieldHeightHalf = FieldHeight / 2;
        private const float Step = 1f;

        public static int Hash(float2 position)
        {
            var quantized = new int2(math.floor(position / Step));
            return quantized.x + FieldWidthHalf + (quantized.y + FieldHeightHalf) * FieldWidth;
        }

        private static float2 Correction(float2 position)
        {
            var quantized = new int2(math.floor(position / Step));
            return new float2(quantized.x * Step + Step / 2, quantized.y * Step + Step / 2);
        }

        public static NativeArray<int> GetPhysicsEntityInRadius(
            float2 center,
            float radius,
            NativeMultiHashMap<int, int> bucketList
        )
        {
            var list = new NativeList<int>(Allocator.Temp);

            var foundSize = (int)math.ceil(radius / Step);
            var cellSize = foundSize * 2 + 1;

            for (var i = 0; i < cellSize; i++)
            {
                for (var j = 0; j < cellSize; j++)
                {
                    var x = i - foundSize;
                    var y = j - foundSize;
                    var newCenter = center + new float2(x, y) * Step;
                    var hash = Hash(newCenter);
                    var found = bucketList.TryGetFirstValue(hash, out var index, out var iterator);
                    while (found)
                    {
                        list.Add(index);
                        found = bucketList.TryGetNextValue(out index, ref iterator);
                    }
                }
            }

            return new NativeArray<int>(list.AsArray(), Allocator.Temp);
        }

        public static NativeArray<int> GetSpecialIDInRadius(
            float2 center,
            float radius,
            NativeMultiHashMap<int, int> bucketList,
            NativeHashMap<int, PhysicsData> physicsList,
            PhysicsCollisionID collisionID
        )
        {
            var list = new NativeList<int>(Allocator.Temp);

            var foundSize = (int)math.ceil(radius / Step);
            var cellSize = foundSize * 2 + 1;

            for (var i = 0; i < cellSize; i++)
            {
                for (var j = 0; j < cellSize; j++)
                {
                    var x = i - foundSize;
                    var y = j - foundSize;
                    var newCenter = center + new float2(x, y) * Step;
                    var hash = Hash(newCenter);
                    var found = bucketList.TryGetFirstValue(hash, out var index, out var iterator);
                    while (found)
                    {
                        if (physicsList[index].CollisionID == collisionID)
                        {
                            list.Add(index);
                        }
                        found = bucketList.TryGetNextValue(out index, ref iterator);
                    }
                }
            }

            return new NativeArray<int>(list.AsArray(), Allocator.Temp);
        }

        public static NativeArray<int> GetNearestSpecialIDInRadius(
            float2 center,
            float radius,
            int number,
            NativeMultiHashMap<int, int> bucketList,
            NativeHashMap<int, Translation> translationList,
            NativeHashMap<int, PhysicsData> physicsList,
            PhysicsCollisionID collisionID
        )
        {
            var distanceList = new NativeList<EntityDistance>(Allocator.Temp);
            var list = new NativeList<int>(Allocator.Temp);

            var foundSize = (int)math.ceil(radius / Step);
            var cellSize = foundSize * 2 + 1;

            for (var i = 0; i < cellSize; i++)
            {
                for (var j = 0; j < cellSize; j++)
                {
                    var x = i - foundSize;
                    var y = j - foundSize;
                    var newCenter = center + new float2(x, y) * Step;
                    var hash = Hash(newCenter);
                    var found = bucketList.TryGetFirstValue(hash, out var index, out var iterator);
                    while (found)
                    {
                        if (physicsList[index].CollisionID == collisionID)
                        {
                            distanceList.Add(new EntityDistance
                            {
                                Index = index,
                                Distance = math.distance(translationList[index].Value.xz, center)
                            });
                        }
                        found = bucketList.TryGetNextValue(out index, ref iterator);
                    }
                }
            }

            distanceList.Sort();

            for (var i = 0; i < distanceList.Length && i < number; i++)
            {
                list.Add(distanceList[i].Index);
            }

            return new NativeArray<int>(list.AsArray(), Allocator.Temp);
        }
        //
        // public static NativeArray<float2> GetMostMonsterPosInRadius(
        //     float2 center,
        //     float radius,
        //     int number,
        //     NativeMultiHashMap<int, int> bucketList,
        //     NativeHashMap<int, PhysicsData> physicsList
        // )
        // {
        //     var densityList = new NativeList<MonsterDensity>(Allocator.Temp);
        //     var list = new NativeList<float2>(Allocator.Temp);
        //
        //     var foundSize = (int)math.ceil(radius / Step);
        //     var cellSize = foundSize * 2 + 1;
        //
        //     for (var i = 0; i < cellSize; i++)
        //     {
        //         for (var j = 0; j < cellSize; j++)
        //         {
        //             var x = i - foundSize;
        //             var z = j - foundSize;
        //             var newCenter = center + new float2(x, z) * Step;
        //             var hash = Hash(newCenter);
        //             var tempSize = 0;
        //             var found = bucketList.TryGetFirstValue(hash, out var index, out var iterator);
        //             while (found)
        //             {
        //                 if (physicsList[index].RoleType != RoleType.Player)
        //                 {
        //                     tempSize++;
        //                 }
        //                 found = bucketList.TryGetNextValue(out index, ref iterator);
        //             }
        //
        //             if (tempSize > 0)
        //             {
        //                 densityList.Add(new MonsterDensity()
        //                 {
        //                     Position = Correction(newCenter),
        //                     Number = tempSize
        //                 });
        //             }
        //         }
        //     }
        //
        //     densityList.Sort();
        //
        //     for (var i = 0; i < densityList.Length && i < number; i++)
        //     {
        //         list.Add(densityList[i].Position);
        //     }
        //
        //     return new NativeArray<float2>(list.AsArray(), Allocator.Temp);
        // }
    }
}