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

            using var positions = new NativeHashSet<int3>(9, Allocator.Temp);

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
                if (positions.Contains(p.ValueRO))
                {
                    positions.Remove(p.ValueRO);
                }
                else
                {
                    ecb.RemoveComponent<PendingTag>(e);
                    ecb.AddComponent<RemoveTag>(e);
                }

            }

            var archetype = state.EntityManager.CreateArchetype(FrArchetypes.Chunk);


            foreach (var position in positions)
            {
                var entity = ecb.CreateEntity(archetype);

                ecb.SetComponent<FrPosition>(entity, position);
                ecb.SetComponent<LocalTransform>(entity,(FrPosition)position);

                ecb.SetComponent(entity,
                    new BlocksInChunk
                    {
                        entitiesRef = BlocksInChunkBlob.CreateReference()
                    });

                ecb.AddComponent<StateFreshTag>(entity);


            }

            ecb.Playback(state.EntityManager);

        }
    }

    [BurstCompile]
    public partial struct ChunkFreshSystem : ISystem
    {

        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {

            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (c, e) in SystemAPI.Query<RefRW<Chunk>>().WithAll<StateFreshTag>()
                .WithEntityAccess())
            {
                ecb.RemoveComponent<StateFreshTag>(e);
                ecb.AddComponent<PendingTag>(e);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct ChunkRemoveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (bic, entity) in SystemAPI.Query<RefRW<BlocksInChunk>>().WithAll<Chunk, RemoveTag>()
                .WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                bic.ValueRW.entitiesRef.Dispose();
            }

            ecb.Playback(state.EntityManager);
        }

        public void OnDestroy(ref SystemState state)
        {
            foreach (var bic in SystemAPI.Query<RefRW<BlocksInChunk>>().WithAll<Chunk>())
            {
                bic.ValueRW.entitiesRef.Dispose();
            }
        }
    }
}