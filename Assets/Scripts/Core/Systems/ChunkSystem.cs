using Core.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Core.Systems
{
    public partial struct ChunkSystem : ISystem
    {
        public static EntityArchetype archetype;

        public void OnCreate(ref SystemState state)
        {
            archetype = state.EntityManager.CreateArchetype(ComponentType.ReadWrite<Chunk>(),
                ComponentType.ReadWrite<Position>());
        }
    }


    public partial struct ChunkLoadSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {

            using var positions = new NativeList<int3>();

            foreach (var ltw in SystemAPI.Query<RefRO<LocalToWorld>>().WithAll<ActivePointer>())
            {
                var p = ltw.ValueRO.Position;

                var cp = new int3(Mathf.FloorToInt(p.x / 16), 0, Mathf.FloorToInt(p.z / 16));
                //计算8米内的chunk
                
                Debug.Log(cp);
            }

            //TODO 加载坐标点内的chunk，卸载不在其内的chunk
        }
    }
}