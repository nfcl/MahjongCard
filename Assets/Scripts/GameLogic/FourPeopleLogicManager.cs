using Checker;
using Data;
using DG.Tweening;
using Mirror;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class FourPeopleLogicManager : NetworkBehaviour
    {
        public LogicPaiShan paiShan;
        public LogicPlayer[] players;
        public (FengKind feng, int ju, int chang) round;
        public int currentPlayerIndex;
        public int liZhiNum;
        public Wait<Action> wait;

        public LogicPlayer currentPlayer => players[currentPlayerIndex];
        public RoundInfo roundInfo => new RoundInfo
        {
            changFeng = (FengKind)round.ju,
            lastCardNum = paiShan.LastDrawCardCount,
            isNoBodyMingPai = players.All(_ => !_.ming.isMingPai)
        };

        public void NextPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        }
        public Choice[] GetChoiceAfterDrawCard(bool isLingShang)
        {
            List<Choice> choices = new List<Choice>
            {
                //打牌
                ChoicePlayCard.NormalPlayCard()
            };
            {
                var tingPaiChoice = EachHandCardTingPaiResult.Check(
                    new CheckInfo
                    {
                        roundInfo = roundInfo,
                        selfInfo = currentPlayer.selfInfo,
                        mingInfo = currentPlayer.ming.mingInfo,
                        hePaiInfo = new HePaiInfo { isLingShangPai = isLingShang, isRongHe = false }
                    },
                    currentPlayer.hand.Cards, currentPlayer.ming.Groups, Yi.Default
                );
                ClientEachCardTingPais clientTingPaiChoice = ClientEachCardTingPais.Create(
                    tingPaiChoice,
                    1,
                    _ => CountPlayerLastCardNum(currentPlayerIndex, _),
                    (_, __) => currentPlayer.zhenTingRecoder[_ * 9 + __]
                );
                //立直
                choices.Add(ChoiceLiZhi.LiZhi(clientTingPaiChoice));
                //自摸
                if (tingPaiChoice.isHePai)
                {
                    choices.Add(ChoiceZiMo.ZiMo());
                }
            }
            //暗杠或加杠
            if (paiShan.CanGang && currentPlayer.CheckDrawCardGang(out ChoiceGang choice))
            {
                choices.Add(choice);
            }
            //流局（九种九牌）
            if (currentPlayer.selfInfo.drewCardNum == 1 && players.Count(_ => _.ming.Count() != 0) == 0)
            {
                choices.Add(new ChoiceJiuZhongJiuPai());
            }
            return choices.ToArray();
        }
        public Choice[] GetChoiceAfterPlayCard(LogicPlayer player, CardKind playedCard)
        {
            List<Choice> choices = new List<Choice>();
            //碰
            {
                if(player.CheckPeng(out ChoicePeng choice, player.playerIndex, playedCard))
                {
                    choices.Add(choice);
                }
            }
            //吃
            {
                if (player.CheckChi(out ChoiceChi choice, player.playerIndex, playedCard))
                {
                    choices.Add(choice);
                }
            }
            //明杠
            {
                if (paiShan.CanGang && player.CheckPlayCardGang(out ChoiceGang choice, player.playerIndex, playedCard))
                {
                    choices.Add(choice);
                }
            }
            //荣和
            {
                if (CheckRongHe(player, playedCard, false))
                {
                    choices.Add(ChoiceRongHe.RongHe());
                }
            }
            return choices.ToArray();
        }
        public int CountPlayerLastCardNum(int playerIndex, CardKind card)
        {
            return 4 - players.Sum(_ =>
            {
                return _.ming.groups.Sum(_ => _.CountCardNum(card))
                    + _.paiHe.CountCardNum(card)
                    + _.playerIndex == playerIndex ? _.hand.CountCardNum(card) : 0;
            }) - paiShan.GetBaoPai().Count(_ => _.huaseKind == card.huaseKind && _.huaseNum == card.huaseNum);
        }
        public bool CheckRongHe(LogicPlayer player, CardKind card, bool isLingShang)
        {
            var hand = player.hand.Cards.Append(card).ToArray();
            var result = EachDrewCardTingPaiResult.Check(
                new CheckInfo
                {
                    roundInfo = roundInfo,
                    selfInfo = currentPlayer.selfInfo,
                    mingInfo = currentPlayer.ming.mingInfo,
                    hePaiInfo = new HePaiInfo { isLingShangPai = isLingShang, isRongHe = true }
                },
                hand,
                player.ming.Groups,
                card,
                Yi.Default
            );
            return !result.tingPai.IsWuYi && !player.zhenTingRecoder[card.huaseKind * 9 + card.huaseNum];
        }

        /// <summary>
        /// 游戏开始事件
        /// </summary>
        public virtual void OnGameStart()
        {
            round = (FengKind.Dong, 1, 1);

            players = new LogicPlayer[4];
            for (int i = 0; i < players.Length; ++i)
            {
                players[i] = new LogicPlayer(i, 5, 20, 35000);
            }

            liZhiNum = 0;
        }
        /// <summary>
        /// 一轮游戏开始事件
        /// </summary>
        public virtual void OnGameRoundStart()
        {
            paiShan = new LogicPaiShan();

            for (int i = 0; i < players.Length; ++i)
            {
                players[i].RoundStart((FengKind)((DataManager.playerNum + i - round.ju) % DataManager.playerNum), i == round.ju);
            }

            currentPlayerIndex = round.ju;
        }
        /// <summary>
        /// 配牌事件
        /// </summary>
        public virtual void OnConfigurCard()
        {
            for (int i = 0; i < players.Length; ++i)
            {
                players[i].ConfigurCard(paiShan.GetConfigurCard());
            }
        }
        /// <summary>
        /// 玩家回合开始事件
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnPlayerRoundStart()
        {
            OnPlayerDrawCard(currentPlayer, paiShan.GetDrawCard(), false);
        }
        private void ProcessDrawCardChoices(LogicPlayer player, Action action)
        {
            wait = null;

            switch (action.kind)
            {
                case ActionKind.None:
                    {
                        break;
                    }
                case ActionKind.PlayCard:
                    {
                        ActionPlayCard resultAction = action as ActionPlayCard;
                        OnPlayerPlayCard(player, resultAction.card, false);
                        break;
                    }
            }
        }//和 > 碰 = 杠 > 吃
        private void ProcessPlayCardChoices((LogicPlayer,Action)[] playerChoices)
        {
            wait = null;

        }
        public virtual void OnPlayerDrawCard(LogicPlayer player, CardKind card, bool isLingShang) 
        {
            player.DrawCard(card);

            WaitPlayer<Action> waitPlayer = WaitPlayer<Action>.WaitForPlayerSelect(
                currentPlayer,
                new ActionPlayCard(currentPlayer.LastDrewCard)
            );
            wait = new Wait<Action>(
                new WaitPlayer<Action>[] { waitPlayer },
                _ =>
                {
                    ProcessDrawCardChoices(_[0].Item1, _[0].Item2);
                }
            );

            OnSendPlayerChoice(player, wait.uuid, GetChoiceAfterDrawCard(isLingShang), true);

            wait.AlarmStartAll(this);
        }
        /// <summary>
        /// 玩家打出牌
        /// </summary>
        public virtual void OnPlayerPlayCard(LogicPlayer player, CardKind card, bool isLiZhi)
        {
            player.PlayCard(card);

            List<(LogicPlayer, Choice[])> choices = new List<(LogicPlayer, Choice[])>();

            foreach(var exceptPlayer in players)
            {
                if (exceptPlayer.playerIndex != player.playerIndex)
                {
                    var temp = GetChoiceAfterPlayCard(exceptPlayer, card);

                    if (temp.Length != 0)
                    {
                        choices.Add((exceptPlayer, temp));
                    }
                }
            }

            if (choices.Count != 0)
            {
                wait = new Wait<Action>(
                    choices
                        .Select(_ => WaitPlayer<Action>.WaitForPlayerSelect(_.Item1, new ActionSkip()))
                        .ToArray(),
                    ProcessPlayCardChoices
                );

                choices.ForEach((_) => OnSendPlayerChoice(_.Item1, wait.uuid, _.Item2, false));

                wait.AlarmStartAll(this);
            }
            else
            {
                OnPlayerRoundEnd(player);
            }

        }
        public virtual void OnSendPlayerChoice(LogicPlayer player, long uuid, Choice[] choices, bool isDrawCard) { }

        public virtual void OnPlayerRoundEnd(LogicPlayer player)
        {
            NextPlayer();

            DOTween.Sequence().AppendInterval(2).AppendCallback(() =>
            {
                OnPlayerRoundStart();
            });
        }
    }
}