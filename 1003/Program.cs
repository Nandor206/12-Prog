using System.Text.Json;

namespace _1003
{
    internal class Program
    {
        struct DATA
        {
            public int CursorX { get; set; }
            public int CursorY { get; set; }
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
        static int[,] foregroundsArray = new int[Console.WindowHeight, Console.WindowWidth];
        static char[,] shadesArray = new char[Console.WindowHeight, Console.WindowWidth];
        static char[] shades = ['█', '▓', '▒', '░'];
        static int opacity = 0;

        static void Main(string[] args)
        {
            info.CursorX = Console.WindowWidth / 2;
            info.CursorY = Console.WindowHeight / 2;
            info.background = ConsoleColor.White;
            info.cursorColor = 0;

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
                case "Y": New(); listOverWrite((int)info.background); break;
                case "N": LoadFile(); break;
            }


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
                        break;

                    case ConsoleKey.W:
                        New();
                        break;

                    case ConsoleKey.G:
                        pen = pen == PenStatus.Down ? PenStatus.Up : PenStatus.Down;
                        Console.Write("\b \b");
                        Console.ForegroundColor = info.cursorColor;
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
                        Console.Write("\b \b");
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
            int newX = (info.CursorX + x + Console.WindowWidth) % Console.WindowWidth;
            int newY = (info.CursorY + y + Console.WindowHeight) % Console.WindowHeight;

            Console.SetCursorPosition(newX, newY);
            info.CursorX = newX; info.CursorY = newY;

            if (pen == PenStatus.Down)
            {
                Console.Write(shades[opacity]);
                Console.SetCursorPosition(info.CursorX, info.CursorY);
                foregroundsArray[info.CursorY, info.CursorX] = (int)info.cursorColor;
                shadesArray[info.CursorX, info.CursorY] = shades[opacity];
            }
            if (pen == PenStatus.Up)
            {
                Console.Write(' ');
                Console.SetCursorPosition(info.CursorX, info.CursorY);
                //shadesArray[info.CursorX, info.CursorY] = '\0';
            }
            if (pen == PenStatus.Eraser)
            {
                Console.Write(' ');
                Console.SetCursorPosition(info.CursorX, info.CursorY);
                shadesArray[info.CursorX, info.CursorY] = '\0';
            }
        }

        static void KurzorSzin(ConsoleColor szin)
        {
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write("A kurzor színe: ");
            Console.BackgroundColor = szin;
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(" ");
            Console.BackgroundColor = szin;

            Console.SetCursorPosition(info.CursorX, info.CursorY);
        }

        static void Radir()
        {
            Console.CursorTop = 1;
            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            if (pen == PenStatus.Eraser)
            {
                Console.Write("A radír be van kapcsolva ");
                Console.BackgroundColor = info.background;
            }
            else
            {
                Console.Write("                         ");
            }

            Console.SetCursorPosition(info.CursorX, info.CursorY);
        }

        static void BasicInfo()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Szimpla pixelart-os program.");
            Console.WriteLine("Köszönöm támogatásod éshogy megvetted ezt a játékot.");
            Console.WriteLine("\nW-vel kezdesz új lapot, szóközzel váltasz színt (16 szín van) és G-vel rakod le a tollat illetve emeled fel.");
            Console.WriteLine("H-val lesz megint fehér, L-vel váltasz háttérszínt, J-vel törlöd a képernyőt a háttérszínre.");
            Console.WriteLine("Nyilakkal mozogsz, és bárhol tudsz rajzolni.\n");
            Console.WriteLine("Menteni az S betűvel tudsz, mentést megnyitni a Z-vel.");
            Console.WriteLine("Ezeket a a fájlokat .rajz kiterjesztéssel menti a program a dokumentumokba.");

            Console.WriteLine("\nKnown bug: ha kirakod nagyba a programot míg a kurzor színe más pl. kék akkor az egész kék lesz.");

            Console.Title = "PixelArt maker";
        }

        static void listOverWrite(int num)
        {
            for (int i = 0; i < foregroundsArray.GetLength(0); i++)
            {
                for (int j = 0; j < foregroundsArray.GetLength(1); j++)
                {
                    foregroundsArray[i, j] = num;
                }
            }
        }

        static void szinChange()
        {
            info.cursorColor = info.cursorColor == ConsoleColor.White ? 0 : info.cursorColor + 1;
            Console.SetCursorPosition(info.CursorX, info.CursorY);
            KurzorSzin(info.cursorColor);
            Console.ForegroundColor = info.cursorColor;

            if (pen == PenStatus.Down)
            {
                Console.Write(shades[opacity]);
                Console.CursorLeft--;
            }
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
            int num = 0;
            try
            {
                num = int.Parse(Console.ReadLine()!);
                info.background = (ConsoleColor)num;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.BackgroundColor = (ConsoleColor)num;
            Console.CursorSize = 100;
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
            KurzorSzin(info.cursorColor);
        }

        static void LoadFile()
        {
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("Milyen néven van mentve a fájl?");
            Console.Write("Kérlek írd ide: ");
            Read(Console.ReadLine()!);

            KurzorSzin(info.cursorColor);
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
                    foregroundColors[i].Add((ConsoleColor)foregroundsArray[i, j]);
                    Opacities[i].Add(shadesArray[i, j]);
                }
            }

            info.foregrounds = foregroundColors;

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

        static void Read(string filename)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename + ".rajz");
            if (!Path.Exists(path))
            {
                Console.WriteLine("A fájl nem található itt, biztos jól írtad be a nevét?");
                return;
            }

            string file = File.ReadAllText(path);
            DATA data = JsonSerializer.Deserialize<DATA>(file);

            info = data;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < info.foregrounds.Count; i++)
            {
                for (int j = 0; j < info.foregrounds[i].Count; j++)
                {
                    if (info.shades[i][j] == '\0')
                    {
                        Console.BackgroundColor = info.background;
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.ForegroundColor = info.foregrounds[i][j];
                        Console.Write(shadesArray[i, j]);
                    }
                }
            }
        }
    }
}
