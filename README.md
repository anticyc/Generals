**Generals**
=
拓竹杯软件设计大赛参赛作品

借鉴generals.io游戏机制

_游戏简介_
-
Generals.io 是一款激动人心的多人策略游戏，将你置于指挥军队的位置，目标只有一个：**征服地图，击败敌人**。
 
通过结合战术计划和快速决策，你需要智胜对手，夺取他们的基地，主宰战场。每一局都是对你战略思维的快速考验，因为你需要扩展领土，保护自己的将军，并对敌人发动决定性的攻击。
 
25回合占过的空地涨一兵，每回合塔涨涨一兵。

开局你有一个王，王也是塔，游戏目标：占领其他玩家的王。

占领一个空地要两个兵，一个留在原地，一个去占地。

占领一个原本有兵的地需要原本的兵+2个兵，一个留在原地，一个占地，其他对抗原本的兵。

一个野生的塔里会有不属于任何人的，不会增长的40个兵。

占领的其他玩家的王后，你会拥有他的所有领土

_实现功能：_
-

1. 自己设置地图大小和玩家数，联机创建地图，选择玩家颜色

2. 原generals.io基本机制

3. 原创：

   设置灯塔、陷阱功能——灯塔需要在山地上设置，可以照明指定方向前方四个位置，一定时间后照明效果消失；

   陷阱需要放置在沼泽处，可以吞掉任意玩家投入的的兵力，除设置者之外不可见

   实现这些功能需要从自己的王城减少50兵力

**地图创新**——地图为六边形，通过**U\I\O\J\K\L**进行方向操控

**灯塔、陷阱设置**——点击放置处按下**P**键即可防止陷阱和准备灯塔，通过**U\I\O\J\K\L**指定灯塔方向
