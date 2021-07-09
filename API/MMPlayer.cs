using Exiled.API.Enums;
using CustomPlayerEffects;
using System;
using System.Text;
using MEC;
using MurderMystery.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System.Collections.Generic;

namespace MurderMystery.API
{
    public class MMPlayer
    {
        internal MMPlayer(Player player) => Player = player;

        public static List<MMPlayer> List { get; } = new List<MMPlayer>();

        public Player Player { get; }
        public MMRole Role
        {
            get
            {
                return _role;
            }
            internal set
            {
                _role = value;

                if (MurderMystery.Singleton.GamemodeManager.PlayersSetup)
                {
                    Timing.CallDelayed(0f, () =>
                    {
                        GamemodeManager.UpdateSpectatorInfo();
                    });
                }
            }
        }
        private MMRole _role;

		public bool IsAlive => Role == MMRole.Innocent || Role == MMRole.Murderer || Role == MMRole.Detective;

        public static MMPlayer Get(Player player)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].Player == player)
                {
                    return List[i];
                }
            }

            return null;
        }

        public static bool Get(Player player, out MMPlayer ply)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].Player == player)
                {
                    ply = List[i];
                    return true;
                }
            }

            ply = null;
            return false;
        }

		internal void SetupPlayer(List<MMPlayer> murderers, List<MMPlayer> detectives)
		{
			Timing.CallDelayed(0f, () =>
			{
				if (IsAlive)
				{
					Player.SetRole(RoleType.ClassD, true, false);
					Player.Inventory.Clear();
					Player.AddItem(ItemType.Painkillers);
					Player.Ammo[(int)AmmoType.Nato9] = int.MaxValue;
				}
				else
				{
					Player.SetRole(RoleType.Spectator, false, false);
				}
				BroadcastRoleInfo(murderers, detectives);
			});
		}

		private void BroadcastRoleInfo(List<MMPlayer> murderers, List<MMPlayer> detectives)
		{
			Player.Broadcast(15, Role.GetRoleInfo());

			switch (Role) // Broadcast extra info for special roles.
			{
				case MMRole.Murderer:
					return;
				case MMRole.Detective:
					return;
			}
		}

		internal void GiveEquipment()
		{
			switch (Role)
			{
				case MMRole.Murderer:
					Player.AddItem(ItemType.KeycardFacilityManager);
					Player.AddItem(new Inventory.SyncItemInfo() { durability = 12, id = ItemType.GunCOM15, modBarrel = 1, modOther = 0, modSight = 0 });
					Player.AddItem(ItemType.SCP268);

					Player.Broadcast(10, "<size=30><color=#ff0000>You have recieved your equipment.</color></size>");
					break;
				case MMRole.Detective:
					Player.AddItem(ItemType.KeycardNTFCommander);
					Player.AddItem(new Inventory.SyncItemInfo() { durability = 12, id = ItemType.GunCOM15, modBarrel = 0, modOther = 0, modSight = 0 });
					Player.AddItem(ItemType.Medkit);

					Player.Broadcast(10, "<size=30><color=#0000ff>You have recieved your equipment.</color></size>");
					break;
			}
		}

		public bool TryGetWeaponItem(out Inventory.SyncItemInfo weapon)
		{
			for (int i = 0; i < Player.Inventory.items.Count; i++)
			{
				if (Player.Inventory.items[i].id == ItemType.GunCOM15)
				{
					weapon = Player.Inventory.items[i];
					return true;
				}
			}

			weapon = new Inventory.SyncItemInfo();
			return false;
		}

		public bool TryGetKeycardItem(out Inventory.SyncItemInfo keycard)
		{
			for (int i = 0; i < Player.Inventory.items.Count; i++)
			{
				if (Player.Inventory.items[i].id == ItemType.KeycardFacilityManager || Player.Inventory.items[i].id == ItemType.KeycardNTFCommander)
				{
					keycard = Player.Inventory.items[i];
					return true;
				}
			}

			keycard = new Inventory.SyncItemInfo();
			return false;
		}


		// Events.
		internal void PickingUpItem(PickingUpItemEventArgs ev)
		{
			if (ev.Pickup.ItemId == ItemType.KeycardFacilityManager) { ev.IsAllowed = false; return; }
			if (ev.Pickup.ItemId == ItemType.KeycardNTFCommander) { ev.IsAllowed = false; return; }
			if (ev.Pickup.ItemId == ItemType.SCP268) { ev.IsAllowed = false; return; }

			switch (Role)
			{
				case MMRole.Innocent:
					if (ev.Pickup.ItemId == ItemType.GunCOM15)
					{
						if (ev.Pickup.weaponMods.Barrel != 1)
						{
							if (Player.Inventory.items.Count > 6)
							{
								Player.ShowHint("<b><size=25><color=#ff0000>You must have atleast 2 available slots to pickup the detectives weapon.</color></size></b>", 5f);
								ev.IsAllowed = false;
								return;
							}
							else
							{
								if (!Player.GetEffectActive<Blinded>())
								{
									// Remove keycard if found.
									if (GamemodeManager.TryGetNearestDetectiveKeycard(ev.Pickup.Networkposition, out Pickup keycard))
									{
										keycard.Delete();
									}

									// Give detective keycard.
									Player.AddItem(ItemType.KeycardNTFCommander);

									// Tell the player they picked up the item.
									Player.ShowHint("<b><size=25><color=#0000ff>You have picked up the detectives weapon.</color></size></b>", 5f);

									// Set them to detective.
									Role = MMRole.Detective;

									ev.IsAllowed = true;
								}
							}
						}
						else
						{
							ev.IsAllowed = false;
							Player.ShowHint("<b><size=25><color=#ff0000>You can't pickup a murderers weapon.</color></size></b>", 5f);
						}
					}
					return;
				case MMRole.Murderer:
					if (ev.Pickup.ItemId == ItemType.GunCOM15)
					{
						Player.ShowHint("<b><size=25><color=#ff0000>You can't pickup another weapon. ( ͡° ͜ʖ ͡°)</color></size></b>", 5f);
						ev.IsAllowed = false;
					}
					return;
				case MMRole.Detective:
					if (ev.Pickup.ItemId == ItemType.GunCOM15)
					{
						Player.ShowHint("<b><size=25><color=#0000ff>You can't pickup another weapon. ( ͡° ͜ʖ ͡°)</color></size></b>", 5f);
						ev.IsAllowed = false;
					}
					return;
			}
		}

		internal void DroppingItem(DroppingItemEventArgs ev)
		{
			if (Round.ElapsedTime.TotalSeconds < 5) { ev.IsAllowed = false; return; }

			if (ev.Item.id == ItemType.KeycardFacilityManager) { ev.IsAllowed = false; return; }
			if (ev.Item.id == ItemType.KeycardNTFCommander) { ev.IsAllowed = false; return; }
			if (ev.Item.id == ItemType.GunCOM15) { ev.IsAllowed = false; return; }
			if (ev.Item.id == ItemType.SCP268) { ev.IsAllowed = false; return; }
		}

		internal void Shooting(ShootingEventArgs ev)
		{
			Player targetPlayer = Player.Get(ev.Target);

			if (targetPlayer != null)
			{
				MMPlayer target = MMPlayer.Get(targetPlayer);

				if (Role == MMRole.Murderer && target.Role == MMRole.Murderer)
				{
                    ev.IsAllowed = false;
                    Player.ShowHint("<b><size=30>You can't shoot a fellow murderer.</size></b>", 3f);
                    return;
				}
				if (Role == MMRole.Detective && target.Role == MMRole.Detective)
				{
                    ev.IsAllowed = false;
                    Player.ShowHint("<b><size=30>You can't shoot a fellow detective.</size></b>", 3f);
                    return;
				}
			}
		}
		
		internal void Died(DiedEventArgs ev)
		{
			if (ev.Killer != null)
			{
				MMPlayer killer = MMPlayer.Get(ev.Killer);

				if (Role == MMRole.Innocent && killer.Role == MMRole.Detective) // Detective kills an innocent player.
				{
					// Blind the detective.
					killer.Player.EnableEffect<Blinded>(30); // The blinded effect tells the coroutines and events they are not allowed to pickup the weapons.

					// Remove detectives items and role.
					if (killer.TryGetWeaponItem(out Inventory.SyncItemInfo weapon))
					{
						killer.Player.DropItem(weapon);

						if (GamemodeManager.TryGetNearestDetectiveWeapon(killer.Player.Position, out Pickup weaponPickup))
						{
							MurderMystery.Singleton.GamemodeManager.CoroutineManager.RunServerCoroutine("detective_weapon", GamemodeManager.DetectiveWeapon(weaponPickup));
						}
						else
						{
							Log.Error($"[MMPlayer::Died] Unable to find detectives weapon (pickup) on applying coroutines. Player: {killer.Player.Nickname} ({killer.Player.Id})");
						}
					}
					else
					{
						Log.Error($"[MMPlayer::Died] Unable to find detectives weapon (item) on dropping. Player: {killer.Player.Nickname} ({killer.Player.Id})");
					}
					if (killer.TryGetKeycardItem(out Inventory.SyncItemInfo keycard))
					{
						killer.Player.DropItem(keycard);
					}
					else
					{
						Log.Error($"[MMPlayer::Died] Unable to find detectives keycard (item) on dropping. Player: {killer.Player.Nickname} ({killer.Player.Id})");
					}

					killer.Role = MMRole.Innocent;
				}
				else if (Role == MMRole.Detective)
				{
					if (GamemodeManager.TryGetNearestDetectiveWeapon(Player.ReferenceHub.characterClassManager.NetworkDeathPosition, out Pickup weaponPickup))
					{
						MurderMystery.Singleton.GamemodeManager.CoroutineManager.RunServerCoroutine("detective_weapon", GamemodeManager.DetectiveWeapon(weaponPickup));
					}
					else
					{
						Log.Error($"[MMPlayer::Died] Unable to find detectives weapon (pickup) on death. Player: {killer.Player.Nickname} ({killer.Player.Id})");
					}
				}
			}

			Role = MMRole.Spectator;
		}

		internal void ChangedRole(ChangedRoleEventArgs ev)
		{
			if (Round.ElapsedTime.TotalSeconds <= 5 || !MurderMystery.Singleton.GamemodeManager.Started) { return; }

            if (Role != MMRole.Spectator)
            {
                Role = MMRole.Spectator;
				Player.Broadcast(5, "<size=30>Your role has been set to spectator due to a class change.</size>");
				Log.Debug($"[MMPlayer::ChangingRole] Player {Player.Nickname} ({Player.Id}) has been set to spectator due to a class change.");
			}
		}

		internal void SpawningRagdoll(SpawningRagdollEventArgs ev)
		{
			ev.PlayerNickname = $"[{Role.ToColoredString()}] {ev.PlayerNickname}";
		}

		internal void Hurting(HurtingEventArgs ev)
		{
			if (Get(ev.Attacker, out MMPlayer attacker))
			{
				if (Role == attacker.Role)
				{
					ev.IsAllowed = false;
					ev.Amount = 0;
				}
			}
		}

        internal MMPlayer Verified(VerifiedEventArgs ev)
        {
            if (MurderMystery.Singleton.GamemodeManager.Started)
            {
                Role = MMRole.Spectator;

                Player.Broadcast(10, "<size=30>Murder Mystery gamemode is currently in progress.</size>\n<size=20>" + MurderMystery.Singleton.VersionStr + "</size>");
            }
            /*else if (CharacterClassManager._host.GetComponent<CharacterClassManager>().LaterJoinPossible())
            {
				// Late join.
            }*/
			else
			{
                Role = MMRole.None;

                Player.Broadcast(15, "<size=30>Murder Mystery gamemode is enabled for this round.</size>\n<size=20>" + MurderMystery.Singleton.VersionStr + "</size>");
			}

			MurderMystery.Singleton.GamemodeManager.CoroutineManager.PlayerCoroutines.Add(this, new List<CoroutineManager.Coroutine>());

            return this; // Required to add to list.
        }

        internal MMPlayer Destroying(DestroyingEventArgs ev)
        {
            return this; // Required to remove from list.
        }

		internal static void ClearList()
		{
			List.Clear();

			Log.Debug("[MMPlayer::ClearList] The player list has been cleared.", MurderMystery.Singleton.LogDebug);
		}
    }
}
