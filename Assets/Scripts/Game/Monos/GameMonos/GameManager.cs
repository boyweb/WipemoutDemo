using Game.Components.GameComponents;
using Unity.Entities;
using UnityEngine;

namespace Game.Monos.GameMonos
{
    public class GameManager : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new GameStatusData());
        }
    }
}