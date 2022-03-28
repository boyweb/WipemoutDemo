using Unity.Entities;
using Unity.Transforms;

namespace Game.Systems.SystemGroups
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class WipemoutPhysicsSystemGroup : ComponentSystemGroup
    {
    }
}