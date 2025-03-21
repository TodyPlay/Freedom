using Core.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    /// <summary>
    /// 孵化器
    /// </summary>
    public class GameSettingsAuthoring : MonoBehaviour
    {
        [Header("种子")] public int seed;

        public GameObject nt;

        public GameObject playerPrefab;
        

        public string savingPath;

        class WordAuthoringBaker : Baker<GameSettingsAuthoring>
        {
            public override void Bake(GameSettingsAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                var entityNt = GetEntity(authoring.nt, TransformUsageFlags.Dynamic);
                var playerPrefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic);

                AddComponent(entity,
                    new GameSettings
                    {
                        seed = authoring.seed,
                        nt = entityNt,
                        savingPath = authoring.savingPath,
                        playerPrefab = playerPrefab,
                    });
            }
        }
    }

    public struct GameSettings : IComponentData
    {
        public int seed;

        //泥土
        public Entity nt;

        public Entity playerPrefab;

        //存档位置
        public FixedString128Bytes savingPath;
    }
}