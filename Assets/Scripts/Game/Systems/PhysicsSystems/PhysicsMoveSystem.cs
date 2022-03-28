using Game.Components.GameComponents;
using Game.Components.PhysicsComponents;
using Game.Systems.SystemGroups;
using Helper;
using Unity.Entities;
using Unity.Transforms;

namespace Game.Systems.PhysicsSystems
{
    [UpdateAfter(typeof(PhysicsCollisionSystem))]
    [UpdateInGroup(typeof(WipemoutPhysicsSystemGroup))]
    public partial class PhysicsMoveSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            RequireSingletonForUpdate<GameStatusData>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = GetSingleton<GameStatusData>().DeltaTime;

            Entities
                .WithBurst()
                .WithAll<PhysicsData>()
                .ForEach((
                    ref Translation translation,
                    ref PhysicsData physicsData
                ) => {
                    if (physicsData.IsKnockBack)
                    {
                        translation.Value += physicsData.KnockBackVelocity.ToFloat3() * deltaTime;
                        physicsData.KnockBackDuration -= deltaTime;
                        if (physicsData.KnockBackDuration <= 0)
                        {
                            physicsData.IsKnockBack = false;
                            physicsData.LastKnockBack = 0;
                        }
                    }
                    else
                    {
                        translation.Value += physicsData.FinalVelocity.ToFloat3() * deltaTime;
                    }
                })
                .ScheduleParallel();
        }
    }
}