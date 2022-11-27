using DevourDev.Database.Interfaces;
using DevourDev.MonoExtentions;
using DevourEncoding.Serialization.Xml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevourDev.Database
{
    public abstract class Database<Entity> where Entity : IEntity
    {
        public event Action<bool, string> OnLockStatusChanged;

        private readonly object _lockerObject = new();

        private readonly Dictionary<long, Entity> _entities;

        private readonly List<Entity> _trashcan;
        private long _lastEntityID;


        public Database()
        {
            _entities = new();
            _trashcan = new(1024);
        }


        public long NextUniqueID => LastEntityID + 1;
        public int EntitiesCount => Entities.Count;

        protected Dictionary<long, Entity> Entities => _entities;
        protected long LastEntityID { get => _lastEntityID; set => _lastEntityID = value; }


        public void ClearEntities()
        {
            _entities.Clear();
        }

        public void EditEntity(Entity edited)
        {
            lock (_lockerObject)
            {
                OnLockStatusChanged?.Invoke(true, nameof(EditEntity));

                AddEntityToTrashcan(Entities[edited.UniqueID]);
                Entities[edited.UniqueID] = edited;

                OnLockStatusChanged?.Invoke(false, nameof(EditEntity));
            }
        }

        public void AddEntity(Entity ent, bool autoSetID)
        {
            lock (_lockerObject)
            {
                OnLockStatusChanged?.Invoke(true, nameof(AddEntity));

                AddEntity_vrtl(ent, autoSetID);

                OnLockStatusChanged?.Invoke(false, nameof(AddEntity));
            }
        }

        protected virtual void AddEntity_vrtl(Entity ent, bool autoSetID)
        {
            long newEntityID = autoSetID ? NextUniqueID : ent.UniqueID;
            Entities.Add(newEntityID, ent);
            LastEntityID = newEntityID;
        }
        public void RemoveEntity(long id)
        {
            lock (_lockerObject)
            {
                OnLockStatusChanged?.Invoke(true, nameof(RemoveEntity));

                RemoveEntity_vrtl(id);

                OnLockStatusChanged?.Invoke(false, nameof(RemoveEntity));
            }
        }


        protected virtual void RemoveEntity_vrtl(long id)
        {
            AddEntityToTrashcan(Entities[id]);
            Entities.Remove(id);
        }
        public void RemoveEntity(Entity ent)
        {
            lock (_lockerObject)
            {
                OnLockStatusChanged?.Invoke(true, nameof(RemoveEntity));

                RemoveEntity_vrtl(ent);

                OnLockStatusChanged?.Invoke(false, nameof(RemoveEntity));
            }
        }

        protected virtual void RemoveEntity_vrtl(Entity ent)
        {
            AddEntityToTrashcan(Entities[ent.UniqueID]);
            Entities.Remove(ent.UniqueID);
        }
        public void SaveEntities(string path, string fileName)
        {
            lock (_lockerObject)
            {
                OnLockStatusChanged?.Invoke(true, nameof(SaveEntities));

                SaveEntities_vrtl(path, fileName);

                OnLockStatusChanged?.Invoke(false, nameof(SaveEntities));
            }
        }

        protected virtual void SaveEntities_vrtl(string path, string fileName)
        {
            (long, Entity)[] data = new (long, Entity)[EntitiesCount];
            int i = 0;
            foreach (var ent in Entities)
            {
                data[i] = (ent.Key, ent.Value);
                i++;
            }
            DevourXml.SaveXmlToFile(path, fileName, data);
        }
        public void LoadEntities(string filepath)
        {
            lock (_lockerObject)
            {
                OnLockStatusChanged?.Invoke(true, nameof(LoadEntities));
                var entities = DevourXml.LoadXmlFile<(long, Entity)[]>(filepath);
                LoadEntities_vrtl(entities);

                OnLockStatusChanged?.Invoke(false, nameof(LoadEntities));
            }
        }

        protected virtual void LoadEntities_vrtl((long, Entity)[] entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                (long, Entity) e = entities[i];
                var k = e.Item1;
                var v = e.Item2;

                Entities.Add(k, v);
                LastEntityID = k;
            }
        }
        public bool TryFindByID(long id, out Entity ent)
        {
            return Entities.TryGetValue(id, out ent);
        }

        protected void AddEntityToTrashcan(long id)
        {
            _trashcan.Add(Entities[id]);
        }
        protected void AddEntityToTrashcan(Entity ent)
        {
            AddEntityToTrashcan(ent.UniqueID);
        }
        public void ClearTrashcan()
        {
            lock (_lockerObject)
            {
                OnLockStatusChanged?.Invoke(true, nameof(ClearTrashcan));
                _trashcan.Clear();
                OnLockStatusChanged?.Invoke(false, nameof(ClearTrashcan));
            }
        }
        public Entity[] GetTrashcanContent()
        {
            lock (_lockerObject)
            {
                OnLockStatusChanged?.Invoke(true, nameof(GetTrashcanContent));
                OnLockStatusChanged?.Invoke(false, nameof(GetTrashcanContent));
                return _trashcan.ToArray();
            }
        }
    }
}
