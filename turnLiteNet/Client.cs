using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;


namespace turnLiteNet
{
    public class Client : INetEventListener
    {
        static int idCounter = 0;
        int id = 0;

        public Client()
        {
            id = idCounter++;
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
            if (_netClient == null)
            {
                return;
            }

            _netClient.PollEvents();
            if (_netClient == null)
            {
                return;
            }


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
            Debug.Log(String.Format("[CLIENT {0}] We connected to " + peer.EndPoint, id));
        }

        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
        {
            Debug.Log(String.Format("[CLIENT {0}] We received error " + socketErrorCode, id));
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
            if (_netClient == null)
            {
                //We're disabled //TODO: find a better approach for that... later
                return;
            }

            Debug.Log(String.Format("[CLIENT {0}] OnNetworkReceiveUnconnected", id));
            if (messageType == UnconnectedMessageType.DiscoveryResponse && _netClient.PeersCount == 0)
            {
                if (reader.AvailableBytes > 0)
                {
                    byte data = reader.GetByte();
                    if (data == Server.E_ALLOWED)
                    {
                        Debug.Log(String.Format("[CLIENT {0}] Received discovery response. Connecting to: " + remoteEndPoint, id));
                        _netClient.Connect(remoteEndPoint);

                    }
                    else if (data == Server.E_NOTALLOWED_FULL)
                    {
                        //stop doing anything by deleting _netClient
                        Debug.Log(String.Format("[CLIENT {0}] Received discovery response. Not Allowed to connect FULL" + remoteEndPoint, id));
                        _netClient = null;


                    }
                }
            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log(String.Format("[CLIENT {0}] We disconnected because " + disconnectInfo.Reason, id));

            Debug.Log(String.Format("[CLIENT {0}] We disconnected because socketErrorCode " + disconnectInfo.SocketErrorCode, id));
        }
    }
}
