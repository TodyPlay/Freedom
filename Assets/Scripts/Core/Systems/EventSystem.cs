using Core.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Core.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct EventSystemRemove : ISystem
    {
        [BurstCompile]
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