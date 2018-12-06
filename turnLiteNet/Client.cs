using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;


namespace turnLiteNet
{
    public class Client : INetEventListener
    {
        public Client()
        {
            Start();
        }
        private NetManager _netClient;

        private float _newBallPosX;

        void Start()
        {
            _netClient = new NetManager(this, "key");
            _netClient.Start();
            _netClient.UpdateTime = 15;
        }

        public void Update()
        {
            _netClient.PollEvents();

            var peer = _netClient.GetFirstPeer();
            if (peer != null && peer.ConnectionState == ConnectionState.Connected)
            {
                //Fixed delta set to 0.05
                //var pos = _clientBallInterpolated.transform.position;
                //pos.x = Mathf.Lerp(_oldBallPosX, _newBallPosX, _lerpTime);
                //_clientBallInterpolated.transform.position = pos;

                ////Basic lerp
                //_lerpTime += Time.deltaTime / Time.fixedDeltaTime;
            }
            else
            {
                _netClient.SendDiscoveryRequest(new byte[] { 1 }, 5000);
            }
        }

        void OnDestroy()
        {
            if (_netClient != null)
                _netClient.Stop();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
        }

        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
        {
            Debug.Log("[CLIENT] We received error " + socketErrorCode);
        }

        public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
        {
            _newBallPosX = reader.GetFloat();

            //var pos = _clientBall.transform.position;

            //_oldBallPosX = pos.x;
            //pos.x = _newBallPosX;

            //_clientBall.transform.position = pos;

        }

        public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.DiscoveryResponse && _netClient.PeersCount == 0)
            {
                Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
                _netClient.Connect(remoteEndPoint);

            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);

            Debug.Log("[CLIENT] We disconnected because socketErrorCode " + disconnectInfo.SocketErrorCode);
        }
    }
}
