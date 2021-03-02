using Exiled.API.Features;
using MEC;
using Exiled.Events.EventArgs;
using System.Linq;
using System.Collections.Generic;
using MurderMystery.Extensions;
using Interactables.Interobjects.DoorUtils;
using MurderMystery.Enums;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using static Broadcast;
using UnityEngine;
using MurderMystery.API;

namespace MurderMystery
{
    public class EventHandlers
    {
        public MurderMystery Plugin => MurderMystery.Singleton;
        public GamemodeManager GamemodeManager => MurderMystery.GamemodeManager;
        public CoroutineManager CoroutineManager => MurderMystery.CoroutineManager;
        internal EventHandlers() { }

        /// Primary Events
        internal void WaitingForPlayers()
        {
            Log.Debug("WaitingForPlayers Primary Event called.", Plugin.Debug);

            GamemodeManager.WaitingForPlayers = true;
            GamemodeManager.EnableSecondary();
        }
        internal void RoundStarted()
        {
            Log.Debug("RoundStarted Primary Event called.", Plugin.Debug);

            if (Plugin.Config.RequireRoundRestart && !GamemodeManager.WaitingForPlayers) { Log.Debug("Round has not restarted, the gamemode will not begin.", Plugin.Debug); return; } else { Log.Debug("Gamemode is starting..."); }

            if (MMPlayer.List.Count() < 8 && !Plugin.Debug)
            {
                Map.Broadcast(10, "<size=30>There must be atleast 8 players to start the gamemode!</size>", BroadcastFlags.Monospaced);
            }

            GamemodeManager.Started = true;

            CoroutineManager.ServerCoroutines.RunAndAdd(SetupEvent()).RunAndAdd(MMPlayer.SetupPlayers());
        }
        internal void RoundEnded(RoundEndedEventArgs ev)
        {
            Log.Debug("RoundEnded Primary Event called.", Plugin.Debug);

            if (!GamemodeManager.Started) { return; }

            GamemodeManager.EnableSecondary(false);

            CoroutineManager.ServerCoroutines.KillAll();
            CoroutineManager.ServerCoroutines.Clear();

            CoroutineManager.PlayerCoroutines.KillAll();
            CoroutineManager.PlayerCoroutines.Clear();

            GamemodeManager.Ended = true;
        }
        internal void RestartingRound()
        {
            Log.Debug("RestartingRound Primary Event called.", Plugin.Debug);

            if (!GamemodeManager.Started) { return; }

            GamemodeManager.EnableGamemode(false);
        }

        // Secondary Events
        internal void Joined(VerifiedEventArgs ev)
        {
            MMPlayer ply = MMPlayer.Get(ev.Player);

            // Send the player a message showing the gamemode is enabled for the current round depending on gamemode status.
            if (GamemodeManager.Enabled && !GamemodeManager.Started)
            {
                ply.Player.Broadcast(10, $"<size=30>Murder Mystery gamemode is enabled for this round.</size>\n<size=20>{MurderMystery.VersionStr}</size>");
            }
            else if (GamemodeManager.Started)
            {
                ply.Player.Broadcast(10, $"<size=30>Murder Mystery gamemode is currently in progress.</size>\n<size=20>{MurderMystery.VersionStr}</size>");
                ply.SoftlySetRole(MMRole.Spectator);
            }
            else if (GamemodeManager.Ended)
            {
                ply.Player.Broadcast(10, $"<size=30>Murder Mystery gamemode has ended.</size>\n<size=20>{MurderMystery.VersionStr}</size>");
            }
        }
        internal void EndingRound(EndingRoundEventArgs ev)
        {
            ev.IsAllowed = false;

            // Make sure the round doesn't end before setting everyones roles.
            if (Round.ElapsedTime.TotalMilliseconds <= 5000) { return; }

            // Check the remaining roles to determine if the game should end.
            if (MMPlayer.List.Innocents().Count() + MMPlayer.List.Detectives().Count() > 0 && MMPlayer.List.Murderers().Count() == 0)
            {
                ev.IsAllowed = true;
                Map.Broadcast(15, "<size=40><color=#00ff00>Innocents won!</color></size>");
            }
            else if (MMPlayer.List.Innocents().Count() + MMPlayer.List.Detectives().Count() == 0 && MMPlayer.List.Murderers().Count() > 0)
            {
                ev.IsAllowed = true;
                Map.Broadcast(15, "<size=40><color=#ff0000>Murderers won!</color></size>");
            }
            else if (MMPlayer.List.Innocents().Count() + MMPlayer.List.Detectives().Count() == 0 && MMPlayer.List.Murderers().Count() == 0)
            {
                ev.IsAllowed = true;
                Map.Broadcast(15, "<size=40><color=#7f7f7f>Draw, everyone loses!</color></size>\n<size=20>also, how did this happen?</size>");
            }
            else if (GamemodeManager.ForceRoundEnd) // End the gamemode forcefully if prompted. (Command should be added later)
            {
                ev.IsAllowed = true;
                Map.Broadcast(15, "<size=40><color=#ffffff>Round has been force ended by an admin.</color></size>");
            }
        }
        internal void Died(DiedEventArgs ev)
        {
            MMPlayer target = MMPlayer.Get(ev.Target);
            MMPlayer killer = MMPlayer.Get(ev.Killer);

            // If the detective killed an innocent, respond accordingly.
            switch (target.Role, killer.Role)
            {
                case (MMRole.Innocent, MMRole.Detective):
                    killer.Player.EnableEffect<Blinded>(15);
                    killer.Player.DropItem(killer.Player.Inventory.items.FirstOrDefault(x => x.id == ItemType.GunCOM15));
                    killer.Player.DropItem(killer.Player.Inventory.items.FirstOrDefault(x => x.id == ItemType.KeycardNTFCommander));
                    killer.SoftlySetRole(MMRole.Innocent);
                    break;
            }

            // Set the killed player to spectator after using the role check.
            target.SoftlySetRole(MMRole.Spectator);
        }
        internal void SpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            // Edit the ragdolls name to contain the role the player was before they died.
            ev.PlayerNickname = $"[{MMPlayer.Get(ev.Owner).Role.GetRoleAsColoredString()}] " + ev.PlayerNickname;
        }
        internal void PickingUpItem(PickingUpItemEventArgs ev)
        {
            MMPlayer ply = MMPlayer.Get(ev.Player);

            // Make sure players aren't picking up items they shouldn't have access to.
            switch (ply.Role, ev.Pickup.ItemId)
            {
                // Any
                case (_, ItemType.KeycardFacilityManager):
                case (_, ItemType.KeycardNTFCommander):
                case (_, ItemType.SCP268):
                    ev.IsAllowed = false;
                    break;
                
                // Innocents
                case (MMRole.Innocent, ItemType.GunCOM15):
                    if (ev.Pickup.weaponMods.Barrel != (int)BarrelType.Suppressor)
                    {
                        if (ply.Player.Inventory.items.Count > 6)
                        {
                            ply.Player.ShowHint("<size=25><color=#ff0000>You must have atleast 2 available slots to pickup the detectives weapon.</color></size>", 5);
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
                        ply.Player.ShowHint("<size=25><color=#ff0000>You can't pickup a murderers weapon.</color></size>", 3);
                    }
                    return;

                // Murderers
                case (MMRole.Murderer, ItemType.GunCOM15):
                    ev.IsAllowed = false;
                    return;

                // Detectives
                case (MMRole.Detective, ItemType.GunCOM15):
                    ev.IsAllowed = false;
                    return;

                default:
                    return;
            }

            void RemoveNearestDetectiveCard()
            {
                // Gets the nearest commander keycard to the gun being picked up and deletes it.
                Object.FindObjectsOfType<Pickup>().Where(x => x.ItemId == ItemType.KeycardNTFCommander).OrderBy(x => Vector3.Distance(x.transform.position, ev.Pickup.transform.position)).First().Delete();
            }
        }
        internal void DroppingItem(DroppingItemEventArgs ev)
        {
            MMPlayer Player = MMPlayer.Get(ev.Player);

            // Ensure special roles don't drop items they shouldn't be able to.
            switch (Player.Role, ev.Item.id)
            {
                // Murderers
                case (MMRole.Murderer, ItemType.KeycardFacilityManager):
                    ev.IsAllowed = false;
                    return;
                case (MMRole.Murderer, ItemType.GunCOM15):
                    ev.IsAllowed = false;
                    return;
                case (MMRole.Murderer, ItemType.SCP268):
                    ev.IsAllowed = false;
                    return;

                // Detectives
                case (MMRole.Detective, ItemType.KeycardNTFCommander):
                    ev.IsAllowed = false;
                    return;
                case (MMRole.Detective, ItemType.GunCOM15):
                    ev.IsAllowed = false;
                    return;
            }
        }
        internal void InteractingLocker(InteractingLockerEventArgs ev)
        {
            // Don't allow locker interaction.
            ev.IsAllowed = false;
        }
        internal void Shooting(ShootingEventArgs ev)
        {
            MMPlayer shooter = MMPlayer.Get(ev.Shooter);
            MMPlayer target;

            if (Player.Get(ev.Target) != null)
            {
                target = MMPlayer.Get(Player.Get(ev.Target));
            } else return;

            // Make sure players aren't shooting a teammate.
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
        internal void TriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            // Don't allow tesla activation.
            ev.IsTriggerable = false;
        }
        internal void RespawningTeam(RespawningTeamEventArgs ev)
        {
            // Don't allow respawn waves.
            ev.IsAllowed = false;
        }
        internal void EnteringFemurBreaker(EnteringFemurBreakerEventArgs ev)
        {
            // Don't allow femur breaker usage.
            ev.IsAllowed = false;
        }

        // Coroutines
        internal IEnumerator<float> SetupEvent()
        {
            Log.Debug("SetupEvent called.", MurderMystery.Singleton.Debug);

            // Update doors to allow access to all rooms in HCZ.
            for (int i = 0; i < Map.Doors.Count; i++)
            {
                switch (Map.Doors[i].RequiredPermissions.RequiredPermissions)
                {
                    case KeycardPermissions.None:
                        continue;
                    case KeycardPermissions.Checkpoints:
                        Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
                        Map.Doors[i].NetworkTargetState = false;
                        continue;
                    default:
                        if (Map.Doors[i].GetNametag() == "106_PRIMARY" || Map.Doors[i].GetNametag() == "106_SECONDARY" || Map.Doors[i].GetNametag() == "106_BOTTOM")
                        {
                            Map.Doors[i].ServerChangeLock(DoorLockReason.Warhead, true);
                            Map.Doors[i].NetworkTargetState = true;
                        }
                        else
                        {
                            Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
                            Map.Doors[i].NetworkTargetState = true;
                        }
                        continue;
                }
            }

            // Wait for item spawns from door updates.
            yield return Timing.WaitForSeconds(0.2f);

            // Remove all items spawned.
            foreach (Pickup item in Object.FindObjectsOfType<Pickup>())
            {
                item.Delete();
            }

            // Lock elevators to light containment to prevent access.
            foreach (Lift lift in Map.Lifts)
            {
                if (lift.Type() == ElevatorType.LczA || lift.Type() == ElevatorType.LczB)
                {
                    lift.Network_locked = true;
                }
            }
        }
        internal IEnumerator<float> DetectiveWeaponPickup(Pickup item)
        {
            while (true)
            {
                foreach (MMPlayer ply in MMPlayer.List.Innocents())
                {
                    if (Vector3.Distance(ply.Player.Position, item.Networkposition) <= 1.7f)
                    {
                        // MAKE RAYCAST CHECK TO MAKE SURE THEY DONT GRAB IT THROUGH WALLS. (dunno how tf to do it so for now i leave it, :troll: will come back to later)

                        if (ply.Player.Inventory.items.Count > 6)
                        {
                            ply.Player.ShowHint("<size=25><color=#ff0000>You must have atleast 2 available slots to pickup the detectives weapon.</color></size>", 0.12f);
                        }
                        else
                        {
                            ply.SoftlySetRole(MMRole.Detective);
                            RemoveNearestDetectiveCard();
                            ply.Player.AddItem(ItemType.KeycardNTFCommander);
                            ply.Player.AddItem(new Inventory.SyncItemInfo() { durability = item.durability, id = ItemType.GunCOM15, modBarrel = (int)BarrelType.None, modOther = (int)OtherType.None, modSight = (int) SightType.None });
                            item.Delete();

                            break;
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }

            void RemoveNearestDetectiveCard()
            {
                // Gets the nearest commander keycard to the gun being picked up and deletes it.
                Object.FindObjectsOfType<Pickup>().Where(x => x.ItemId == ItemType.KeycardNTFCommander).OrderBy(x => Vector3.Distance(x.transform.position, item.transform.position)).First().Delete();
            }
        }
    }
}