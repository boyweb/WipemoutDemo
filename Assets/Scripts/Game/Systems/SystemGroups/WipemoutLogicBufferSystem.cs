using Unity.Entities;

namespace Game.Systems.SystemGroups
{
    [UpdateInGroup(typeof(WipemoutLogicSystemGroup), OrderLast = true)]
    public class WipemoutLogicBufferSystem : EntityCommandBufferSystem
    {
    }
}