using Data;
using DG.Tweening;
using GameLogic;
using GameSceneUI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Card
{
    public class GamePanel : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        public UserInfoItem[] userInfoItems;
        public HandCardUI handCard;
        public Text liZhiNumText;
        public Text changGongNumText;
        public BaoPaiItem[] baoPaiItems;
        public Text roundTimeText;
        public Text globalTimeText;
        public Coroutine alarm;
        public bool isInteractable;

        public ChoicePanel choicePanel;
        public CardChoicePanel cardChoicePanel;
        public Button cancelButton;

        public long uuid;

        private void Awake()
        {
            ClearAlarm();
            uuid = -1;
            SetAlarmText(0, 0);
            choicePanel.Close();
            cardChoicePanel.Close();
        }

        public void SubmitActionPlayCard(UICard card)
        {
            if (SubmitAction(new ActionPlayCard(card.faceKind)))
            {
                handCard.lastCard = card;
            }
        }
        public bool SubmitAction(Action action)
        {
            long tempUuid = uuid;
            uuid = -1;
            if (tempUuid == -1)
            {
                Debug.Log("客户端UUID未通过检测");
                return false;
            }

            ClearAlarm();

            IGameLogicManager.instance.SubmitAction(tempUuid, action);

            return true;
        }
        public void InitChoices(long uuid,  Choice[] choices)
        {
            this.uuid = uuid;
            foreach(var choice in choices)
            {
                switch (choice.kind)
                {
                    case ChoiceKind.PlayCard:
                        {
                            handCard.BanCard(choice as ChoicePlayCard);
                            break;
                        }
                }
            }
        }
        public void ClearAlarm()
        {
            if(alarm != null)
            {
                SetAlarmText(0, 0);
                StopCoroutine(alarm);
                alarm = null;
            }
        }
        private IEnumerator DOAlarm(float roundTime, float globalTime)
        {
            isInteractable = true;
            int currentRoundTime = Mathf.CeilToInt(roundTime), currentGlobalTime = Mathf.CeilToInt(globalTime);
            SetAlarmText(currentRoundTime, currentGlobalTime);
            float startTime = Time.realtimeSinceStartup;
            while (true)
            {
                if(currentRoundTime != 0)
                {
                    yield return new WaitUntil(() => currentRoundTime != Mathf.CeilToInt(roundTime - (Time.realtimeSinceStartup - startTime)));
                    Debug.Log($"currentRoundTime = {currentRoundTime}\nroundTime - (Time.realtimeSinceStartup - startTime)) = {roundTime - (Time.realtimeSinceStartup - startTime)}");
                    currentRoundTime = Mathf.CeilToInt(roundTime - (Time.realtimeSinceStartup - startTime));
                    SetAlarmText(currentRoundTime, currentGlobalTime);
                }
                else if(currentGlobalTime != 0)
                {
                    yield return new WaitUntil(() => currentGlobalTime != Mathf.CeilToInt(roundTime + globalTime - (Time.realtimeSinceStartup - startTime)));
                    currentGlobalTime = Mathf.CeilToInt(roundTime + globalTime - (Time.realtimeSinceStartup - startTime));
                    SetAlarmText(currentRoundTime, currentGlobalTime);
                }
                else
                {
                    isInteractable = false;
                    SetAlarmText(0, 0);
                    alarm = null;
                    break;
                }
            }
            
        }
        public void SetAlarm(float roundTime, float globalTime) 
        {
            alarm = StartCoroutine(DOAlarm(roundTime, globalTime));
        }
        public void SetAlarmText(int roundTime, int globalTime)
        {
            if (roundTime == 0)
            {
                roundTimeText.text = "";
                if (globalTime == 0)
                {
                    globalTimeText.text = "";
                }
                else
                {
                    globalTimeText.text = globalTime.ToString();
                    globalTimeText.rectTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                    globalTimeText.rectTransform.anchoredPosition = new Vector2(0, 3.7f);
                }
            }
            else
            {
                roundTimeText.text = roundTime.ToString();
                if (globalTime == 0)
                {
                    globalTimeText.text = "";
                }
                else
                {
                    globalTimeText.text = $"+{globalTime}";
                    globalTimeText.rectTransform.localScale = new Vector3(0.625f, 0.625f, 0.625f);
                    globalTimeText.rectTransform.anchoredPosition = new Vector2(0, -35.67f);
                }
            }
        }
        public void SyncName(string[] messages)
        {
            for (int i = 0; i < messages.Length; ++i)
            {
                userInfoItems[i].nameText.text = messages[IGameLogicManager.instance.GetRelativeplayerIndex(i)];
            }
        }
        public void SyncChang(int chang)
        {
            changGongNumText.text = (chang - 1).ToString();
        }
        public void SyncLiZhiNum(int liZhiNum)
        {
            liZhiNumText.text = liZhiNum.ToString();
        }
        public void SyncBaoPai(CardKind[] baoPais)
        {
            for (int i = 0; i < baoPais.Length; ++i)
            {
                baoPaiItems[i].ShowBaoPai(baoPais[i]);
            }
            for (int i = baoPais.Length; i < baoPaiItems.Length; ++i)
            {
                baoPaiItems[i].Clear();
            }
            handCard.SyncBaoPai();
        }
        public void Open()
        {
            canvasGroup.Open();
        }
        public void Close()
        {
            canvasGroup.Close();
        }
    }
}