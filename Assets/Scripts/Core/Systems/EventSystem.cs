using Core.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Core.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct EventSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            using var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entity) in SystemAPI.Query<RefRW<EventTag>>().WithEntityAccess())
            {
                entityCommandBuffer.DestroyEntity(entity);
            }

            entityCommandBuffer.Playback(state.EntityManager);
            
        }
    }
}