using DG.Tweening;
using UnityEngine;

namespace Data
{
    public class Wait<T>
    {
        public static long uuidGenerator = long.MinValue;
        public long uuid;
        public delegate void CompleteSingleEvent(T data);
        public delegate void CompleteEvent(T[] data);
        private WaitPlayer<T>[] waits;
        private CompleteEvent onCompleteEvent;
        private float startTime;
        private T[] completedData;
        private int completedNum;

        public Wait(WaitPlayer<T>[] waits, CompleteEvent onCompleteEvent)
        {
            uuid = uuidGenerator++;
            startTime = Time.realtimeSinceStartup;
            this.waits = waits;
            completedData = new T[this.waits.Length];
            this.onCompleteEvent = onCompleteEvent;
            for (int i = 0; i < waits.Length; ++i)
            {
                waits[i].Init(this, i);
            }
            completedNum = 0;
        }
        public void AlarmStartAll(MonoBehaviour mono)
        {
            foreach(WaitPlayer<T> wait in this.waits)
            {
                mono.StartCoroutine(wait.PlayerOutTimeAlarm());
            }
        }
        public float CalculateUsedTime()
        {
            return Time.realtimeSinceStartup - startTime;
        }
        public void OnSingleComplete(int index, T data)
        {
            completedData[index] = data;
            completedNum += 1;
            if(completedNum == waits.Length)
            {
                OnComplete();
            }
        }
        public void OnComplete()
        {
            onCompleteEvent?.Invoke(completedData);
        }
    }
}