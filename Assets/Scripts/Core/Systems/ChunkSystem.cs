using Commons;
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

            foreach (var (p, e) in SystemAPI.Query<RefRO<FrPosition>>()
                .WithAll<PendingTag, Chunk>()
                .WithEntityAccess())
            {
                if (positions.Contains(p.ValueRO.value))
                {
                    positions.Remove(p.ValueRO.value);
                }
                else
                {
                    ecb.AddComponent<RemoveTag>(e);
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


                ecb.SetComponent(entity, new BlocksInChunk
                {
                    entitiesRef = BlocksInChunkBlob.CreateReference()
                });

                ecb.AddComponent<StateFreshTag>(entity);


            }

            ecb.Playback(state.EntityManager);

        }
    }

    [BurstCompile]
    public partial struct ChunkInitializeSystem : ISystem
    {
        EntityArchetype m_blockArchetype;

        public void OnCreate(ref SystemState state)
        {
            m_blockArchetype = state.EntityManager.CreateArchetype(FrArchetypes.Block);
        }

        public void OnUpdate(ref SystemState state)
        {

            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (blocksInChunk, e) in SystemAPI.Query<RefRW<BlocksInChunk>>().WithAll<StateFreshTag>()
                .WithEntityAccess())
            {
                ecb.RemoveComponent<StateFreshTag>(e);
                ecb.AddComponent<PendingTag>(e);

                var length = blocksInChunk.ValueRW.entitiesRef.Value.entities.Length;

                for (var i = 0; i < length; i++)
                {
                    var entity = ecb.CreateEntity(m_blockArchetype);
                    ecb.AddComponent<StateFreshTag>(entity);
                    ecb.SetComponent(entity, new FrPosition()
                    {
                        value = i.GetChunkPosition()
                    });
                    ecb.SetComponent(entity, new ChunkReference
                    {
                        entity = e
                    });
                }

            }


            var componentLookupBlockInChunk = SystemAPI.GetComponentLookup<BlocksInChunk>();
            var componentLookupFrPosition = SystemAPI.GetComponentLookup<FrPosition>();


            ecb.Playback(state.EntityManager);

            foreach (var (block, frPosition, chunkReference, entity) in SystemAPI
                .Query<RefRO<Block>, RefRW<FrPosition>, RefRO<ChunkReference>>()
                .WithAll<StateFreshTag>().WithEntityAccess())
            {
                ecb.RemoveComponent<StateFreshTag>(entity);
                ecb.AddComponent<PendingTag>(entity);
                
                var blocksInChunk = componentLookupBlockInChunk.GetRefRW(chunkReference.ValueRO.entity);
                var chunkFrPosition = componentLookupFrPosition.GetRefRO(chunkReference.ValueRO.entity);
                
                //TODO 为 block 的 local transform赋值

                blocksInChunk.ValueRW.entitiesRef.Value.entities[frPosition.ValueRO.IndexInChunk()].entity = entity;
            }


            ecb.Playback(state.EntityManager);
        }
    }
}