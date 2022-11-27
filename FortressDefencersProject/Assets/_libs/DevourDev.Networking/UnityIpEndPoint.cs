using System;
using System.Net;
using UnityEngine;

namespace DevourDev.Networking
{
    [Serializable]
    public struct UnityIpEndPoint
    {
        [InspectorName("IP")]
        public string IpString;
        public int Port;


        public IPEndPoint GetIPEndPoint()
        {
            return new IPEndPoint(IPAddress.Parse(IpString), Port);
        }
    }
}


