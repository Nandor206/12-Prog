namespace _1003
{

    class Pixel
    {
        public byte X { get; set; }
        public byte Y { get; set; }
        public ConsoleColor Background { get; set; }
        public ConsoleColor Foreground { get; set; }
        public byte Symbol { get; set; }
        public string ToCsvLine()
        {
            return $"{X},{Y},{(int)Foreground},{(int)Background},{Symbol}";
        }
        public static Pixel FromCsvLine(string csvLine)
        {
            var splits = csvLine.Split(',');
            return new Pixel()
            {
                X = byte.Parse(splits[0]),
                Y = byte.Parse(splits[1]),
                Foreground = (ConsoleColor)int.Parse(splits[2]),
                Background = (ConsoleColor)int.Parse(splits[3]),
                Symbol = byte.Parse(splits[4])
            };
        }

        public static byte[] ToBytes(Pixel pixel)
        {
            byte[] bytes = {pixel.X, pixel.Y, (byte)pixel.Background, (byte)pixel.Foreground, pixel.Symbol};
            
            return bytes;
        }

        public static Pixel FromBytes(byte[] bytes)
        {
            return new Pixel()
            {
                X = bytes[0],
                Y = bytes[1],
                Background = (ConsoleColor)bytes[2],
                Foreground = (ConsoleColor)bytes[3],
                Symbol = bytes[4]
            };
        }

    }
    internal class Program
    {
        enum PenStatus
        {
            Up,
            Down,
            Eraser
        }

        static int CursorX = (Console.LargestWindowWidth - 1) / 2;
        static int CursorY = (Console.LargestWindowHeight - 1) / 2;
        static int SelectedOpacity = 0;
        static ConsoleColor BackgroundColor = ConsoleColor.White;
        static ConsoleColor CursorColor = ConsoleColor.Black;

        static PenStatus Pen = PenStatus.Up;
        static ConsoleColor[,] ForegroundsArray = new ConsoleColor[Console.LargestWindowWidth - 1, Console.LargestWindowHeight - 1];
        static int[,] SymbolArray = new int[Console.LargestWindowWidth - 1, Console.LargestWindowHeight - 1];

        static char[] Opacities = ['█', '▓', '▒', '░'];

        static void Main(string[] args)
        {
                int width = Console.LargestWindowWidth;
                int height = Console.LargestWindowHeight;

                if (width > 255 || height > 255)
                {
                    Console.SetWindowSize(255, 255);
                }
                else
                {
                    Console.SetWindowSize(width - 1, height - 1);
                }
            
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

            Console.SetCursorPosition(CursorX, CursorY);
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
                        if (Pen == PenStatus.Down)
                        {
                            Console.Write("\b" + Opacities[SelectedOpacity] + "\b");
                        }
                        break;

                    case ConsoleKey.F1:
                        New();
                        break;

                    case ConsoleKey.F3:
                        opacityChange();
                        if (Pen == PenStatus.Down)
                        {
                            Console.Write(Opacities[SelectedOpacity] + "\b");
                        }
                        break;

                    case ConsoleKey.F4:
                        Pen = Pen == (PenStatus)2 ? PenStatus.Up : Pen += 1;
                        if (Pen == PenStatus.Down)
                        {
                            Console.Write(Opacities[SelectedOpacity] + "\b");
                        }
                        Toll();
                        break;

                    case ConsoleKey.F2:
                        Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
                        break;

                    case ConsoleKey.Escape:
                        Console.SetCursorPosition(0, Console.WindowHeight / 2);
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine("Milyen néven mentsem a fájlt?");
                        Console.Write("Kérlek írd ide: ");
                        SaveToBin(Console.ReadLine()!);
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
            int newX = CursorX + x;
            int newY = CursorY + y;

            if (newY == Console.WindowHeight - 2 || newY == -1)
            {
                newY = CursorY;
            }
            if (newX == Console.WindowWidth || newX == -1)
            {
                newX = CursorX;
            }
            Console.SetCursorPosition(newX, newY);
            CursorX = newX; CursorY = newY;

            if (Pen == PenStatus.Down)
            {
                Console.Write(Opacities[SelectedOpacity]);
                Console.SetCursorPosition(CursorX, CursorY);
                ForegroundsArray[CursorX, CursorY] = CursorColor;
                SymbolArray[CursorX, CursorY] = SelectedOpacity;
            }
            if (Pen == PenStatus.Up)
            {
                Console.SetCursorPosition(CursorX, CursorY);
            }
            if (Pen == PenStatus.Eraser)
            {
                Console.Write(' ');
                Console.SetCursorPosition(CursorX, CursorY);
                SymbolArray[CursorX, CursorY] = 0;
                ForegroundsArray[CursorX, CursorY] = BackgroundColor;
            }
        }

        static void KurzorSzin()
        {
            Console.CursorTop = Console.WindowHeight - 1;
            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write("A kurzor színe: ");
            Console.BackgroundColor = CursorColor;
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(" ║ ");
            Console.BackgroundColor = BackgroundColor;
            Console.SetCursorPosition(CursorX, CursorY);
            Console.ForegroundColor = CursorColor;
            if (Pen == PenStatus.Down)
            {
                Console.Write(Opacities[SelectedOpacity]);
            }
        }

        static void KurzorOpacityLevel()
        {
            Console.CursorTop = Console.WindowHeight - 1;
            Console.CursorLeft = 20;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write($"A kurzor áttetszősége: {SelectedOpacity + 1} ║");
            Console.BackgroundColor = BackgroundColor;

            Console.SetCursorPosition(CursorX, CursorY);
            Console.ForegroundColor = CursorColor;
        }

        static void Toll()
        {
            Console.CursorTop = Console.WindowHeight - 1;
            Console.CursorLeft = 47;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            if (Pen == PenStatus.Down)
            {
                Console.Write("A kurzor le van rakva  ");
            }
            else if (Pen == PenStatus.Up)
            {
                Console.Write("A kurzor felvan engedve ");
            }
            else
            {
                Console.Write("A radír be van kapcsolva");
            }

            Console.SetCursorPosition(CursorX, CursorY);
            Console.BackgroundColor = BackgroundColor;
        }

        static void BasicInfo()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("Szimpla pixelart-os program.");
            Console.WriteLine("Köszönöm támogatásod éshogy megvetted ezt a játékot.");
            Console.WriteLine("Kérlek tedd az ablakot fullscreenbe a legjobb használatért.");
            Console.WriteLine("\nF1-gyel kezdesz új lapot, szóközzel váltasz színt (16 szín van) és F4-gyel váltasz a toll funkciói között: le, fel, radír.");
            Console.WriteLine("F3-mal változtatod a kurzor áttetszőségét.");
            Console.WriteLine("Nyilakkal mozogsz, és bárhol tudsz rajzolni. Ha középre akarsz jutni gyorsan nyomd meg az F2-t.\n");
            Console.WriteLine("Menteni az Escape (ESC) gombbal tudsz, mentést megnyitni pedig az alkalmazás indításakor tudsz.");
            Console.WriteLine("Ezeket a a fájlokat .rajz kiterjesztéssel menti a program a dokumentumokba.");

            //Console.WriteLine("\nKnown bug: ha kirakod nagyba a programot míg a kurzor színe más pl. kék akkor az egész kék lesz.");

            Console.Title = "Rajz maker v0.16";
        }

        static void szinChange()
        {
            CursorColor = CursorColor == ConsoleColor.White ? 0 : CursorColor + 1;
            KurzorSzin();
            Console.BackgroundColor = BackgroundColor;
        }

        static void opacityChange()
        {
            SelectedOpacity = SelectedOpacity == 3 ? 0 : SelectedOpacity + 1;
            KurzorOpacityLevel();
            Console.BackgroundColor = BackgroundColor;
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
                    BackgroundColor = (ConsoleColor)num;
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
            for (int i = 0; i < Console.WindowWidth - 1; i++)
            {
                for (int j = 0; j < Console.WindowHeight - 1; j++)
                {
                    ForegroundsArray[i, j] = Console.BackgroundColor;
                }
            }

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
            ReadFromBin(path);
            TUI();

            if (CursorY >= Console.WindowHeight - 2 || CursorY < 0 || CursorX >= Console.WindowWidth || CursorX < 0)
            {
                CursorY = Console.WindowHeight / 2;
                CursorX = Console.WindowWidth / 2;
            }

        }

        static void SaveToCsv(string filename)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename + ".rajz");

            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            string save_data = "";

            for (byte i = 0; i < Console.WindowWidth; i++)
            {
                for (byte j = 0; j < Console.WindowHeight; j++)
                {
                    if (ForegroundsArray[i, j] == Console.BackgroundColor)
                    {
                        continue;
                    }
                    else
                    {
                        Pixel pixel = new Pixel()
                        {
                            X = i,
                            Y = j,
                            Background = BackgroundColor,
                            Foreground = ForegroundsArray[i, j],
                            Symbol = (byte)SymbolArray[i, j]
                        };
                        save_data += pixel.ToCsvLine();
                        save_data += "\n";
                    }
                }
            }
            
            File.WriteAllText(path, save_data);

            Console.WriteLine("Sikeres mentés");
            Thread.Sleep(500);
            Environment.Exit(0);
        }

        static void ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path);
            BackgroundColor = Pixel.FromCsvLine(lines[0]).Background;
            Console.BackgroundColor = BackgroundColor;
            Console.Clear();

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                for (int j = 0; j < Console.WindowHeight; j++)
                {
                    ForegroundsArray[i, j] = Console.BackgroundColor;
                }
            }

            foreach (var line in lines)
            {
                var pixel = Pixel.FromCsvLine(line);
                Console.SetCursorPosition(pixel.X, pixel.Y);
                Console.ForegroundColor = pixel.Foreground;
                Console.Write(Opacities[pixel.Symbol]);
                ForegroundsArray[pixel.X, pixel.Y] = pixel.Foreground;
                SymbolArray[pixel.X, pixel.Y] = pixel.Symbol;
            }
        }

        static void SaveToBin(string filename)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename + ".rajz");

            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            List<byte> save_data = new List<byte>();

            for (byte i = 0; i < Console.WindowWidth - 1; i++)
            {
                for (byte j = 0; j < Console.WindowHeight - 1; j++)
                {
                    if (ForegroundsArray[i, j] == Console.BackgroundColor)
                    {
                        continue;
                    }
                    else
                    {
                        Pixel pixel = new Pixel()
                        {
                            X = i,
                            Y = j,
                            Background = BackgroundColor,
                            Foreground = ForegroundsArray[i, j],
                            Symbol = (byte)SymbolArray[i, j]
                        };
                        save_data.AddRange(Pixel.ToBytes(pixel));
                    }
                }
            }

            File.WriteAllBytes(path, save_data.ToArray());

            Console.WriteLine("Sikeres mentés");
            Thread.Sleep(500);
            Environment.Exit(0);
        }

        static void ReadFromBin(string path)
        {
            var bytes = File.ReadAllBytes(path);

            byte[] temp = new byte[5];
            Array.Copy(bytes, 0, temp, 0, 5);


            BackgroundColor = Pixel.FromBytes(temp).Background;
            Console.BackgroundColor = BackgroundColor;
            Console.Clear();

            for (int i = 0; i < Console.WindowWidth - 1; i++)
            {
                for (int j = 0; j < Console.WindowHeight - 1; j++)
                {
                    ForegroundsArray[i, j] = Console.BackgroundColor;
                }
            }

            int PixelSzam = bytes.Length / 5;

            for (int i = 0; i < PixelSzam; i++)
            {
                Array.Copy(bytes, i * 5, temp, 0, 5);
                var pixel = Pixel.FromBytes(temp);
                Console.SetCursorPosition(pixel.X, pixel.Y);
                Console.ForegroundColor = pixel.Foreground;
                Console.Write(Opacities[pixel.Symbol]);
                ForegroundsArray[pixel.X, pixel.Y] = pixel.Foreground;
                SymbolArray[pixel.X, pixel.Y] = pixel.Symbol;
            }
        }
    }
}
