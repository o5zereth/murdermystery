using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using MurderMystery.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MurderMystery.API
{
    public class MMPlayer
    {
        private MMPlayer(Player player)
        {
            Player = player;
            Role = MMRole.None;
            DetectiveGunLossCooldown = -1f;
        }

        public Player Player { get; }
        public MMRole Role { get; private set; }
        public float DetectiveGunLossCooldown { get; private set; }

        internal void SoftlySetRole(MMRole role)
        {
            Role = role;
        }

        public static List<MMPlayer> List { get; } = new List<MMPlayer>();

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

        // Coroutines
        internal static IEnumerator<float> SetupPlayers()
        {
            Log.Debug("SetupPlayers called.", MurderMystery.Singleton.Debug);

            yield return Timing.WaitForSeconds(0.2f);

            foreach (MMPlayer ply in List.OfRole(MMRole.None))
            {
                Log.Debug("Player found in list without a role, setting to innocent for role calculation...", MurderMystery.Singleton.Debug);
                ply.Role = MMRole.Innocent;
            }

            CalculateRoleCounts(List.OfRoleCount(MMRole.Innocent), out int m, out int d);

            while (List.OfRoleCount(MMRole.Murderer) < m || List.OfRoleCount(MMRole.Detective) < d)
            {
                if (List.OfRoleCount(MMRole.Murderer) < m)
                {
                    Random rng = new Random();

                    int r = rng.Next(List.OfRoleCount(MMRole.Innocent));
                    List.OfRole(MMRole.Innocent)[r].Role = MMRole.Murderer;
                }
                if (List.OfRoleCount(MMRole.Detective) < d)
                {
                    Random rng = new Random();

                    int r = rng.Next(List.OfRoleCount(MMRole.Innocent));
                    List.OfRole(MMRole.Innocent)[r].Role = MMRole.Detective;
                }
            }

            if (MurderMystery.Singleton.Debug)
            {
                for (int i = 0; i < List.Count; i++)
                {
                    switch (List[i].Role)
                    {
                        case MMRole.None:
                            Log.Error($"{List[i].Player.Nickname} ({List[i].Player.Id}) was selected as a NONE. (An error occured.)");
                            continue;
                        case MMRole.Spectator:
                            Log.Debug($"{List[i].Player.Nickname} ({List[i].Player.Id}) was selected as a spectator. (In overwatch mode.)");
                            continue;
                        case MMRole.Innocent:
                            Log.Debug($"{List[i].Player.Nickname} ({List[i].Player.Id}) was selected as an innocent.");
                            continue;
                        case MMRole.Murderer:
                            Log.Debug($"{List[i].Player.Nickname} ({List[i].Player.Id}) was selected as a murderer.");
                            continue;
                        case MMRole.Detective:
                            Log.Debug($"{List[i].Player.Nickname} ({List[i].Player.Id}) was selected as a detective.");
                            continue;
                    }
                }
            }

            foreach (MMPlayer ply in List)
            {
                if (ply.IsAlive())
                {
                    ply.Player.SetRole(RoleType.ClassD);
                    Timing.CallDelayed(0.5f, () =>
                    {
                        ply.Player.Position = RoleType.Scp049.GetRandomSpawnPoint();
                        ply.Player.AddItem(ItemType.Painkillers);
                        ply.Player.Ammo[(int)AmmoType.Nato9] = int.MaxValue;
                        BroadcastRoleInfo(ply);
                    });
                }
                else
                {
                    ply.Player.SetRole(RoleType.Spectator);
                    BroadcastRoleInfo(ply);
                }
            }

            MurderMystery.CoroutineManager.RunServerCoroutine(HandOutEquipment(MurderMystery.Singleton.Config.EquipmentTime));
        }
        internal static IEnumerator<float> HandOutEquipment(float delay)
        {
            yield return Timing.WaitForSeconds(delay);

            foreach (MMPlayer ply in List.AliveList())
            {
                switch (ply.Role)
                {
                    case MMRole.Murderer:
                        ply.Player.AddItem(ItemType.KeycardFacilityManager);
                        ply.Player.AddItem(new Inventory.SyncItemInfo() { durability = 12f, id = ItemType.GunCOM15, modBarrel = (int)BarrelType.Suppressor, modOther = (int)OtherType.None, modSight = (int)SightType.None });
                        ply.Player.AddItem(ItemType.SCP268);
                        ply.Player.Broadcast(10, "<size=30><color=#ff0000>You have received your equipment.</color></size>");
                        break;
                    case MMRole.Detective:
                        ply.Player.AddItem(ItemType.KeycardNTFCommander);
                        ply.Player.AddItem(new Inventory.SyncItemInfo() { durability = 12f, id = ItemType.GunCOM15, modBarrel = (int)BarrelType.None, modOther = (int)OtherType.None, modSight = (int)SightType.None });
                        ply.Player.AddItem(ItemType.Medkit);
                        ply.Player.Broadcast(10, "<size=30><color=#0000ff>You have received your equipment.</color></size>");
                        break;
                    default:
                        continue;
                }
            }
        }
        internal IEnumerator<float> DetectiveGunLossTimer(float waitTime)
        {
            DetectiveGunLossCooldown = waitTime;

            while (DetectiveGunLossCooldown > 0f)
            {
                yield return Timing.WaitForSeconds(1f);
                DetectiveGunLossCooldown -= 1f;
            }

            yield break;
        }

        internal static void BroadcastRoleInfo(MMPlayer ply)
        {
            switch (ply.Role)
            {
                case MMRole.None:
                    ply.Player.Broadcast(15, "An error occured while setting your role.");
                    return;
                case MMRole.Spectator:
                    ply.Player.Broadcast(15, "<size=30>You are in overwatch and have been set to spectator.</size>");
                    return;
                case MMRole.Innocent:
                    ply.Player.Broadcast(15, "<size=30>You are an <color=#00ff00>Innocent</color>.\nYou must <color=#ff00ff>survive</color>, and avoid <color=#ff0000>Murderers</color>.</size>");
                    return;
                case MMRole.Murderer:
                    ply.Player.Broadcast(10, "<size=30>You are a <color=#ff0000>Murderer</color>.\nYou must <color=#ff0000>kill all</color> <color=#00ff00>innocents</color> and <color=#0000ff>detectives</color>.</size>");
                    ply.Player.Broadcast(10, GetFellowsString());
                    ply.Player.Broadcast(5, "<size=30><color=#ff0000>You will recieve your equipment shortly.</color></size>");
                    return;
                case MMRole.Detective:
                    ply.Player.Broadcast(10, "<size=30>You are a <color=#0000ff>Detective</color>.\nYou must <color=#ff0000>kill all murderers</color> and <color=#00ff00>protect innocents</color>.</size>");
                    ply.Player.Broadcast(10, GetFellowsString());
                    ply.Player.Broadcast(5, "<size=30><color=#0000ff>You will recieve your equipment shortly.</color></size>");
                    return;
            }

            string GetFellowsString()
            {
                switch (ply.Role)
                {
                    case MMRole.Murderer:
                        List<MMPlayer> Murderers = List.OfRole(MMRole.Murderer);
                        if (Murderers.Count == 1)
                        {
                            return "<color=#ff0000><size=30>You are alone and the only murderer.\nGodspeed.</size></color>";
                        }
                        else
                        {
                            StringBuilder builder = new StringBuilder();

                            builder.Append("<color=#ff0000><size=30>Remember your fellow murderers:</size><size=25>\n");

                            for (int i = 0; i < Murderers.Count; i++)
                            {
                                if (Murderers[i].Player.Id != ply.Player.Id)
                                {
                                    builder.Append(i == Murderers.Count - 1 ? Murderers[i].Player.Nickname : Murderers[i].Player.Nickname + ", ");
                                }
                            }

                            builder.Append("</size></color>");

                            return builder.ToString();
                        }
                    case MMRole.Detective:
                        List<MMPlayer> Detectives = List.OfRole(MMRole.Murderer);
                        if (Detectives.Count == 1)
                        {
                            return "<color=#0000ff><size=30>You are alone and the only detective.\nGodspeed.</size></color>";
                        }
                        else
                        {
                            StringBuilder builder = new StringBuilder();

                            builder.Append("<color=#0000ff><size=30>Remember your fellow murderers:</size><size=25>\n");

                            for (int i = 0; i < Detectives.Count; i++)
                            {
                                if (Detectives[i].Player.Id != ply.Player.Id)
                                {
                                    builder.Append(i == Detectives.Count - 1 ? Detectives[i].Player.Nickname : Detectives[i].Player.Nickname + ", ");
                                }
                            }

                            builder.Append("</size></color>");

                            return builder.ToString();
                        }
                    default:
                        return null;
                }
            }
        }
        internal static void CalculateRoleCounts(int playercount, out int m, out int d)
        {
            if (playercount == 1)
            {
                m = 1;
                d = 0;
                return;
            }

            if (playercount < 1 / MurderMystery.Singleton.Config.MurdererPercent)
            {
                m = 1;
            }
            else
            {
                m = (int)Math.Floor(playercount * MurderMystery.Singleton.Config.MurdererPercent);
            }
            if (playercount < 1 / MurderMystery.Singleton.Config.DetectivePercent)
            {
                d = 1;
            }
            else
            {
                d = (int)Math.Floor(playercount * MurderMystery.Singleton.Config.DetectivePercent);
            }
        }

        internal static void Add(VerifiedEventArgs ev)
        {
            Log.Debug("Player added to list.", MurderMystery.Singleton.Debug);
            List.Add(new MMPlayer(ev.Player));
        }
        internal static void Remove(DestroyingEventArgs ev)
        {
            Log.Debug("Player removed from list.", MurderMystery.Singleton.Debug);
            List.Remove(Get(ev.Player));
        }
    }
}