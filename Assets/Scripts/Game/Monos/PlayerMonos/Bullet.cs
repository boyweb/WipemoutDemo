using Game.Components.PlayerComponents;
using Unity.Entities;
using UnityEngine;

namespace Game.Monos.PlayerMonos
{
    public class Bullet : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new BulletData()
            {
                Damage = 1,
                KnockBack = 0.5f,
                Velocity = 50,
                Size = 0.1f
            });
        }
    }
}