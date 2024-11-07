using MainSceneUI;
using Mirror.Discovery;
using System;
using System.Net;

namespace Network
{
    public class NetworkDiscovery : NetworkDiscoveryBase<ClientRequest, ServerResponse>
    {
        public static NetworkDiscovery instance;

        private void Awake()
        {
            if(instance != null)
            {
                DestroyImmediate(this.gameObject);
            }
            instance = this;
        }

        public new void OnServerFound(ServerResponse response)
        {
            MainSceneUIManager.instance.searchPanel.UpdateRoomInfo(response);
        }

        public new void StartDiscovery()
        {
            base.OnServerFound.RemoveAllListeners();
            base.OnServerFound.AddListener(OnServerFound);
            base.StartDiscovery();
        }

        public new void StopDiscovery()
        {
            base.OnServerFound.RemoveAllListeners();
            base.StopDiscovery();
        }

        protected override ServerResponse ProcessRequest(ClientRequest request, IPEndPoint endpoint)
        {
            return new ServerResponse
            {
                serverId = ServerId,
                hostName = DataManager.userInfo.name,
                playerCapacity = 4,
                playerNum = NetworkRoomManager.instance.numPlayers,
                uri = transport.ServerUri()
            };
        }

        protected override void ProcessResponse(ServerResponse response, IPEndPoint endpoint)
        {
            // we received a message from the remote endpoint
            response.endPoint = endpoint;

            // although we got a supposedly valid url, we may not be able to resolve
            // the provided host
            // However we know the real ip address of the server because we just
            // received a packet from it,  so use that as host.
            UriBuilder realUri = new UriBuilder(response.uri)
            {
                Host = response.endPoint.Address.ToString()
            };
            response.uri = realUri.Uri;
            base.OnServerFound?.Invoke(response);
        }
    }
}