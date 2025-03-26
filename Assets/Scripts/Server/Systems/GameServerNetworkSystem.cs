using Unity.Entities;

namespace Server.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class GameServerNetworkSystem : SystemBase
    {
        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {

        }
    }
}