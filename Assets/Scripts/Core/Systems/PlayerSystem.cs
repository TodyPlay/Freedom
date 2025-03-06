using Authoring;
using Core.Components;
using Core.Saved;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

namespace Core.Systems
{
    public partial struct PlayerSystem : ISystem
    {
        public static EntityArchetype archetype;

        public void OnCreate(ref SystemState state)
        {
            archetype = state.EntityManager.CreateArchetype(
                ComponentType.ReadWrite<Player>(),
                ComponentType.ReadWrite<ActivePointer>(),
                ComponentType.ReadWrite<LocalTransform>(),
                ComponentType.ReadWrite<LocalToWorld>()
            );
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (p, t) in SystemAPI.Query<RefRO<Player>, RefRW<LocalTransform>>())
            {
                t.ValueRW.Position = new float3(Random.value * 3, Random.value * 3, Random.value * 3);
            }
        }

        public void OnDestroy(ref SystemState state)
        {
            foreach (var (p, t) in SystemAPI.Query<RefRO<Player>, RefRO<LocalTransform>>())
            {
                PlayerSaved.Saving(new PlayerSaved.Content
                {
                    id = p.ValueRO.id, name = p.ValueRO.name.Value, position = t.ValueRO.Position
                });
            }
        }
    }

    public partial class PlayerJoinSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<GameSettings>();
        }

        protected override void OnUpdate()
        {

            var gameSettings = SystemAPI.GetSingleton<GameSettings>();

            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            using var playerIds = new NativeList<int>(Allocator.Temp);

            foreach (var player in SystemAPI.Query<RefRO<Player>>())
            {
                playerIds.Add(player.ValueRO.id);
            }

            foreach (var join in SystemAPI.Query<RefRO<PlayerJoinEvent>>())
            {

                var localTransform = LocalTransform.Identity;

                if (playerIds.Contains(join.ValueRO.id))
                {
                    continue;
                }

                if (PlayerSaved.Loading(join.ValueRO.id, out var content))
                {

                    localTransform.Position = content.position;
                }
                var entity = ecb.Instantiate(gameSettings.playerPrefab);

                ecb.AddComponent(entity, new Player
                {
                    id = join.ValueRO.id, name = join.ValueRO.name
                });

                ecb.SetComponent(entity, localTransform);

                ecb.AddComponent<ActivePointer>(entity);

            }

            ecb.Playback(EntityManager);

        }
    }
}