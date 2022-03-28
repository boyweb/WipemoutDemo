using Game.Components.GameComponents;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Systems.GameSystems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class GameManagerSystem : SystemBase
    {
        private const float MaxDeltaTime = 1 / 30f;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            RequireSingletonForUpdate<GameStatusData>();
        }

        protected override void OnUpdate()
        {
            var gameStatus = GetSingleton<GameStatusData>();
            gameStatus.DeltaTime = math.min(Time.DeltaTime, MaxDeltaTime);
            SetSingleton(gameStatus);
        }
    }
}