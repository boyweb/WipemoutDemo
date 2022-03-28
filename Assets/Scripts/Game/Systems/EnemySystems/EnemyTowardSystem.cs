using Game.Components.EnemyComponents;
using Game.Components.PhysicsComponents;
using Game.Components.PlayerComponents;
using Game.Systems.SystemGroups;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Systems.EnemySystems
{
    [UpdateInGroup(typeof(WipemoutLogicSystemGroup))]
    public partial class EnemyTowardSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            RequireSingletonForUpdate<PlayerData>();
        }

        protected override void OnUpdate()
        {
            var playerEntity = GetSingletonEntity<PlayerData>();
            var playerTranslation = EntityManager.GetComponentData<Translation>(playerEntity);

            Entities
                .WithBurst()
                .WithAll<EnemyData>()
                .ForEach((
                    ref PhysicsData physicsData,
                    in Translation translation,
                    in EnemyData enemyData
                ) => {
                    var direction = math.normalize(playerTranslation.Value.xy - translation.Value.xy);
                    physicsData.Velocity = direction * enemyData.Velocity;
                })
                .ScheduleParallel();
        }
    }
}