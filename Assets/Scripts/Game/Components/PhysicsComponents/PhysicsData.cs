using Unity.Entities;
using Unity.Mathematics;

namespace Game.Components.PhysicsComponents
{
    public struct PhysicsData : IComponentData
    {
        public PhysicsCollisionType CollisionType;
        public PhysicsCollisionID CollisionID;

        public float Radius;
        public float Mass;

        public float2 Velocity;
        public float2 FinalVelocity;

        #region Knock Back

        public bool IsKnockBack;
        public float KnockBackDuration;
        public float2 KnockBackVelocity;
        public float LastKnockBack;

        #endregion
    }
}