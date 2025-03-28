using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Core.Components
{
    public struct FreshTag : IComponentData
    {
    }

    public struct RemoveTag : IComponentData
    {
    }

    public struct PendingTag : IComponentData
    {
    }

    public struct ChunkPosition : IComponentData
    {
        public int x, z;
    }

    public struct BlockPosition : IComponentData
    {
        public int x, y, z;
    }

    public struct FrPosition : IComponentData
    {
        public int x, y, z;

        public static implicit operator LocalTransform(FrPosition position)
        {
            return new LocalTransform
            {
                Position = new float3(position.x, position.y, position.z), Rotation = quaternion.identity, Scale = 1f
            };
        }

        public static implicit operator FrPosition(LocalTransform transform)
        {
            return new FrPosition
            {
                x = Mathf.FloorToInt(transform.Position.x),
                y = Mathf.FloorToInt(transform.Position.y),
                z = Mathf.FloorToInt(transform.Position.z)
            };
        }

        /// <summary>
        /// index in chunk
        /// </summary>
        /// <param name="position">position in chunk</param>
        /// <returns></returns>
        public static implicit operator int(FrPosition position)
        {
            return position.x + position.z * Chunk.SIZE_X + position.y * Chunk.SIZE_X * Chunk.SIZE_Z;
        }

        /// <summary>
        /// position in chunk
        /// </summary>
        /// <param name="indexInChunk">index in chunk</param>
        /// <returns></returns>
        public static implicit operator FrPosition(int indexInChunk)
        {
            var x = indexInChunk % Chunk.SIZE_X;
            var z = indexInChunk / Chunk.SIZE_X % Chunk.SIZE_Z;
            var y = indexInChunk / Chunk.SIZE_X / Chunk.SIZE_Z;

            return new FrPosition
            {
                x = x, y = y, z = z
            };
        }

        public static implicit operator int3(FrPosition position)
        {
            return new int3
            {
                x = position.x, y = position.y, z = position.z
            };
        }

        public static implicit operator FrPosition(int3 position)
        {
            return new FrPosition
            {
                x = position.x, y = position.y, z = position.z
            };
        }
    }

    public static class FrArchetypes
    {
        public static ReadOnlySpan<ComponentType> Block => new[]
        {
            ComponentType.ReadWrite<Block>(), ComponentType.ReadWrite<FrPosition>(),
            ComponentType.ReadWrite<ChunkReference>(), ComponentType.ReadWrite<LocalToWorld>(),
            ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadWrite<Parent>()
        };

        public static ReadOnlySpan<ComponentType> Chunk => new[]
        {
            ComponentType.ReadWrite<Chunk>(), ComponentType.ReadWrite<FrPosition>(),
            ComponentType.ReadWrite<LocalToWorld>(), ComponentType.ReadWrite<LocalTransform>(),
            ComponentType.ReadWrite<BlocksInChunk>(),
        };


        public static ReadOnlySpan<ComponentType> Player
        {
            [BurstDiscard]
            get
            {
                return new[]
                {
                    ComponentType.ReadWrite<Player>(), ComponentType.ReadWrite<ActivePointer>(),
                    ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadWrite<LocalToWorld>()
                };
            }
        }
    }

    public static class FrMeshes
    {
    }
}