using DG.Tweening;
using System.Linq;
using UnityEngine;

namespace Data
{
    public class Wait<T>
    {
        public static long uuidGenerator = long.MinValue;
        public long uuid;
        public delegate void CompleteSingleEvent(T data);
        public delegate void CompleteEvent((LogicPlayer, T)[] data);
        private WaitPlayer<T>[] waits;
        private CompleteEvent onCompleteEvent;
        private float startTime;
        private T[] completedData;

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
        public (bool result, string reason) PlayerComplete(LogicPlayer player, T data)
        {
            Debug.Log("TryPlayerComplete");

            WaitPlayer<T> wait = waits.Where(_ => _.player.playerIndex == player.playerIndex).FirstOrDefault();

            Debug.Log("FindWait");

            if (wait == null)
            {
                Debug.Log("不在等待列表的玩家提交数据");
                return (false, "不在等待列表的玩家提交数据");
            }
            else if (wait.isComplete)
            {
                Debug.Log("玩家已提交过数据");
                return (false, "玩家已提交过数据");
            }

            Debug.Log($"{waits.Count(_ => _.isComplete)} / {waits.Length}");

            wait.OnComplete(data);

            Debug.Log($"{waits.Count(_ => _.isComplete)} / {waits.Length}");
            
            return (true, null);
        }
        public void OnSingleComplete(int index, T data)
        {
            completedData[index] = data;
            if (waits.Count(_ => _.isComplete) == waits.Length)
            {
                OnComplete();
            }
        }
        public void OnComplete()
        {
            Debug.Log("所有玩家完成提交");
            onCompleteEvent?.Invoke(completedData.Select((_, index) => (waits[index].player, _)).ToArray());
        }
    }
}