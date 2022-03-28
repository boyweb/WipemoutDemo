using Unity.Entities;
using Unity.Transforms;

namespace Game.Systems.SystemGroups
{
    [UpdateAfter(typeof(WipemoutPhysicsSystemGroup))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class WipemoutLogicSystemGroup : ComponentSystemGroup
    {
    }
}