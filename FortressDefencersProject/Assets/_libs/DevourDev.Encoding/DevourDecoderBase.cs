using DevourEncoding.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using static DevourEncoding.DataTypeSize;

namespace DevourEncoding
{
    public class DevourDecoderBase
    {
        private Memory<byte> _encodedData;
        private int _position;


        public DevourDecoderBase()
        {
            _position = 0;
        }
        public DevourDecoderBase(Memory<byte> encodedData, int startPosition = 0)
        {
            _encodedData = encodedData;
            _position = startPosition;
        }


        protected int Position => _position;


        public void SetData(Memory<byte> encodedData) => _encodedData = encodedData;


        protected int GetNextElementLength()
        {
            return ReadInt();
        }


        #region Reads

        public int ReadInt()
        {
            return BitConverter.ToInt32(TakeBytes(INT_SIZE));
        }
        public int[] ReadInts()
        {
            return ReadInts(GetNextElementLength());
        }
        public int[] ReadInts(int count)
        {
            int[] data = new int[count];

            for (int i = 0; i < count; i++)
            {
                data[i] = ReadInt();
            }

            return data;
        }
        public int[][] ReadIntArrays()
        {
            return ReadIntArrays(GetNextElementLength());
        }
        public int[][] ReadIntArrays(int count)
        {
            int[][] data = new int[count][];
            for (int i = 0; i < count; i++)
            {
                data[i] = ReadInts();
            }

            return data;
        }
        public bool ReadBool()
        {
            return BitConverter.ToBoolean(TakeBytes(BOOL_SIZE));
        }
        public bool[] ReadBools()
        {
            return ReadBools(GetNextElementLength());
        }
        public bool[] ReadBools(int count)
        {
            var data = new bool[count];
            for (int i = 0; i < count; i++)
            {
                data[i] = ReadBool();
            }
            return data;
        }
        public byte ReadByte()
        {
            return TakeByte();
        }
        public byte[] ReadBytes()
        {
            int length = GetNextElementLength();
            return ReadBytes(length);

        }
        public byte[] ReadBytes(int count)
        {
            return TakeBytes(count).ToArray();
        }
        public char ReadChar()
        {
            return BitConverter.ToChar(TakeBytes(CHAR_SIZE));
        }
        public char[] ReadChars()
        {
            int length = GetNextElementLength();
            return ReadChars(length);
        }
        public char[] ReadChars(int count)
        {
            char[] data = new char[count];

            for (int i = 0; i < count; i++)
            {
                data[i] = ReadChar();
            }

            return data;
        }
        public decimal ReadDecimal()
        {
            return new decimal(ReadInts(DECIMAL_SIZE));
        }
        public decimal[] ReadDecimals()
        {
            return ReadDecimals(GetNextElementLength());
        }
        public decimal[] ReadDecimals(int count)
        {
            decimal[] data = new decimal[count];

            for (int i = 0; i < count; i++)
            {
                data[i] = ReadDecimal();
            }

            return data;
        }
        public double ReadDouble()
        {
            return BitConverter.ToDouble(TakeBytes(DOUBLE_SIZE));
        }
        public double[] ReadDoubles()
        {
            return ReadDoubles(GetNextElementLength());
        }
        public double[] ReadDoubles(int count)
        {
            double[] data = new double[count];

            for (int i = 0; i < count; i++)
            {
                data[i] = ReadDouble();
            }

            return data;
        }
        public short ReadInt16()
        {
            return BitConverter.ToInt16(TakeBytes(SHORT_SIZE));
        }
        public long ReadInt64()
        {
            return BitConverter.ToInt64(TakeBytes(LONG_SIZE));
        }
        public long[] ReadLongs()
        {
            return ReadLongs(GetNextElementLength());
        }
        public long[] ReadLongs(int count)
        {
            long[] data = new long[count];

            for (int i = 0; i < count; i++)
            {
                data[i] = ReadInt64();
            }

            return data;
        }
        public byte[][] ReadByteArrays()
        {
            return ReadByteArrays(GetNextElementLength());
        }
        public byte[][] ReadByteArrays(int count)
        {
            var data = new byte[count][];
            for (int i = 0; i < count; i++)
            {
                data[i] = ReadBytes();
            }

            return data;

        }
        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }
        public float ReadSingle()
        {
            return BitConverter.ToSingle(TakeBytes(FLOAT_SIZE));
        }
        public float[] ReadSingles()
        {
            int c = GetNextElementLength();
            var data = new float[c];
            for (int i = 0; i < c; i++)
            {
                data[i] = ReadSingle();
            }

            return data;
        }
        public string ReadString()
        {
            return Encoding.UTF8.GetString(TakeBytes(GetNextElementLength()));
        }
        public string[] ReadStrings()
        {
            int c = GetNextElementLength();
            var data = new string[c];
            for (int i = 0; i < c; i++)
            {
                data[i] = ReadString();
            }

            return data;
        }


        public ushort ReadUInt16()
        {
            return (ushort)ReadInt16();
        }
        public uint ReadUInt32()
        {
            return (uint)ReadInt();
        }
        public ulong ReadUInt64()
        {
            return (ulong)ReadInt64();
        }
        public DateTime ReadDateTime()
        {
            return DateTime.FromBinary(ReadInt64());
        }
        public TimeSpan ReadTimeSpan()
        {
            return new TimeSpan(ReadInt64());
        }
        public IPAddress ReadIPAddress()
        {
            return new IPAddress(ReadBytes());
        }
        public IPEndPoint ReadIPEndPoint()
        {
            var ip = ReadIPAddress();
            var port = ReadInt();
            return new IPEndPoint(ip, port);
        }

        public OperatingSystem ReadOperatingSystem()
        {
            PlatformID plId = (PlatformID)ReadByte();
            int verMin = ReadInt();
            int verMaj = ReadInt();
            int verBuild = ReadInt();
            int verRev = ReadInt();

            Version ver = new(verMin, verMaj, verBuild, verRev);

            return new OperatingSystem(plId, ver);
        }

        public T ReadXml<T>()
        {
            var data = ReadBytes();
            return Serialization.Xml.DevourXml.Deserialize<T>(data);
        }
        public T[] ReadXmls<T>()
        {
            return ReadXmls<T>(GetNextElementLength());
        }
        public T[] ReadXmls<T>(int count)
        {
            T[] data = new T[count];
            for (int i = 0; i < count; i++)
            {
                data[i] = ReadXml<T>();
            }

            return data;
        }

        public virtual T ReadDecodable<T>() where T : IDecodable<DevourDecoderBase>, new()
        {
            var d = new T();
            d.Decode(this);
            return d;
        }

        public virtual T[] ReadDecodables<T>() where T : IDecodable<DevourDecoderBase>, new()
        {
            int amount = GetNextElementLength();
            T[] data = new T[amount];

            for (int i = 0; i < amount; i++)
            {
                data[i] = new();
                data[i].Decode(this);
            }

            return data;
        }
        public virtual T[][] ReadDecodablesArray<T>() where T : IDecodable<DevourDecoderBase>, new()
        {
            int amount = GetNextElementLength();
            T[][] data = new T[amount][];

            for (int i = 0; i < amount; i++)
            {
                data[i] = ReadDecodables<T>();

                //data[i] = new T[GetNextElementLength()];
                //for (int j = 0; j < data[i].Length; j++)
                //{
                //    data[i][j] = new T();
                //    data[i][j].Decode(this);
                //}

            }

            return data;
        }

        #endregion

        protected ReadOnlySpan<byte> TakeBytes(int amount)
        {
            var ros = _encodedData.Slice(_position, amount).Span;
            MovePosition(amount);
            return ros;
        }
        protected byte TakeByte()
        {
            byte b = _encodedData.Span[_position];
            MovePosition(1);
            return b;
        }
        protected void MovePosition(int value)
        {
            _position += value;
        }
    }
}
