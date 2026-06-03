using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace 贪吃蛇
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }
    internal interface IDraw
    {
        void Draw();
    }
    internal interface ISceneUpdate
    {
        void Update();
    }
    abstract class BeginOrEndBaseScene : ISceneUpdate
    {
        protected int nowSelIndex = 0;
        protected string strTitle;
        protected string strOne;

        public abstract void EnterJDoSomething();
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
    internal class Food : GameObject
    {
        public Food(Snake snake)
        {
            RandomPos(snake);
        }

        public override void Draw()
        {
            Console.SetCursorPosition(pos.x, pos.y);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("¤");
        }
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
    enum E_SceneType
    {
        Begin,
        Game,
        End
    }
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
    abstract internal class GameObject : IDraw
    {
        public Position pos;
        public abstract void Draw();
    }
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
        public void Draw()
        {
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].Draw();
            }
        }
    }
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

        public static bool operator ==(Position p1, Position p2)
        {
            if (p1.x == p2.x && p1.y == p2.y)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(Position p1, Position p2)
        {
            if (p1.x == p2.x && p1.y == p2.y)
            {
                return false;
            }
            return true;
        }
    }
    enum E_MoveDir
    {
        Up,
        Down,
        Left,
        Right,
    }
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
        public void CheckEatFood(Food food)
        {
            if (bodies[0].pos == food.pos)
            {
                food.RandomPos(this);

                AddBody();
            }
        }
        private void AddBody()
        {
            SnakeBody frontBody = bodies[nowNum - 1];
            bodies[nowNum] = new SnakeBody(E_SnakeBody_Type.Body, frontBody.pos.x, frontBody.pos.y);
            ++nowNum;
        }
    }
    enum E_SnakeBody_Type
    {
        Head,
        Body
    }

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
}
