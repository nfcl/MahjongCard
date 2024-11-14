using System.Collections;
using UnityEngine;

namespace Data
{
    public class WaitPlayer<T>
    {
        private Wait<T> wait;
        private int index;
        private Wait<T>.CompleteSingleEvent onCompleteEvent;
        private Wait<T>.CompleteSingleEvent onOutTimeEvent;
        public LogicPlayer player;
        public T defaultData;
        public bool isComplete;

        public WaitPlayer(LogicPlayer player, T defaultData)
        {
            this.player = player;
            this.defaultData = defaultData;
            isComplete = false;
        }

        public void Init(Wait<T> wait, int index)
        {
            this.wait = wait;
            this.index = index;
        }

        public void CalculateLastTime()
        {
            float usedTime = wait.CalculateUsedTime();
            float usedGlobalTime = usedTime - player.roundWaitTime;
            if (usedGlobalTime > 0)
            {
                player.globalWaitTime -= usedGlobalTime;
            }
        }

        public void OnComplete(T data)
        {
            isComplete = true;
            onCompleteEvent?.Invoke(data);
            wait.OnSingleComplete(index, data);
        }
        public void OnOutTime()
        {
            isComplete = true;
            onOutTimeEvent?.Invoke(defaultData);
            wait.OnSingleComplete(index, defaultData);
        }
        public IEnumerator PlayerOutTimeAlarm()
        {
            yield return new WaitForSecondsRealtime(player.roundWaitTime + player.globalWaitTime + 0.5f);
            if (!isComplete)
            {
                OnOutTime();
            }
        }

        public static WaitPlayer<T> WaitForPlayerSelect(LogicPlayer player, T defaultData)
        {
            WaitPlayer<T> newWait = new WaitPlayer<T>(player, defaultData);
            newWait.onCompleteEvent = _ =>
            {
                newWait.CalculateLastTime();
            };
            newWait.onOutTimeEvent = _ =>
            {
                newWait.CalculateLastTime();
            };
            return newWait;
        }
    }
}