using Mirror.Discovery;
using System.Net;

namespace Network
{
    public class NetworkDiscovery : NetworkDiscoveryBase<ClientRequest, ServerResponse>
    {
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