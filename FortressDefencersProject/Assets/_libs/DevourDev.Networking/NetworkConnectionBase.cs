namespace DevourDev.Networking
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using DevourEncoding;
    using DevourDev.Networking.Packets;
    using System.Collections.Generic;

    public abstract class NetworkConnectionBase<NetConnection, Resolver, PacketContent, Encoder, Decoder, Packet> : IDisposable
        where NetConnection : NetworkConnectionBase<NetConnection, Resolver, PacketContent, Encoder, Decoder, Packet>
        where Resolver : Packets.PacketsResolver<PacketContent, Encoder, Decoder>
        where Encoder : DevourEncoderBase, new()
        where Decoder : DevourDecoderBase, new()
        where PacketContent : IPacketContentBase<Encoder, Decoder>
        where Packet : PacketBase<Packet, PacketContent, Encoder, Decoder, Resolver>, new()
    {

        private const int HEADER_SIZE = 4;

        private readonly Socket _socket;
        private readonly byte[] _buffer;

        private Action<Memory<byte>> _onDataReceivedAction;
        private Action<object, Exception> _onErrorAction;
        private CancellationTokenSource _receivingLoopCTS;

        private readonly object _sendingLocker = new();
        private readonly object _receivingLocker = new();

        private readonly object _requestingLocker = new();


        public NetworkConnectionBase(int bufferSize)
        {
            _socket = DevNet.GetTcpSocket();
            _buffer = new byte[bufferSize];
        }
        public NetworkConnectionBase(Socket connectedSocket, int bufferSize)
        {
            _socket = connectedSocket;
            _buffer = new byte[bufferSize];
        }
        public NetworkConnectionBase(EndPoint ep, int bufferSize)
        {
            _socket = DevNet.ConnectTo(ep);
            _buffer = new byte[bufferSize];
        }


        public Socket Socket => _socket;

        public Action<Memory<byte>> OnDataReceivedAction { get => _onDataReceivedAction; set => _onDataReceivedAction = value; }
        protected Action<object, Exception> OnErrorAction { get => _onErrorAction; set => _onErrorAction = value; }


        public void Connect(EndPoint ep)
        {
            _socket.Connect(ep);
        }
        public async Task ConnectAsync(EndPoint ep)
        {
            await _socket.ConnectAsync(ep);
        }
        public void StartReceivingLoop()
        {
            _receivingLoopCTS = new();

            Task.Factory.StartNew(PacketsReceivingLoop, _receivingLoopCTS.Token,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void CancelReceivingLoop()
        {
            _receivingLoopCTS.Cancel();
        }


        public PacketContent Request(PacketContent content, Resolver resolver)
        {
            SendData(PacketBase<Packet, PacketContent, Encoder, Decoder, Resolver>.CreateRequest(content).ToBinarySpan());
            var lenth = ReceivePacket(false);
            var p = PacketBase<Packet, PacketContent, Encoder, Decoder, Resolver>.Unpack(new Memory<byte>(_buffer, 0, lenth), resolver);
            return p.Content;
        }
        public async Task<PacketContent> RequestAsync(PacketContent content, Resolver resolver)
        {
            await SendDataAsync(PacketBase<Packet, PacketContent, Encoder, Decoder, Resolver>.CreateRequest(content).ToBinaryMemory());
            var lenth = await ReceivePacketAsync(false);
            var p = PacketBase<Packet, PacketContent, Encoder, Decoder, Resolver>.Unpack(new Memory<byte>(_buffer, 0, lenth), resolver);
            return p.Content;
        }


        public async Task<PacketContent> RequestAsyncLocking(PacketContent content, Resolver resolver)
        {
            return await Task.Run(() =>
             {
                 lock (_requestingLocker)
                 {
                     SendData(PacketBase<Packet, PacketContent, Encoder, Decoder, Resolver>.CreateRequest(content).ToBinaryArray());
                     var len = ReceivePacket(false);
                     var p = PacketBase<Packet, PacketContent, Encoder, Decoder, Resolver>.Unpack(new Memory<byte>(_buffer, 0, len), resolver);
                     return p.Content;
                 }
             });
        }
        public void Response(PacketContent content)
        {
            SendData(PacketBase<Packet, PacketContent, Encoder, Decoder, Resolver>.CreateResponse(content).ToBinarySpan());
        }
        public void Message(PacketContent content)
        {
            SendData(PacketBase<Packet, PacketContent, Encoder, Decoder, Resolver>.CreateMessage(content).ToBinarySpan());
        }
        public async Task MessageAsync(PacketContent content)
        {
            await SendDataAsync(PacketBase<Packet, PacketContent, Encoder, Decoder, Resolver>.CreateMessage(content).ToBinaryMemory());
        }

        public void SendData(byte[] data) //todo: add locks (fields are already created)
        {
            try
            {
                _socket.Send(data);
            }
            catch (SocketException sex)
            {
                _onErrorAction?.Invoke(this, sex);
            }
        }
        public void SendData(Span<byte> data) //todo: add locks (fields are already created)
        {
            try
            {
                _socket.Send(data);
            }
            catch (SocketException sex)
            {
                _onErrorAction?.Invoke(this, sex);
            }
        }
        public async Task SendDataAsync(Memory<byte> data) //todo: add locks (fields are already created)
        {
            try
            {
                await _socket.SendAsync(data, SocketFlags.None);
            }
            catch (SocketException sex)
            {
                _onErrorAction?.Invoke(this, sex);
            }

        }

        /// <summary>
        /// Not invokes OnDataReceived event.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private void ReceiveBytes(int count, byte[] buffer) //todo: add locks (fields are already created)
        {
            try
            {
                if (buffer.Length <= count)
                {
                    throw new OutOfMemoryException($"buffer size is {buffer.Length}, bytes to receive: {count}."); //уважаемая ситуация BloodTrail
                }

                int left = count;

                while (left != 0)
                {
                    left -= _socket.Receive(buffer, count - left, left, SocketFlags.None);
                }
            }
            catch (SocketException sex)
            {
                //UnityEngine.Debug.LogError($"Error in ReceiveBytes in NetworkConnection.cs: {sex}");
                _onErrorAction?.Invoke(this, sex);
            }
            catch (Exception ex)
            {
                _onErrorAction?.Invoke(this, ex);
            }

        }
        private async Task<byte[]> ReceiveBytesAsync(int count, byte[] buffer) //todo: add locks (fields are already created)
        {
            try
            {
                int left = count;

                await Task.Run(() =>
                {
                    while (left != 0)
                        left -= _socket.Receive(buffer, count - left, left, SocketFlags.None);
                });

                return new Memory<byte>(buffer, 0, count).ToArray();

            }
            catch (SocketException sex)
            {
                // UnityEngine.Debug.LogError($"Error in ReceiveBytes in NetworkConnection.cs: {sex}");
                _onErrorAction?.Invoke(this, sex);
                return null;
            }
            catch (Exception ex)
            {
                _onErrorAction?.Invoke(this, ex);
                return null;
            }
        }
        //private async Task<Memory<byte>> ReceiveBytesAsync(int count, byte[] buffer)
        //{
        //    try
        //    {
        //        int left = count;

        //        await Task.Run(() =>
        //         {
        //             while (left != 0)
        //                 left -= _socket.Receive(buffer, count - left, left, SocketFlags.None);
        //         });

        //        return new Memory<byte>(buffer, 0, count);

        //    }
        //    catch (SocketException sockEx)
        //    {
        //        UnityEngine.Debug.LogError($"Error in ReceiveBytes in NetworkConnection.cs: {sockEx}");
        //        OnError?.Invoke(this);
        //        return null;
        //    }
        //}

        //private async Task<Memory<byte>> ReceiveBytesAsync(int count, byte[] buffer)
        //{
        //    try
        //    {
        //        var mem = new Memory<byte>(buffer, 0, count);

        //        int received = await _socket.ReceiveAsync(mem, SocketFlags.None);

        //        if (received != count)
        //        {
        //            UnityEngine.Debug.LogError("ReceiveBytesAsync received bytes count != needed count");
        //            throw new Exception();
        //        }

        //        return mem;
        //    }
        //    catch (SocketException sockEx)
        //    {
        //        UnityEngine.Debug.LogError($"Error in ReceiveBytes in NetworkConnection.cs: {sockEx}");
        //        OnError?.Invoke(this);
        //        return null;
        //    }

        //}

        public int ReceivePacket(bool invokeEvent = true)
        {
            ReceiveBytes(HEADER_SIZE, _buffer);
            int packetLength = BitConverter.ToInt32(_buffer) - HEADER_SIZE;
            ReceiveBytes(packetLength, _buffer);

            if (invokeEvent)
                _onDataReceivedAction?.Invoke(new Memory<byte>(_buffer, 0, packetLength));

            return packetLength;
        }

        public async Task<int> ReceivePacketAsync(bool invokeEvent = true)
        {
            var headerData = await ReceiveBytesAsync(HEADER_SIZE, _buffer);
            //int packetLength = BitConverter.ToInt32(headerData.Span) - HEADER_SIZE;
            int packetLength = BitConverter.ToInt32(headerData) - HEADER_SIZE;
            await ReceiveBytesAsync(packetLength, _buffer);

            if (invokeEvent)
                _onDataReceivedAction?.Invoke(new Memory<byte>(_buffer, 0, packetLength));

            return packetLength;
        }

        private void PacketsReceivingLoop()
        {
            while (true)
            {
                try
                {
                    _receivingLoopCTS.Token.ThrowIfCancellationRequested();
                    ReceivePacket();
                }
                catch (TaskCanceledException tcex)
                {
                    //UnityEngine.Debug.Log("PacketsReceivingLoop canceled: " + tcex.Message);
                    break;
                }
                catch (Exception ex)
                {
                    //UnityEngine.Debug.LogError("PacketsReceivingLoop() error: " + ex);
                    _onErrorAction?.Invoke(this, ex);
                    break;
                }
            }
        }

        public void Dispose()
        {
            if (_receivingLoopCTS != null)
            {
                try
                {
                    _receivingLoopCTS.Cancel();

                }
                catch (Exception)
                {

                }
                try
                {
                    ((IDisposable)_receivingLoopCTS).Dispose();

                }
                catch (Exception) { }
            }

            DevNet.CloseSocket(_socket);
        }
    }
}
