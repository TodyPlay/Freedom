using System.Runtime.CompilerServices;
using Core.Components;
using Unity.Mathematics;
using Unity.Transforms;

namespace Commons
{
    public static class Utility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexInChunk(this FrPosition position)
        {
            return position.value.x + position.value.z * Chunk.SIZE_X + position.value.y * Chunk.SIZE_X * Chunk.SIZE_Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 GetChunkPosition(this int indexInChunk)
        {
            var x = indexInChunk % Chunk.SIZE_X;
            var z = indexInChunk / Chunk.SIZE_X % Chunk.SIZE_Z;
            var y = indexInChunk / Chunk.SIZE_X / Chunk.SIZE_Z;

            return new int3(x, y, z);
        }


    }
}