using System;
using Game.Components.EnemyComponents;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI
{
    public class EntitiesCounter : MonoBehaviour
    {
        [SerializeField] private Text counter;

        private void LateUpdate()
        {
            var count = World.DefaultGameObjectInjectionWorld.EntityManager
                .CreateEntityQuery(new ComponentType(typeof(EnemyData)))
                .CalculateEntityCount();

            counter.text = $"Entities: {count}";
        }
    }
}