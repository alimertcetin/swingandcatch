using System.Collections.Generic;
using UnityEngine;

namespace XIV.EventSystem
{
    public static class XIVEventSystem
    {
        class EventHelperMono : MonoBehaviour
        {
            static EventHelperMono instance;
            public static EventHelperMono Instance
            {
                get
                {
                    if (instance) return instance;
                    
                    instance = new GameObject("XIVEventHelper").AddComponent<EventHelperMono>();
                    DontDestroyOnLoad(instance);
                    return instance;
                }
            }
            
            public List<IEvent> events = new List<IEvent>();

            void Update()
            {
                for (int i = events.Count - 1; i >= 0; i--)
                {
                    var @event = events[i];
                    @event.Update(Time.deltaTime);
                    if (@event.IsDone())
                    {
                        @event.Complete();
                        events.RemoveAt(i);
                    }
                }
            }

            void OnDestroy()
            {
                instance = null;
            }
        }

        public static void SendEvent(IEvent @event)
        {
            EventHelperMono.Instance.events.Add(@event);
        }

        public static void CancelEvent(IEvent @event)
        {
            List<IEvent> events = EventHelperMono.Instance.events;
            int index = events.IndexOf(@event);
            if (index < 0) return;
            events[index].Cancel();
            events.RemoveAt(index);
        }
        
        public static T GetEvent<T>() where T : IEvent
        {
            List<IEvent> events = EventHelperMono.Instance.events;
            int count = events.Count;
            for (int i = 0; i < count; i++)
            {
                if (events[i] is T @event) return @event;
            }

            return default;
        }
    }
}