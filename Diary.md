# 日记捏

_好唐的小标题_

## 2024年

### 2024年11月

#### 2024年11月6日

项目启动，实际上几天前已经启动了，但是为了删运行文件夹（不小心）把整个UnityProject整个Shift+Delete了。

于是乎重新开了个项目并预先上传git，然后在这写写README。

事实上删的那个本来也搞得不是很爽外加有点架构小问题，因此这种行为也许被潜意识默许了（×。

反正现在又是两手空空的开始了。

目前的计划是先完成主场景玩家登录，加入房间等。

（刚刚又不小心习惯的Ctrl+A+X了）现在先不加美术素材什么的只进行技术验证之类的。

#### 2024年11月8日

其实应该是7日的（更新这段的时候刚好差不多0点）。

已经整到玩家加入房间的逻辑了，进展比较顺利。

测试了一下也没出现之前实机运行的Host无法被编辑器内运行的Client搜索到的情况了（希望后面也不要出现）。

不过现在加了版本管理的话就算出现了还能倒退版本看看到底怎么个事（嘻）。

现在是8日的晚上（已经回到家了）。

终于发现了困扰我很久的（包括之前被删项目的）一个离奇问题，那就是RoomPlayer单例的设置和销毁逻辑。

由于RoomPlayer会在同一个场景生成多个，但我又天真的还在用Awake设置。因此每有一个新玩家加入就会导致单例的被重新设置为新加入的玩家的RoomPlayer

错误的逻辑↓
```C#

    private void Awake()
    {
        instance = this;
    }
    private void OnDestroy()
    {
        instance = null;
    }

```

正确的逻辑应该为同时判断IsLocalPlayer和IsOwned（好像其中一个也行），这样才能保证RoomPlayer的单例始终保持在自身客户端的RoomPlayer上。

正确的逻辑↓
```C#

    private void OnDestroy()
    {
        if(isLocalPlayer && isOwned)
        {
            instance = null;
        }
    }

    public override void OnStartClient()
    {
        ...

        if (isLocalPlayer)
        {
            if (this.isOwned)
            {
                instance = this;
            }
            ...
        }
    }

```

目前已经算是把房间功能做好了，接下来就可以开始动手从房间跳转到游戏的逻辑了。

#### 2024年11月10日

目前是10日0点刚出头，忙活了一下午+晚上去把雀魂的场景搬下来。但是由于网页版雀魂是由LayaAir制作的，因此模型还扒不下来，只能自己动手做了一个。

然后对着原场景一步步还原（类似了），目前把牌河，手牌（非UI），中间那个记录信息的盘子还原了，不过有些小出入。

比较难受的大概是因为技术不足，实体牌的贴图实际上是分成了一个Model贴牌背什么的，然后一个SpriteRender显示牌面。牌面资源倒是提供了透明和非透明的版本。

但是！但是非透明的版本底色居然不是白色的。不想扣颜色的我选择了透明的版本，然后发现由于本身它一张牌的图像大小就很小，部分牌（特别是九筒）线条很细，真正放到SpriteRender上的时候颜色很淡，非常影响观感...

后续也许可以试试PS把透明度拉高点。

（PS:上面那句话说完就去试了试效果还真不错）

#### 2024年11月12日

嘻，又是这种时候写捏。

最近两天忙于框架搭建所以没空更新，目前对游戏逻辑框架的构思是这样的↓

IGameLogicManager           一个接口供外部调用。

FourPeopleLogicManager      逻辑处理器用于游戏逻辑的处理。

FourPeopleManager           功能处理器继承了前两者用于实现服务端和客户端的功能对接。

由于构想中需要游戏场景复用多个游戏模式，因此必然不可能将单例放在后两者上，所以我单独拖出来了一个接口供外部调用。

唯一需要注意的大概就是接口里同时含有服务端和客户端的方法，需要区分(不过现在也没什么服务端)。

然后是Wait和WaitPlayer这两个，我预想中是为了方便统一倒计时和玩家选择的实现。

WaitPlayer有OnComplete和OnOutTime两种方法向Wait提交数据，前者由外部调用外部提供数据提交，后者由定时器到点自动提交预设的默认数据。

每次Wait收到提交后保存数据并判断是否所有玩家都提交了数据，如果是便通过onCompleteEvent向逻辑处理器提交数据并处理。

目前如何优雅的让外部调用WaitPlayer的OnComplete还在想。

为了保证不会产生过时的提交（预想中Wait只在逻辑处理器中有一个，如果后面保持这种模式的话，可能会产生信号差的客户端在过一段时间后提交数据给新的Wait实例的情况），我为Wait设置了一个UUID，到时候会在给客户端发送选择时将这个UUID也发过去，这样提交时只需将UUID进行对比即可筛选掉。

好，就是这样，嘻

#### 2024年11月14日

依旧是这个点...

今天把倒计时和玩家选择的逻辑大致实现了一下。

目前将其分为：Choice 和 Action。

服务端在玩家进行了某些操作（如抽牌和打牌）后，在逻辑处理器计算每位玩家可能的操作Choice并集合在一个数组发送给对应的玩家。

这个Choice数组将由客户端UI表达，玩家选择了某个Choice后会生成Action数据提交给服务端，再由服务端统一提交给Wait的OnComplete回调完成一次玩家操作。

今天发现的一个比较严重的疏忽是所有的Mirror网络数据类的自定义序列化方法因为读写器类型没有加this前缀导致Weaver根本没有使用这些方法而是自动生成的方法（因为都是由基础类型组合的）。

发现这个问题还是因为ChoicePlayCard是继承自Choice类的。

```C#

    public class Choice
    {
        public ChoiceKind kind;

        public Choice()
        {
            kind = ChoiceKind.None;
        }
        public Choice(ChoiceKind kind)
        {
            this.kind = kind;
        }
    }
    public class ChoicePlayCard : Choice
    {
        public bool isWhite;
        public CardKind[] cards;

        public ChoicePlayCard() : base(ChoiceKind.PlayCard) { }

        public static ChoicePlayCard NormalPlayCard()
        {
            return new ChoicePlayCard()
            {
                cards = new CardKind[0],
                isWhite = false,
            };
        }
    }

```

而为了在Mirror中实现派生类转换为基类后进行数据交互，我们需要在序列化方法中提前指明该基类实例是由什么派生类转换来的

```

    public static void WriteChoice(this NetworkWriter writer, Choice choice)
    {
        writer.Write<ChoiceKind>(choice.kind);
        Debug.Log(choice.kind);
        switch (choice.kind)
        {
            case ChoiceKind.PlayCard:
                ChoicePlayCard total = choice as ChoicePlayCard;
                writer.WriteBool(total.isWhite);
                writer.WriteArray(total.cards);
                Debug.Log($"{total.isWhite} {CardKind.ToString(total.cards)}");
                break;
        }
    }
    public static Choice ReadChoice(this NetworkReader reader)
    {
        ChoiceKind kind = reader.Read<ChoiceKind>();
        Debug.Log(kind);
        switch (kind)
        {
            case ChoiceKind.PlayCard:
                ChoicePlayCard choicePlayCard = new ChoicePlayCard
                {
                    isWhite = reader.ReadBool(),
                    cards = reader.ReadArray<CardKind>(),
                };
                Debug.Log($"{choicePlayCard.isWhite} {CardKind.ToString(choicePlayCard.cards)}");
                return choicePlayCard;
        }
        return new Choice();
    }

```

这样在接受时也可以得到派生类的信息进行构造。但是在实际中我发现得到的派生类实例永远都是null，因此我怀疑序列化方法有问题。在尝试打Log信息后我发现序列化方法根本没被调用，对比文档后我才发现读写器忘记加this前缀了，加上后序列化方法即可正常被调用了。