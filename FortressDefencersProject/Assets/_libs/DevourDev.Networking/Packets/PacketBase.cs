using System;

namespace DevourDev.Networking.Packets
{
    public abstract class PacketBase<Packet, PacketContent, Encoder, Decoder, Resolver>
        where Packet : PacketBase<Packet, PacketContent, Encoder, Decoder, Resolver>, new()
         where Encoder : DevourEncoding.DevourEncoderBase, new()
        where Decoder : DevourEncoding.DevourDecoderBase, new()
        where PacketContent : IPacketContentBase<Encoder, Decoder>
        where Resolver : PacketsResolver<PacketContent, Encoder, Decoder>

    {
        private PacketType _type;
        private PacketContent _content;


        public PacketBase()
        {

        }


        public PacketType Type { get => _type; protected set => _type = value; }
        public PacketContent Content { get => _content; protected set => _content = value; }


        /// <summary>
        /// To binary data.
        /// </summary>
        /// <returns></returns>
        public byte[] ToBinaryArray()
        {
            return ToList().ToArray();
        }
        public System.Collections.Generic.List<byte> ToList()
        {
            try
            {
                Encoder e = new();
                e.Write((int)1);
                e.Write((byte)Type);
                Content.Encode(e);
                e.OverrideData(BitConverter.GetBytes(e.EncodedData.Count), 0);
                return e.EncodedData;

            }
            catch (Exception ex)
            {
                var exception = new Exception($"Error in Packing packet in Packet.cs: {ex} :(");
                UnityEngine.Debug.LogError(exception);
                throw exception;
            }
        }
        public Span<byte> ToBinarySpan()
        {
            return ToBinaryArray().AsSpan();
        }
        public Memory<byte> ToBinaryMemory()
        {
            return ToBinaryArray().AsMemory();
        }


        /// <summary>
        /// Decode binary data to Packet.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static Packet Unpack(Memory<byte> data, Resolver resolver, bool readPacketLength = false)
        {
            try
            {
                Decoder d = new Decoder();
                d.SetData(data);

                if (readPacketLength)
                    d.ReadInt();
                var p = new Packet
                {
                    Type = (PacketType)d.ReadByte()
                };
                var content = resolver.GetPacket(d.ReadInt());
                content.Decode(d, false);
                p.Content = content;
                return p;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error in Unpacking packet in Packet.cs: {ex} :(");
                return null;
            }
        }

        public static Packet CreateRequest(PacketContent content)
        {
            var p = new Packet
            {
                Type = PacketType.Request,
                Content = content
            };
            return p;
        }

        public static Packet CreateResponse(PacketContent content)
        {
            var p = new Packet
            {
                Type = PacketType.Response,
                Content = content
            };
            return p;
        }

        public static Packet CreateMessage(PacketContent content)
        {
            var p = new Packet
            {
                Type = PacketType.Message,
                Content = content
            };
            return p;
        }
    }

}
