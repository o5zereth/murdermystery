using Exiled.API.Features;
using MEC;
using System.Collections.Generic;

namespace MurderMystery.API
{
    public class CoroutineManager
    {
		internal CoroutineManager()
		{
			ServerCoroutines = new List<Coroutine>();
			PlayerCoroutines = new Dictionary<MMPlayer, List<Coroutine>>();
		}

        public List<Coroutine> ServerCoroutines { get; }
		public Dictionary<MMPlayer, List<Coroutine>> PlayerCoroutines { get; }


		public void RunServerCoroutine(string id, IEnumerator<float> coroutine)
		{
			ServerCoroutines.Add(new Coroutine(id, coroutine));
		}

		public void RunPlayerCoroutine(string id, MMPlayer player, IEnumerator<float> coroutine)
		{
			if (PlayerCoroutines.TryGetValue(player, out List<Coroutine> list))
			{
				list.Add(new Coroutine(id, coroutine));
			}
			else
			{
				Log.Error("[CoroutineManager::RunPlayerCoroutine] Failed to find player coroutine list!");
			}
		}

		internal void KillServerCoroutinesById(string id)
		{
			foreach (Coroutine coroutine in ServerCoroutines)
			{
				if (coroutine.Id == id)
				{
					Log.Debug("[CoroutineManager::KillServerCoroutinesById] Killing coroutine with id: " + id);

					Timing.KillCoroutines(coroutine.Handle);
				}
			}
		}

		internal void KillAll()
		{
			Log.Debug("[CoroutineManager::KillAll] Killing all coroutines.", MurderMystery.Singleton.LogDebug);

			foreach (Coroutine coroutine in ServerCoroutines)
			{
				Timing.KillCoroutines(coroutine.Handle);
			}

			ServerCoroutines.Clear();

			foreach (KeyValuePair<MMPlayer, List<Coroutine>> pair in PlayerCoroutines)
			{
				foreach (Coroutine coroutine in pair.Value)
				{
					Timing.KillCoroutines(coroutine.Handle);
				}
			}

			PlayerCoroutines.Clear();

			Coroutine.coroutines.Clear();
		}

		public struct Coroutine
		{
			public Coroutine(string id, IEnumerator<float> coroutine)
			{
				Id = id;
				Handle = Timing.RunCoroutine(coroutine);

				coroutines.Add(this);
			}

			public string Id { get; }
			public CoroutineHandle Handle { get; }

			internal static readonly List<Coroutine> coroutines = new List<Coroutine>();
		}
	}
}
