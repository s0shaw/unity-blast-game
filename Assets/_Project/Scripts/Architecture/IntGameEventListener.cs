using UnityEngine;
using UnityEngine.Events;

namespace GemBlast.Architecture
{
    [System.Serializable]
    public class UnityIntEvent : UnityEvent<int> { }

    public class IntGameEventListener : MonoBehaviour
    {
        public IntGameEvent Event;
        public UnityIntEvent Response;

        private void OnEnable()
        {
            if (Event != null) Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            if (Event != null) Event.UnregisterListener(this);
        }

        public void OnEventRaised(int value)
        {
            Response?.Invoke(value);
        }
    }
}
