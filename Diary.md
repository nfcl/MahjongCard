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