using Unity.Entities;
using Unity.Mathematics;

namespace Core.Components
{
    public struct Position : IComponentData
    {
        public int3 value;
    }

}