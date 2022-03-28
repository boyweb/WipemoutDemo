using Unity.Entities;

namespace Game.Components.EnemyComponents
{
    public struct EnemySpawnerData : IComponentData
    {
        public Entity EnemyEntity;
    }
}