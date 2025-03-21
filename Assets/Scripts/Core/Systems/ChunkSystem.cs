using Core.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Core.Systems
{
    [BurstCompile]
    public partial struct ChunkSystem : ISystem
    {
    }


    [BurstCompile]
    public partial struct ChunkLoadSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {

            using var positions = new NativeHashSet<int3>(16, Allocator.Temp);

            foreach (var ltw in SystemAPI.Query<RefRO<LocalToWorld>>().WithAll<ActivePointer>())
            {
                var position = ltw.ValueRO.Position;

                var chunk = new int3
                {
                    x = Mathf.FloorToInt(position.x / Chunk.SIZE_X),
                    y = 0,
                    z = Mathf.FloorToInt(position.z / Chunk.SIZE_Z)
                };

                //根据chunk的配置，加载3*3范围的所有chunk
                for (var x = -1; x <= 1; x++)
                {
                    for (var z = -1; z <= 1; z++)
                    {
                        var p = new int3
                        {
                            x = chunk.x + x, y = 0, z = chunk.z + z
                        };

                        positions.Add(p);
                    }
                }

            }

            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (p, e) in SystemAPI.Query<RefRO<FrPosition>>().WithAll<Chunk>().WithEntityAccess())
            {
                if (positions.Contains(p.ValueRO.value))
                {
                    positions.Remove(p.ValueRO.value);
                }
                else
                {
                    ecb.DestroyEntity(e);
                }

            }

            var archetype = state.EntityManager.CreateArchetype(FrArchetypes.Chunk);


            foreach (var position in positions)
            {
                var entity = ecb.CreateEntity(archetype);

                ecb.SetComponent(entity, new FrPosition
                {
                    value = position
                });

                ecb.SetComponent(entity, new LocalTransform
                {
                    Position = new float3
                    {
                        x = position.x * Chunk.SIZE_X, y = position.y, z = position.z * Chunk.SIZE_Z
                    },
                    Rotation = quaternion.identity,
                    Scale = 1
                });
                
                

                using var builder = new BlobBuilder(Allocator.Temp);
                ref var chunk = ref builder.ConstructRoot<BlocksInChunkBlob>();
                builder.Allocate(ref chunk.entities, Chunk.SIZE_X * Chunk.SIZE_Z * Chunk.SIZE_Y);

                var blobAssetReference = builder.CreateBlobAssetReference<BlocksInChunkBlob>(Allocator.Persistent);

                ecb.SetComponent(entity, new BlocksInChunk
                {
                    entitiesRef = blobAssetReference
                });


            }

            ecb.Playback(state.EntityManager);

        }
    }
}