using Unity.Entities;

namespace Game.Components.EnemyComponents
{
    public struct EnemyDamageBufferData : IBufferElementData
    {
        public float Damage;
        public float KnockBack;
    }
}