using System;
using kcp2k;
using Unity.Entities;

namespace Server.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class GameServerNetworkSystem : SystemBase
    {
        KcpServer m_kcpServer;

        protected override void OnCreate()
        {

            var server = new KcpServer(
                OnConnected: OnConnected,
                OnData: OnData,
                OnDisconnected: OnDisconnected,
                OnError: OnError,
                new KcpConfig()
            );


            server.Start(6679);
            m_kcpServer = server;

        }

        protected override void OnUpdate()
        {
            m_kcpServer?.Tick();
        }

        protected override void OnDestroy()
        {
            m_kcpServer?.Stop();
        }

        #region callback

        void OnConnected(int id)
        {

        }

        void OnData(int id, ArraySegment<byte> data, KcpChannel channel)
        {

        }

        void OnDisconnected(int id)
        {

        }

        void OnError(int id, ErrorCode code, string msg)
        {

        }

        #endregion
    }
}