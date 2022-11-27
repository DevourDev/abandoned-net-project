using DevourDev.Base;
using FD.Units;
using DevourDev.Networking;
using FD.Networking;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using DevourDev.Base.HeavyRandom;

namespace FD.Global.Sides
{
    /// <summary>
    /// rename me
    /// </summary>
    public class PlayerGameInfo
    {
        private readonly long _accID;
        private readonly int[] _deck;


        public PlayerGameInfo(long accID, int[] deck)
        {
            _deck = deck;
            _accID = accID;
        }


        public long AccID => _accID;
        public int[] Deck => _deck;
    }

    /// <summary>
    /// Называется "Сторона", но по факту - Игрок. Один Игрок и всё, что с ним связано:
    /// его ресурсы (деньги и Юниты), мета-данные (ID Игрока, за какую Команду играет),
    /// а также данные для обмена данными по Сети.
    /// </summary>
    public abstract class GameSideBase : IUniqueReadonly, System.IDisposable
    {
        private readonly int _sideID;
        private readonly PlayerResources _resources;

        private readonly Queue<IPacketContent> _requestsQ;
        private readonly Queue<IPacketContent> _responsesQ;

        private PlayerGameInfo _playerInfo;


        public GameSideBase()
        {
            Lost = false;
            _sideID = GameManager.Instance.Sides.Count;
            _resources = new PlayerResources(500, 500);
            _requestsQ = new(128);
            _responsesQ = new(128);
        }


        public bool Lost { get; set; }
        public int UniqueID => _sideID;

        public PlayerResources Resources => _resources;
        public PlayerGameInfo PlayerInfo => _playerInfo;

        protected Queue<IPacketContent> RequestsQ => _requestsQ;
        protected Queue<IPacketContent> ResponsesQ => _responsesQ;


        public bool TryGetRequest(out IPacketContent r) => _requestsQ.TryDequeue(out r);
        public void AddResponse(IPacketContent r) => _responsesQ.Enqueue(r);

        public void AddRequestToQ(IPacketContent req, ResponsingConnection state)
        {
            RequestsQ.Enqueue(req);
        }


        public virtual void SetPlayerInfo(PlayerGameInfo info)
        {
            _playerInfo = info;
        }

        public virtual void GenerateShowCase()
        {
            int[] slots = new int[5];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = _playerInfo.Deck.RandElement();
            }

            Resources.Showcase.Set(slots);
        }
        public virtual void AddUnit(UnitOnSceneBase u)
        {
            _resources.ActiveUnits.Add(u.UniqueID, u);
        }

        public void RemoveUnit(UnitOnSceneBase u)
        {
            RemoveUnit(u.UniqueID);
        }

        public void RemoveUnit(int uID)
        {
            _resources.ActiveUnits.Remove(uID);
        }

        public void SendQueuedResponses()
        {
            while (ResponsesQ.TryDequeue(out var res))
                HandleResponse(res);
        }

        protected abstract void HandleResponse(IPacketContent res);

        public virtual void Dispose()
        {
            _resources.Dispose();
        }
    }
}