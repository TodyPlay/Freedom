using Unity.Entities;
using Unity.Mathematics;

namespace Core.Components
{
    public struct Chunk : IComponentData
    {
        public static readonly int3 SIZE = new(16, 256, 16);
    }
}