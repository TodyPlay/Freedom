using Unity.Burst;
using Unity.Entities;

namespace Client.Systems
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class ChunkRenderSystem : SystemBase
    {
        protected override void OnUpdate()
        {

        }
    }
}