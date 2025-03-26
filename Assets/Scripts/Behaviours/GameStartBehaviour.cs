using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using TestingSystem = Test.TestingSystem;
using WorldFlags = Unity.Entities.WorldFlags;

namespace Behaviours
{
    public class GameStartBehaviour : MonoBehaviour
    {
        public Button createClientButton;

        void Awake()
        {
            createClientButton.onClick.AddListener(CreateServer);
        }

        static void CreateServer()
        {
            var world = new World("Game Server", WorldFlags.GameServer);


            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world,
                DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default |
                                                         WorldSystemFilterFlags.ServerSimulation)
            );


            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);

        }
    }
}