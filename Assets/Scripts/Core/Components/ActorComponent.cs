using Unity.Entities;

namespace Core.Components
{
    //HP
    public struct HealthPoint : IComponentData
    {
        public float value;
    }

    //MP
    public struct MagicPoint : IComponentData
    {
        public float value;
    }

    //活跃点，这个组件所在实体附近的实体会被加入为活跃对象，系统支队活跃对象进行更新
    public struct ActivePointer : IComponentData
    {
    }

    //激活的方块，即需要进行跟新的实体
    public struct Active : IComponentData
    {
    }
}