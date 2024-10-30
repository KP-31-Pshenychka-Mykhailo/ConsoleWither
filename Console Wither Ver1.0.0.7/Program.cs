using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics.Eventing.Reader;
using Console_Wither_Ver1._0._0._7;

namespace Console_Wither_Ver1._0._0._7
    {

        class Program
        {
            private const double WIDTH_OFFSET = 1.5;
            
            static char[,] map = new char[100, 100];
          
            static int playerRow = 50; 
                                       
            static int playerCol = 50;
            

            
            static Random random = new Random();
           

            
            static int selectedMonsterIndex = 0;
         
            static object lockObject = new object();
           
            static bool isGameplayActive = true;
      




            static void Main(string[] args)
            {
                Console.InputEncoding = Encoding.Unicode;
                Console.OutputEncoding = Encoding.Unicode;

                string imageFirst = "NameOfImage";
                int time = 3200;
                DataProgram.StartGameData();
                Draw(imageFirst, time);
                Console.Clear();
                Console.Clear();
                Console.Clear();
                ShowMenu();
            }
            static void ShowMenu()
            {

                string menuItemsStart;

                if (File.Exists(DataProgram.DataFile))
                {
                    menuItemsStart = "continue"; 
                }
                else
                {
                    menuItemsStart = "Start"; 
                }


                DataProgram.SaveData();




                string[] menuItems = { $"{menuItemsStart}", "Settings", "Exit" };
                int selectedItemIndex = 0;

                Console.CursorVisible = false;

                while (true)
                {
                    Console.Clear();

                    PrintMenuCentered(menuItems, selectedItemIndex);

                    ConsoleKeyInfo key = Console.ReadKey();

                    if (key.Key == ConsoleKey.DownArrow)
                    {
                        selectedItemIndex = (selectedItemIndex + 1) % menuItems.Length;
                    }
                    else if (key.Key == ConsoleKey.UpArrow)
                    {
                        selectedItemIndex = (selectedItemIndex - 1 + menuItems.Length) % menuItems.Length;
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        if (selectedItemIndex == 0)
                        {
                            Console.WriteLine("\n...");
                            Thread.Sleep(1500);
                            Console.Clear();
                            string imageLoader = "NameOfImage";
                            int time = 4000;
                            Draw(imageLoader, time);
                            Console.Clear();
                            Console.Clear();
                            Console.Clear();
                            InitializeMap();
                            Thread moveSectionsThread = new Thread(new ThreadStart(MoveSections));
                            moveSectionsThread.Start();
                            GamePlay();

                        }
                        else if (selectedItemIndex == 1)
                        {
                            Console.WriteLine("\n...");
                            Thread.Sleep(500);
                            Settings();
                            Console.ReadKey();
                            ShowMenu();
                        }
                        else if (selectedItemIndex == 2)
                        {
                            Console.WriteLine("\nExit from game...");
                            Thread.Sleep(100);
                            DataProgram.SaveData();
                            Environment.Exit(0);
                            break;

                        }
                    }
                }
            }
            static void Settings()
            {
                Console.Clear();
                Console.WriteLine("Visibility range (max = 20) Enter:");
                int sizeOfAreaSettings = Convert.ToInt32(Console.ReadLine());
                if (sizeOfAreaSettings <= 20)
                {
                    DataProgram.VisiableArea = sizeOfAreaSettings;
                    DataProgram.VisiableArea = DataProgram.VisiableArea;
                    DataProgram.SaveData();
                    Console.WriteLine($"Now the drawing range: {DataProgram.VisiableArea}");
                    Thread.Sleep(2000);
                    ShowMenu();
                }
                else
                {
                    Console.WriteLine("Enter less, and it will be cotton");
                    Thread.Sleep(2000);
                    Settings();
                }
            }
            static void PrintMenuCentered(string[] menuItems, int selectedItemIndex)
            {

                int centerY = Console.WindowHeight / 2 - menuItems.Length / 2;


                for (int i = 0; i < menuItems.Length; i++)
                {

                    int centerX = Console.WindowWidth / 2 - menuItems[i].Length / 2;


                    Console.SetCursorPosition(centerX, centerY + i);


                    if (i == selectedItemIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine(menuItems[i]);
                    Console.ResetColor();
                }
            }
            static async Task Draw(string nameOfPicture, int timeSleep)
            {


                var defaultImagePath = $"LinkOnPath{nameOfPicture}";
                var numberzero = 0;

                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Images| *.bmp; *png; *.jpg; *.JPEG"
                };

                while (numberzero < 1)
                {

                    Console.Clear();
                    var defaultBitmap = new Bitmap(defaultImagePath);
                    defaultBitmap = ResizeBitmap(defaultBitmap);
                    defaultBitmap.ToGrayscale();

                    var defaultConverter = new BitmapToASCIIConverter(defaultBitmap);
                    var defaultRows = defaultConverter.Convert();

                    foreach (var row in defaultRows)
                        Console.WriteLine(row);

                    Console.SetCursorPosition(0, 0);

                    numberzero++;
                }
                Thread.Sleep(timeSleep);
                Console.Clear();


            }
            private static Bitmap ResizeBitmap(Bitmap bitmap)
            {
                var maxWidth = 350;
                var newHeight = bitmap.Height / WIDTH_OFFSET * maxWidth / bitmap.Width;
                if (bitmap.Width > maxWidth || bitmap.Height > newHeight)
                    bitmap = new Bitmap(bitmap, new Size(maxWidth, (int)newHeight));
                return bitmap;
            }
            static void InitializeMap()
            {
                
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        map[i, j] = '█';
                    }
                }

                
                PlaceHouse(10, 10);
                PlaceHouse(10, 85);
                PlaceHouse(85, 10);
                PlaceHouse(85, 85);

              
                for (int i = 0; i < 100; i++)
                {
                    map[i, i] = '≈'; 
                    if (i + 1 < 100)
                        map[i + 1, i] = '≈';
                }

                
                for (int i = 1; i <= 3; i++)
                {
                    int bridgePosition = i * 25;
                    for (int j = bridgePosition - 2; j <= bridgePosition + 2; j++)
                    {
                        map[j, j] = '═'; 
                        if (j + 1 < 100)
                            map[j + 1, j] = '═';
                    }
                }

              
                for (int i = 0; i < 100; i += 10)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        if (map[i, j] == '█')
                        {
                            map[i, j] = '░'; 
                        }
                    }
                }

                
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        if (map[i, j] == '█')
                        {
                            map[i, j] = '▒';
                        }
                    }
                }

               
                map[5, 5] = '1';
                map[5, 15] = '2';
                map[15, 5] = '3';

                
                map[playerRow, playerCol] = '*';
            }
            static void PlaceHouse(int row, int col)
            {
                for (int i = row; i < row + 5; i++)
                {
                    for (int j = col; j < col + 5; j++)
                    {
                        map[i, j] = 'H'; 
                    }
                }
            }
            static void MoveSection(ref int sectionRow, ref int sectionCol, char sectionChar)
            {
                lock (lockObject)
                {
                    map[sectionRow, sectionCol] = '░'; 
                    int newRow = (sectionRow + 1) % 100;
                    int newCol = (sectionCol + 1) % 100;

                    if (map[newRow, newCol] != '≈' && map[newRow, newCol] != 'H')
                    {
                        sectionRow = newRow;
                        sectionCol = newCol;
                    }

                    map[sectionRow, sectionCol] = sectionChar;
                }
            }
            static void MoveSections()
            {
                int sectionRow1 = 5, sectionCol1 = 5;
                int sectionRow2 = 5, sectionCol2 = 15;
                int sectionRow3 = 15, sectionCol3 = 5;

                while (true)
                {
                    if (isGameplayActive)
                    {
                        MoveSection(ref sectionRow1, ref sectionCol1, '1');
                        MoveSection(ref sectionRow2, ref sectionCol2, '2');
                        MoveSection(ref sectionRow3, ref sectionCol3, '3');
                        PrintVisibleArea();
                    }
                    Thread.Sleep(1000);
                }
            }
            static bool IsValidPosition(int row, int col)
            {
                return row >= 0 && row < 100 && col >= 0 && col < 100 && map[row, col] != 'H';
            }
            static void PrintVisibleArea()
            {
                lock (lockObject)
                {
                    if (!isGameplayActive) return;
                    Console.Clear();
                    int consoleWidth = Console.WindowWidth;
                    int consoleHeight = Console.WindowHeight;
                    int offsetX = (consoleWidth - DataProgram.VisiableArea * 2) / 2;
                    int offsetY = (consoleHeight - DataProgram.VisiableArea) / 2;

                    for (int i = playerRow - DataProgram.VisiableArea / 2; i < playerRow + DataProgram.VisiableArea / 2; i++)
                    {
                        for (int j = playerCol - DataProgram.VisiableArea / 2; j < playerCol + DataProgram.VisiableArea / 2; j++)
                        {
                            if (i >= 0 && i < 100 && j >= 0 && j < 100)
                            {
                                Console.SetCursorPosition(offsetX + (j - (playerCol - DataProgram.VisiableArea / 2)) * 2, offsetY + (i - (playerRow - DataProgram.VisiableArea / 2)));
                                if (i == playerRow && j == playerCol)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write("* "); //gamer
                                }
                                else
                                {
                                    switch (map[i, j])
                                    {
                                        case 'H':
                                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                                            break;
                                        case '≈':
                                            Console.ForegroundColor = ConsoleColor.Blue;
                                            break;
                                        case '1':
                                        case '2':
                                        case '3':
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            break;
                                        case '▒':
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            break;
                                        case '░':
                                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                                            break;
                                        default:
                                            Console.ForegroundColor = ConsoleColor.White;
                                            break;
                                    }
                                    Console.Write(map[i, j] + " ");
                                }
                            }
                            else
                            {
                                Console.SetCursorPosition(offsetX + (j - (playerCol - DataProgram.VisiableArea / 2)) * 2, offsetY + (i - (playerRow - DataProgram.VisiableArea / 2)));
                                
                                Console.Write("  ");
                            }
                        }
                    }
                    Console.ResetColor();
                }
            }
            static void GamePlay()
            {
                isGameplayActive = true;
                InitializeMap();
                PrintVisibleArea();

                while (true)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);


                    int newPlayerRow = playerRow;
                    int newPlayerCol = playerCol;

                    if (key.Key == ConsoleKey.E)
                    {
                        if (map[playerRow, playerCol] == '1' || map[playerRow, playerCol] == '2' || map[playerRow, playerCol] == '3')
                        {
                            isGameplayActive = false;
                            lock (lockObject)
                            {
                                Console.Clear();
                                Console.WriteLine("This is the enemy!!!! \n Attack !!");
                            }
                            Thread.Sleep(3000);
                            Fight();
                            isGameplayActive = true;
                            PrintVisibleArea();
                        }
                    }
                    else if (key.Key == ConsoleKey.Q)
                    {
                        isGameplayActive = false;
                        lock (lockObject)
                        {
                            ShowMenu();
                        }
                        isGameplayActive = true;


                        PrintVisibleArea();

                    }

                    else if (key.Key == ConsoleKey.I)
                    {
                        isGameplayActive = false;
                        lock (lockObject)
                        {
                            MenuInventory();
                        }
                        isGameplayActive = true;


                        PrintVisibleArea();

                    }
                    else if (key.Key == ConsoleKey.LeftArrow && playerCol > 0)
                    {
                        newPlayerCol--;
                    }
                    else if (key.Key == ConsoleKey.RightArrow && playerCol < 99)
                    {
                        newPlayerCol++;
                    }
                    else if (key.Key == ConsoleKey.UpArrow && playerRow > 0)
                    {
                        newPlayerRow--;
                    }
                    else if (key.Key == ConsoleKey.DownArrow && playerRow < 99)
                    {
                        newPlayerRow++;
                    }


                    if (IsValidPosition(newPlayerRow, newPlayerCol))
                    {

                        playerRow = newPlayerRow;
                        playerCol = newPlayerCol;


                        PrintVisibleArea();
                    }
                }
            }
            static void ApplyPotionEffects(string potionName)
            {
                switch (potionName)
                {
                    case "Зілля здоров'я":
                        DataProgram.Health += 50;
                        Console.WriteLine("Health increased by 50.");

                        DataProgram.SaveData();
                        break;
                    case "Зілля магії":
                        DataProgram.Magic *= 2;
                        Console.WriteLine("Magic is increased by 2 times.");
                        DataProgram.Magic = DataProgram.Magic;
                        DataProgram.SaveData();
                        break;
                    default:
                        Console.WriteLine("There is no such thing.");
                        break;
                }
            }
            static void MenuInventory()
            {
                Dictionary<string, Dictionary<string, string>> equipment = new Dictionary<string, Dictionary<string, string>>
        {
            {"Equipment", new Dictionary<string, string>
                {
                    {"Shield", "Defense: +10"},
                    {"Armor", "Defense: +20"},
                    {"Mantle", "Defense: +15"}
                }
            },
            {"Weapon", new Dictionary<string, string>
                {
                    {"Sword", "Damage: +20"},
                    {"Bow", "Damage: +15"},
                    {"Silver Sword", "Damage: +10"}
                }
            },
            {"Potion", new Dictionary<string, string>
                {
                    {"Health Potion", "Restores 50 HP"},
                    {"Potion of Magic", "Increases magic by 2 times"}
                }
            }
        };

                string[] categories = { "Equipment", "Weapons", "Potions" };
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"Health :{DataProgram.Health} Magic:{DataProgram.Magic} XP:{DataProgram.XP} Crowns: {DataProgram.Cash}");
                    Console.WriteLine("Оберіть категорію:");

                    for (int i = 0; i < categories.Length; i++)
                    {
                        Console.WriteLine($"{i + 1}. {categories[i]}");
                    }

                    Console.WriteLine("0. Exit");

                    int choice;
                    if (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > categories.Length)
                    {
                        Console.WriteLine("Invalid value. Try again");
                        Console.ReadLine();
                        continue;
                    }

                    if (choice == 0)
                        break;

                    string category = categories[choice - 1];
                    Console.Clear();
                    Console.WriteLine($"Selected category: {category}\n");

                    if (category == "Potion")
                    {
                        Console.WriteLine("Choose a potion:");

                        int index = 1;
                        foreach (var item in equipment[category])
                        {
                            Console.WriteLine($"{index}. {item.Key}: {item.Value}");
                            index++;
                        }

                        int potionChoice;
                        if (!int.TryParse(Console.ReadLine(), out potionChoice) || potionChoice < 1 || potionChoice > equipment[category].Count)
                        {
                            Console.WriteLine("Invalid value. Try again");
                            Console.ReadLine();
                            continue;
                        }

                        string potionName = equipment[category].Keys.ToArray()[potionChoice - 1];
                        ApplyPotionEffects(potionName);
                        equipment[category].Remove(potionName);
                    }
                    else
                    {
                        foreach (var item in equipment[category])
                        {
                            Console.WriteLine($"{item.Key}: {item.Value}");
                        }
                    }

                    Console.WriteLine("\nPress any key to continue");
                    Console.ReadKey();
                }
            }
            static List<Monster> GenerateMonsters()
            {
                List<Monster> monsters = new List<Monster>();
                int numberOfMonsters = random.Next(3, 7);

                for (int i = 0; i < numberOfMonsters; i++)
                {
                    int monsterHealth = random.Next(3, 11);
                    monsters.Add(new Monster(monsterHealth));
                }

                return monsters;
            }
            static void ShowMonsters(List<Monster> monsters)
            {
                Console.Clear();
                Console.WriteLine("Monsters are attacking you! Be ready for battle!");

                for (int i = 0; i < monsters.Count; i++)
                {
                    if (i == selectedMonsterIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"\nMonster {i + 1}: Health: {monsters[i].Health}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"\nMonster {i + 1}: Health: {monsters[i].Health}");
                    }
                }
            }
            static void ShowAttackCards()
            {
                Console.WriteLine("1. Attack (does 3 damage)");
                Console.WriteLine("2. Magic (deals 5 damage)");
                Console.WriteLine("3. Strong attack (does 8 damage)");
            }
            static int GetAttackDamage(int attack)
            {
                int choice = 0;
                int chanceToAttack;

                while (choice < 1 || choice > 3)
                {
                    Console.Write("Choose an attacking card (1-3): ");
                    int.TryParse(Console.ReadLine(), out choice);
                }

                switch (choice)
                {
                    case 1:
                        return 3;
                    case 2:
                        DataProgram.Magic -= 5;
                       
                        return 5;
                    case 3:
                    Random rand = new Random();
                    chanceToAttack = GenerateBiasedRandom(rand);
                    if (chanceToAttack == 1)
                    {
                        attack = 0;
                    }
                    else if (chanceToAttack == 2)
                    {
                        attack = 8;
                    }

                    return attack;
                    default:
                        return 0;
                }
            }
            static int GenerateBiasedRandom(Random rand)
            {
                double probability = rand.NextDouble();
                if (probability < 0.65)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            static void Fight()
            {
                lock (lockObject)
                {
                    while (true)
                    {
                        List<Monster> monsters = GenerateMonsters();
                        Console.WriteLine("Monsters are attacking you! Be ready for battle!");
                        int fictiveNum = 0;
                        while (DataProgram.Health > 0 && monsters.Count > 0)
                        {
                            ShowMonsters(monsters);
                            Console.WriteLine($"\nYour Lives: {DataProgram.Health} \nYour Magic: {DataProgram.Magic}");

                            ConsoleKeyInfo keyInfo;
                            do
                            {
                                keyInfo = Console.ReadKey(true);
                                if (keyInfo.Key == ConsoleKey.UpArrow && selectedMonsterIndex > 0)
                                {
                                    selectedMonsterIndex--;
                                }
                                else if (keyInfo.Key == ConsoleKey.DownArrow && selectedMonsterIndex < monsters.Count - 1)
                                {
                                    selectedMonsterIndex++;
                                }
                                ShowMonsters(monsters);
                                Console.WriteLine($"\nYour Lives: {DataProgram.Health} \nYour Magic: {DataProgram.Magic}");
                            } while (keyInfo.Key != ConsoleKey.Enter);

                            Monster selectedMonster = monsters[selectedMonsterIndex];

                            Console.WriteLine($"\nYou have selected the monster {selectedMonsterIndex + 1}! Health of selected monster: {selectedMonster.Health}");

                            Console.WriteLine("\nYour attacking cards:");
                            ShowAttackCards();

                            int attackDamage = GetAttackDamage(fictiveNum);
                            selectedMonster.TakeDamage(attackDamage);

                            Console.WriteLine($"You dealt {attackDamage} damage to the monster! Monster Health: {selectedMonster.Health}");

                            if (selectedMonster.Health <= 0)
                            {
                                Console.WriteLine("The monster is defeated! Let's move on to the next...");
                                DataProgram.XP++;
                                monsters.RemoveAt(selectedMonsterIndex);
                                if (monsters.Count == 0)
                                {
                                    Console.WriteLine("\nCongratulations! You have defeated all the monsters!");

                                    DataProgram.Cash += 20;
                                    DataProgram.Magic = 15;
                                    DataProgram.SaveData();
                                    ShowMenu();
                                }
                                else
                                {
                                    if (selectedMonsterIndex >= monsters.Count)
                                    {
                                        selectedMonsterIndex = monsters.Count - 1;
                                    }
                                    Console.WriteLine("\nChoose the next monster.");
                                }
                            }
                            else
                            {
                                int monsterAttack = random.Next(1, 11);
                                DataProgram.Health -= monsterAttack;
                                Console.WriteLine($"The monster attacks back and deals {monsterAttack} damage to you!");
                                
                                
                                Thread.Sleep(3000);

                                if (DataProgram.Health <= 0)
                                {
                                    Console.WriteLine("\nYou lost! Game over.");
                                    DataProgram.SaveData();
                                    ShowMenu();


                                }
                            }
                        }
                    }
                }
            }
        }
        class Monster
        {
            public int Health { get; private set; }

            public Monster(int health)
            {
                Health = health;
            }

            public void TakeDamage(int damage)
            {
                Health -= damage;
                if (Health < 0)
                {
                    Health = 0;
                }
            }
        }

}

