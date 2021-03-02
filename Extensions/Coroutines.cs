using Exiled.API.Features;
using MEC;
using MurderMystery.API;
using System.Collections.Generic;

namespace MurderMystery.Extensions
{
    public static class Coroutines
    {
        public static void KillAll(this Dictionary<MMPlayer, List<CoroutineHandle>> dict)
        {
            Log.Debug("KillAll was called. (Killing all player coroutines)", MurderMystery.Singleton.Debug);

            foreach (KeyValuePair<MMPlayer, List<CoroutineHandle>> keyValuePair in dict)
            {
                foreach (CoroutineHandle handle in keyValuePair.Value)
                {
                    Timing.KillCoroutines(handle);
                }
            }
        }

        public static void KillAll(this List<CoroutineHandle> list)
        {
            Log.Debug("KillAll was called. (Killing all server coroutines)", MurderMystery.Singleton.Debug);

            foreach (CoroutineHandle handle in list)
            {
                Timing.KillCoroutines(handle);
            }
        }

        public static Dictionary<MMPlayer, List<CoroutineHandle>> RunAndAdd(this Dictionary<MMPlayer, List<CoroutineHandle>> dict, IEnumerator<float> coroutine, MMPlayer key)
        {
            Log.Debug("RunAndAdd was called. (Player coroutine)", MurderMystery.Singleton.Debug);

            CoroutineHandle handle = Timing.RunCoroutine(coroutine);
            List<CoroutineHandle> list;

            if (dict.ContainsKey(key))
            {
                dict.TryGetValue(key, out list);
                list.Add(handle);
            }
            else
            {
                list = new List<CoroutineHandle>() { handle };
                dict.Add(key, list);
            }

            return dict;
        }

        public static List<CoroutineHandle> RunAndAdd(this List<CoroutineHandle> list, IEnumerator<float> coroutine)
        {
            Log.Debug("RunAndAdd was called. (Server coroutine)", MurderMystery.Singleton.Debug);

            CoroutineHandle handle = Timing.RunCoroutine(coroutine);
            list.Add(handle);

            return list;
        }
    }
}