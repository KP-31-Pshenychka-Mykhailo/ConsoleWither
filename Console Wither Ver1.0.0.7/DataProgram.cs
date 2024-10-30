using System;
using System.IO;
using System.Globalization;
using System.Resources;
using Console_Wither_Ver1._0._0._7;
using System.Xml.XPath;

static partial class DataProgram
{
    public static string SaveDataDocumentsPath { get; } = $@"C:\Users\{Environment.UserName}\OneDrive\Documents\ConsoleWitherBeta";
    public static string DataFile { get; } = SaveDataDocumentsPath + @"\Data.txt";
    public static int Health { get; set; }
    public static int Cash { get; set; }
    public static int Magic { get; set; }
    public static int XP { get; set; }
    public static int VisiableArea { get; set; }



    public static void StartGameData()
    {
        try
        {

            Directory.CreateDirectory(SaveDataDocumentsPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }

        if (File.Exists(DataFile))
        {
            string[] lines = File.ReadAllLines(DataFile);
            int tempHealth, tempCash, tempMagic, tempXP, tempVisiableArea;
            if (lines.Length >= 5 &&
                    int.TryParse(lines[0], out tempHealth) &&
                    int.TryParse(lines[1], out tempCash) &&
                    int.TryParse(lines[2], out tempMagic) &&
                    int.TryParse(lines[3], out tempXP) &&
                    int.TryParse(lines[4], out tempVisiableArea))
            {
                Health = tempHealth;
                Cash = tempCash;
                Magic = tempMagic;
                XP = tempXP;
                VisiableArea = tempVisiableArea;
            }
            else
            {
                Health = 100;
                Cash = 0;
                Magic = 100;
                XP = 0;
                VisiableArea = 20;
            }
        }
        else
        {

            Console.WriteLine("Файл не найден. Используются значения по умолчанию.");
            Health = 100;
            Cash = 0;
            Magic = 100;
            XP = 0;
            VisiableArea = 20;
        }



    }
    public static void SaveData()
    {
        string[] outputLines = { Health.ToString(), Cash.ToString(), Magic.ToString(), XP.ToString(), VisiableArea.ToString() };
        File.WriteAllLines(DataFile, outputLines);
    }


    public static string PathData => SaveDataDocumentsPath;
}
