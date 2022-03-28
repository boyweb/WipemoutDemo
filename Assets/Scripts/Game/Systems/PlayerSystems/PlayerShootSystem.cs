using Game.Components.GameComponents;
using Game.Components.PlayerComponents;
using Game.Systems.SystemGroups;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Systems.PlayerSystems
{
    [UpdateInGroup(typeof(WipemoutLogicSystemGroup))]
    public partial class PlayerShootSystem : SystemBase
    {
        private const float BulletSeparate = 0.5f;

        protected override void OnCreate()
        {
            base.OnCreate();

            RequireSingletonForUpdate<PlayerData>();
            RequireSingletonForUpdate<GameStatusData>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = GetSingleton<GameStatusData>().DeltaTime;

            var playerEntity = GetSingletonEntity<PlayerData>();
            var playerTranslation = EntityManager.GetComponentData<Translation>(playerEntity);

            var playerData = GetSingleton<PlayerData>();
            if (playerData.ShootSeparateCounter < playerData.ShootSeparate)
            {
                playerData.ShootSeparateCounter += deltaTime;
                SetSingleton(playerData);
            }
            else
            {
                playerData.ShootSeparateCounter -= playerData.ShootSeparate;
                SetSingleton(playerData);

                var bulletEntities = new NativeArray<Entity>(playerData.BulletNumber, Allocator.Temp);
                EntityManager.Instantiate(playerData.BulletEntity, bulletEntities);

                var startPos = playerTranslation.Value.x - (playerData.BulletNumber - 1) * BulletSeparate / 2f;
                for (var i = 0; i < playerData.BulletNumber; i++)
                {
                    var bulletEntity = bulletEntities[i];

                    var x = startPos + i * BulletSeparate;
                    EntityManager.SetComponentData(bulletEntity, new Translation()
                    {
                        Value = new float3(x, playerTranslation.Value.y, 0),
                    });
                }
            }
        }
    }
}