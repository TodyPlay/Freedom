using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Core.Components
{
    public struct FrPosition : IComponentData
    {
        public int3 value;
    }

    public static class FrArchetypes
    {
        public static ReadOnlySpan<ComponentType> Chunk => new[]
        {
            ComponentType.ReadWrite<Chunk>(),
            ComponentType.ReadWrite<FrPosition>(),
            ComponentType.ReadWrite<LocalToWorld>(),
            ComponentType.ReadWrite<LocalTransform>(),
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