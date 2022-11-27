namespace DevourDev.Networking
{
    using DevourDev.Networking;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;




    public static class DevNet
    {
        public static Socket GetTcpSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public static Socket GetTcpSocket(int port, bool listen = false, int backLog = 5)
        {
            var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Bind(GetIPEP(port));
            if (listen)
                s.Listen(backLog);
            return s;
        }

        public static IPEndPoint GetIPEP(int port)
        {
            return new IPEndPoint(IPAddress.Any, port);
        }
        public static IPEndPoint GetIPEP()
        {
            return new IPEndPoint(IPAddress.Any, 0);
        }

        public static void CloseSocket(Socket s)
        {
            if (s == null)
                return;

            try
            {
                if (s != null && !s.Connected)
                    s.Disconnect(true);
                s.Shutdown(SocketShutdown.Both);
            }
            catch (System.Exception)
            {

            }
            finally
            {
                s.Close(0);
            }
        }
        public static Socket ConnectTo(EndPoint ep)
        {
            var s = GetTcpSocket();
            s.Connect(ep);
            return s;
        }
        public static async Task<Socket> ConnectToAsync(EndPoint ep)
        {
            var s = GetTcpSocket();
            await s.ConnectAsync(ep);
            return s;
        }

        public static Task ExecuteDelayed(int msDelay, System.Action act)
        {
            return Task.Run(() =>
            {
                Task.Delay(msDelay).Wait();
                act?.Invoke();
            });
        }
        public static Task ExecuteDelayed(int msDelay, System.Action act, System.Threading.CancellationToken token)
        {
            return Task.Run(() =>
            {
                Task.Delay(msDelay).Wait();
                try
                {
                    token.ThrowIfCancellationRequested();
                    act?.Invoke();
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (System.OperationCanceledException)
                {
                    return;
                }
            }, token);
        }


    }
}
