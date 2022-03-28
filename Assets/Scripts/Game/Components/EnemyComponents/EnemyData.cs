using Unity.Entities;

namespace Game.Components.EnemyComponents
{
    public struct EnemyData : IComponentData
    {
        public float HP;

        public float Velocity;
    }
}