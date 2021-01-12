using Exiled.API.Features;
using MEC;
using System.Collections.Generic;

namespace MurderMystery.Extensions
{
    public static class Coroutines
    {
        public static void KillAll(this List<CoroutineHandle> list)
        {
            Log.Debug("KillAll was called.", MurderMystery.Singleton.Debug);

            foreach (CoroutineHandle handle in list)
            {
                Timing.KillCoroutines(handle);
            }
        }

        public static List<CoroutineHandle> RunAndAdd(this List<CoroutineHandle> list, IEnumerator<float> coroutine)
        {
            Log.Debug("RunAndAdd was called.", MurderMystery.Singleton.Debug);

            CoroutineHandle handle = Timing.RunCoroutine(coroutine);
            list.Add(handle);

            return list;
        }
    }
}