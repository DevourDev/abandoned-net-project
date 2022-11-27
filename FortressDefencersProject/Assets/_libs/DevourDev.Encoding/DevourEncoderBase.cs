using DevourEncoding.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DevourEncoding
{
    public class DevourEncoderBase
    {
        private readonly List<byte> _encodedData;


        public DevourEncoderBase()
        {
            _encodedData = new();
        }


        public List<byte> EncodedData => _encodedData;


        #region constant-sized data
        public void Write(int data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteData(newData);
        }
        public void Write(uint data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteData(newData);
        }
        public void Write(short data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteData(newData);
        }
        public void Write(ushort data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteData(newData);
        }
        public void Write(bool data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteData(newData);
        }
        public void Write(char data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteData(newData);
        }
        public void Write(byte data)
        {
            WriteData(data);
        }
        public void Write(long data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteData(newData);
        }
        public void Write(ulong data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteData(newData);
        }
        public void Write(float data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteData(newData);
        }
        public void Write(double data)
        {
            byte[] newData = BitConverter.GetBytes(data);
            WriteData(newData);
        }
        public void Write(decimal data)
        {
            int[] bits = decimal.GetBits(data);
            foreach (var b in bits)
            {
                Write(b);
            }
        }

        public void Write(DateTime data)
        {
            Write(data.ToBinary());
        }
        public void Write(TimeSpan data)
        {
            Write(data.Ticks);
        }
        public void Write(IPAddress data)
        {
            Write(data.GetAddressBytes());
        }
        public void Write(IPEndPoint data)
        {
            Write(data.Address);
            Write(data.Port);
        }
        public void Write(OperatingSystem data)
        {
            Write((byte)data.Platform);
            Write(data.Version.Minor);
            Write(data.Version.Major);
            Write(data.Version.Build);
            Write(data.Version.Revision);
        }
        #endregion

        #region non-constant-sized data
        public void Write(ICollection<float> data)
        {
            if (CheckForNull(data))
                return;
            WriteNextElementLength(data.Count);
            foreach (var item in data)
            {
                Write(item);
            }
        }
        public void Write(byte[] data)
        {
            if (CheckForNull(data))
                return;
            WriteNextElementLength(data.Length);
            WriteData(data);
        }
        public void Write(bool[] data)
        {
            if (CheckForNull(data))
                return;
            WriteNextElementLength(data.Length);
            foreach (var item in data)
            {
                Write(item);
            }
        }
        public void Write(ICollection<int> data)
        {
            if (CheckForNull(data))
                return;
            WriteNextElementLength(data.Count);
            foreach (var item in data)
            {
                Write(item);
            }
        }
        public void Write(string data)
        {
            var encodedData = Encoding.UTF8.GetBytes(data);
            WriteNextElementLength(encodedData.Length);
            WriteData(encodedData);
        }
        public void Write(ICollection<string> data)
        {
            if (CheckForNull(data))
                return;
            WriteNextElementLength(data.Count);
            foreach (var item in data)
            {
                Write(item);
            }
        }
        public void Write(long[] data)
        {
            if (CheckForNull(data))
                return;
            WriteNextElementLength(data.Length);
            foreach (var item in data)
            {
                Write(item);
            }
        }

        public void Write(byte[][] data)
        {
            if (CheckForNull(data))
                return;
            WriteNextElementLength(data.Length);
            foreach (var item in data)
            {
                Write(item);
            }
        }

        public void Write(int[][] data)
        {
            if (CheckForNull(data))
                return;

            WriteNextElementLength(data.Length);
            foreach (var item in data)
            {
                Write(item);
            }
        }

        public void Write(IEncodable<DevourEncoderBase> data) //this is hell.......
        {
            data.Encode(this);
        }

        public void Write(ICollection<IEncodable<DevourEncoderBase>> data)
            => WriteBaseEncodables(data);
        public void WriteBaseEncodables<T>(ICollection<T> data) where T : IEncodable<DevourEncoderBase>
        {
            if (CheckForNull(data))
                return;
            WriteNextElementLength(data.Count);
            foreach (var d in data)
            {
                Write(d);
            }
        }

        public void WriteXml<T>(T data)
        {
            var serialized = Serialization.Xml.DevourXml.Serialize(data);
            Write(serialized);
        }

        public void WriteXmls<T>(T[] datas)
        {
            if (CheckForNull(datas))
                return;
            WriteNextElementLength(datas.Length);

            foreach (var d in datas)
            {
                WriteXml(d);
            }
        }

        #endregion

        protected void WriteNextElementLength(int length)
        {
            Write(length);
        }

        protected bool CheckForNull(object o)
        {
            if (o == null)
            {
                WriteNextElementLength(0);
                return true;
            }

            return false;
        }

        protected void WriteData(byte[] newData)
        {
            _encodedData.AddRange(newData);
        }
        protected void WriteData(byte newData)
        {
            _encodedData.Add(newData);
        }
        public void OverrideData(byte data, int index)
        {
            //mb check for out of range? 

            _encodedData[index] = data;
        }
        public void OverrideData(byte[] data, int startIndex)
        {
            for (int i = startIndex, j = 0; j < data.Length; ++i, ++j)
            {
                _encodedData[i] = data[j];
            }
        }
    }
}
