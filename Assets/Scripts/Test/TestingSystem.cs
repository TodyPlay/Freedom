using Core.Components;
using Unity.Entities;

namespace Test
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct TestingSystem : ISystem
    {
        ComponentLookup<ChunkPosition> m_componentLookup;


        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.CreateEntity(typeof(Chunk), typeof(ChunkPosition));
            m_componentLookup = state.GetComponentLookup<ChunkPosition>();
        }


        public void OnUpdate(ref SystemState state)
        {
            m_componentLookup.Update(ref state);
            foreach (var (c, e) in SystemAPI.Query<RefRW<Chunk>>().WithEntityAccess())
            {
                m_componentLookup.GetRefRW(e).ValueRW.x++;
                m_componentLookup.GetRefRW(e).ValueRW.z = (int)SystemAPI.Time.ElapsedTime;
            }
        }
    }
}