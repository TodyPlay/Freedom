using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Client.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct GameClientNetworkSystem : ISystem
    {
        public NetworkDriver driver;

        public void OnCreate(ref SystemState state)
        {
            driver = NetworkDriver.Create();

            var connection = driver.Connect(NetworkEndpoint.Parse("127.0.0.1", 9000));

            var connectionEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(connectionEntity, new NetworkStreamConnection
            {
                Value = connection
            });
            state.EntityManager.AddComponent<NetworkStreamInGame>(connectionEntity);
        }

        public void OnDestroy(ref SystemState state)
        {
            driver.Dispose();
        }
    }
}