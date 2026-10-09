using System.Collections.Generic;
using UnityEngine;

namespace GemBlast.Architecture
{
    [CreateAssetMenu(menuName = "GemBlast/Events/Int Game Event")]
    public class IntGameEvent : GameEvent
    {
        private readonly List<IntGameEventListener> intListeners = new List<IntGameEventListener>();

        public void Raise(int value)
        {
            base.Raise();
            
            for (int i = intListeners.Count - 1; i >= 0; i--)
            {
                intListeners[i].OnEventRaised(value);
            }
        }

        public void RegisterListener(IntGameEventListener listener)
        {
            if (!intListeners.Contains(listener))
                intListeners.Add(listener);
        }

        public void UnregisterListener(IntGameEventListener listener)
        {
            if (intListeners.Contains(listener))
                intListeners.Remove(listener);
        }
    }
}
