using Unity.Entities;
using Unity.Networking.Transport;

namespace Server.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class GameServerNetworkSystem : SystemBase
    {
        public NetworkDriver driver;

        protected override void OnCreate()
        {
            driver = NetworkDriver.Create();
            driver.Bind(NetworkEndpoint.AnyIpv4.WithPort(9000));
            driver.Listen();

        }

        protected override void OnUpdate()
        {
            driver.ScheduleUpdate().Complete();
        }

        protected override void OnDestroy()
        {
            driver.Dispose();
        }
    }
}