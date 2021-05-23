# **KS Mino Todo**

### 参考资料
- [TETR.IO WIKI](https://tetris.wiki/TETR.IO)
- [Guideline计分标准](https://harddrop.com/wiki/Scoring)
- [俄罗斯方块 灰机Wiki](https://tetris.huijiwiki.com/wiki/%E9%A6%96%E9%A1%B5#)
---
### 代码优化
- [x] 把`game.currentMino`改名为`activeMino`
- [x] 优化field数组：
    1. 不再将`activeMino`作为`field.array`里的数值存储。
    2. 去掉`ghostMino`在`field.array`中的存储，用阴影距离代替。
    3. 在`field`对象中增加一个`LandHeight(Mino m)`的函数，用来计算`ghostMino`位置的高度。
    4. 重写`activeMino`和`ghostMino`的显示。
    5. 改完以后，`field.array`里的元素只剩下0\~8
       - 0是没有方块，1\~7是Mino ID，8是灰色方块。
- [x] 把`ClearType`类的`GetClearMessage()`函数改为重写类的`ToString()`。
- [x] 把每帧都刷新所有方块变成只在发生变化时刷新。
  - 在Hold变化时刷新Hold。
  - 在Next变化时刷新Next。
  - 在消行、锁定时刷新所有方块。
  - 在Active Mino发生变化时刷新Active Mino和Ghost Mino。
- [x] 把`Game.Harddrop()`和`Game.LockMino()`分开执行，让`Harddrop()`只负责把方块降落到底部。
- [x] 修复bug:硬降不重置`lockTimer`导致新方块直接在顶部锁定的bug。
- [ ] 重写`CheckClearType()`函数
  - 消除行数
  - 是否为T-Spin
  - 是否为PC
  - 是否为B2B
  - Combo数量
---
### 结构调整
- [x] 在`Game`类中添加操作符字符串，用栈判断操作是否为spin。
  |  下   |  左   |  右   | 顺时针 | 逆时针 |
  | :---: | :---: | :---: | :----: | :----: |
  |   d   |   l   |   r   |   cC   |   zZ   |
- [ ] 在`Field`类中添加表示每列高度的数组`top[col]`

---
### 基本功能
- [x] 暂停菜单
- [ ] 键位设置

---
### 游戏模式
- [ ] Sprint：20L，40L，1000L，自定义
- [ ] 由Sprint衍生出的20TSD
- [ ] Cheese模式

---
### 新功能
- [ ] 在`Game`类中增加分数计算功能
  - 消行计分表，锁定时加分
    |       类型       |   分数   |
    | :--------------: | :------: |
    |      Single      |   200    |
    |      Double      |   200    |
    |      Triple      |   200    |
    |      Tetris      |   200    |
    |       TSM        |   200    |
    |       TSS        |   200    |
    |       TSD        |   200    |
    |       TST        |   200    |
    | **Back to back** | ***1.5** |
    |      n Ren       |  +n*50   |
    |        PC        |  +3000   |
    |   **Level n**    |  **\*n**  |
  - 硬降和软降加分
    - 硬降每格+2分，软降每格+1分。
- [ ] 落块特效

      
- [x] 游戏暂停功能
- [ ] 垃圾行
  - 在`Field`类里增加`Garbage(int n)`,随机生成n行直列垃圾行
  - 该函数在`Game.LockMino()`之后、`Game.NextMino()`之前执行
- [ ] 垃圾行等待条
  - 存储方式为一个int队列。
---
### 拓展功能
- [ ] PC提示线
  - 根据场上的方块数量，以及最高堆积高度，显示两条PC提示线。
- [ ] 终点线
  - 在40L，20TSD等游戏目标是消除一定行数的模式中启用终点线，当剩余行数小于20时显示。
- [ ] 7Bag提示器
  - 在屏幕上显示当前bag中剩下的方块。
- [ ] PPS提示器
  - 在块数超过7之后，如果PPS小于设定的阈值，就在屏幕上显示一只蜗牛。
- [ ] box练习模式
  - 练习J+(S/Z/O)+L组成3\*4或者4\*3盒子的模式