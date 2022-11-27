using DevourDev.MonoBase.AbilitiesSystem;
using FD.Networking;
using FD.Networking.Realm.GamePackets;
using FD.Units;
using System;
using System.Collections.Generic;

namespace FD.Global
{
    public class GameStateRegistrator
    {
        //private List<UnitOnSceneBase> _newUnits;
        private Dictionary<int, PlayerNewResourcesPacket> _newPlayersResources;
        private Dictionary<int, UnitUpdatedEvent> _updatedUnits;
        private Dictionary<int, UnitUpdatedEvent> _lastTurnUpdatedUnits;
        private List<UnitDiedEvent> _deathesEvents;



        public GameStateRegistrator()
        {
            //_newUnits = new();
            _newPlayersResources = new();
            _updatedUnits = new();
            _lastTurnUpdatedUnits = new();
            _deathesEvents = new();
        }

        //debug
        //private int i = 0;
        //private int updatesCounter = 0;
        public byte[] GetSharedGameStateMessageData()
        {
            var sgsm = new SharedGameStateMessage(
                _newPlayersResources.Values,
                _updatedUnits.Values,
                _deathesEvents);

            //i++;
            //updatesCounter += _updatedTransformsEvents.Count;
            //if (i > 10)
            //{
            //    UnityEngine.Debug.LogError($"Transforms updated now: {_updatedTransformsEvents.Count}, average: {(float)updatesCounter / i}");
            //    i = 0;
            //    updatesCounter = 0;
            //}

            //invalid cast error !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! (fixed somehow)
            var data = Packet.CreateMessage(sgsm).ToBinaryArray();
            Clear();
            return data;
        }


        public void AddNewUnit(UnitOnSceneBase u)
        {
            if (!_newPlayersResources.TryGetValue(u.Owner.UniqueID, out var resPack))
            {
                resPack = new PlayerNewResourcesPacket(true);
                _newPlayersResources.Add(u.Owner.UniqueID, resPack);
            }

            resPack.AddUnit(u);


        }

        private UnitUpdatedEvent GetOrCreateUnitUpdateEvent(UnitOnSceneBase u)
        {
            if (!_updatedUnits.TryGetValue(u.UniqueID, out var fu))
            {
                #region fixed tofix
                //tofix (now):  (all solved)
                // Clientside transforms sync (sometimes after large amount of spawnings) (solved)
                // clientsideUnits rotating on 1 place, other units - going throw walls (off the ground limits)  (solved)
                // probably UniqueIDs bug (nope)  (solved)

                //upd: upper or another problem is caused by wrong 'TransformChanged' flag. No. Not flag - clientside handling.  (solved)
                //need to fix (adjust) Interpolator.

                //attempting to adjust Interpolator... [13:17]...  (solved)
                #endregion

                fu = new UnitUpdatedEvent();
                fu.UnitUniqueID = u.UniqueID;
                _updatedUnits.Add(u.UniqueID, fu);
                fu.SetAsFilledWithLists();
            }

            return fu;
        }
        private bool TryGetPreviousUnitUpdateEvent(UnitOnSceneBase u, out UnitUpdatedEvent puue)
        {
            return _lastTurnUpdatedUnits.TryGetValue(u.UniqueID, out puue);
        }

        public void AddTransformUpdate(UnitOnSceneBase u)
        {
            var fu = GetOrCreateUnitUpdateEvent(u);

            fu.UpdatedTransform = new TransformPacket
            {
                Position = u.transform.position,
                YRotation = u.transform.rotation.eulerAngles.y,
            };

            if (_lastTurnUpdatedUnits.TryGetValue(u.UniqueID, out var ltfu))
            {
                if (ltfu.UpdatedTransform.YRotation == u.transform.rotation.eulerAngles.y
                && ltfu.UpdatedTransform.Position == u.transform.position)
                {
                    fu.TransformChanged = false;
                    return;
                }
            }

            fu.TransformChanged = true;
        }

        public void AddDynamicStatUpdate(UnitOnSceneBase u)
        {
            var updateEvent = GetOrCreateUnitUpdateEvent(u);
            TryGetPreviousUnitUpdateEvent(u, out var lastTurnUpdateEvent);
            var dstats = u.RealStats.Common.DynamicStats.Stats;
            foreach (var ds in dstats)
            {
                var udsp = new UpdatedDynamicStatPacket();
                udsp.StatID = ds.Key.UniqueID;
                udsp.Current = ds.Value.Current;

                if (lastTurnUpdateEvent != null)
                {
                    if (lastTurnUpdateEvent.UpdatedDynamicStatsDic.TryGetValue(ds.Key.UniqueID, out var lastDs))
                    {
                        if (lastDs.MinValue != ds.Value.Min
                            || lastDs.MaxValue != ds.Value.Max
                            || lastDs.RegenValue != ds.Value.Regen)
                        {
                            udsp.BoundsChanged = true;
                            udsp.MinValue = ds.Value.Min;
                            udsp.MaxValue = ds.Value.Max;
                            udsp.RegenValue = ds.Value.Regen;
                        }

                    }
                }

                updateEvent.AddUpdatedDynamicStat(ds.Key.UniqueID, udsp);
            }

            //todo: handle dynamic stats can be removed (?) and added (?)

        }

        //TODO: updating content should be added via callbacks and handled other way than current.

        // public void AddAbilityStateUpdateLinear
        public void AddAbilityStateUpdate(UnitOnSceneBase u, FD.Units.Abilities.UnitAbilityState abState)
        {
            var updateEvent = GetOrCreateUnitUpdateEvent(u);

            AbilityStagePacket lastTurnAbility = null;

            if (TryGetPreviousUnitUpdateEvent(u, out var lastTurnUpdateEvent))
            {
                if (lastTurnUpdateEvent.UpdatedAbilitiesStagesDic.TryGetValue(abState.Reference.UniqueID, out lastTurnAbility))
                {

                }
            }


            var abStatePacket = new AbilityStagePacket();
            updateEvent.AddUpdatedAbilityStage(abState.Reference.UniqueID, abStatePacket);
            abStatePacket.AbilityID = abState.Reference.UniqueID;
            abStatePacket.Target = abState.Target.ToPacketContent();
            abStatePacket.TargetChanged = lastTurnAbility == null || !abState.Target.Equals(lastTurnAbility.Target);
            abStatePacket.Stage = abState.CurrentStage;

        }


        //public void AddAbilityStateUpdate(UnitOnSceneBase u)
        //{
        //    //var e = new AbilityStagePacket();
        //    //var abID = abObj.UniqueID;
        //    //e.AbilityID = abID;
        //    //var state = u.AbilitiesCollection.Collection[abID];
        //    //e.Stage = state.CurrentStage;
        //    //e.Target = NetworkUnitTarget.FromTarget(state.Target);

        //    //_updatedAbilityStatesEvents.Add(e);

        //    var abilities = u.AbilitiesCollection.Collection;
        //    var lastTurnAbilities = GetOrCreateUnitUpdateEvent(u).UpdatedAbilitiesStagesDic;

        //    foreach (var ab in abilities)
        //    {
        //        var abKey = ab.Key;
        //        var abValue = ab.Value;
        //        var abStatePacket = new AbilityStagePacket();
        //        abStatePacket.AbilityID = abKey;
        //        abStatePacket.Stage = abValue.CurrentStage;
        //        abStatePacket.TargetChanged = !lastTurnAbilities.TryGetValue(abKey, out var lastAb)
        //            && !lastAb.Target.Equals(abValue.Target);

        //        if (abStatePacket.TargetChanged)
        //        {
        //            abStatePacket.Target = NetworkUnitTarget.FromTarget(abValue.Target);
        //        }
        //    }
        //}

        public void AddDeath(UnitOnSceneBase u)
        {
            var e = new UnitDiedEvent
            {
                UnitUniqueID = u.UniqueID,
                KillerUnitUniqueID = u.Lasthitter.UniqueID
            };

            _deathesEvents.Add(e);
        }


        public void Clear()
        {
            _newPlayersResources.Clear();
            _deathesEvents.Clear();

            _lastTurnUpdatedUnits.Clear();
            var tmp = _lastTurnUpdatedUnits;
            _lastTurnUpdatedUnits = _updatedUnits;
            _updatedUnits = tmp;
        }
    }
}