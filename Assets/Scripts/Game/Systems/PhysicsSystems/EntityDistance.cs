using System;

namespace Game.Systems.PhysicsSystems
{
    public struct EntityDistance : IComparable<EntityDistance>
    {
        public int Index;
        public float Distance;

        public int CompareTo(EntityDistance other)
        {
            return Distance.CompareTo(other.Distance);
        }
    }
}