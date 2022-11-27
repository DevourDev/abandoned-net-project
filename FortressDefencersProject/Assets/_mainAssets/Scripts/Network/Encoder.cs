using DevourEncoding;
using DevourEncoding.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Networking
{
    public class Encoder : DevourEncoding.DevourEncoderBase
    {
        public void Write(Vector3 data)
        {
            Write(data.x);
            Write(data.y);
            Write(data.z);
        }
        public void Write(ICollection<Vector3> data)
        {
            if (CheckForNull(data))
                return;
            WriteNextElementLength(data.Count);
            foreach (var d in data)
            {
                Write(d);
            }
        }

        public void Write(Quaternion data)
        {
            Write(data.x);
            Write(data.y);
            Write(data.z);
            Write(data.w);
        }

        public void Write(ICollection<Quaternion> data)
        {
            if (CheckForNull(data))
                return;
            WriteNextElementLength(data.Count);
            foreach (var d in data)
            {
                Write(d);
            }
        }
        public virtual void Write(IEncodable<Encoder> data)
        {
            data.Encode(this);
        }

        public virtual void Write(ICollection<IEncodable<Encoder>> data)
            => WriteEncodables(data);

        public virtual void WriteEncodables<T>(ICollection<T> data) where T : IEncodable<Encoder>
        {
            if (CheckForNull(data))
                return;

            WriteNextElementLength(data.Count);
            foreach (var d in data)
            {
                Write(d);
            }
        }

        public virtual void WriteEncodables<T>(ICollection<ICollection<T>> data) where T : IEncodable<Encoder>
        {
            if (CheckForNull(data))
                return;

            WriteNextElementLength(data.Count);
            foreach (var d in data)
            {
                WriteEncodables(d);
            }
        }
        //public virtual void WriteEncodable(IEncodable<Encoder> data)
        //{
        //    data.Encode(this);
        //}

        //public virtual void WriteEncodables<T>(ICollection<T> data) where T : IEncodable<Encoder>
        //{
        //    if (CheckForNull(data))
        //        return;

        //    WriteNextElementLength(data.Count);
        //    foreach (var d in data)
        //    {
        //        WriteEncodable(d);
        //    }
        //}

    }
}
