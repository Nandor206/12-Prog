using System.Drawing;
using System.Text.Json;

namespace _1003
{
    internal class Program
    {
        static int CursorX = Console.WindowWidth / 2;
        static int CursorY = Console.WindowHeight / 2;

        static int szin = 0;
        static int back = 15;
        static bool toll = false;
        static bool radir = false;
        static int[,] array = new int[Console.WindowHeight, Console.WindowWidth];

        struct ADAT
        { // Csak a mentéshez kell (nem tudja rendesen olvaassni nélküle)
            public List<List<int>> ints { get; set; }
        }

        static void Main(string[] args)
        {
            listOverWrite(15);
            Info();

            Console.WriteLine("Újat szeretnél kezdeni? (Y/n)");
            switch (Console.ReadLine()!.ToUpper())
            {
                case "Y": New(); break;
                case "N": Load(); break;
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
                        toll = !toll;
                        Console.Write("\b \b");
                        Console.BackgroundColor = (ConsoleColor)szin;
                        break;

                    case ConsoleKey.H:
                        radir = !radir;
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
                        Console.WriteLine("Milyen néven mentsük a fájlt?");
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
            (CursorX, CursorY) = Console.GetCursorPosition();
            int newX = (CursorX + x + Console.WindowWidth) % Console.WindowWidth;
            int newY = (CursorY + y + Console.WindowHeight) % Console.WindowHeight;

            Console.SetCursorPosition(newX, newY);
            CursorX = newX; CursorY = newY;

            if (toll)
            {
                Console.Write(" ");
                Console.SetCursorPosition(CursorX, CursorY);
                array[CursorY, CursorX] = szin;
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

            Console.SetCursorPosition(CursorX, CursorY);
        }

        static void Radir()
        {
            Console.CursorTop = 1;
            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            if (radir)
            {
                Console.Write("A radír be van kapcsolva ");
                Console.BackgroundColor = (ConsoleColor)back;
                toll = true;
            }
            else
            {
                Console.Write("                         ");
                toll = false;
            }

            Console.SetCursorPosition(CursorX, CursorY);
        }

        static void Info()
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
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = num;
                }
            }
        }

        static void szinChange()
        {
            szin = (szin == 15 ? 0 : szin + 1);
            Console.SetCursorPosition(CursorX, CursorY);
            KurzorSzin(szin);

            if (!toll)
            {
                Console.BackgroundColor = (ConsoleColor)back;
                Console.Write(" ");
                Console.CursorLeft--;
            }
            else
            {
                Console.BackgroundColor = (ConsoleColor)szin;
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

        static void Load()
        {
            Console.SetCursorPosition(0, Console.WindowHeight / 2);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Milyen néven van mentve a fájl?");
            Console.Write("Kérlek írd ide: ");
            Read(Console.ReadLine()!);
            Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
            KurzorSzin(szin);
        }

        static void Save(string filename)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename + ".rajz");

            List<List<int>> list = new List<List<int>>();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                list.Add(new List<int>());
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    list[i].Add(array[i, j]);
                }
            }

            ADAT adat = new ADAT();
            adat.ints = list;

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
            Thread.Sleep(1000);
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
            ADAT data = JsonSerializer.Deserialize<ADAT>(file);
            List<List<int>> list = data.ints;

            Console.Clear();
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count; j++)
                {
                    Console.BackgroundColor = (ConsoleColor)list[i][j];
                    Console.Write(" ");
                }
            }
        }
    }
}
