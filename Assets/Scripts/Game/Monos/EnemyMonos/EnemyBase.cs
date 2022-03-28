using Game.Components.EnemyComponents;
using Game.Components.PhysicsComponents;
using Unity.Entities;
using UnityEngine;

namespace Game.Monos.EnemyMonos
{
    public class EnemyBase : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new EnemyData()
            {
                HP = 2,
                Velocity = 5,
            });
            dstManager.AddComponentData(entity, new PhysicsData()
            {
                CollisionID = PhysicsCollisionID.Enemy,
                CollisionType = PhysicsCollisionType.Dynamic,

                Radius = 0.5f,
                Mass = 1,
            });
            dstManager.AddBuffer<EnemyDamageBufferData>(entity);
        }
    }
}