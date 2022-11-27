using DevourEncoding.Interfaces;
using System;
using UnityEngine;

namespace FD.Networking
{
    public class Decoder : DevourEncoding.DevourDecoderBase
    {
        public Decoder() : base()
        {

        }
        public Decoder(Memory<byte> encodedData, int startPosition = 0) : base(encodedData, startPosition)
        {
        }

        public Vector3 ReadVector3()
        {
            var value = new Vector3(ReadSingle(), ReadSingle(), ReadSingle());
            return value;
        }

        public Vector3[] ReadVector3s()
        {
            return ReadVector3s(GetNextElementLength());
        }

        public Vector3[] ReadVector3s(int count)
        {
            Vector3[] value = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadVector3();
            }

            return value;
        }

        public Quaternion ReadQuaternion()
        {
            var value = new Quaternion(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
            return value;
        }

        public Quaternion[] ReadQuaternions()
        {
            return ReadQuaternions(GetNextElementLength());
        }
        public Quaternion[] ReadQuaternions(int count)
        {
            var value = new Quaternion[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = ReadQuaternion();
            }

            return value;
        }

        public new T ReadDecodable<T>() where T : IDecodable<Decoder>, new()
        {
            var d = new T();
            d.Decode(this);
            return d;
        }

        public new T[] ReadDecodables<T>() where T : IDecodable<Decoder>, new()
        {
            int amount = GetNextElementLength();
            T[] data = new T[amount];

            for (int i = 0; i < amount; i++)
            {
                data[i] = new T();
                data[i].Decode(this);
            }

            return data;
        }

        public new T[][] ReadDecodablesArray<T>() where T : IDecodable<Decoder>, new()
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
    }


}
