using FD.Networking.Realm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Test
{
    public class EncoderDecoder_test : MonoBehaviour
    {
        private System.Random _r = new();
        private System.Security.Cryptography.RandomNumberGenerator _rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        private void Start()
        {
          
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Test_UsersInitialDataResponse();
            }
        }

        private void Test_UsersInitialDataResponse() // OK!
        {
            UsersInitialDataResponse res = new();
            res.Result = true;
            int[] p1Deck = { 0 };
            int[] p2Deck = { 0 };
            int[][] playersDecks = new int[2][] { p1Deck, p2Deck };
            res.PlayersDecks = playersDecks;

            byte[][] playerKeys = new byte[2][];
            for (int i = 0; i < playerKeys.Length; i++)
            {
                playerKeys[i] = new byte[2048];
                _rng.GetBytes(playerKeys[i]);
            }

            res.PlayersEnterRealmKeys = playerKeys;

            var e = new FD.Networking.Encoder();
            var d = new FD.Networking.Decoder();
            res.Encode(e);
            var encodedData = e.EncodedData.ToArray();

            d.SetData(encodedData);

            UsersInitialDataResponse decoded = new();
            decoded.Decode(d);
        }

        private void GardenRealmPacketSimulation() // not tested
        {
            byte[][] playerKeys = new byte[2][];
            for (int i = 0; i < playerKeys.Length; i++)
            {
                playerKeys[i] = new byte[2048];
                _rng.GetBytes(playerKeys[i]);
            }

            int[][] playerDecks = new int[][] { new int[] { 0 }, new int[] { 0 } };


        }

        private void IntsArray_test() // OK!
        {
            int[][] intsArrays = new int[500][];
            for (int i = 0; i < intsArrays.Length; i++)
            {
                intsArrays[i] = new int[512];
                for (int k = 0; k < intsArrays[i].Length; k++)
                {
                    intsArrays[i][k] = _r.Next();
                }
            }

            var e = new FD.Networking.Encoder();
            var d = new FD.Networking.Decoder();
            e.Write(intsArrays);
            byte[] data = e.EncodedData.ToArray();

            d.SetData(data);
            var decodedData = d.ReadIntArrays();

            for (int i = 0; i < intsArrays.Length; i++)
            {
                for (int j = 0; j < intsArrays[i].Length; j++)
                {
                    if (intsArrays[i][j] != decodedData[i][j])
                    {
                        Debug.LogError("WRONG");
                        return;
                    }
                }
            }

            Debug.Log($"{nameof(IntsArray_test)} passed.");
        }

        private void Bytes_test()
        {
            byte[] bytes = new byte[200_000];
            _r.NextBytes(bytes);

            var e = new FD.Networking.Encoder();
            var d = new FD.Networking.Decoder();

            e.Write(bytes);
            byte[] data = e.EncodedData.ToArray();

            d.SetData(data);
            var decodedData = d.ReadBytes();

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] != decodedData[i])
                {
                    Debug.LogError("WRONG");
                    return;
                }
            }

            Debug.Log($"{nameof(Bytes_test)} passed.");
        } // OK!

        private void BytesArrays_test() // OK!
        {
            byte[][] bytesArrays = new byte[500][];
            for (int i = 0; i < bytesArrays.Length; i++)
            {
                bytesArrays[i] = new byte[512];
                _r.NextBytes(bytesArrays[i]);
            }

            var e = new FD.Networking.Encoder();
            var d = new FD.Networking.Decoder();
            e.Write(bytesArrays);
            byte[] data = e.EncodedData.ToArray();

            d.SetData(data);
            var decodedData = d.ReadByteArrays();

            for (int i = 0; i < bytesArrays.Length; i++)
            {
                for (int j = 0; j < bytesArrays[i].Length; j++)
                {
                    if (bytesArrays[i][j] != decodedData[i][j])
                    {
                        Debug.LogError("WRONG");
                        return;
                    }
                }
            }

            Debug.Log($"{nameof(BytesArrays_test)} passed.");
        }
    }
}
