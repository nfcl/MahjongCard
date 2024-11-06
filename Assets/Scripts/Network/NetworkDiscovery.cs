using Mirror.Discovery;
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

        protected override ServerResponse ProcessRequest(ClientRequest request, IPEndPoint endpoint)
        {
            return new ServerResponse
            {

            };
        }

        protected override void ProcessResponse(ServerResponse response, IPEndPoint endpoint)
        {

        }
    }
}