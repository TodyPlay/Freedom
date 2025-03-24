using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Core.Components
{
    //标识Chunk待初始化，初始化完成后移除这个组件
    public struct StateFreshTag : IComponentData
    {
    }

    public struct RemoveTag : IComponentData
    {
    }

    public struct PendingTag : IComponentData
    {
    }

    public struct FrPosition : IComponentData
    {
        public int x, y, z;

        public static implicit operator LocalTransform(FrPosition position)
        {
            return new LocalTransform
            {
                Position = new float3(position.x, position.y, position.z),
                Rotation = quaternion.identity,
                Scale = 1f
            };
        }

        public static implicit operator FrPosition(LocalTransform transform)
        {
            return new FrPosition()
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

            return new FrPosition { x = x, y = y, z = z };
        }

        public static implicit operator int3(FrPosition position)
        {
            return new int3 { x = position.x, y = position.y, z = position.z };
        }

        public static implicit operator FrPosition(int3 position)
        {
            return new FrPosition { x = position.x, y = position.y, z = position.z };
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
        static Mesh _planeMesh;

        public static Mesh PlantMesh
        {
            get
            {

                if (_planeMesh is not null)
                {
                    return _planeMesh;
                }
                var mesh = new Mesh();

                // 定义顶点
                var vertices = new Vector3[4];
                vertices[0] = new Vector3(0, 0, 0); // 左下角
                vertices[1] = new Vector3(0, 0, 1); // 左上角
                vertices[2] = new Vector3(1, 0, 1); // 右下角
                vertices[3] = new Vector3(1, 0, 0); // 右上角

                // 定义三角形
                var triangles = new int[6];
                triangles[0] = 0; // 第一个三角形
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2; // 第二个三角形
                triangles[4] = 3;
                triangles[5] = 0;

                // 定义 UV 坐标
                var uv = new Vector2[4];
                uv[0] = new Vector2(0, 0); // 左下角
                uv[1] = new Vector2(0, 1); // 左上角
                uv[2] = new Vector2(1, 0); // 右下角
                uv[3] = new Vector2(1, 1); // 右上角

                // 赋值给 Mesh
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.uv = uv;

                // 重新计算法线和边界
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                _planeMesh = mesh;


                return _planeMesh;
            }
        }
    }
}
