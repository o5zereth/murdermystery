using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Interactables.Interobjects.DoorUtils;
using MEC;
using MurderMystery.API;
using MurderMystery.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Broadcast;

namespace MurderMystery
{
    public class EventHandlers
    {
        internal EventHandlers() { }

        /// Primary Events
        internal void WaitingForPlayers()
        {
            Log.Debug("WaitingForPlayers Primary Event called.", MurderMystery.Singleton.Debug);

            MurderMystery.GamemodeManager.WaitingForPlayers = true;
            MurderMystery.GamemodeManager.EnableSecondary();
        }
        internal void RoundStarted()
        {
            Log.Debug("RoundStarted Primary Event called.", MurderMystery.Singleton.Debug);

            if (MurderMystery.Singleton.Config.RequireRoundRestart && !MurderMystery.GamemodeManager.WaitingForPlayers) { Log.Debug("Round has not restarted, the gamemode will not begin.", MurderMystery.Singleton.Debug); return; } else { Log.Debug("Gamemode is starting..."); }

            if (MMPlayer.List.Count < 8 && !MurderMystery.Singleton.Debug)
            {
                Map.Broadcast(10, "<size=30>There must be atleast 8 players to start the gamemode!</size>", BroadcastFlags.Monospaced);
                MurderMystery.GamemodeManager.EnableGamemode(false);
                return;
            }

            MurderMystery.GamemodeManager.Started = true;

            MurderMystery.CoroutineManager.RunServerCoroutine(SetupEvent());
            MurderMystery.CoroutineManager.RunServerCoroutine(MMPlayer.SetupPlayers());
            MurderMystery.CoroutineManager.RunServerCoroutine(RoundTimer(MurderMystery.Singleton.Config.RoundTime));
        }
        internal void RoundEnded(RoundEndedEventArgs ev)
        {
            Log.Debug("RoundEnded Primary Event called.", MurderMystery.Singleton.Debug);

            if (!MurderMystery.GamemodeManager.Started) { return; }

            MurderMystery.GamemodeManager.EnableSecondary(false);

            MurderMystery.GamemodeManager.Ended = true;
        }
        internal void RestartingRound()
        {
            Log.Debug("RestartingRound Primary Event called.", MurderMystery.Singleton.Debug);

            if (!MurderMystery.GamemodeManager.Started) { return; }

            MurderMystery.GamemodeManager.EnableGamemode(false);
        }

        // Secondary Events
        internal void Joined(VerifiedEventArgs ev)
        {
            MMPlayer ply = MMPlayer.Get(ev.Player);

            // Send the player a message showing the gamemode is enabled for the current round depending on gamemode status.
            if (MurderMystery.GamemodeManager.Enabled && !MurderMystery.GamemodeManager.Started)
            {
                ply.Player.Broadcast(10, $"<size=30>Murder Mystery gamemode is enabled for this round.</size>\n<size=20>{MurderMystery.VersionStr}</size>");
            }
            else if (MurderMystery.GamemodeManager.Started)
            {
                ply.Player.Broadcast(10, $"<size=30>Murder Mystery gamemode is currently in progress.</size>\n<size=20>{MurderMystery.VersionStr}</size>");
                ply.SoftlySetRole(MMRole.Spectator);
            }
            else if (MurderMystery.GamemodeManager.Ended)
            {
                ply.Player.Broadcast(10, $"<size=30>Murder Mystery gamemode has ended.</size>\n<size=20>{MurderMystery.VersionStr}</size>");
            }
        }
        internal void EndingRound(EndingRoundEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.IsRoundEnded = false;

            // Make sure the round doesn't end before setting everyones roles.
            if (Round.ElapsedTime.TotalMilliseconds <= 5000) { return; }

            // Check the remaining roles to determine if the game should end.
            if (MMPlayer.List.InnocentsCount() + MMPlayer.List.DetectivesCount() > 0 && MMPlayer.List.MurderersCount() == 0) // End the gamemode if there are no murderers left.
            {
                Map.ClearBroadcasts();
                Map.Broadcast(30, "\n<size=80><color=#00ff00><b>Innocents win</b></color></size>\n<size=30>All murderers have been defeated.</size>");
                ev.IsAllowed = true;
                ev.IsRoundEnded = true;
            }
            if (MMPlayer.List.InnocentsCount() + MMPlayer.List.DetectivesCount() == 0 && MMPlayer.List.MurderersCount() > 0) // End the gamemode if there are no innocents or detectives left.
            {
                Map.ClearBroadcasts();
                Map.Broadcast(30, "\n<size=80><color=#ff0000><b>Murderers win</b></color></size>\n<size=30>All innocents have been defeated.</size>");
                ev.IsAllowed = true;
                ev.IsRoundEnded = true;
            }
            if (MMPlayer.List.InnocentsCount() + MMPlayer.List.DetectivesCount() == 0 && MMPlayer.List.MurderersCount() == 0) // End the gamemode if there are no roles left alive.
            {
                Map.ClearBroadcasts();
                Map.Broadcast(30, "\n<size=80><color=#7f7f7f><b>Stalemate</b></color></size>\n<size=30>All players have been killed.</size>");
                ev.IsAllowed = true;
                ev.IsRoundEnded = true;
            }
            if (MurderMystery.GamemodeManager.ForceRoundEnd) // End the gamemode forcefully if prompted.
            {
                Map.ClearBroadcasts();
                Map.Broadcast(30, "\n<size=80><color=#7f7f7f><b>Stalemate</b></color></size>\n<size=30>Round was force ended by an administrator.</size>");
                ev.IsAllowed = true;
                ev.IsRoundEnded = true;
            }
            if (MurderMystery.GamemodeManager.RoundEndTime <= 0f) // End the gamemode if the murderers run out of time.
            {
                Map.ClearBroadcasts();
                Map.Broadcast(30, "\n<size=80><color=#00ff00><b>Innocents win</b></color></size>\n<size=30>Murderers ran out of time and lost.</size>");
                ev.IsAllowed = true;
                ev.IsRoundEnded = true;
            }
        }
        internal void Died(DiedEventArgs ev)
        {
            MMPlayer target = MMPlayer.Get(ev.Target);
            MMPlayer killer = MMPlayer.Get(ev.Killer);

            switch (target.Role, killer.Role)
            {
                case (MMRole.Innocent, MMRole.Detective):
                    killer.Player.EnableEffect<Blinded>(30);
                    killer.Player.DropItem(killer.Player.Inventory.items.FirstOrDefault(x => x.id == ItemType.GunCOM15));
                    killer.Player.DropItem(killer.Player.Inventory.items.FirstOrDefault(x => x.id == ItemType.KeycardNTFCommander));
                    MurderMystery.CoroutineManager.RunPlayerCoroutine(killer.DetectiveGunLossTimer(30f), killer);
                    killer.SoftlySetRole(MMRole.Innocent);
                    if (GetNearestDetectiveWeapon(killer.Player.Position, out Pickup item))
                    {
                        MurderMystery.CoroutineManager.RunPickupCoroutine(DetectiveWeaponPickup(item), item);
                    }
                    else
                    {
                        Log.Error("Error finding detective weapon on losing his weapon!");
                    }
                    break;
                case (MMRole.Detective, _):
                    if (GetNearestDetectiveWeapon(target.Player.ReferenceHub.characterClassManager.NetworkDeathPosition, out Pickup item2))
                    {
                        MurderMystery.CoroutineManager.RunPickupCoroutine(DetectiveWeaponPickup(item2), item2);
                    }
                    else
                    {
                        Log.Error("Error finding detective weapon on his death!");
                    }
                    break;
            }

            // Set the killed player to spectator after using the role check.
            target.SoftlySetRole(MMRole.Spectator);

            bool GetNearestDetectiveWeapon(Vector3 pos, out Pickup pickup)
            {
                try
                {
                    pickup = Object.FindObjectsOfType<Pickup>().Where(x => x.ItemId == ItemType.GunCOM15 && !MurderMystery.CoroutineManager.CoroutinedPickups.Contains(x) && x.weaponMods.Barrel == 0).OrderBy(x => Vector3.Distance(x.Networkposition, pos)).First();
                    return true;
                }
                catch
                {
                    pickup = null;
                    return false;
                }
            }
        }
        internal void SpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            // Edit the ragdolls name to contain the role the player was before they died.
            ev.PlayerNickname = $"[{MMPlayer.Get(Player.Get(ev.PlayerId)).Role.GetRoleAsColoredString()}] " + ev.PlayerNickname;
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
                    return;
                
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
            // Don't allow locker interaction unless it is a medkit locker.
            if (ev.Chamber.name != "Medkit")
            {
                ev.IsAllowed = false;
            }
        }
        internal void OpeningGenerator(OpeningGeneratorEventArgs ev)
        {
            // Don't allow opening generators.
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
        internal void ChangingRole(ChangingRoleEventArgs ev)
        {
            MMPlayer ply = MMPlayer.Get(ev.Player);

            if (Round.ElapsedTime.TotalMilliseconds <= 5000 || !MurderMystery.GamemodeManager.Started) { return; }

            Timing.CallDelayed(1f, () =>
            {
                if (ev.NewRole == RoleType.Spectator && ply.Role != MMRole.Spectator)
                {
                    ply.SoftlySetRole(MMRole.Spectator);
                }
                else if (ply.Role != MMRole.Spectator)
                {
                    ply.SoftlySetRole(MMRole.Spectator);
                    ply.Player.Broadcast(15, "<size=30>Your role has been set to spectator due to a role change,\nif this is unexpected then please contact the developer.</size>");
                }
            });
        }

        // Coroutines
        internal IEnumerator<float> SetupEvent()
        {
            Log.Debug("SetupEvent called.", MurderMystery.Singleton.Debug);

            // Update doors to lock most rooms in HCZ.
            for (int i = 0; i < Map.Doors.Count; i++)
            {
                Map.Doors[i].NetworkTargetState = true;

                switch (Map.Doors[i].RequiredPermissions.RequiredPermissions, Map.Doors[i].GetNametag())
                {
                    // No permssion doors.
                    case (KeycardPermissions.None, _):
                        Map.Doors[i].NetworkTargetState = false;
                        continue;
                    
                    // Checkpoints.
                    case (KeycardPermissions.Checkpoints, _):
                        Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
                        Map.Doors[i].NetworkTargetState = false;
                        continue;

                    // Tagged doors.
                    case (_, "HCZ_ARMORY"):
                        Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
                        Map.Doors[i].NetworkTargetState = false;
                        continue;
                    case (_, "049_ARMORY"):
                        Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
                        Map.Doors[i].NetworkTargetState = false;
                        continue;
                    case (_, "096"):
                        Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
                        Map.Doors[i].NetworkTargetState = false;
                        continue;
                    case (_, "106_PRIMARY"):
                        Map.Doors[i].ServerChangeLock(DoorLockReason.Warhead, true);
                        Map.Doors[i].NetworkTargetState = true;
                        continue;
                    case (_, "106_SECONDARY"):
                        Map.Doors[i].ServerChangeLock(DoorLockReason.Warhead, true);
                        Map.Doors[i].NetworkTargetState = true;
                        continue;
                    case (_, "106_BOTTOM"):
                        Map.Doors[i].ServerChangeLock(DoorLockReason.Warhead, true);
                        Map.Doors[i].NetworkTargetState = true;
                        continue;
                    case (_, "HID"):
                        Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
                        Map.Doors[i].NetworkTargetState = false;
                        continue;

                    // Any other door.
                    default:
                        Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
                        Map.Doors[i].NetworkTargetState = true;
                        continue;
                }
            }

            // Wait for item spawns from door updates.
            yield return Timing.WaitForSeconds(0.5f);

            // Remove all items spawned.
            foreach (Pickup item in Object.FindObjectsOfType<Pickup>())
            {
                item.Delete();
            }

            // Lock elevators to light containment to prevent access.
            for (int i = 0; i < Map.Lifts.Count; i++)
            {
                if (Map.Lifts[i].Type() != ElevatorType.Scp049)
                {
                    Map.Lifts[i].Network_locked = true;
                }
            }
        }
        internal IEnumerator<float> DetectiveWeaponPickup(Pickup item)
        {
            while (true)
            {
                //CheckOutOfBounds();

                foreach (MMPlayer ply in MMPlayer.List)
                {
                    if (ply.Role != MMRole.Innocent || ply.Player.Role == RoleType.Spectator)
                    {
                        continue;
                    }

                    if (Vector3.Distance(ply.Player.Position, item.Networkposition) <= 1.7f)
                    {
                        // MAKE RAYCAST CHECK TO MAKE SURE THEY DONT GRAB IT THROUGH WALLS. (dunno how tf to do it so for now i leave it, :troll: will come back to later)

                        if (ply.Player.Inventory.items.Count > 6)
                        {
                            ply.Player.ShowHint("<size=25><color=#ff0000>You must have atleast 2 available slots to pickup the detectives weapon.</color></size>", 0.12f);
                        }
                        else
                        {
                            if (ply.DetectiveGunLossCooldown <= 0f)
                            {
                                ply.SoftlySetRole(MMRole.Detective);
                                RemoveNearestDetectiveCard();
                                ply.Player.AddItem(ItemType.KeycardNTFCommander);
                                ply.Player.AddItem(new Inventory.SyncItemInfo() { durability = item.durability, id = ItemType.GunCOM15, modBarrel = 0, modOther = 0, modSight = 0 });
                                MurderMystery.CoroutineManager.CoroutinedPickups.Remove(item);
                                item.Delete();

                                ply.Player.ShowHint("<b><size=25><color=#0000ff>You have picked up the detectives weapon.</color></size></b>", 5f);

                                yield break;
                            }
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }

            void RemoveNearestDetectiveCard()
            {
                // Gets the nearest commander keycard to the gun being picked up and deletes it.
                try
                {
                    Object.FindObjectsOfType<Pickup>().Where(x => x.ItemId == ItemType.KeycardNTFCommander).OrderBy(x => Vector3.Distance(x.transform.position, item.transform.position)).First().Delete();
                }
                catch
                {
                    return;
                }
            }

            /*void CheckOutOfBounds()
            {

            }*/
        }
        internal IEnumerator<float> RoundTimer(float roundTime)
        {
            MurderMystery.GamemodeManager.RoundEndTime = roundTime;

            while (true)
            {
                yield return Timing.WaitForSeconds(1f);
                MurderMystery.GamemodeManager.RoundEndTime -= 1f;
            }
        }
    }
}