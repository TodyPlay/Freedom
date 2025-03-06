using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Core.Components
{
    public struct Block : IComponentData
    {
    }

    //方块元数据
    [Serializable]
    public class BlockMetadata
    {
        public int id;

        public string name;

        //预制体ab地址
        public string prefabAddress;
    }

    public interface IBlockRepository
    {
        BlockMetadata FromChunkPosition(int3 position);
    }
}