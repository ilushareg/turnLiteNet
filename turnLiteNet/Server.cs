using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;

namespace turnLiteNet
{
    public class Server: INetEventListener
    {
        static int MaxConnections = 2;

        public const byte E_ALLOWED = 9;
        public const byte E_NOTALLOWED_FULL = 10;

        private NetManager _netServer;
        //private NetPeer _ourPeer;
        List<NetPeer> _peersList = new List<NetPeer>();

        private NetDataWriter _dataWriter;

        public Server()
        {
            Start();
        }

        void Start()
        {
            _dataWriter = new NetDataWriter();
            _netServer = new NetManager(this, MaxConnections, "key");


            _netServer.Start(5000);
            _netServer.DiscoveryEnabled = true;
            _netServer.UpdateTime = 15;
        }

        public void Update()
        {
            _netServer.PollEvents();
        }

        void FixedUpdate()
        {
            foreach(NetPeer p in _peersList)
            {
                //_serverBall.transform.Translate(1f * Time.fixedDeltaTime, 0f, 0f);
                //_dataWriter.Reset();
                //_dataWriter.Put(_serverBall.transform.position.x);
                //_ourPeer.Send(_dataWriter, DeliveryMethod.Sequenced);
            }
        }

        void OnDestroy()
        {
            if (_netServer != null)
                _netServer.Stop();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[SERVER] We have new peer " + peer.EndPoint);

            _peersList.Add(peer);
        }

        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
        {
            Debug.Log("[SERVER] error " + socketErrorCode);
        }

        public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.DiscoveryRequest)
            {
                if(_peersList.Count >= MaxConnections)
                {
                    //TODO: do we need to act differently if max connections reached?
                    Debug.Log("[SERVER] Received discovery request. MaxConnections reached, do nothing");
                    _netServer.SendDiscoveryResponse(new byte[] { E_NOTALLOWED_FULL }, remoteEndPoint);
                }
                else
                {
                    Debug.Log("[SERVER] Received discovery request. Send discovery response");
                    _netServer.SendDiscoveryResponse(new byte[] { E_ALLOWED }, remoteEndPoint);
                }
            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
            _peersList.RemoveAll(x => x == peer);

        }

        public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
        {
        }

    }
}
