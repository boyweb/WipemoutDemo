using System.Collections.Generic;
using Game.Components.EnemyComponents;
using Unity.Entities;
using UnityEngine;

namespace Game.Monos.EnemyMonos
{
    public class EnemySpawner : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        [SerializeField] private GameObject enemyPrefab;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new EnemySpawnerData
            {
                EnemyEntity = conversionSystem.GetPrimaryEntity(enemyPrefab)
            });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(enemyPrefab);
        }
    }
}