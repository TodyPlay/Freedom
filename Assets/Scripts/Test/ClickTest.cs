using System;
using Commons;
using Core.Components;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class ClickTest : MonoBehaviour
    {
        public Button button;

        void Awake()
        {
            button.onClick.AddListener(() =>
            {
                var world = World.DefaultGameObjectInjectionWorld;

                var em = world.EntityManager;

                var entity = em.CreateEntity();

                em.AddComponentData(entity, new ChunkPosition());
                em.AddComponentData(entity, new GhostOwner());


            });
        }
    }
}