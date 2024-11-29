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
        public (LogicPlayer player, Choice[] choices)[] playerChoices;

        public LogicPlayer currentPlayer => players[currentPlayerIndex];
        public RoundInfo roundInfo => new RoundInfo
        {
            changFeng = (FengKind)round.ju,
            lastCardNum = paiShan.LastDrawCardCount,
            isNoBodyMingPai = players.All(_ => !_.ming.isMingPai)
        };

        public T GetPlayerChoice<T>(LogicPlayer player, ChoiceKind kind) where T : Choice
        {
            return playerChoices
                .First(_ => _.player.playerIndex == player.playerIndex)
                .choices.First(_ => _.kind == kind) as T;
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
                if (clientTingPaiChoice.cards.Length != 0)
                {
                    if (currentPlayer.ming.mingInfo.isMenQianQing)
                    {
                    //立直
                    choices.Add(ChoiceLiZhi.LiZhi(clientTingPaiChoice));
                    }
                    //自摸
                    if (tingPaiChoice.isHePai)
                    {
                        choices.Add(ChoiceZiMo.ZiMo());
                    }
                }
            }
            //暗杠或加杠
            if (paiShan.CanGang && currentPlayer.CheckDrawCardGang(out ChoiceGang choice))
            {
                choices.Add(choice);
            }
            //流局（九种九牌）
            if (
                currentPlayer.selfInfo.drewCardNum == 1
                && players.Count(_ => _.ming.Count() != 0) == 0
                && currentPlayer.CheckJiuZhongJiuPai()
            )
            {
                choices.Add(new ChoiceJiuZhongJiuPai());
            }
            return choices.ToArray();
        }
        public Choice[] GetChoiceAfterPlayCard(LogicPlayer other, LogicPlayer self, CardKind playedCard)
        {
            List<Choice> choices = new List<Choice>();
            //碰
            {
                if(self.CheckPeng(out ChoicePeng choice, self.playerIndex, playedCard))
                {
                    choices.Add(choice);
                }
            }
            //吃
            {
                if (
                    (other.playerIndex + 1) % DataManager.playerNum == self.playerIndex
                    && self.CheckChi(out ChoiceChi choice, self.playerIndex, playedCard)
                )
                {
                    choices.Add(choice);
                }
            }
            //明杠
            {
                if (paiShan.CanGang && self.CheckPlayCardGang(out ChoiceGang choice, playedCard))
                {
                    choices.Add(choice);
                }
            }
            //荣和
            {
                if (CheckRongHe(self, playedCard, false))
                {
                    choices.Add(ChoiceRongHe.RongHe());
                }
            }
            return choices.ToArray();
        }
        public Choice[] GetChoiceAfterMingMingCard(LogicPlayer player, LogicMingPaiGroup group)
        {
            List<Choice> choices = new List<Choice>();

            //打牌 食替相关 吃或碰了什么牌就不能在本回合打出相同的牌
            if (group.kind == MingPaiKind.Chi)
            {
                int pos = group.OtherCardPosition();
                if (pos == 0)
                {
                    if (group.otherCard.huaseNum + 3 <= 8)
                    {
                        choices.Add(ChoicePlayCard.BanPlayCard(
                            new CardKind[]
                            {
                                    group.otherCard,
                                    new CardKind(group.otherCard.huaseKind, group.otherCard.huaseNum + 3)
                            }
                        ));
                    }
                    else
                    {
                        choices.Add(ChoicePlayCard.BanPlayCard(
                            new CardKind[]
                            {
                                    group.otherCard
                            }
                        ));
                    }
                }
                else if (pos == 1)
                {
                    choices.Add(ChoicePlayCard.BanPlayCard(new CardKind[] { group.otherCard }));
                }
                else if (pos == 2)
                {
                    if (group.otherCard.huaseNum - 3 >= 0)
                    {
                        choices.Add(ChoicePlayCard.BanPlayCard(
                            new CardKind[]
                            {
                                    group.otherCard,
                                    new CardKind(group.otherCard.huaseKind, group.otherCard.huaseNum - 3)
                            }
                        ));
                    }
                    else
                    {
                        choices.Add(ChoicePlayCard.BanPlayCard(
                            new CardKind[]
                            {
                                    group.otherCard
                            }
                        ));
                    }
                }
            }
            else if (group.kind == MingPaiKind.Peng)
            {
                choices.Add(ChoicePlayCard.BanPlayCard(new CardKind[] { group.otherCard }));
            }
            else
            {
                choices.Add(ChoicePlayCard.NormalPlayCard());
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
            return result.tingPai.IsTingPai && !result.tingPai.IsWuYi && !player.zhenTingRecoder[card.huaseKind * 9 + card.huaseNum];
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
                //players[i] = new LogicPlayer(i, 5, 20, 35000);
                players[i] = new LogicPlayer(i, 1000, 1000, 35000);
            }

            liZhiNum = 0;
        }
        /// <summary>
        /// 一轮游戏开始事件
        /// </summary>
        public virtual void OnGameRoundStart()
        {
            paiShan = new LogicPaiShan(0);

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
        public virtual void OnPlayerRoundStart(LogicPlayer player, bool needDrawCard)
        {
            currentPlayerIndex = player.playerIndex;

            if (needDrawCard)
            {
                OnPlayerDrawCard(currentPlayer, paiShan.GetDrawCard(), false);
            }
            else
            {
                playerChoices = null;

                WaitPlayer<Action> waitPlayer = WaitPlayer<Action>.WaitForPlayerSelect(
                    player,
                    new ActionPlayCard(player.LastDrewCard)
                );
                wait = new Wait<Action>(
                    new WaitPlayer<Action>[] { waitPlayer },
                    _ =>
                    {
                        ProcessMingMingCardChoices(_[0].Item1, _[0].Item2);
                    }
                );

                LogicMingPaiGroup group = player.ming.groups.Last();

                Choice[] choices = GetChoiceAfterMingMingCard(player, group);

                playerChoices = new (LogicPlayer player, Choice[] choices)[]
                {
                    (player, choices)
                };

                OnSendPlayerChoice(player, wait.uuid, choices, true);

                wait.AlarmStartAll(this);
            }
        }
        private void ProcessDrawCardChoices(LogicPlayer player, Action action)
        {
            wait = null;

            switch (action.kind)
            {
                case ActionKind.PlayCard:
                    {
                        ActionPlayCard totalAction = action as ActionPlayCard;
                        OnPlayerPlayCard(player, totalAction.card, false);
                        break;
                    }
                case ActionKind.Gang:
                    {
                        ActionGang totalAction = action as ActionGang;
                        ChoiceGang data = GetPlayerChoice<ChoiceGang>(player, ChoiceKind.Gang);
                        OnPlayerMingCard(
                            player,
                            data.choices[totalAction.index].kind switch
                            {
                                ChoiceGang.GangKind.AnGang => MingPaiKind.AnGang,
                                    ChoiceGang.GangKind.JiaGang => MingPaiKind.JiaGang,
                                _ => throw new System.Exception("抽牌时不应存在明杠")
                            },
                            data.choices[totalAction.index].cards
                        );
                        break;
                    }
                case ActionKind.LiZhi:
                    {
                        ActionLiZhi totalAction = action as ActionLiZhi;
                        CardKind liZhiCard = totalAction.card;
                        OnPlayerPlayCard(player, liZhiCard, true);
                        break;
                    }
                case ActionKind.ZiMo:
                    {
                        break;
                    }
                default:
                    {
                        throw new System.Exception($"抽牌时不应存在该类型的操作{action.kind}");
                    } 
            }
        }//和 > 碰/杠 > 吃
        private void ProcessPlayCardChoices((LogicPlayer,Action)[] playerChoices)
        {
            wait = null;

            var ordered = playerChoices.OrderBy(_ =>
                _.Item2.kind switch
                {
                    ActionKind.RongHe => 1,
                    ActionKind.Gang => 2,
                    ActionKind.Peng => 3,
                    ActionKind.Chi => 4,
                    ActionKind.Skip => 5,
                    _ => throw new System.Exception("打牌时不应存在其他Action"),
                }
            );

            var firstChoice = ordered.First();

            switch (firstChoice.Item2.kind)
            {
                case ActionKind.RongHe:
                    {
                        break;
                    }
                case ActionKind.Gang:
                    {
                        ActionGang totalAction = firstChoice.Item2 as ActionGang;
                        ChoiceGang data = GetPlayerChoice<ChoiceGang>(firstChoice.Item1, ChoiceKind.Gang);
                        OnPlayerMingCard(firstChoice.Item1, MingPaiKind.MingGang, data.choices[totalAction.index].cards);
                        break;
                    }
                case ActionKind.Peng:
                    {
                        ActionPeng totalAction = firstChoice.Item2 as ActionPeng;
                        ChoicePeng data = GetPlayerChoice<ChoicePeng>(firstChoice.Item1, ChoiceKind.Peng);
                        OnPlayerMingCard(firstChoice.Item1, MingPaiKind.Peng, data.choices[totalAction.index]);
                        break;
                    }
                case ActionKind.Chi:
                    {
                        ActionChi totalAction = firstChoice.Item2 as ActionChi;
                        ChoiceChi data = GetPlayerChoice<ChoiceChi>(firstChoice.Item1, ChoiceKind.Chi);
                        OnPlayerMingCard(firstChoice.Item1, MingPaiKind.Chi, data.choices[totalAction.index]);
                        break;
                    }
                case ActionKind.Skip:
                    {
                        OnPlayerRoundEnd(currentPlayer);
                        break;
                    }
            }
        }
        private void ProcessMingMingCardChoices(LogicPlayer player, Action action)
        {
            wait = null;

            switch (action.kind)
            {
                case ActionKind.PlayCard:
                    {
                        ActionPlayCard totalAction = action as ActionPlayCard;
                        OnPlayerPlayCard(player, totalAction.card, false);
                        break;
                    }
            }
        }
        public virtual void OnPlayerDrawCard(LogicPlayer player, CardKind card, bool isLingShang) 
        {
            player.DrawCard(card);
        }
        public virtual void AfterPlayerDrawCard(LogicPlayer player, CardKind card, bool isLingShang)
        {
            WaitPlayer<Action> waitPlayer = WaitPlayer<Action>.WaitForPlayerSelect(
                player,
                new ActionPlayCard(player.LastDrewCard)
            );
            wait = new Wait<Action>(
                new WaitPlayer<Action>[] { waitPlayer },
                _ =>
                {
                    ProcessDrawCardChoices(_[0].Item1, _[0].Item2);
                }
            );

            Choice[] choices = GetChoiceAfterDrawCard(isLingShang);

            playerChoices = new (LogicPlayer player, Choice[] choices)[]
            {
                (player, choices)
            };

            OnSendPlayerChoice(player, wait.uuid, choices, true);

            wait.AlarmStartAll(this);
        }
        /// <summary>
        /// 玩家打出牌
        /// </summary>
        public virtual void OnPlayerPlayCard(LogicPlayer player, CardKind card, bool isLiZhi)
        {
            if (isLiZhi)
            {
                if (roundInfo.isNoBodyMingPai && player.selfInfo.drewCardNum == 1)
                {
                    player.selfInfo.isLiangLiZhi = true;
                }
                else
                {
                    player.selfInfo.isLiZhi = true;
                }
                player.selfInfo.hasYiFa = true;
            }

            player.PlayCard(card);
        }

        public virtual void AfterPlayerPlayCard(LogicPlayer player, CardKind card, bool isLiZhi)
        {
            List<(LogicPlayer, Choice[])> choices = new List<(LogicPlayer, Choice[])>();

            foreach (var exceptPlayer in players)
            {
                if (exceptPlayer.playerIndex != player.playerIndex)
                {
                    var temp = GetChoiceAfterPlayCard(player, exceptPlayer, card);

                    if (temp.Length != 0)
                    {
                        choices.Add((exceptPlayer, temp));
                    }
                }
            }

            playerChoices = null;

            if (choices.Count != 0)
            {
                wait = new Wait<Action>(
                    choices
                        .Select(_ => WaitPlayer<Action>.WaitForPlayerSelect(_.Item1, new ActionSkip()))
                        .ToArray(),
                    ProcessPlayCardChoices
                );

                playerChoices = choices.ToArray();

                choices.ForEach((_) => OnSendPlayerChoice(_.Item1, wait.uuid, _.Item2, false));

                wait.AlarmStartAll(this);
            }
            else
            {
                OnPlayerRoundEnd(player);
            }
        }

        public virtual void OnPlayerMingCard(LogicPlayer player, MingPaiKind kind, CardKind[] cards)
        {
            switch (kind)
            {
                case MingPaiKind.Peng:
                    {
                        LogicMingPaiGroup group = new LogicMingPaiGroup
                        {
                            kind = MingPaiKind.Peng,
                            otherCard = cards[2],
                            selfCard = cards.Take(2).ToArray(),
                            self2Otherdistance = IGameLogicManager.instance.GetPlayerDistance(player.playerIndex, currentPlayer.playerIndex),
                            signKind = cards[0].huaseKind,
                            signNum = cards[0].huaseNum
                        };
                        player.MingMingPai(currentPlayer, group);
                        break;
                    }
                case MingPaiKind.Chi:
                    {
                        CardKind signCard = cards.OrderBy(_ => _.huaseNum).First();
                        LogicMingPaiGroup group = new LogicMingPaiGroup
                        {
                            kind = MingPaiKind.Chi,
                            otherCard = cards[2],
                            selfCard = cards.Take(2).ToArray(),
                            self2Otherdistance = IGameLogicManager.instance.GetPlayerDistance(player.playerIndex, currentPlayer.playerIndex),
                            signKind = signCard.huaseKind,
                            signNum = signCard.huaseNum
                        };
                        player.MingMingPai(currentPlayer, group);
                        break;
                    }
                case MingPaiKind.MingGang:
                    {
                        LogicMingPaiGroup group = new LogicMingPaiGroup
                        {
                            kind = MingPaiKind.MingGang,
                            otherCard = cards[3],
                            selfCard = cards.Take(3).ToArray(),
                            self2Otherdistance = IGameLogicManager.instance.GetPlayerDistance(player.playerIndex, currentPlayer.playerIndex),
                            signKind = cards[0].huaseKind,
                            signNum = cards[0].huaseNum
                        };
                        player.MingMingPai(currentPlayer, group);
                        break;
                    }
                case MingPaiKind.AnGang:
                    {
                        LogicMingPaiGroup group = new LogicMingPaiGroup
                        {
                            kind = MingPaiKind.AnGang,
                            otherCard = new CardKind(-1),
                            selfCard = cards,
                            self2Otherdistance = IGameLogicManager.instance.GetPlayerDistance(player.playerIndex, currentPlayer.playerIndex),
                            signKind = cards[0].huaseKind,
                            signNum = cards[0].huaseNum
                        };
                        player.AnMingPai(group);
                        break;
                    }
                case MingPaiKind.JiaGang:
                    {
                        player.JiaGang(cards[3]);
                        break;
                    }
                case MingPaiKind.BaBei:
                    {
                        LogicMingPaiGroup group = new LogicMingPaiGroup
                        {
                            kind = MingPaiKind.BaBei,
                            otherCard = new CardKind(-1),
                            selfCard = cards,
                            self2Otherdistance = IGameLogicManager.instance.GetPlayerDistance(player.playerIndex, currentPlayer.playerIndex),
                            signKind = cards[0].huaseKind,
                            signNum = cards[0].huaseNum
                        };
                        player.AnMingPai(group);
                        break;
                    }
            }

            if(kind == MingPaiKind.AnGang)
            {
                player.selfInfo.hasYiFa = false;
            }
            else
            {
                players.Foreach((_, _index) =>
                {
                    if (_index != player.playerIndex)
                    {
                        _.selfInfo.hasYiFa = false;
                    }
                });
            }

        }

        public virtual void OnSendPlayerChoice(LogicPlayer player, long uuid, Choice[] choices, bool isDrawCard) { }

        public virtual void OnPlayerRoundEnd(LogicPlayer player)
        {
            DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
            {
                OnPlayerRoundStart(players[(currentPlayerIndex + 1) % DataManager.playerNum], true);
            });
        }
    }
}