using System.Text.Json;

namespace _1003
{
    internal class Program
    {
        struct data
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

        data general = new data();
        static PenStatus pen = PenStatus.Up;
        static int[,] foregroundsArray = new int[Console.WindowHeight, Console.WindowWidth];
        static char[,] shadesArray = new char[Console.WindowHeight, Console.WindowWidth];
        static char[] shades = ['█', '▓', '▒', '░'];
        static int opacity = 0;

        static void Main(string[] args)
        {
            general.CursorX = Console.WindowWidth / 2;
            general.CursorY = Console.WindowHeight / 2;
            general.background = 15;
            general.cursorColor = 0;

            BasicInfo();

            Console.WriteLine("Újat szeretnél kezdeni? (Y/n)");
            switch (Console.ReadLine()!.ToUpper())
            {
                case "Y": New(); listOverWrite(general.background); break;
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
                        2 > pen > 0 ? PenStatus.Up : PenStatus.Down;
                        Console.Write("\b \b");
                        Console.BackgroundColor = (ConsoleColor)szin;
                        break;

                    case ConsoleKey.H:
                        pen == 2 ? PenStatus.Up : PenStatus.Eraser;
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
            (general.CursorX, general.CursorY) = Console.GetCursorPosition();
            int newX = (general.CursorX + x + Console.WindowWidth) % Console.WindowWidth;
            int newY = (general.CursorY + y + Console.WindowHeight) % Console.WindowHeight;

            Console.SetCursorPosition(newX, newY);
            CursorX = newX; CursorY = newY;

            if (pen == 1)
            {
                Console.Write(shades[opacity]);
                Console.SetCursorPosition(general.CursorX, general.CursorY);
                foregroundsArray[general.CursorY, general.CursorX] = szin;
                shadesArray[general.CursorX, general.CursorY] = shades[opacity];
            }
            if (pen == 2)
            {
                Console.Write(' ');
                Console.SetCursorPosition(general.CursorX, general.CursorY);
                shadesArray[general.CursorX, general.CursorY] = '\0';
            }
        }

        static void KurzorSzin(int szin)
        {
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write("A kurzor színe: ");
            Console.BackgroundColor = (ConsoleColor)szin;
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(" ");
            Console.BackgroundColor = (ConsoleColor)szin;

            Console.SetCursorPosition(general.CursorX, general.CursorY);
        }

        static void Radir()
        {
            Console.CursorTop = 1;
            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            if (pen == 2)
            {
                Console.Write("A radír be van kapcsolva ");
                Console.BackgroundColor = (ConsoleColor)back;
            }
            else
            {
                Console.Write("                         ");
            }

            Console.SetCursorPosition(CursorX, CursorY);
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

            Console.WriteLine("\nNyomj meg egy gombot, hogy kezdj.");
            Console.ReadKey();

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
            szin = szin == 15 ? 0 : szin + 1;
            Console.SetCursorPosition(CursorX, CursorY);
            KurzorSzin(szin);

            if (pen == 0)
            {
                Console.BackgroundColor = (ConsoleColor)back;
                Console.Write(' ');
                Console.CursorLeft--;
            }
            else
            {
                Console.ForegroundColor = (ConsoleColor)szin;
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
            Console.Write("Írd ide a színhez kapcsolódó számot a kiválasztáshoz: ");
            int num = 0;
            try
            {
                num = int.Parse(Console.ReadLine()!);
                general.background = (ConsoleColor)num;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.BackgroundColor = (ConsoleColor)num;
            Console.CursorSize = 100;
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
            KurzorSzin(szin); // Kurzor színének kiírása
        }

        static void LoadFile()
        {
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Milyen néven van mentve a fájl?");
            Console.Write("Kérlek írd ide: ");
            Read(Console.ReadLine()!);
            
            KurzorSzin(szin);
        }

        static void Save(string filename)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename + ".rajz");

            List<List<ConsoleColor>> foregroundColors = new List<List<ConsoleColor>>();
            List<List<int>> Opacities = new List<List<int>>();
            for (int i = 0; i < foregroundsArray.GetLength(0); i++)
            {
                foregroundColors.Add(new List<int>());
                Opacities.Add(new List<int>());
                for (int j = 0; j < foregroundsArray.GetLength(1); j++)
                {
                    foregroundColors[i].Add((ConsoleColor)foregroundsArray[i, j]);
                    Opacities[i].Add(shades[i, j]);
                }
            }

            general.foregrounds = foregroundColors;

            string data = JsonSerializer.Serialize(adat);

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
            data data = JsonSerializer.Deserialize<data>(file);

            general = data;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < general.foregrounds.Count; i++)
            {
                for (int j = 0; j < general.foregrounds[i].Count; j++)
                {
                    if (general.shades[i][j] == '\0')
                    {
                        Console.BackgroundColor = general.background;
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.ForegroundColor = general.foregrounds[i][j];
                        Console.Write(shades[i][j]);
                    }
                }
            }
        }
    }
}
