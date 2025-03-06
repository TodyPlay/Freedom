using Core.Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class PlayerJoin : MonoBehaviour
    {
        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnJoinPlayer);
        }

        void OnJoinPlayer()
        {

            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;

            var entity = entityManager.CreateEntity(ComponentType.ReadWrite<EventTag>(),
                ComponentType.ReadWrite<PlayerJoinEvent>());

            entityManager.SetComponentData(entity, new PlayerJoinEvent()
            {
                id = 1, name = "123"
            });

        }
    }
}