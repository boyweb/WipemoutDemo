using Game.Components.PhysicsComponents;
using Game.Components.PlayerComponents;
using Game.Systems.PhysicsSystems;
using Game.Systems.SystemGroups;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Systems.PlayerSystems
{
    [UpdateBefore(typeof(PhysicsInitSystem))]
    [UpdateInGroup(typeof(WipemoutPhysicsSystemGroup))]
    public partial class PlayerInputSystem : SystemBase
    {
        private PlayerInputActions _playerInputActions;

        private bool _pressed;
        private Vector2 _targetPos;

        protected override void OnCreate()
        {
            base.OnCreate();

            RequireSingletonForUpdate<PlayerData>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.PlayerMove.Enable();
            _playerInputActions.PlayerMove.Target.performed += TargetOnperformed;
            _playerInputActions.PlayerMove.Touch.performed += TouchOnperformed;
            _playerInputActions.PlayerMove.Touch.canceled += TouchOncanceled;
        }

        private void TouchOncanceled(InputAction.CallbackContext obj)
        {
            _pressed = false;
        }

        private void TouchOnperformed(InputAction.CallbackContext obj)
        {
            _pressed = true;
        }

        private void TargetOnperformed(InputAction.CallbackContext obj)
        {
            _targetPos = obj.ReadValue<Vector2>();
        }

        protected override void OnUpdate()
        {
            var playerEntity = GetSingletonEntity<PlayerData>();
            var player = GetSingleton<PlayerData>();
            var playerTranslation = EntityManager.GetComponentData<Translation>(playerEntity);
            var playerPhysics = EntityManager.GetComponentData<PhysicsData>(playerEntity);

            if (!_pressed)
            {
                playerPhysics.Velocity = float2.zero;
                EntityManager.SetComponentData(playerEntity, playerPhysics);
            }
            else
            {
                var targetPos = Camera.main.ScreenToWorldPoint(_targetPos);
                var direction = math.normalize(((float3)targetPos).xy - playerTranslation.Value.xy);
                playerPhysics.Velocity = direction * player.Velocity;
                EntityManager.SetComponentData(playerEntity, playerPhysics);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _playerInputActions.PlayerMove.Target.performed -= TargetOnperformed;
            _playerInputActions.PlayerMove.Touch.performed -= TouchOnperformed;
            _playerInputActions.PlayerMove.Touch.canceled -= TouchOncanceled;
        }
    }
}