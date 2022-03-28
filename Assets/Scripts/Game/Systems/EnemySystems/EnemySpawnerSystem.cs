using Game.Components.EnemyComponents;
using Game.Components.GameComponents;
using Game.Components.PlayerComponents;
using Game.Systems.SystemGroups;
using Helper;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Systems.EnemySystems
{
    [UpdateInGroup(typeof(WipemoutLogicSystemGroup))]
    public partial class EnemySpawnerSystem : SystemBase
    {
        private const float Distance = 100f;

        private const float SpawnSeparate = 0.25f;

        protected override void OnCreate()
        {
            base.OnCreate();

            RequireSingletonForUpdate<PlayerData>();
            RequireSingletonForUpdate<EnemySpawnerData>();
            RequireSingletonForUpdate<GameStatusData>();
        }

        private float _ticker = 0;
        private float _gameTime = 0;

        protected override void OnUpdate()
        {
            var deltaTime = GetSingleton<GameStatusData>().DeltaTime;
            _gameTime += deltaTime;

            if (_ticker > 0)
            {
                _ticker -= deltaTime;
                return;
            }

            var playerEntity = GetSingletonEntity<PlayerData>();
            var playerTranslation = EntityManager.GetComponentData<Translation>(playerEntity);

            _ticker = SpawnSeparate;
            var number = Mathf.CeilToInt(_gameTime / 2);
            number.CheckMax(50);

            var enemySpawnerData = GetSingleton<EnemySpawnerData>();

            var enemyEntities = new NativeArray<Entity>(number, Allocator.Temp);
            EntityManager.Instantiate(enemySpawnerData.EnemyEntity, enemyEntities);

            for (var i = 0; i < number; i++)
            {
                var enemyEntity = enemyEntities[i];

                var angle = Random.Range(-15f, 15f);
                var target = new Vector2(0, 1).Rotate(angle) * Distance;

                EntityManager.SetComponentData(enemyEntity, new Translation
                {
                    Value = playerTranslation.Value + new float3(target.x, target.y, 0)
                });
            }
        }
    }
}