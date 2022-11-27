using System;
using System.Collections.Generic;
using FD.Networking.Database.Entities.Account;

namespace FD.Networking.Database
{
    public class AccountsDatabase : DevourDev.Database.Database<Account>
    {
        private readonly Dictionary<string, long> _byLogin;

        public AccountsDatabase() : base()
        {
            _byLogin = new();
        }


        public bool CheckLoginAvailability(string l) => !_byLogin.ContainsKey(l);

        public bool TryFindByLogin(string login, out Account acc)
        {
            if (_byLogin.TryGetValue(login.ToLower(), out var id))
            {
                acc = Entities[id];
                return true;
            }
            acc = null;
            return false;
        }

        public bool TryRegistrateAccount(string validSourceLogin, string doubleHashedPassword, EmailAddress validEmail, out Account acc)
        {
            if (!CheckLoginAvailability(validSourceLogin.ToLower()))
            {
                acc = null;
                return false;
            }

            acc = new Account(NextUniqueID, new SecureData
            {
                LogInData = new(validSourceLogin, doubleHashedPassword, validEmail),
                History = new(),
            });
            AddEntity(acc, false);
            return true;

        }

        public EKey UpdateEKey(long accID, EKeyType t)
        {
            return UpdateEKey(Entities[accID], t);
        }
        public EKey UpdateEKey(string login, EKeyType t)
        {
            return UpdateEKey(_byLogin[login], t);
        }
        public EKey UpdateEKey(Account acc, EKeyType t)
        {
            var k = GenerateEKey(acc, t);
            acc.Temporary.SetKey(k);
            return k;
        }

        public void DeleteEKey(long accID, EKeyType t)
        {
            DeleteEKey(Entities[accID], t);
        }
        public void DeleteEKey(string login, EKeyType t)
        {
            DeleteEKey(_byLogin[login], t);
        }
        public void DeleteEKey(Account acc, EKeyType t)
        {
            acc.Temporary.DeleteKey(t);
        }

        public static EKey GenerateEKey(Account acc, EKeyType t)
        {
            return EKey.Generate(t, BitConverter.GetBytes(acc.AccountID));
        }

        protected override void AddEntity_vrtl(Account ent, bool autoSetID)
        {
            base.AddEntity_vrtl(ent, autoSetID);
            _byLogin.Add(ent.SecureData.LogInData.Login, ent.UniqueID);
        }
        protected override void RemoveEntity_vrtl(Account ent)
        {
            base.RemoveEntity_vrtl(ent);
            _byLogin.Remove(ent.SecureData.LogInData.Login);
        }

        protected override void LoadEntities_vrtl((long, Account)[] entities)
        {
            base.LoadEntities_vrtl(entities);
            _byLogin.Clear();
            for (int i = 0; i < entities.Length; i++)
            {
                (long, Account) e = entities[i];
                _byLogin.Add(e.Item2.SecureData.LogInData.Login, e.Item1);
            }

        }
    }
}
