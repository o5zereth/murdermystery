using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using MEC;
using MurderMystery.Extensions;
using MurderMystery.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Handlers = Exiled.Events.Handlers;

namespace MurderMystery.API
{
    public class GamemodeManager
    {
        internal GamemodeManager() // Explicitly set
		{
			EventHandlers = new EventHandlers();
			Harmony = new Harmony("com.zereth.murdermystery");
			CoroutineManager = new CoroutineManager();
		}

        public bool Enabled { get; private set; } = false;
        public bool SecondaryEventsEnabled { get; private set; } = false;
        public bool Started { get; private set; } = false;
		public bool PlayersSetup { get; private set; } = false;
		public bool ForcingRoundEnd { get; internal set; } = false;
		public bool EquipmentGiven { get; private set; } = false;

		public EventHandlers EventHandlers { get; }
		public Harmony Harmony { get; }
		public CoroutineManager CoroutineManager { get; }

        // Primary functions.
        internal void ToggleGamemode(bool enable)
        {
            Log.Debug($"[GamemodeManager::ToggleGamemode] {(enable ? "Enabling" : "Disabling")} gamemode.", MurderMystery.Singleton.LogDebug);

            if (enable)
            {
				if (Enabled) { Log.Debug($"[GamemodeManager::ToggleGamemode] Already enabled! StackTrace: {Environment.StackTrace}", MurderMystery.Singleton.LogDebug); return; }

                Handlers.Server.WaitingForPlayers += EventHandlers.WaitingForPlayers;
				Handlers.Server.RoundStarted += EventHandlers.RoundStarted;
				Handlers.Server.RoundEnded += EventHandlers.RoundEnded;
				Handlers.Server.RestartingRound += EventHandlers.RestartingRound;

				Enabled = true;
            }
            else
            {
				if (!Enabled) { Log.Debug($"[GamemodeManager::ToggleGamemode] Already disabled! StackTrace: {Environment.StackTrace}", MurderMystery.Singleton.LogDebug); return; }

                Handlers.Server.WaitingForPlayers -= EventHandlers.WaitingForPlayers;
				Handlers.Server.RoundStarted -= EventHandlers.RoundStarted;
				Handlers.Server.RoundEnded -= EventHandlers.RoundEnded;
				Handlers.Server.RestartingRound -= EventHandlers.RestartingRound;

				if (SecondaryEventsEnabled) { ToggleSecondaryEvents(false); MMPlayer.ClearList(); }

				Timing.CallDelayed(1f, () =>
				{
					if (CustomInfoPatch.Singleton.Patched) { CustomInfoPatch.Singleton.Patch(false); }
					if (RoundStartPatch.Singleton.Patched) { RoundStartPatch.Singleton.Patch(false); }
					if (RoundSummaryPatch.Singleton.Patched) { RoundSummaryPatch.Singleton.Patch(false); }
				});

				CoroutineManager.KillAll();

				Started = false;
				PlayersSetup = false;
				ForcingRoundEnd = false;
				EquipmentGiven = false;

				Enabled = false;
            }
        }

        internal void ToggleSecondaryEvents(bool enable)
        {
            Log.Debug($"[GamemodeManager::ToggleSecondaryEvents] {(enable ? "Enabling" : "Disabling")} secondary events.", MurderMystery.Singleton.LogDebug);

            if (enable)
            {
				if (SecondaryEventsEnabled) { Log.Debug($"[GamemodeManager::ToggleSecondaryEvents] Already enabled! StackTrace: {Environment.StackTrace}", MurderMystery.Singleton.LogDebug); return; }


				Handlers.Player.Verified += EventHandlers.Verified;
				Handlers.Player.Destroying += EventHandlers.Destroying;

				Handlers.Player.Spawning += EventHandlers.Spawning;
				Handlers.Player.InteractingLocker += EventHandlers.InteractingLocker;
				Handlers.Player.OpeningGenerator += EventHandlers.OpeningGenerator;
				Handlers.Player.TriggeringTesla += EventHandlers.TriggeringTesla;
				Handlers.Player.EnteringFemurBreaker += EventHandlers.EnteringFemurBreaker;

				Handlers.Player.PickingUpItem += EventHandlers.PickingUpItem;
				Handlers.Player.DroppingItem += EventHandlers.DroppingItem;
				Handlers.Player.Shooting += EventHandlers.Shooting;
				Handlers.Player.Died += EventHandlers.Died;
				Handlers.Player.ChangedRole += EventHandlers.ChangedRole;
				Handlers.Player.SpawningRagdoll += EventHandlers.SpawningRagdoll;
				Handlers.Player.Hurting += EventHandlers.Hurting;

				Handlers.Server.EndingRound += EventHandlers.EndingRound;
				Handlers.Server.RespawningTeam += EventHandlers.RespawningTeam;

				ServerConsole.FriendlyFire = true;
				CharacterClassManager.LaterJoinEnabled = false;
				FriendlyFireConfig.PauseDetector = true;

				SecondaryEventsEnabled = true;
            }
            else
            {
				if (!SecondaryEventsEnabled) { Log.Debug($"[GamemodeManager::ToggleSecondaryEvents] Already disabled! StackTrace: {Environment.StackTrace}", MurderMystery.Singleton.LogDebug); return; }

				Handlers.Player.Verified -= EventHandlers.Verified;
				Handlers.Player.Destroying -= EventHandlers.Destroying;

				Handlers.Player.Spawning -= EventHandlers.Spawning;
				Handlers.Player.InteractingLocker -= EventHandlers.InteractingLocker;
				Handlers.Player.OpeningGenerator -= EventHandlers.OpeningGenerator;
				Handlers.Player.TriggeringTesla -= EventHandlers.TriggeringTesla;
				Handlers.Player.EnteringFemurBreaker -= EventHandlers.EnteringFemurBreaker;

				Handlers.Player.PickingUpItem -= EventHandlers.PickingUpItem;
				Handlers.Player.DroppingItem -= EventHandlers.DroppingItem;
				Handlers.Player.Shooting -= EventHandlers.Shooting;
				Handlers.Player.Died -= EventHandlers.Died;
				Handlers.Player.ChangedRole -= EventHandlers.ChangedRole;
				Handlers.Player.SpawningRagdoll -= EventHandlers.SpawningRagdoll;
				Handlers.Player.Hurting -= EventHandlers.Hurting;

				Handlers.Server.EndingRound -= EventHandlers.EndingRound;
				Handlers.Server.RespawningTeam -= EventHandlers.RespawningTeam;

				ServerConsole.FriendlyFire = GameCore.ConfigFile.ServerConfig.GetBool("friendly_fire", false);
				CharacterClassManager.LaterJoinEnabled = GameCore.ConfigFile.ServerConfig.GetBool("later_join_enabled", true);
				FriendlyFireConfig.PauseDetector = false;

				SecondaryEventsEnabled = false;
            }
        }

        internal void StartGamemode()
        {
            if (MMPlayer.List.Count < MurderMystery.Singleton.Config.MinimumPlayersStart && !MurderMystery.Singleton.DebugVersion)
            {
				Log.Info("[GamemodeManager::StartGamemode] There were not enough players to start the gamemode.");
                Map.Broadcast(10, $"<size=30>There must be atleast {MurderMystery.Singleton.Config.MinimumPlayersStart} players to start the gamemode!</size>", Broadcast.BroadcastFlags.Monospaced);
                ToggleGamemode(false);
                return;
            }
			
			CoroutineManager.RunServerCoroutine("prepare_map", PrepareMap());
			CoroutineManager.RunServerCoroutine("prepare_players", PreparePlayers());

			Started = true;
        }

		// Coroutines.
		private static IEnumerator<float> PrepareMap()
		{
			if (MMPlayer.List.Count < MurderMystery.Singleton.Config.MinimumPlayersOpenHeavy)
			{
				Log.Debug($"Locking most heavy rooms.", MurderMystery.Singleton.LogDebug);

				// Lock most heavy rooms.
				for (int i = 0; i < Map.Doors.Count; i++)
				{
					switch (Map.Doors[i].RequiredPermissions.RequiredPermissions)
					{
						// No permssion doors.
						case KeycardPermissions.None:
							Map.Doors[i].NetworkTargetState = true;
							Map.Doors[i].NetworkTargetState = false;
							continue;
                    
						// Checkpoints.
						case KeycardPermissions.Checkpoints:
							Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
							continue;
						
						default:
							switch (Map.Doors[i].GetNametag())
							{
								// Tagged doors.
								case "HCZ_ARMORY":
									Map.Doors[i].NetworkTargetState = true;
									Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
									Map.Doors[i].NetworkTargetState = false;
									continue;

								case "049_ARMORY":
									Map.Doors[i].NetworkTargetState = true;
									Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
									Map.Doors[i].NetworkTargetState = false;
									continue;

								case "096":
									Map.Doors[i].NetworkTargetState = true;
									Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
									Map.Doors[i].NetworkTargetState = false;
									continue;

								case "106_PRIMARY":
									Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
									Map.Doors[i].NetworkTargetState = true;
									continue;

								case "106_SECONDARY":
									Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
									Map.Doors[i].NetworkTargetState = true;
									continue;

								case "106_BOTTOM":
									Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
									Map.Doors[i].NetworkTargetState = true;
									continue;

								case "HID":
									Map.Doors[i].NetworkTargetState = true;
									Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
									Map.Doors[i].NetworkTargetState = false;
									continue;

								case "079_PRIMARY":
									Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
									continue;

								case "079_SECONDARY":
									Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
									continue;

								// Any other door.
								default:
									Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
									Map.Doors[i].NetworkTargetState = true;
									continue;
							}
					}
				}

				for (int i = 0; i < Map.Lifts.Count; i++)
				{
					if (Map.Lifts[i].Type() != ElevatorType.Scp049)
					{
						Map.Lifts[i].Network_locked = true;
					}
				}
			}
			else
			{
				Log.Debug($"Unlocking all heavy rooms.", MurderMystery.Singleton.LogDebug);

				// Unlock all heavy rooms.
				for (int i = 0; i < Map.Doors.Count; i++)
				{
					Map.Doors[i].NetworkTargetState = true;

					switch (Map.Doors[i].RequiredPermissions.RequiredPermissions)
					{
						// No permssion doors.
						case KeycardPermissions.None:
							Map.Doors[i].NetworkTargetState = true;
							Map.Doors[i].NetworkTargetState = false;
							continue;
                    
						// Checkpoints.
						case KeycardPermissions.Checkpoints:
							Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
							continue;
						
						default:
							switch (Map.Doors[i].GetNametag())
							{
								// Tagged doors.
								case "106_PRIMARY":
									Map.Doors[i].ServerChangeLock(DoorLockReason.Warhead, true);
									Map.Doors[i].NetworkTargetState = true;
									continue;

								case "106_SECONDARY":
									Map.Doors[i].ServerChangeLock(DoorLockReason.Warhead, true);
									Map.Doors[i].NetworkTargetState = true;
									continue;

								case "106_BOTTOM":
									Map.Doors[i].ServerChangeLock(DoorLockReason.Warhead, true);
									Map.Doors[i].NetworkTargetState = true;
									continue;

								// Any other door.
								default:
									Map.Doors[i].ServerChangeLock(DoorLockReason.AdminCommand, true);
									Map.Doors[i].NetworkTargetState = true;
									continue;
							}
					}
				}

				for (int i = 0; i < Map.Lifts.Count; i++)
				{
					ElevatorType type = Map.Lifts[i].Type();

					if (type == ElevatorType.LczA || type == ElevatorType.LczB)
					{
						Map.Lifts[i].Network_locked = true;
					}
				}
			}

			yield return Timing.WaitForSeconds(0.5f);

			Pickup[] pickups = UnityEngine.Object.FindObjectsOfType<Pickup>();

			for (int i = 0; i < pickups.Length; i++)
			{
				pickups[i].Delete();
			}
		}

		private IEnumerator<float> PreparePlayers()
		{
			yield return Timing.WaitForOneFrame;

			List<MMPlayer> nones = MMPlayer.List.GetRole(MMRole.None);

			for (int i = 0; i < nones.Count; i++)
			{
				nones[i].Role = MMRole.Innocent;
				Log.Debug($"Player {nones[i].Player.Nickname} ({nones[i].Player.Id}) set to innocent for role calculation.", MurderMystery.Singleton.LogDebug);
			}

			CalculateRoleCounts(nones.Count, out int m, out int d);
			Log.Debug($"Calculated role counts: Murderers: {m}, Detectives: {d}", MurderMystery.Singleton.LogDebug);

			System.Random rng = new System.Random();

			while (MMPlayer.List.GetRoleCount(MMRole.Murderer) < m)
			{
				List<MMPlayer> innos = MMPlayer.List.GetRole(MMRole.Innocent);
				innos[rng.Next(innos.Count)].Role = MMRole.Murderer;
			}
			while (MMPlayer.List.GetRoleCount(MMRole.Detective) < d)
			{
				List<MMPlayer> innos = MMPlayer.List.GetRole(MMRole.Innocent);
				innos[rng.Next(innos.Count)].Role = MMRole.Detective;
			}

            if (MurderMystery.Singleton.LogDebug)
            {
                for (int i = 0; i < MMPlayer.List.Count; i++)
                {
                    switch (MMPlayer.List[i].Role)
                    {
                        case MMRole.None:
                            Log.Error($"{MMPlayer.List[i].Player.Nickname} ({MMPlayer.List[i].Player.Id}) was selected as a NONE. (An error occured.)");
                            continue;
                        case MMRole.Spectator:
                            Log.Debug($"{MMPlayer.List[i].Player.Nickname} ({MMPlayer.List[i].Player.Id}) was selected as a spectator. (In overwatch mode.)");
                            continue;
                        case MMRole.Innocent:
                            Log.Debug($"{MMPlayer.List[i].Player.Nickname} ({MMPlayer.List[i].Player.Id}) was selected as an innocent.");
                            continue;
                        case MMRole.Murderer:
                            Log.Debug($"{MMPlayer.List[i].Player.Nickname} ({MMPlayer.List[i].Player.Id}) was selected as a murderer.");
                            continue;
                        case MMRole.Detective:
                            Log.Debug($"{MMPlayer.List[i].Player.Nickname} ({MMPlayer.List[i].Player.Id}) was selected as a detective.");
                            continue;
                    }
                }
            }

			List<MMPlayer> murderers = MMPlayer.List.GetRole(MMRole.Murderer);
			List<MMPlayer> detectives = MMPlayer.List.GetRole(MMRole.Detective);

			for (int i = 0; i < MMPlayer.List.Count; i++)
			{
				MMPlayer.List[i].SetupPlayer(murderers, detectives);
			}

			for (int murd = 0; murd < murderers.Count; murd++)
			{
				for (int i = 0; i < murderers.Count; i++)
				{
					murderers[i].Player.SetPlayerInfoForTargetOnly(murderers[murd].Player, MMRole.Murderer.ToColoredString());
				}
			}

			for (int det = 0; det < detectives.Count; det++)
			{
				for (int i = 0; i < detectives.Count; i++)
				{
					detectives[i].Player.SetPlayerInfoForTargetOnly(detectives[det].Player, MMRole.Detective.ToColoredString());
				}
			}

			yield return Timing.WaitForSeconds(0.25f);

			PlayersSetup = true;

			CoroutineManager.RunServerCoroutine("handout_equipment", HandoutEquipment(MurderMystery.Singleton.Config.EquipmentTime));

			UpdateSpectatorInfo();
		}

		private IEnumerator<float> HandoutEquipment(int delay)
		{
			yield return Timing.WaitForSeconds((float)delay);

			for (int i = 0; i < MMPlayer.List.Count; i++)
			{
				MMPlayer.List[i].GiveEquipment();
			}

			EquipmentGiven = true;
		}

		internal static IEnumerator<float> DetectiveWeapon(Pickup item)
		{
			while (item != null)
			{
				List<MMPlayer> innocents = MMPlayer.List.GetRole(MMRole.Innocent);

				for (int i = 0; i < innocents.Count; i++)
				{
					if (innocents[i].Player.Role != RoleType.Spectator)
					{
						if (Vector3.Distance(innocents[i].Player.Position, item.Networkposition) <= 1.7f)
						{
							if (!Physics.Linecast(innocents[i].Player.ReferenceHub.playerMovementSync.RealModelPosition, item.Networkposition, Server.Host.ReferenceHub.weaponManager.raycastServerMask))
							{
								if (innocents[i].Player.Inventory.items.Count > 6)
								{
									innocents[i].Player.ShowHint("<b><size=25><color=#ff0000>You must have atleast 2 available slots to pickup the detectives weapon.</color></size></b>", 5f);
								}
								else
								{
									if (!innocents[i].Player.GetEffectActive<Blinded>()) // Check if the player is blinded (gun loss cooldown)
									{
										// Remove keycard if found.
										if (TryGetNearestDetectiveKeycard(item.Networkposition, out Pickup keycard))
										{
											keycard.Delete();
										}

										// Give detective items.
										innocents[i].Player.AddItem(ItemType.KeycardNTFCommander);
										innocents[i].Player.AddItem(new Inventory.SyncItemInfo() { durability = item.durability, id = ItemType.GunCOM15, modBarrel = 0, modOther = 0, modSight = 0 });
										item.Delete();

										// Tell the player they picked up the item.
										innocents[i].Player.ShowHint("<b><size=25><color=#0000ff>You have picked up the detectives weapon.</color></size></b>", 5f);

										// Set them as detective
										innocents[i].Role = MMRole.Detective;

										yield break; // Kill the coroutine.
									}
								}
							}
						}
					}
				}

				yield return Timing.WaitForSeconds(0.25f);
			}
		}

		// Other important functions.
		public static void CalculateRoleCounts(int playerCount, out int m, out int d)
		{
			switch (playerCount)
			{
				case 0:
					m = 0;
					d = 0;
					return;
				case 1:
					m = 1;
					d = 0;
					return;
				case 2:
					m = 1;
					d = 1;
					return;
				
				default:
					if (playerCount < 1 / MurderMystery.Singleton.Config.MurdererPercentage)
					{
						m = 1;
					}
					else
					{
						m = (int)Math.Round(playerCount * MurderMystery.Singleton.Config.MurdererPercentage);
					}
					if (playerCount < 1 / MurderMystery.Singleton.Config.DetectivePercentage)
					{
						d = 1;
					}
					else
					{
						d = (int)Math.Round(playerCount * MurderMystery.Singleton.Config.DetectivePercentage);
					}
					return;
			}
		}

		public static bool TryGetNearestDetectiveKeycard(Vector3 pos, out Pickup keycard)
		{
			try
			{
				keycard = UnityEngine.Object.FindObjectsOfType<Pickup>().Where(x => x.ItemId == ItemType.KeycardNTFCommander).OrderBy(x => Vector3.Distance(x.transform.position, pos)).First();
				return true;
			}
			catch
			{
				keycard = null;
				return false;
			}
		}

		public static bool TryGetNearestDetectiveWeapon(Vector3 pos, out Pickup weapon)
		{
			try
			{
				weapon = UnityEngine.Object.FindObjectsOfType<Pickup>().Where(x => x.ItemId == ItemType.GunCOM15 && x.weaponMods.Barrel == 0).OrderBy(x => Vector3.Distance(x.transform.position, pos)).First();
				return true;
			}
			catch
			{
				weapon = null;
				return false;
			}
		}

		internal static void UpdateRoleForSpectators(MMPlayer player)
        {
			List<MMPlayer> spectators = MMPlayer.List.GetRole(MMRole.Spectator);

			for (int spec = 0; spec < spectators.Count; spec++)
            {
				spectators[i].Player.SetPlayerInfoForTargetOnly(detectives[det].Player, MMRole.Detective.ToColoredString());
			}
		}

		internal static void UpdateRoleForMurderers(MMPlayer player)
        {
			List<MMPlayer> murderers = MMPlayer.List.GetRole(MMRole.Murderer);

			for (int murd = 0; murd < murderers.Count; murd++)
			{

			}
		}

		internal static void UpdateRoleForDetectives(MMPlayer player)
		{
			List<MMPlayer> detectives = MMPlayer.List.GetRole(MMRole.Detective);

			for (int det = 0; det < detectives.Count; det++)
			{

			}
		}

		internal static void UpdateSpectatorInfo()
		{
			List<MMPlayer> spectators = MMPlayer.List.GetRole(MMRole.Spectator);

			for (int spec = 0; spec < spectators.Count; spec++)
			{
				for (int i = 0; i < MMPlayer.List.Count; i++)
				{
					switch (MMPlayer.List[i].Role)
					{
						case MMRole.Innocent:
							spectators[spec].Player.SendFakeSyncVar(MMPlayer.List[i].Player.ReferenceHub.networkIdentity, typeof(CharacterClassManager), "NetworkCurClass", (sbyte)RoleType.ClassD);
							continue;
						case MMRole.Murderer:
							spectators[spec].Player.SendFakeSyncVar(MMPlayer.List[i].Player.ReferenceHub.networkIdentity, typeof(CharacterClassManager), "NetworkCurClass", (sbyte)RoleType.Tutorial);
							continue;
						case MMRole.Detective:
							spectators[spec].Player.SendFakeSyncVar(MMPlayer.List[i].Player.ReferenceHub.networkIdentity, typeof(CharacterClassManager), "NetworkCurClass", (sbyte)RoleType.NtfCommander);
							continue;
					}
				}
			}
		}

		internal static void UpdateMurdererInfo()
        {
			List<MMPlayer> murderers = MMPlayer.List.GetRole(MMRole.Murderer);

			for (int murd = 0; murd < murderers.Count; murd++)
			{
				for (int i = 0; i < murderers.Count; i++)
				{
					murderers[i].Player.SetPlayerInfoForTargetOnly(murderers[murd].Player, MMRole.Murderer.ToColoredString());
				}
			}
		}

		internal static void UpdateDetectiveInfo()
        {
			List<MMPlayer> detectives = MMPlayer.List.GetRole(MMRole.Detective);

			for (int det = 0; det < detectives.Count; det++)
			{
				for (int i = 0; i < detectives.Count; i++)
				{
					detectives[i].Player.SetPlayerInfoForTargetOnly(detectives[det].Player, MMRole.Detective.ToColoredString());
				}
			}
		}
	
		public string GetData()
		{
			StringBuilder builder = new StringBuilder();

			builder.AppendLine("\n<size=25>GamemodeManager Data:");

			builder.AppendLine($"Enabled: {(Enabled ? "<color=#00ff00>True</color>" : "<color=#ff0000>False</color>")}");
			builder.AppendLine($"SecondaryEventsEnabled: {(SecondaryEventsEnabled ? "<color=#00ff00>True</color>" : "<color=#ff0000>False</color>")}");
			builder.AppendLine($"Started: {(Started ? "<color=#00ff00>True</color>" : "<color=#ff0000>False</color>")}");
			builder.AppendLine($"PlayersSetup: {(PlayersSetup ? "<color=#00ff00>True</color>" : "<color=#ff0000>False</color>")}");
			builder.AppendLine($"ForcingRoundEnd: {(ForcingRoundEnd ? "<color=#00ff00>True</color>" : "<color=#ff0000>False</color>")}");
			builder.AppendLine($"EquipmentGiven: {(EquipmentGiven ? "<color=#00ff00>True</color>" : "<color=#ff0000>False</color>")}");

			builder.Append("</size>");

			return builder.ToString();
		}
    }
}