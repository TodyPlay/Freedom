using Unity.Collections;
using Unity.Entities;

namespace Core.Components
{
    public struct EventTag : IComponentData
    {
    }

    public struct PlayerJoinEvent : IComponentData
    {
        public int id;

        public FixedString64Bytes name;
    }
}