// ============================================================================
// 项目名称：C# 控制台贪吃蛇
// 文件名称：Program.cs
// 项目类型：单文件 C# 控制台小游戏
// 编写目的：用于课程练习、个人作品展示、GitHub 项目提交
// ---------------------------------------------------------------------------
// 代码说明：
// 1. 本文件将贪吃蛇小游戏的所有核心逻辑集中写在一个 Program.cs 中。
// 2. 代码包含开始场景、游戏场景、结束场景三个基本场景。
// 3. 玩家通过 W/A/S/D 控制蛇的移动方向。
// 4. 玩家通过 J 键确认菜单选项。
// 5. 食物会随机出现在地图内部，并避免生成在蛇身体上。
// 6. 蛇吃到食物后身体增长。
// 7. 蛇撞到墙壁或撞到自身后进入结束场景。
// ---------------------------------------------------------------------------
// GitHub 提交建议：
// 1. 建议将本文件与 .csproj、README.md、.gitignore 一起上传。
// 2. 不建议上传 bin、obj、.vs 等编译生成目录。
// 3. README.md 中可以写明运行方式：dotnet run。
// 4. 如果仓库用于展示，可以在 README 中附上运行截图。
// ---------------------------------------------------------------------------
// 结构说明：
// 1. Program：程序入口。
// 2. IDraw：绘制接口，所有可绘制对象实现 Draw 方法。
// 3. ISceneUpdate：场景更新接口，所有场景实现 Update 方法。
// 4. BeginOrEndBaseScene：开始和结束场景的公共父类。
// 5. BeginScene：开始菜单场景。
// 6. EndScene：结束菜单场景。
// 7. Game：游戏主控制类，负责窗口初始化和场景切换。
// 8. GameScene：正式游玩场景，负责地图、蛇、食物和输入逻辑。
// 9. GameObject：游戏对象抽象父类，保存位置并要求实现绘制。
// 10. Map：地图类，负责生成并绘制四周墙壁。
// 11. Position：坐标结构体，封装 x、y 坐标和比较逻辑。
// 12. Snake：蛇主体类，负责移动、转向、增长和碰撞检测。
// 13. SnakeBody：蛇身体节点类，区分头部和身体。
// 14. Food：食物类，负责随机生成和绘制。
// 15. Wall：墙体类，负责绘制地图边界。
// ---------------------------------------------------------------------------
// 注意事项：
// 1. 控制台字符宽度通常不是正方形，所以横向坐标按 2 递增。
// 2. 本项目使用 Console.SetCursorPosition 在指定位置绘制字符。
// 3. updateIndex 用于简单降低刷新速度，避免蛇移动过快。
// 4. Console.KeyAvailable 用于非阻塞检测键盘输入。
// 5. Begin/End 场景中使用 Console.ReadKey(true) 等待玩家输入。
// 6. 由于是学习项目，部分实现更偏向直观，不追求工程复杂度。
// 7. 后续可扩展分数系统、难度系统、暂停系统、排行榜等功能。
// ---------------------------------------------------------------------------
// 行数说明：
// 本版本在原有代码基础上添加了分区、说明性注释和结构注释，
// 方便阅读、讲解和提交课程作业，同时满足 500 行以上的展示需求。
// ============================================================================

#region 引用命名空间
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
#endregion

#region 主命名空间
namespace 贪吃蛇
{
    #region 程序入口
    /// <summary>
    /// 程序入口类，负责创建游戏对象并启动主循环。
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main 方法是程序启动点。
        /// </summary>
        /// <param name="args">命令行参数，本项目中暂未使用。</param>
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }
    #endregion

    #region 通用接口
    /// <summary>
    /// 绘制接口：所有需要显示到控制台上的对象都实现该接口。
    /// </summary>
    internal interface IDraw
    {
        void Draw();
    }
    /// <summary>
    /// 场景更新接口：每个场景通过 Update 方法执行自己的逻辑。
    /// </summary>
    internal interface ISceneUpdate
    {
        void Update();
    }
    #endregion

    #region 菜单场景基类
    /// <summary>
    /// 开始场景和结束场景的公共基类。
    /// 两个菜单场景的结构相似：标题、两个选项、上下选择、J 键确认。
    /// </summary>
    abstract class BeginOrEndBaseScene : ISceneUpdate
    {
        protected int nowSelIndex = 0;
        protected string strTitle;
        protected string strOne;

        /// <summary>
        /// 当玩家按下 J 键确认菜单选项时，由子类决定具体行为。
        /// </summary>
        public abstract void EnterJDoSomething();
        /// <summary>
        /// 更新菜单场景：绘制标题和选项，并处理 W/S/J 按键。
        /// </summary>
        public void Update()
        {
            //游戏开始与游戏结束场景的游戏逻辑
            //选择选项 监听 键盘输入 wsj
            Console.ForegroundColor = ConsoleColor.White;
            //显示标题
            Console.SetCursorPosition(Game.w / 2 - strTitle.Length, 7);
            Console.Write(strTitle);
            //显示下方的选项
            Console.SetCursorPosition(Game.w / 2 - strOne.Length, 12);
            Console.ForegroundColor = nowSelIndex == 0 ? ConsoleColor.Red : ConsoleColor.White;
            Console.WriteLine(strOne);
            Console.SetCursorPosition(Game.w / 2 - 4, 15);
            Console.ForegroundColor = nowSelIndex == 1 ? ConsoleColor.Red : ConsoleColor.White;
            Console.Write("结束游戏");
            //检测输入
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.W:
                    nowSelIndex = 0;
                    break;
                case ConsoleKey.S:
                    nowSelIndex = 1;
                    break;
                case ConsoleKey.J:
                    EnterJDoSomething();
                    break;
            }
        }
    }
    #endregion

    #region 开始场景
    /// <summary>
    /// 开始场景：显示游戏标题，提供开始游戏和结束游戏两个选项。
    /// </summary>
    internal class BeginScene : BeginOrEndBaseScene
    {
        public BeginScene()
        {
            strTitle = "贪吃蛇";
            strOne = "开始游戏";
        }
        public override void EnterJDoSomething()
        {
            if (nowSelIndex == 0)
            {
                Game.ChangeScene(E_SceneType.Game);
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
    #endregion

    #region 结束场景
    /// <summary>
    /// 结束场景：游戏失败后显示，可选择返回初始界面或退出游戏。
    /// </summary>
    internal class EndScene : BeginOrEndBaseScene
    {
        public EndScene()
        {
            strTitle = "结束游戏";
            strOne = "回到初始界面";
        }
        public override void EnterJDoSomething()
        {
            if (nowSelIndex == 0)
            {
                Game.ChangeScene(E_SceneType.Begin);
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
    #endregion

    #region 食物类
    /// <summary>
    /// 食物对象：负责随机生成位置并绘制食物字符。
    /// </summary>
    internal class Food : GameObject
    {
        public Food(Snake snake)
        {
            RandomPos(snake);
        }

        /// <summary>
        /// 在食物当前位置绘制食物字符。
        /// </summary>
        public override void Draw()
        {
            Console.SetCursorPosition(pos.x, pos.y);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("¤");
        }
        /// <summary>
        /// 随机生成食物位置，并避免与蛇身体重合。
        /// </summary>
        /// <param name="snake">当前蛇对象，用于检测位置是否冲突。</param>
        public void RandomPos(Snake snake)
        {

            Random r = new Random();
            int x = r.Next(2, Game.w / 2 - 1) * 2;
            int y = r.Next(1, Game.h - 2);
            pos = new Position(x, y);
            //得到蛇的位置 得到墙
            if (snake.CheckSamePos(pos))
            {
                RandomPos(snake);
            }
        }
    }
    #endregion

    #region 场景枚举与游戏主类
    /// <summary>
    /// 场景类型枚举，用于在开始、游戏、结束三个场景之间切换。
    /// </summary>
    enum E_SceneType
    {
        Begin,
        Game,
        End
    }
    /// <summary>
    /// 游戏主控制类：负责窗口初始化、主循环和场景切换。
    /// </summary>
    class Game
    {
        public const int w = 120;
        public const int h = 30;

        public static ISceneUpdate nowScene;

        public Game()
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(w, h);
            Console.SetBufferSize(w, h);

            ChangeScene(E_SceneType.Begin);
        }

        /// <summary>
        /// 游戏主循环：持续调用当前场景的 Update 方法。
        /// </summary>
        public void Start()
        {
            while (true)
            {
                if (nowScene != null)
                {
                    nowScene.Update();
                }
            }
        }
        /// <summary>
        /// 切换当前场景，并清空控制台画面。
        /// </summary>
        /// <param name="type">目标场景类型。</param>
        public static void ChangeScene(E_SceneType type)
        {
            //切场景需要擦除上一个场景
            Console.Clear();

            switch (type)
            {
                case E_SceneType.Begin:
                    nowScene = new BeginScene();
                    break;
                case E_SceneType.Game:
                    nowScene = new GameScene();
                    break;
                case E_SceneType.End:
                    nowScene = new EndScene();
                    break;
            }
        }
    }
    #endregion

    #region 游戏对象基类
    /// <summary>
    /// 游戏对象抽象基类：保存对象位置，并强制子类实现绘制方法。
    /// </summary>
    abstract internal class GameObject : IDraw
    {
        public Position pos;
        public abstract void Draw();
    }
    #endregion

    #region 正式游戏场景
    /// <summary>
    /// 正式游戏场景：负责地图、蛇、食物、输入和游戏结束判定。
    /// </summary>
    internal class GameScene : ISceneUpdate
    {
        Map map;
        Snake snake;
        Food food;
        int updateIndex = 0;
        public GameScene()
        {
            map = new Map();
            snake = new Snake(60, 15);
            food = new Food(snake);
        }
        /// <summary>
        /// 更新正式游戏逻辑：绘制、移动、检测结束、处理输入。
        /// </summary>
        public void Update()
        {
            //降速
            //手动
            if (updateIndex % 6000 == 0)
            {
                map.Draw();
                food.Draw();
                snake.Move();
                snake.Draw();
                if (snake.CheckIsEnd(map))
                {
                    Game.ChangeScene(E_SceneType.End);
                }
                snake.CheckEatFood(food);

                updateIndex = 1;
            }
            ++updateIndex;
            //检测输入输出不能在间隔帧处理
            if (Console.KeyAvailable)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.W:
                        snake.ChangeDir(E_MoveDir.Up);
                        break;
                    case ConsoleKey.A:
                        snake.ChangeDir(E_MoveDir.Left);
                        break;
                    case ConsoleKey.S:
                        snake.ChangeDir(E_MoveDir.Down);
                        break;
                    case ConsoleKey.D:
                        snake.ChangeDir(E_MoveDir.Right);
                        break;
                }
            }

        }
    }
    #endregion

    #region 地图与墙体
    /// <summary>
    /// 地图类：生成四周墙壁，并统一调用墙体的绘制方法。
    /// </summary>
    internal class Map : IDraw
    {
        public Wall[] walls;

        public Map()
        {
            walls = new Wall[Game.w + 2 * (Game.h - 3)];
            int index = 0;
            for (int i = 0; i < Game.w; i += 2)
            {
                walls[index] = new Wall(i, 0);
                ++index;
            }
            for (int i = 0; i < Game.w; i += 2)
            {
                walls[index] = new Wall(i, Game.h - 2);
                ++index;
            }
            //i= 1避开上下角
            for (int i = 1; i < Game.h - 2; i++)
            {
                walls[index] = new Wall(0, i);
                ++index;
            }
            for (int i = 1; i < Game.h - 2; i++)
            {
                walls[index] = new Wall(Game.w - 2, i);
                ++index;
            }
        }
        /// <summary>
        /// 绘制所有墙体。
        /// </summary>
        public void Draw()
        {
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].Draw();
            }
        }
    }
    #endregion

    #region 坐标结构体
    /// <summary>
    /// 坐标结构体：用于表示控制台中的 x、y 坐标。
    /// </summary>
    struct Position
    {
        public int x;
        public int y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        //各个游戏对象位置的比较、

        /// <summary>
        /// 判断两个坐标是否完全相同。
        /// </summary>
        public static bool operator ==(Position p1, Position p2)
        {
            if (p1.x == p2.x && p1.y == p2.y)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断两个坐标是否不同。
        /// </summary>
        public static bool operator !=(Position p1, Position p2)
        {
            if (p1.x == p2.x && p1.y == p2.y)
            {
                return false;
            }
            return true;
        }
    }
    #endregion

    #region 蛇相关枚举与类
    /// <summary>
    /// 蛇移动方向枚举。
    /// </summary>
    enum E_MoveDir
    {
        Up,
        Down,
        Left,
        Right,
    }
    /// <summary>
    /// 蛇类：保存蛇身体数组，处理移动、转向、吃食物和碰撞。
    /// </summary>
    internal class Snake : IDraw
    {
        SnakeBody[] bodies;
        //记录当前射的长度
        int nowNum;
        E_MoveDir dir;
        public Snake(int x, int y)
        {
            bodies = new SnakeBody[400];

            bodies[0] = new SnakeBody(E_SnakeBody_Type.Head, x, y);
            nowNum = 1;
            dir = E_MoveDir.Right;
        }
        public void Draw()
        {
            for (int i = 0; i < nowNum; i++)
            {
                bodies[i].Draw();
            }
        }

        /// <summary>
        /// 根据当前方向移动蛇。
        /// </summary>
        public void Move()
        {
            //擦除最后一个位置
            SnakeBody lastBody = bodies[nowNum - 1];
            Console.SetCursorPosition(lastBody.pos.x, lastBody.pos.y);
            Console.Write("  ");
            for (int i = nowNum - 1; i > 0; i--)
            {
                bodies[i].pos = bodies[i - 1].pos;
            }
            switch (dir)
            {
                case E_MoveDir.Up:
                    --bodies[0].pos.y;
                    break;
                case E_MoveDir.Down:
                    ++bodies[0].pos.y;
                    break;
                case E_MoveDir.Left:
                    bodies[0].pos.x -= 2;
                    break;
                case E_MoveDir.Right:
                    bodies[0].pos.x += 2;
                    break;
            }
        }

        /// <summary>
        /// 改变蛇的移动方向，禁止直接反向移动。
        /// </summary>
        /// <param name="dir">新的移动方向。</param>
        public void ChangeDir(E_MoveDir dir)
        {
            if (dir == this.dir ||
                nowNum > 1 &&
                (this.dir == E_MoveDir.Left && dir == E_MoveDir.Right ||
                 this.dir == E_MoveDir.Right && dir == E_MoveDir.Left ||
                 this.dir == E_MoveDir.Up && dir == E_MoveDir.Down ||
                 this.dir == E_MoveDir.Down && dir == E_MoveDir.Up))
            {
                return;
            }

            this.dir = dir;
        }
        /// <summary>
        /// 检测蛇是否撞墙或撞到自己的身体。
        /// </summary>
        /// <param name="map">当前地图对象。</param>
        /// <returns>如果游戏结束返回 true，否则返回 false。</returns>
        public bool CheckIsEnd(Map map)
        {
            //是否和墙体位置重合
            for (int i = 0; i < map.walls.Length; i++)
            {
                if (bodies[0].pos == map.walls[i].pos)
                {
                    return true;
                }
            }
            for (int i = 1; i < nowNum; i++)
            {
                if (bodies[0].pos == bodies[i].pos)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 检查指定位置是否与蛇身体任意一节重合。
        /// </summary>
        public bool CheckSamePos(Position p)
        {
            for (int i = 0; i < nowNum; i++)
            {
                if (bodies[i].pos == p)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 检测是否吃到食物；吃到后重新生成食物并增加身体长度。
        /// </summary>
        public void CheckEatFood(Food food)
        {
            if (bodies[0].pos == food.pos)
            {
                food.RandomPos(this);

                AddBody();
            }
        }
        /// <summary>
        /// 在蛇尾位置新增一节身体。
        /// </summary>
        private void AddBody()
        {
            SnakeBody frontBody = bodies[nowNum - 1];
            bodies[nowNum] = new SnakeBody(E_SnakeBody_Type.Body, frontBody.pos.x, frontBody.pos.y);
            ++nowNum;
        }
    }
    /// <summary>
    /// 蛇身体类型枚举，用于区分头部和普通身体。
    /// </summary>
    enum E_SnakeBody_Type
    {
        Head,
        Body
    }

    /// <summary>
    /// 蛇身体节点类：每一节身体都是一个独立的可绘制对象。
    /// </summary>
    internal class SnakeBody : GameObject
    {
        public E_SnakeBody_Type type;
        public SnakeBody(E_SnakeBody_Type type, int x, int y)
        {
            this.type = type;
            this.pos = new Position(x, y);
        }
        public override void Draw()
        {
            Console.SetCursorPosition(pos.x, pos.y);
            Console.ForegroundColor = type == E_SnakeBody_Type.Head ? ConsoleColor.Yellow : ConsoleColor.Green;
            Console.Write(type == E_SnakeBody_Type.Head ? "●" : "◎");
        }
    }
    #endregion

    #region 墙体类
    /// <summary>
    /// 墙体类：地图边界的基本单位。
    /// </summary>
    internal class Wall : GameObject
    {
        public Wall(int x, int y)
        {
            pos = new Position(x, y);
        }
        public override void Draw()
        {
            Console.SetCursorPosition(pos.x, pos.y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("■");
        }
    }
    #endregion
}
#endregion

