using Unity.Entities;

namespace Game.Components.PlayerComponents
{
    public struct PlayerData : IComponentData
    {
        public float Velocity;

        public float ShootSeparate;
        public float ShootSeparateCounter;

        public Entity BulletEntity;
        public int BulletNumber;
    }
}
