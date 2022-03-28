using Game.Components.PhysicsComponents;
using Game.Systems.SystemGroups;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Systems.PhysicsSystems
{
    [UpdateInGroup(typeof(WipemoutPhysicsSystemGroup))]
    public partial class PhysicsInitSystem : SystemBase
    {
        public NativeMultiHashMap<int, int> BucketList;
        public NativeHashMap<int, Entity> EntityList;
        public NativeHashMap<int, Translation> TranslationList;
        public NativeHashMap<int, PhysicsData> PhysicsList;

        protected override void OnUpdate()
        {
            var query = GetEntityQuery(new ComponentType(typeof(PhysicsData)), new ComponentType(typeof(Translation)));
            var count = query.CalculateEntityCount();

            if (!BucketList.IsCreated) BucketList = new NativeMultiHashMap<int, int>(count, Allocator.Persistent);
            else BucketList.Clear();
            BucketList.Capacity = math.max(count, BucketList.Capacity);
            var bucketWriter = BucketList.AsParallelWriter();

            if (!EntityList.IsCreated) EntityList = new NativeHashMap<int, Entity>(count, Allocator.Persistent);
            else EntityList.Clear();
            EntityList.Capacity = math.max(count, EntityList.Capacity);
            var entityWriter = EntityList.AsParallelWriter();

            if (!TranslationList.IsCreated) TranslationList = new NativeHashMap<int, Translation>(count, Allocator.Persistent);
            else TranslationList.Clear();
            TranslationList.Capacity = math.max(count, TranslationList.Capacity);
            var translationWriter = TranslationList.AsParallelWriter();

            if (!PhysicsList.IsCreated) PhysicsList = new NativeHashMap<int, PhysicsData>(count, Allocator.Persistent);
            else PhysicsList.Clear();
            PhysicsList.Capacity = math.max(count, PhysicsList.Capacity);
            var physicsWriter = PhysicsList.AsParallelWriter();

            Entities
                .WithBurst()
                .WithAll<PhysicsData>()
                .ForEach((
                    Entity entity,
                    in Translation translation,
                    in PhysicsData physicsData
                ) => {
                    bucketWriter.Add(PhysicsSystemHelper.Hash(translation.Value.xy), entity.Index);

                    entityWriter.TryAdd(entity.Index, entity);
                    translationWriter.TryAdd(entity.Index, translation);
                    physicsWriter.TryAdd(entity.Index, physicsData);
                })
                .ScheduleParallel();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            BucketList.Dispose();
            EntityList.Dispose();
            TranslationList.Dispose();
            PhysicsList.Dispose();
        }
    }
}