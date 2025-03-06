using Unity.Collections;
using Unity.Entities;

namespace Core.Components
{
    public struct Player : IComponentData
    {
        public int id;

        public FixedString64Bytes name;
    }
}