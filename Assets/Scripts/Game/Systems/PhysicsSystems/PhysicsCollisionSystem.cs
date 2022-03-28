using Game.Components.PhysicsComponents;
using Game.Systems.SystemGroups;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Game.Systems.PhysicsSystems
{
    [UpdateAfter(typeof(PhysicsInitSystem))]
    [UpdateInGroup(typeof(WipemoutPhysicsSystemGroup))]
    public partial class PhysicsCollisionSystem : SystemBase
    {
        private const float PushCoefficient = 10;

        protected override void OnUpdate()
        {
            var physicsSystem = World.GetExistingSystem<PhysicsInitSystem>();
            var bucketList = physicsSystem.BucketList;
            var translationList = physicsSystem.TranslationList;
            var physicsList = physicsSystem.PhysicsList;

            Dependency = Entities
                .WithReadOnly(translationList)
                .WithReadOnly(physicsList)
                .WithReadOnly(bucketList)
                .WithBurst()
                .WithAll<PhysicsData>()
                .ForEach((
                    ref PhysicsData physicsData,
                    in Entity entity,
                    in Translation translation
                ) => {
                    physicsData.FinalVelocity = physicsData.Velocity;
                    if (physicsData.CollisionType == PhysicsCollisionType.Static) return;

                    var collisionCount = 1;
                    var pushVelocity = float2.zero;

                    var indexList = PhysicsSystemHelper.GetPhysicsEntityInRadius(translation.Value.xy, 1f, bucketList);
                    foreach (var index in indexList)
                    {
                        if (index == entity.Index) continue;

                        var otherTranslation = translationList[index];
                        var otherPhysicsData = physicsList[index];

                        var distance = math.distance(translation.Value, otherTranslation.Value);
                        var collisionDistance = physicsData.Radius + otherPhysicsData.Radius;
                        var toMeDirection = math.normalize(translation.Value.xy - otherTranslation.Value.xy);

                        if (distance <= collisionDistance && math.dot(toMeDirection, physicsData.Velocity) < 0)
                        {
                            collisionCount++;
                            physicsData.FinalVelocity += otherPhysicsData.Velocity;

                            var percent = (collisionDistance - distance) / collisionDistance;
                            pushVelocity += toMeDirection * (PushCoefficient * percent);
                        }
                    }

                    physicsData.FinalVelocity /= collisionCount;
                    physicsData.FinalVelocity += pushVelocity;
                })
                .ScheduleParallel(Dependency);

            CompleteDependency();
        }
    }
}