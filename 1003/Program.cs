using System.Text.Json;

namespace _1003
{
    internal class Program
    {
        struct DATA
        {
            public int CursorX { get; set; }
            public int CursorY { get; set; }
            public int opacity { get; set; }
            public ConsoleColor background { get; set; }
            public ConsoleColor cursorColor { get; set; }
            public List<List<char>> shades { get; set; }
            public List<List<ConsoleColor>> foregrounds { get; set; }
        }

        enum PenStatus
        {
            Up,
            Down,
            Eraser
        }

        static DATA info = new DATA();
        static PenStatus pen = PenStatus.Up;
        static ConsoleColor[,] foregroundsArray = new ConsoleColor[Console.WindowHeight, Console.WindowWidth];
        static char[,] shadesArray = new char[Console.WindowHeight, Console.WindowWidth];
        static char[] shades = ['█', '▓', '▒', '░'];

        static void Main(string[] args)
        {
            info.CursorX = Console.WindowWidth / 2;
            info.CursorY = Console.WindowHeight / 2;
            info.background = ConsoleColor.White;
            info.cursorColor = 0;
            info.opacity = 0;

            BasicInfo();

            Console.WriteLine("Újat rajzot szeretnél kezdeni? (Y/n)");
            string input = Console.ReadLine()!.ToUpper().Trim();
            while (input != "Y" && input != "N")
            {
                Console.Write("Kérlek próbáld újra: ");
                input = Console.ReadLine()!.ToUpper();
            }
            switch (input)
            {
                case "Y": New(); break;
                case "N": LoadFile(); break;
            }

            Console.SetCursorPosition(info.CursorX, info.CursorY);
            Console.CursorSize = 100;

            while (true)
            {
                var key = Console.ReadKey().Key;
                switch (key)
                {
                    case ConsoleKey.RightArrow: Move(1, 0); break;
                    case ConsoleKey.LeftArrow: Move(-1, 0); break;
                    case ConsoleKey.UpArrow: Move(0, -1); break;
                    case ConsoleKey.DownArrow: Move(0, 1); break;

                    case ConsoleKey.Spacebar:
                        szinChange();
                        if (pen == PenStatus.Down)
                        {
                            Console.Write("\b"+shades[info.opacity]);
                        }
                        break;

                    case ConsoleKey.W:
                        New();
                        break;

                    case ConsoleKey.Q:
                        opacityChange();
                        if (pen == PenStatus.Down)
                        {
                            Console.Write(shades[info.opacity] + "\b");
                        }
                        else
                        {
                            Console.Write("\b  \b");
                        }
                        break;

                    case ConsoleKey.G:
                        pen = pen == PenStatus.Down ? PenStatus.Up : PenStatus.Down;
                        if (pen == PenStatus.Down)
                        {
                            Console.Write("\b" + shades[info.opacity] + "\b");
                        }
                        else
                        {
                            Console.Write("\b \b");
                        }
                        Toll();
                        break;

                    case ConsoleKey.H:
                        if (pen == PenStatus.Down || pen == PenStatus.Up)
                        {
                            pen = PenStatus.Eraser;
                        }
                        else
                        {
                            pen = PenStatus.Up;
                        }
                        Radir();
                        Console.Write("\b  \b");
                        break;

                    case ConsoleKey.Enter:
                        Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
                        break;
                    case ConsoleKey.S:
                        Console.SetCursorPosition(0, Console.WindowHeight / 2);
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine("Milyen néven mentsem a fájlt?");
                        Console.Write("Kérlek írd ide: ");
                        Save(Console.ReadLine()!);
                        break;

                    case ConsoleKey:
                        Console.Write("\b \b");
                        break;
                }

            }
        }

        static void Move(int x, int y)
        {
            (info.CursorX, info.CursorY) = Console.GetCursorPosition();
            int newX = info.CursorX + x;
            int newY = info.CursorY + y;

            if (newY == Console.WindowHeight - 2 || newY == -1)
            {
                newY = info.CursorY;
            }
            if (newX == Console.WindowWidth || newX == -1)
            {
                newX = info.CursorX;
            }
            Console.SetCursorPosition(newX, newY);
            info.CursorX = newX; info.CursorY = newY;

            if (pen == PenStatus.Down)
            {
                Console.Write(shades[info.opacity]);
                Console.SetCursorPosition(info.CursorX, info.CursorY);
                foregroundsArray[info.CursorY, info.CursorX] = info.cursorColor;
                shadesArray[info.CursorY, info.CursorX] = shades[info.opacity];
            }
            if (pen == PenStatus.Up)
            {
                Console.SetCursorPosition(info.CursorX, info.CursorY);
            }
            if (pen == PenStatus.Eraser)
            {
                Console.Write(' ');
                Console.SetCursorPosition(info.CursorX, info.CursorY);
                shadesArray[info.CursorY, info.CursorX] = ' ';
            }
        }

        static void KurzorSzin()
        {
            Console.CursorTop = Console.WindowHeight - 1;
            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write("A kurzor színe: ");
            Console.BackgroundColor = info.cursorColor;
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(" ║ ");
            Console.BackgroundColor = info.background;
            Console.SetCursorPosition(info.CursorX, info.CursorY);
            Console.ForegroundColor = info.cursorColor;
            if (pen == PenStatus.Down)
            {
                Console.Write(shades[info.opacity]);
            }
        }

        static void KurzorOpacityLevel()
        {
            Console.CursorTop = Console.WindowHeight - 1;
            Console.CursorLeft = 20;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write($"A kurzor áttetszősége: {info.opacity + 1} ║");
            Console.BackgroundColor = info.background;

            Console.SetCursorPosition(info.CursorX, info.CursorY);
            Console.ForegroundColor = info.cursorColor;
        }

        static void Radir()
        {
            Console.CursorTop = Console.WindowHeight - 1;
            Console.CursorLeft = 47;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            if (pen == PenStatus.Eraser)
            {
                Console.Write("A radír be van kapcsolva ");
            }
            else
            {
                Console.Write("                         ");
            }

            Console.SetCursorPosition(info.CursorX, info.CursorY);
            Console.BackgroundColor = info.background;
        }

        static void Toll()
        {
            Console.CursorTop = Console.WindowHeight - 1;
            Console.CursorLeft = 47;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            if (pen == PenStatus.Down)
            {
                Console.Write("A kurzor le van rakva  ");
            }
            else if (pen == PenStatus.Up)
            {
                Console.Write("A kurzor felvan engedve ");
            }

            Console.SetCursorPosition(info.CursorX, info.CursorY);
            Console.BackgroundColor = info.background;
        }

        static void BasicInfo()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("Szimpla pixelart-os program.");
            Console.WriteLine("Köszönöm támogatásod éshogy megvetted ezt a játékot.");
            Console.WriteLine("\nW-vel kezdesz új lapot, szóközzel váltasz színt (16 szín van) és G-vel rakod le a tollat illetve emeled fel.");
            Console.WriteLine("H-val kapcsolod a radírt, Q-val a kurzor áttetszőségét.");
            Console.WriteLine("Nyilakkal mozogsz, és bárhol tudsz rajzolni. Ha középre akarsz jutni nyomd meg az Enter-t.\n");
            Console.WriteLine("Menteni az S betűvel tudsz, mentést megnyitni az alkalmazás indításakor tudsz.");
            Console.WriteLine("Ezeket a a fájlokat .rajz kiterjesztéssel menti a program a dokumentumokba.");

            Console.WriteLine("\nKnown bug: ha kirakod nagyba a programot míg a kurzor színe más pl. kék akkor az egész kék lesz.");

            Console.Title = "Rajz maker v0.16";
        }

        static void szinChange()
        {
            info.cursorColor = info.cursorColor == ConsoleColor.White ? 0 : info.cursorColor + 1;
            KurzorSzin();
            Console.BackgroundColor = info.background;
        }

        static void opacityChange()
        {
            info.opacity = info.opacity == 3 ? 0 : info.opacity + 1;
            KurzorOpacityLevel();
            Console.BackgroundColor = info.background;
        }

        static void New()
        {
            Console.Clear();
            Console.WriteLine("Válasz háttérszínt:");
            for (int i = 0; i < 16; i++)
            {
                Console.WriteLine($"{i} - {(ConsoleColor)i}");
            }
            Console.Write("\nÍrd ide a színhez kapcsolódó számot a kiválasztáshoz: ");
            int? num = null;
            while (num == null)
            {
                try
                {
                    string input = Console.ReadLine()!.Trim();
                    num = int.Parse(input);
                    info.background = (ConsoleColor)num;
                }
                catch (FormatException)
                {
                    Console.Write("Kérlek számot írj be: ");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write("Kérlek próbáld újra: ");
                }
            }
            Console.BackgroundColor = (ConsoleColor)num;
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
            TUI();
        }

        static void TUI()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                if (i / 18 == 1 && i == 18)
                {
                    Console.Write("╦");
                }
                else if (i / 45 == 1 && i == 45)
                {
                    Console.Write("╦");
                }
                else
                {
                    Console.Write("═");
                }
            }
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }
            KurzorSzin();
            KurzorOpacityLevel();
            Toll();
        }

        static void LoadFile()
        {
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("Milyen néven van mentve a fájl?");
            Console.Write("Kérlek írd ide: ");

            string filename = Console.ReadLine()!;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename + ".rajz");
            while (!Path.Exists(path))
            {
                Console.WriteLine("A fájl nem található itt, biztos jól írtad be a nevét?");
                Console.Write("Kérlek próbáld újra: ");
                filename = Console.ReadLine()!;
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename + ".rajz");
            }
            Read(path);
            TUI();

            if (info.CursorY >= Console.WindowHeight - 2)
            {
                info.CursorY = Console.WindowHeight / 2;
                info.CursorX = Console.WindowWidth / 2;
            }

        }

        static void Save(string filename)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename + ".rajz");

            List<List<ConsoleColor>> foregroundColors = new List<List<ConsoleColor>>();
            List<List<char>> Opacities = new List<List<char>>();
            for (int i = 0; i < foregroundsArray.GetLength(0); i++)
            {
                foregroundColors.Add(new List<ConsoleColor>());
                Opacities.Add(new List<char>());
                for (int j = 0; j < foregroundsArray.GetLength(1); j++)
                {
                    foregroundColors[i].Add(foregroundsArray[i, j]);
                    Opacities[i].Add(shadesArray[i, j]);
                }
            }

            info.foregrounds = foregroundColors;
            info.shades = Opacities;

            string data = JsonSerializer.Serialize(info);

            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(data);
            }

            Console.WriteLine("Sikeres mentés");
            Thread.Sleep(500);
            Environment.Exit(0);
        }

        static void Read(string path)
        {
            string file = File.ReadAllText(path);
            DATA data = JsonSerializer.Deserialize<DATA>(file);

            info = data;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = info.background;

            for (int i = 0; i < info.foregrounds.Count; i++)
            {
                for (int j = 0; j < info.foregrounds[i].Count; j++)
                {
                    if (info.shades[i][j] == ' ' || info.shades[i][j] == '\0')
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.ForegroundColor = info.foregrounds[i][j];
                        Console.Write(info.shades[i][j]);
                        foregroundsArray[i, j] = info.foregrounds[i][j];
                        shadesArray[i, j] = info.shades[i][j];
                    }
                }
            }
        }
    }
}
