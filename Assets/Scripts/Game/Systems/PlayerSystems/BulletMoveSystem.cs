using Game.Components.EnemyComponents;
using Game.Components.GameComponents;
using Game.Components.PhysicsComponents;
using Game.Components.PlayerComponents;
using Game.Systems.PhysicsSystems;
using Game.Systems.SystemGroups;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Systems.PlayerSystems
{
    [UpdateInGroup(typeof(WipemoutLogicSystemGroup))]
    public partial class BulletMoveSystem : SystemBase
    {
        private const float MaxDistance = 200;

        private WipemoutLogicBufferSystem _wipemoutLogicBufferSystem;
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _wipemoutLogicBufferSystem = World.GetOrCreateSystem<WipemoutLogicBufferSystem>();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            RequireSingletonForUpdate<GameStatusData>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = GetSingleton<GameStatusData>().DeltaTime;

            var wipemoutLogicWriter = _wipemoutLogicBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var endWriter = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            var physicsInitSystem = World.GetExistingSystem<PhysicsInitSystem>();
            var bucketList = physicsInitSystem.BucketList;
            var entityList = physicsInitSystem.EntityList;
            var translationList = physicsInitSystem.TranslationList;
            var physicsList = physicsInitSystem.PhysicsList;

            Dependency = Entities
                .WithReadOnly(bucketList)
                .WithReadOnly(entityList)
                .WithReadOnly(translationList)
                .WithReadOnly(physicsList)
                .WithBurst()
                .WithAll<BulletData>()
                .ForEach((
                    int entityInQueryIndex,
                    Entity entity,
                    ref BulletData bulletData,
                    ref Translation translation
                ) => {
                    var distance = new float3(0, 1, 0) * bulletData.Velocity * deltaTime;
                    bulletData.MoveDistance += math.length(distance);
                    translation.Value += distance;

                    if (bulletData.MoveDistance >= MaxDistance)
                    {
                        endWriter.DestroyEntity(entityInQueryIndex, entity);
                        return;
                    }

                    var targetList = PhysicsSystemHelper.GetNearestSpecialIDInRadius(
                        translation.Value.xy,
                        bulletData.Size,
                        1,
                        bucketList,
                        translationList,
                        physicsList,
                        PhysicsCollisionID.Enemy
                    );
                    if (targetList.Length <= 0) return;

                    var index = targetList[0];
                    var enemyEntity = entityList[index];
                    var enemyTranslation = translationList[index];
                    var enemyPhysics = physicsList[index];

                    if (math.distance(translation.Value.xy, enemyTranslation.Value.xy) > bulletData.Size + enemyPhysics.Radius) return;

                    wipemoutLogicWriter.AppendToBuffer(
                        entityInQueryIndex,
                        enemyEntity,
                        new EnemyDamageBufferData
                        {
                            Damage = bulletData.Damage,
                            KnockBack = bulletData.KnockBack,
                        }
                    );

                    endWriter.DestroyEntity(entityInQueryIndex, entity);
                })
                .ScheduleParallel(Dependency);

            _wipemoutLogicBufferSystem.AddJobHandleForProducer(Dependency);
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}