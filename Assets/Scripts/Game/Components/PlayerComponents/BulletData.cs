using Unity.Entities;

namespace Game.Components.PlayerComponents
{
    public struct BulletData : IComponentData
    {
        public float Velocity;
        
        public float Damage;
        public float KnockBack;
        public float Size;

        public float MoveDistance;
    }
}