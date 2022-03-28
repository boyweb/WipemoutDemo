using Game.Components.EnemyComponents;
using Game.Components.GameComponents;
using Game.Components.PhysicsComponents;
using Game.Components.PlayerComponents;
using Game.Systems.SystemGroups;
using Helper;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Systems.EnemySystems
{
    [UpdateInGroup(typeof(WipemoutLaterLogicSystemGroup))]
    public partial class EnemyDamageSystem : SystemBase
    {
        private const float KnockBackMaxTime = 0.25f;
        private const float KnockBackMaxDistance = 1f;

        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            RequireSingletonForUpdate<GameStatusData>();
            RequireSingletonForUpdate<PlayerData>();

            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var playerEntity = GetSingletonEntity<PlayerData>();
            var playerTranslation = EntityManager.GetComponentData<Translation>(playerEntity);

            var endWriter = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var deltaTime = GetSingleton<GameStatusData>().DeltaTime;

            Dependency = Entities
                .WithBurst()
                .WithAll<EnemyData>()
                .ForEach((
                    int entityInQueryIndex,
                    Entity entity,
                    ref DynamicBuffer<EnemyDamageBufferData> damageList,
                    ref EnemyData enemyData,
                    ref PhysicsData physicsData,
                    in Translation translation
                ) => {
                    var knockBack = float.NegativeInfinity;

                    foreach (var damageData in damageList)
                    {
                        enemyData.HP -= damageData.Damage;
                        knockBack.GetMax(damageData.KnockBack);
                    }
                    damageList.Clear();

                    if (enemyData.HP <= 0)
                    {
                        endWriter.DestroyEntity(entityInQueryIndex, entity);
                    }
                    else
                    {
                        if (knockBack > 0 && knockBack > physicsData.LastKnockBack)
                        {
                            var knockBackTime = KnockBackMaxTime * knockBack;
                            knockBackTime.CheckMax(KnockBackMaxTime);
                            var knockBackDirection = math.normalize(translation.Value.xy - playerTranslation.Value.xy);
                            var knockBackDistance = KnockBackMaxDistance * knockBack;
                            knockBackDistance.CheckMax(KnockBackMaxDistance);

                            physicsData.IsKnockBack = true;
                            physicsData.KnockBackDuration = knockBackTime;
                            physicsData.KnockBackVelocity = knockBackDirection * knockBackDistance / knockBackTime;
                        }
                    }
                })
                .ScheduleParallel(Dependency);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}