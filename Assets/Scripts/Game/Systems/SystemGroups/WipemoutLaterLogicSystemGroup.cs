using Unity.Entities;

namespace Game.Systems.SystemGroups
{
    [UpdateAfter(typeof(WipemoutLogicSystemGroup))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class WipemoutLaterLogicSystemGroup : ComponentSystemGroup
    {
        
    }
}