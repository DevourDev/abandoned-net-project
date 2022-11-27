using DevourDev.Database.Interfaces;
using DevourEncoding.Serialization.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FD.Networking.Database.Entities.Account
{
    public class AccountPublicData
    {
        public string NickName;
        //profile picture url...


        public AccountPublicData()
        {
            NickName = "NewUser";
        }
        public AccountPublicData(string nickName)
        {
            NickName = nickName;
        }
    }
    public class Account : IEntity
    {
        public long AccountID;
        public AccountPublicData PublicData;
        public SecureData SecureData;
        public GameStatistics GameStatistics;
        public TmpData Temporary;


        public Account()
        {
            AccountID = -1;
            PublicData = new();
            SecureData = new();
            GameStatistics = new();
            Temporary = new();
        }
        public Account(long accID, SecureData secureData)
        {
            AccountID = accID;
            SecureData = secureData;
            PublicData = new(secureData.LogInData.SourceLogin);
            GameStatistics = new();
            Temporary = new();
        }


        public long UniqueID { get => AccountID; set => AccountID = value; }

        public byte[] Encode()
        {
            return DevourXml.Serialize(this);
        }


        public static Account Registrate(string sourceLogin, string clientSideHashedPassword,
           EmailAddress email, IPEndPoint ep, PlatformID platformID, Version version, DateTime clientDT, long id)
        {
            Account a = new()
            {
                SecureData = new()
                {
                    LogInData = new(sourceLogin, clientSideHashedPassword, email),
                    History = new(new AccountCreatedAction(ep, platformID, version, clientDT, clientSideHashedPassword)),
                },
                GameStatistics = new(),
                Temporary = new(),
            };

            return a;
        }
    }

    public class TmpData
    {
        public List<EKey> Keys;

        public TmpData()
        {
            Keys = new();
        }

        public bool TryGetKey(EKeyType type, out EKey value)
        {
            foreach (var k in Keys)
            {
                if (k.Type == type)
                {
                    value = k;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public void SetKey(EKey value)
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                if (Keys[i].Type == value.Type)
                {
                    Keys[i] = value;
                    return;
                }
            }

            Keys.Add(value);
        }

        public void DeleteKey(EKeyType type)
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                if (Keys[i].Type == type)
                {
                    Keys.RemoveAt(i);
                }
            }
        }
    }

    [Obsolete("Use TmpData.GetKey instead", true)]
    public class EKeys
    {
        public EKey[] GameSessionKey;
        public EKey GardenMessageListeningConnectionKey;
        /// <summary>
        /// also uses as request-response connection key
        /// </summary>
        public EKey RealmSessionKey;
        public EKey RealmMessageListeningConnectionKey;
        public EKey ResetPasswordKey;

    }

    /// <summary>
    /// byte[256]
    /// </summary>
    public struct EKey
    {
        public const int EKEY_SIZE = 256;

        public EKeyType Type;
        public byte[] Key;
        public DateTime GenerationDate;

        public bool KeyEqual(byte[] otherKey)
        {
            return Key.SequenceEqual(otherKey);
        }
        public static EKey Generate(EKeyType t)
        {
            return GenerateBase(t, EKEY_SIZE);
        }
        public static EKey Generate(EKeyType t, byte[] tailData)
        {
            if (tailData.Length > EKEY_SIZE / 2)
                throw new ArgumentOutOfRangeException(paramName: tailData.ToString(), message: "Too large tail data.");

            EKey sk = GenerateBase(t, EKEY_SIZE);

            for (int i = 0, j = EKEY_SIZE - tailData.Length; i < tailData.Length; i++, j++)
            {
                sk.Key[j] = tailData[i];
            }

            return sk;
        }

        private static EKey GenerateBase(EKeyType t, int length)
        {
            Random r = new();
            var sk = new EKey
            {
                GenerationDate = DateTime.Now,
                Type = t,
                Key = new byte[length]
            };
            r.NextBytes(sk.Key);
            return sk;
        }
    }


}
