using Exiled.API.Features;
using MEC;
using Exiled.Events.EventArgs;
using System.Linq;
using System.Collections.Generic;
using Handlers = Exiled.Events.Handlers;
using MurderMystery.Extensions;
using Interactables.Interobjects.DoorUtils;
using MurderMystery.Utils;
using MurderMystery.Enums;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using static Broadcast;
using UnityEngine;

namespace MurderMystery
{
    public class EventHandlers
    {
        public MurderMystery Plugin => MurderMystery.Singleton;
        public GamemodeStatus GamemodeStatus => MurderMystery.GamemodeStatus;
        internal EventHandlers() { }

        internal List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

        internal bool ForceRoundEnd = false;

        private void WaitingForPlayers()
        {
            Log.Debug("WaitingForPlayers Primary Event called.", Plugin.Debug);
            GamemodeStatus.WaitingForPlayers = true;
            EnableSecondary();
        }
        private void RoundStarted()
        {
            Log.Debug("RoundStarted Primary Event called.", Plugin.Debug);

            if (Plugin.Config.RequireRoundRestart && !GamemodeStatus.WaitingForPlayers) { Log.Debug("Round has not restarted, the gamemode will not begin.", Plugin.Debug); return; } else { Log.Debug("Gamemode is starting..."); }

            if (MMPlayer.List.Count() < 8 && !Plugin.Debug)
            {
                Map.Broadcast(10, "<size=30>There must be atleast 8 players to start the gamemode!</size>", BroadcastFlags.Monospaced);
            }

            GamemodeStatus.Started = true;

            Coroutines.RunAndAdd(SetupEvent()).RunAndAdd(MMPlayer.SetupPlayers());
        }
        private void RoundEnded(RoundEndedEventArgs ev)
        {
            Log.Debug("RoundEnded Primary Event called.", Plugin.Debug);

            if (!GamemodeStatus.Started) { return; }

            EnableSecondary(false);
            Coroutines.KillAll();
            Coroutines = new List<CoroutineHandle>();

            GamemodeStatus.Ended = true;
        }
        private void RestartingRound()
        {
            Log.Debug("RestartingRound Primary Event called.", Plugin.Debug);

            if (!GamemodeStatus.Started) { return; }

            EnableGamemode(false);
        }

        private void Joined(JoinedEventArgs ev)
        {
            MMPlayer ply = MMPlayer.Get(ev.Player);

            if (GamemodeStatus.Enabled && !GamemodeStatus.Started)
            {
                ply.Player.Broadcast(10, $"<size=30>Murder Mystery gamemode is enabled for this round.</size>\n<size=20>{MurderMystery.VersionStr}</size>");
            }
            else if (GamemodeStatus.Started)
            {
                ply.Player.Broadcast(10, $"<size=30>Murder Mystery gamemode is currently in progress.</size>\n<size=20>{MurderMystery.VersionStr}</size>");
                ply.SoftlySetRole(MMRole.Spectator);
            }
            else if (GamemodeStatus.Ended)
            {
                ply.Player.Broadcast(10, $"<size=30>Murder Mystery gamemode has ended.</size>\n<size=20>{MurderMystery.VersionStr}</size>");
            }
        }
        private void EndingRound(EndingRoundEventArgs ev)
        {
            ev.IsAllowed = false;

            if (Round.ElapsedTime.TotalMilliseconds <= 5000) { return; }

            if (MMPlayer.List.Innocents().Count() + MMPlayer.List.Detectives().Count() > 0 && MMPlayer.List.Murderers().Count() == 0)
            {
                ev.IsAllowed = true;
                Map.Broadcast(15, "<size=30><color=#00ff00>Innocents won!</color></size>");
            }
            else if (MMPlayer.List.Innocents().Count() + MMPlayer.List.Detectives().Count() == 0 && MMPlayer.List.Murderers().Count() > 0)
            {
                ev.IsAllowed = true;
                Map.Broadcast(15, "<size=30><color=#ff0000>Murderers won!</color></size>");
            }
            else if (MMPlayer.List.Innocents().Count() + MMPlayer.List.Detectives().Count() == 0 && MMPlayer.List.Murderers().Count() == 0)
            {
                ev.IsAllowed = true;
                Map.Broadcast(15, "<size=30><color=#7f7f7f>Draw, everyone loses!</color></size>\n<size=20>also, how did this happen?</size>");
            }
            else if (ForceRoundEnd)
            {
                ev.IsAllowed = true;
                Map.Broadcast(15, "<size=30><color=#ffffff>Round has been force ended by an admin.</color></size>");
            }
        }
        private void Dying(DyingEventArgs ev)
        {
            MMPlayer target = MMPlayer.Get(ev.Target);
            target.RoleBeforeDeath = target.Role;
        }
        private void Died(DiedEventArgs ev)
        {
            MMPlayer target = MMPlayer.Get(ev.Target);
            MMPlayer killer = MMPlayer.Get(ev.Killer);

            target.SoftlySetRole(MMRole.Spectator);

            switch (target.RoleBeforeDeath, killer.Role)
            {
                case (MMRole.Innocent, MMRole.Detective):
                    killer.Player.EnableEffect<Flashed>(30);
                    killer.Player.DropItem(killer.Player.Inventory.items.FirstOrDefault(x => x.id == ItemType.GunCOM15));
                    break;
            }
        }
        private void SpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            ev.PlayerNickname = $"[{MMPlayer.Get(ev.Owner).RoleBeforeDeath.GetRoleAsColoredString()}] " + ev.PlayerNickname;
        }
        private void PickingUpItem(PickingUpItemEventArgs ev)
        {
            MMPlayer ply = MMPlayer.Get(ev.Player);

            switch (ply.Role, ev.Pickup.ItemId)
            {
                case (MMRole.Innocent, ItemType.KeycardFacilityManager):
                case (MMRole.Innocent, ItemType.KeycardNTFCommander):
                    ev.IsAllowed = false;
                    return;
                case (MMRole.Innocent, ItemType.GunCOM15):
                    if (ev.Pickup.weaponMods.Barrel != (int)BarrelType.Suppressor)
                    {
                        if (ply.Player.Inventory.items.Count > 6)
                        {
                            ply.Player.Broadcast(5, "<size=25><color=#ff0000>You must have atleast 2 available slots to pickup the detectives weapon.</color></size>", BroadcastFlags.Monospaced);
                            ev.IsAllowed = false;
                            return;
                        }
                        else
                        {
                            ply.SoftlySetRole(MMRole.Detective);
                            RemoveNearestDetectiveCard();
                            ply.Player.AddItem(ItemType.KeycardNTFCommander);
                            return;
                        }
                    }
                    else
                    {
                        ev.IsAllowed = false;
                        ply.Player.Broadcast(3, "<size=30><color=#ff0000>You can't pickup a murderers weapon.</color></size>", BroadcastFlags.Monospaced);
                    }
                    return;
                case (MMRole.Innocent, ItemType.SCP268):
                    ev.IsAllowed = false;
                    return;


                case (MMRole.Murderer, ItemType.KeycardFacilityManager):
                case (MMRole.Murderer, ItemType.KeycardNTFCommander):
                    ev.IsAllowed = false;
                    return;
                case (MMRole.Murderer, ItemType.GunCOM15):
                    ev.IsAllowed = false;
                    return;
                case (MMRole.Murderer, ItemType.SCP268):
                    ev.IsAllowed = false;
                    return;

                case (MMRole.Detective, ItemType.KeycardFacilityManager):
                case (MMRole.Detective, ItemType.KeycardNTFCommander):
                    ev.IsAllowed = false;
                    return;
                case (MMRole.Detective, ItemType.GunCOM15):
                    ev.IsAllowed = false;
                    return;
                case (MMRole.Detective, ItemType.SCP268):
                    ev.IsAllowed = false;
                    return;

                default:
                    return;
            }

            void RemoveNearestDetectiveCard()
            {
                Pickup item = ev.Pickup;

                UnityEngine.Object.FindObjectsOfType<Pickup>().Where(x => x.ItemId == ItemType.KeycardNTFCommander).OrderBy(x => Vector3.Distance(x.transform.position, item.transform.position)).First().Delete();
            }
        }
        private void DroppingItem(DroppingItemEventArgs ev)
        {
            MMPlayer Player = MMPlayer.Get(ev.Player);

            switch (Player.Role, ev.Item.id)
            {
                case (MMRole.Murderer, ItemType.KeycardFacilityManager):
                    ev.IsAllowed = false;
                    return;
                case (MMRole.Murderer, ItemType.GunCOM15):
                    ev.IsAllowed = false;
                    return;
                case (MMRole.Murderer, ItemType.SCP268):
                    ev.IsAllowed = false;
                    return;

                case (MMRole.Detective, ItemType.KeycardNTFCommander):
                    ev.IsAllowed = false;
                    return;
                case (MMRole.Detective, ItemType.GunCOM15):
                    ev.IsAllowed = false;
                    return;
            }
        }
        private void InteractingLocker(InteractingLockerEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        private void Shooting(ShootingEventArgs ev)
        {
            MMPlayer shooter = MMPlayer.Get(ev.Shooter);
            MMPlayer target;

            if (Player.Get(ev.Target) != null)
            {
                target = MMPlayer.Get(Player.Get(ev.Target));
            }
            else { return; }

            switch (shooter.Role, target.Role)
            {
                case (MMRole.Murderer, MMRole.Murderer):
                    ev.IsAllowed = false;
                    shooter.Player.ShowHint("You cannot shoot a fellow murderer.");
                    return;
                case (MMRole.Detective, MMRole.Detective):
                    ev.IsAllowed = false;
                    shooter.Player.ShowHint("You cannot shoot a fellow detective.");
                    return;
            }
        }
        private void TriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            ev.IsTriggerable = false;
        }
        private void RespawningTeam(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }


        internal void EnableGamemode(bool enable = true)
        {
            Log.Debug($"EnableGamemode Primary Function called. {(enable ? "[Enabling]" : "[Disabling]" )}", Plugin.Debug);
            if (enable)
            {
                if (GamemodeStatus.Enabled) { Log.Debug("EnableGamemode: Gamemode is already enabled.", Plugin.Debug); return; }

                EnablePrimary();

                GamemodeStatus.Enabled = true;
            }
            else
            {
                if (!GamemodeStatus.Enabled) { Log.Debug("EnableGamemode: Gamemode is already disabled.", Plugin.Debug); return; }

                EnablePrimary(false);

                if (!GamemodeStatus.Ended && GamemodeStatus.SecondaryEventsEnabled) { EnableSecondary(false); Coroutines.KillAll(); Coroutines = new List<CoroutineHandle>(); }

                GamemodeStatus.Enabled = false;
                GamemodeStatus.Ended = false;
                GamemodeStatus.Started = false;
                GamemodeStatus.WaitingForPlayers = false;
            }
        }

        internal void EnablePrimary(bool enable = true)
        {
            Log.Debug($"EnablePrimary Primary Function called. {(enable ? "[Enabling]" : "[Disabling]")}", Plugin.Debug);
            if (enable)
            {
                if (GamemodeStatus.PrimaryEventsEnabled) { Log.Debug("EnablePrimary: Primary events are already enabled.", Plugin.Debug); return; }

                Handlers.Server.WaitingForPlayers += WaitingForPlayers;
                Handlers.Server.RoundStarted += RoundStarted;
                Handlers.Server.RoundEnded += RoundEnded;
                Handlers.Server.RestartingRound += RestartingRound;

                GamemodeStatus.PrimaryEventsEnabled = true;
            }
            else
            {
                if (!GamemodeStatus.PrimaryEventsEnabled) { Log.Debug("EnablePrimary: Primary events are already disabled.", Plugin.Debug); return; }

                Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
                Handlers.Server.RoundStarted -= RoundStarted;
                Handlers.Server.RoundEnded -= RoundEnded;
                Handlers.Server.RestartingRound -= RestartingRound;

                GamemodeStatus.PrimaryEventsEnabled = false;
            }
        }

        internal void EnableSecondary(bool enable = true)
        {
            Log.Debug($"EnableSecondary Primary Function called. {(enable ? "[Enabling]" : "[Disabling]")}", Plugin.Debug);
            if (enable)
            {
                if (GamemodeStatus.SecondaryEventsEnabled) { Log.Debug("EnableSecondary: Secondary events are already enabled.", Plugin.Debug); return; }

                Handlers.Player.Joined += Joined;
                Handlers.Server.EndingRound += EndingRound;
                Handlers.Player.Dying += Dying;
                Handlers.Player.Died += Died;
                Handlers.Player.SpawningRagdoll += SpawningRagdoll;
                Handlers.Player.PickingUpItem += PickingUpItem;
                Handlers.Player.DroppingItem += DroppingItem;
                Handlers.Player.InteractingLocker += InteractingLocker;
                Handlers.Player.Shooting += Shooting;
                Handlers.Player.TriggeringTesla += TriggeringTesla;
                Handlers.Server.RespawningTeam += RespawningTeam;

                GamemodeStatus.SecondaryEventsEnabled = true;
            }
            else
            {
                if (!GamemodeStatus.SecondaryEventsEnabled) { Log.Debug("EnableSecondary: Secondary events are already disabled.", Plugin.Debug); return; }

                Handlers.Player.Joined -= Joined;
                Handlers.Server.EndingRound -= EndingRound;
                Handlers.Player.Dying -= Dying;
                Handlers.Player.Died -= Died;
                Handlers.Player.SpawningRagdoll -= SpawningRagdoll;
                Handlers.Player.PickingUpItem -= PickingUpItem;
                Handlers.Player.DroppingItem -= DroppingItem;
                Handlers.Player.InteractingLocker -= InteractingLocker;
                Handlers.Player.Shooting -= Shooting;
                Handlers.Player.TriggeringTesla -= TriggeringTesla;
                Handlers.Server.RespawningTeam -= RespawningTeam;

                GamemodeStatus.SecondaryEventsEnabled = false;
            }
        }

        internal IEnumerator<float> SetupEvent()
        {
            Log.Debug("SetupEvent called.", MurderMystery.Singleton.Debug);

            foreach (DoorVariant door in Map.Doors)
            {
                switch (door.RequiredPermissions.RequiredPermissions)
                {
                    case KeycardPermissions.None:
                        continue;
                    case KeycardPermissions.Checkpoints:
                        door.ServerChangeLock(DoorLockReason.AdminCommand, true);
                        door.NetworkTargetState = false;
                        continue;
                    default:
                        door.ServerChangeLock(DoorLockReason.AdminCommand, true);
                        door.NetworkTargetState = true;
                        continue;
                }
            }

            yield return Timing.WaitForSeconds(0.2f);

            foreach (Pickup item in UnityEngine.Object.FindObjectsOfType<Pickup>())
            {
                item.Delete();
            }

            foreach (Lift lift in Map.Lifts)
            {
                if (lift.Type() == ElevatorType.LczA || lift.Type() == ElevatorType.LczB)
                {
                    lift.Network_locked = true;
                }
            }
        }
    }
}