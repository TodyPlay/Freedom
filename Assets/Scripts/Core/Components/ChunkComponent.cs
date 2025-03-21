using Unity.Entities;
using Unity.Mathematics;

namespace Core.Components
{
    public struct Chunk : IComponentData
    {
        public const int SIZE_X = 16;
        public const int SIZE_Y = 256;
        public const int SIZE_Z = 16;
    }

    public struct BlocksInChunk : IComponentData
    {
        public BlobAssetReference<BlocksInChunkBlob> entitiesRef;
    }

    public struct BlocksInChunkBlob
    {
        public BlobArray<Entity> entities;
    }
}