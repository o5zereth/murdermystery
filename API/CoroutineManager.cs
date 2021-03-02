using Exiled.API.Features;
using MEC;
using System.Collections.Generic;

namespace MurderMystery.API
{
    public class CoroutineManager
    {
        internal CoroutineManager() { }

        public List<CoroutineHandle> ServerCoroutines { get; } = new List<CoroutineHandle>();
        public Dictionary<MMPlayer, List<CoroutineHandle>> PlayerCoroutines { get; } = new Dictionary<MMPlayer, List<CoroutineHandle>>();
    }
}