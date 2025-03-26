using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;

namespace Core.Components
{
    public struct Chunk : IComponentData
    {
        public const int SIZE_X = 16;
        public const int SIZE_Y = 256;
        public const int SIZE_Z = 16;

        public static int Length => SIZE_X * SIZE_Y * SIZE_Z;
    }


    public struct BlocksInChunk : IComponentData
    {
        public BlobAssetReference<BlocksInChunkBlob> entitiesRef;
    }

    public struct BlocksInChunkBlob
    {
        public BlobArray<BlockInChunkProvider> blockProviders;


        public static BlobAssetReference<BlocksInChunkBlob> CreateReference()
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var blocksInChunkBlob = ref builder.ConstructRoot<BlocksInChunkBlob>();
            builder.Allocate(ref blocksInChunkBlob.blockProviders, Chunk.SIZE_X * Chunk.SIZE_Z * Chunk.SIZE_Y);

            blocksInChunkBlob.Initialize();

            return builder.CreateBlobAssetReference<BlocksInChunkBlob>(Allocator.Persistent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Initialize()
        {
            for (var i = 0; i < blockProviders.Length; i++)
            {
                blockProviders[i] = new BlockInChunkProvider
                {
                    indexInChunk = i, entity = Entity.Null
                };
            }
        }
    }

    public struct BlockInChunkProvider
    {
        public int indexInChunk;

        public Entity entity;
    }
}
