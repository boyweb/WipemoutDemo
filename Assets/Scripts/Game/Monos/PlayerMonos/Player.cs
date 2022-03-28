using System.Collections.Generic;
using Game.Components.PhysicsComponents;
using Game.Components.PlayerComponents;
using Unity.Entities;
using UnityEngine;

namespace Game.Monos.PlayerMonos
{
    public class Player : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        [Header("Player Data")]
        [SerializeField] private float shootSeparate;
        [SerializeField] private float velocity;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject bulletEntity;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new PlayerData
            {
                BulletEntity = conversionSystem.GetPrimaryEntity(bulletEntity),
                
                Velocity = velocity,
                ShootSeparate = shootSeparate,
                
                BulletNumber = 5
            });
            dstManager.AddComponentData(entity, new PhysicsData()
            {
                CollisionType = PhysicsCollisionType.Static,
                CollisionID = PhysicsCollisionID.Player,

                Mass = 1,
                Radius = 2.5f,
            });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(bulletEntity);
        }
    }
}