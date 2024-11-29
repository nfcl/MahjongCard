using Data;
using GameLogic;
using GameSceneUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public Button liZhiCancelButton;
        public Button tingPaiShowButton;

        public long uuid;
        public ChoiceKind currentMode = ChoiceKind.None;
        public ChoicePlayCard playCardChoice;
        public ClientEachCardTingPais tingPaiChoices;

        private void Awake()
        {
            ClearAlarm();
            uuid = -1;
            SetAlarmText(0, 0);
            choicePanel.Close();
            cardChoicePanel.Close();
            liZhiCancelButton.gameObject.SetActive(false);
            tingPaiShowButton.gameObject.SetActive(false);
        }

        public void CloseTingPai()
        {
            cardChoicePanel.Close();
        }
        public bool ShowTingPai(CardKind card)
        {
            if (tingPaiChoices.cards.Count(_ => CardKind.LogicEqualityComparer.Equals(card, _.playCard)) != 0)
            {

                cardChoicePanel.Open(tingPaiChoices.cards.First(_ => CardKind.LogicEqualityComparer.Equals(card, _.playCard)));

                return true;
            }
            return false;
        }
        public void CancelLiZhi()
        {
            if(currentMode == ChoiceKind.LiZhi)
            {
                if (playCardChoice == null)
                {
                    currentMode = ChoiceKind.None;
                    handCard.BanCard();
                }
                else
                {
                    currentMode = ChoiceKind.PlayCard;
                    handCard.BanCard(playCardChoice);
                }
            }
            liZhiCancelButton.gameObject.SetActive(false);
        }
        public void SubmitActionPlayCard(UICard card)
        {
            if (currentMode == ChoiceKind.PlayCard)
            {
                if (SubmitAction(new ActionPlayCard(card.faceKind)))
                {
                    handCard.lastCard = card;
                }
            }
            else if (currentMode == ChoiceKind.LiZhi)
            {
                liZhiCancelButton.gameObject.SetActive(false);
                if (SubmitAction(new ActionLiZhi(card.faceKind)))
                {
                    handCard.lastCard = card;
                }
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

            currentMode = ChoiceKind.None;
            choicePanel.Close();
            cardChoicePanel.Close();
            handCard.UnBanCard();

            ClearAlarm();

            IGameLogicManager.instance.SubmitAction(tempUuid, action);

            return true;
        }
        public void InitChoices(long uuid,  Choice[] choices, bool isDrawCard)
        {
            tingPaiShowButton.gameObject.SetActive(false);
            cardChoicePanel.Close();

            this.uuid = uuid;

            List<(ChoiceKind, MainSceneUI.PropPanel.D_Void_Void)> data = new List<(ChoiceKind, MainSceneUI.PropPanel.D_Void_Void)>();

            if (isDrawCard)
            {
                data.Add((ChoiceKind.Skip, () => choicePanel.Close()));
            }
            else
            {
                data.Add((ChoiceKind.Skip, () => { choicePanel.Close(); SubmitAction(new ActionSkip()); }));
            }

            playCardChoice = choices.FirstOrDefault(_ => _.kind == ChoiceKind.PlayCard) as ChoicePlayCard;

            if (playCardChoice == null)
            {
                handCard.BanCard();
                currentMode = ChoiceKind.None;
            }
            else
            {
                handCard.BanCard(playCardChoice);
                currentMode = ChoiceKind.PlayCard;
            }

            bool needShowChoicesPanel = false;

            foreach (var choice in choices)
            {
                switch (choice.kind)
                {
                    case ChoiceKind.LiZhi:
                        {
                            needShowChoicesPanel = true;
                            data.Add((
                                ChoiceKind.LiZhi,
                                () =>
                                {
                                    ChoiceLiZhi totalChoice = choice as ChoiceLiZhi;
                                    tingPaiChoices = totalChoice.choices;
                                    handCard.BanCard(totalChoice);
                                    currentMode = ChoiceKind.LiZhi;
                                    choicePanel.Close();
                                    liZhiCancelButton.gameObject.SetActive(true);
                                }
                            ));
                            break;
                        }
                    case ChoiceKind.Gang:
                        {
                            needShowChoicesPanel = true;
                            data.Add((
                                ChoiceKind.Gang,
                                () =>
                                {
                                    ChoiceGang totalChoice = choice as ChoiceGang;
                                    if (totalChoice.choices.Length == 1)
                                    {
                                        SubmitAction(new ActionGang(0));
                                    }
                                    else
                                    {
                                        cardChoicePanel.Open(
                                            totalChoice.choices.Select(_ => _.cards).ToArray(),
                                            (_) =>
                                            {
                                                currentMode = ChoiceKind.None;
                                                SubmitAction(new ActionGang(_));
                                            });
                                        handCard.BanCard();
                                        currentMode = ChoiceKind.Gang;
                                    }
                                }
                            ));
                            break;
                        }
                    case ChoiceKind.Peng:
                        {
                            needShowChoicesPanel = true;
                            data.Add((
                                ChoiceKind.Peng,
                                () =>
                                {
                                    ChoicePeng totalChoice = choice as ChoicePeng;
                                    if (totalChoice.choices.Length == 1)
                                    {
                                        SubmitAction(new ActionPeng(0));
                                    }
                                    else
                                    {
                                        cardChoicePanel.Open(
                                            totalChoice.choices,
                                            _ =>
                                            {
                                                currentMode = ChoiceKind.None;
                                                SubmitAction(new ActionPeng(_));
                                            });
                                        handCard.BanCard();
                                        currentMode = ChoiceKind.Peng;
                                    }
                                }
                            ));
                            break;
                        }
                    case ChoiceKind.Chi:
                        {
                            needShowChoicesPanel = true;
                            data.Add((
                                ChoiceKind.Chi,
                                () =>
                                {
                                    ChoiceChi totalChoice = choice as ChoiceChi;
                                    if (totalChoice.choices.Length == 1)
                                    {
                                        SubmitAction(new ActionChi(0));
                                    }
                                    else
                                    {
                                        cardChoicePanel.Open(
                                            totalChoice.choices,
                                            _ =>
                                            {
                                                currentMode = ChoiceKind.None;
                                                SubmitAction(new ActionChi(_));
                                            });
                                        handCard.BanCard();
                                        currentMode = ChoiceKind.Chi;
                                    }
                                }
                            ));
                            break;
                        }
                    case ChoiceKind.JiuZhongJiuPai:
                        {
                            needShowChoicesPanel = true;
                            data.Add((
                                ChoiceKind.JiuZhongJiuPai,
                                () =>
                                {
                                    currentMode = ChoiceKind.None;
                                    SubmitAction(new ActionJiuZhongJiuPai());
                                }
                            ));
                            break;
                        }
                    case ChoiceKind.ZiMo:
                        {
                            needShowChoicesPanel = true;
                            data.Add((
                                ChoiceKind.ZiMo,
                                () =>
                                {
                                    currentMode = ChoiceKind.None;
                                    SubmitAction(new ActionZimo());
                                }
                            ));
                            break;
                        }
                    case ChoiceKind.RongHe:
                        {
                            needShowChoicesPanel = true;
                            data.Add((
                                ChoiceKind.RongHe,
                                () =>
                                {
                                    SubmitAction(new ActionRongHe());
                                }
                            ));
                            break;
                        }
                }
            }

            if (needShowChoicesPanel)
            {
                choicePanel.Open(data.ToArray());
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
                    //Debug.Log($"currentRoundTime = {currentRoundTime}\nroundTime - (Time.realtimeSinceStartup - startTime)) = {roundTime - (Time.realtimeSinceStartup - startTime)}");
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