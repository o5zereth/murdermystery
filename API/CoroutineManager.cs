using Exiled.API.Features;
using MEC;
using MurderMystery.Extensions;
using System.Collections.Generic;

namespace MurderMystery.API
{
    public class CoroutineManager
    {
        internal CoroutineManager() { }

        public List<Pickup> CoroutinedPickups { get; } = new List<Pickup>();
        public List<CoroutineHandle> ServerCoroutines { get; } = new List<CoroutineHandle>();
        public Dictionary<MMPlayer, List<CoroutineHandle>> PlayerCoroutines { get; } = new Dictionary<MMPlayer, List<CoroutineHandle>>();

        public void RunPickupCoroutine(IEnumerator<float> coroutine, Pickup item)
        {
            Log.Debug("RunPickupCoroutine was called. (Running pickup coroutine)", MurderMystery.Singleton.Debug);

            RunServerCoroutine(coroutine);
            CoroutinedPickups.Add(item);
        }

        public void RunServerCoroutine(IEnumerator<float> coroutine)
        {
            Log.Debug("RunServerCoroutine was called. (Running server coroutine)", MurderMystery.Singleton.Debug);

            CoroutineHandle handle = Timing.RunCoroutine(coroutine);
            
            ServerCoroutines.Add(handle);
        }

        public void RunPlayerCoroutine(IEnumerator<float> coroutine, MMPlayer ply)
        {
            Log.Debug("RunPlayerCoroutine was called. (Running player coroutine)", MurderMystery.Singleton.Debug);

            CoroutineHandle handle = Timing.RunCoroutine(coroutine);

            if (PlayerCoroutines.ContainsKey(ply))
            {
                PlayerCoroutines[ply].Add(handle);
            }
            else
            {
                PlayerCoroutines.Add(ply, new List<CoroutineHandle>() { handle });
            }
        }

        internal void Reset()
        {
            CoroutinedPickups.Clear();

            for (int i = 0; i < ServerCoroutines.Count; i++)
            {
                Timing.KillCoroutines(ServerCoroutines[i]);
            }
            ServerCoroutines.Clear();

            foreach (KeyValuePair<MMPlayer, List<CoroutineHandle>> keyValuePair in PlayerCoroutines)
            {
                for (int i = 0; i < keyValuePair.Value.Count; i++)
                {
                    Timing.KillCoroutines(keyValuePair.Value[i]);
                }
            }
            PlayerCoroutines.Clear();
        }
    }
}