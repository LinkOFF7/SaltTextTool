using System;
using System.Collections.Generic;
using System.Linq;
using salt_strings.Strings;

namespace salt_strings
{
    internal class Program
    {
        const int localeId = 0;
        static void Main(string[] args)
        {
            if(args.Length < 3)
            {
                PrintUsage();
                return;
            }
            List<string> arguments = args.ToList();
            string mode = "";
            if (arguments[0] == "-mode")
            {
                mode = arguments[1];
                
                switch (mode.ToLower())
                {
                    case "skilltree":
                    case "st":
                    case "skill":
                        {
                            arguments.RemoveRange(0, 2);
                            SkillTree(arguments.ToArray());
                            break;
                        }
                    case "dialog":
                    case "dialogs":
                    case "dlg":
                        {
                            arguments.RemoveRange(0, 2);
                            Dialog(arguments.ToArray());
                            break;
                        }
                    case "strings":
                    case "string":
                    case "loctext":
                    case "lt":
                        {
                            arguments.RemoveRange(0, 2);
                            LocText(arguments.ToArray());
                            break;
                        }
                    case "missions":
                    case "mission":
                    case "ms":
                        {
                            arguments.RemoveRange(0, 2);
                            Missions(arguments.ToArray());
                            break;
                        }
                    case "loot":
                    case "lc":
                        {
                            arguments.RemoveRange(0, 2);
                            Loot(arguments.ToArray());
                            break;
                        }
                    case "monsters":
                    case "monster":
                        {
                            arguments.RemoveRange(0, 2);
                            Monsters(arguments.ToArray());
                            break;
                        }
                    default: break;
                }
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Salt and Sacrifice Text Tool by LinkOFF v.0.9");
            Console.WriteLine("");
            Console.WriteLine("Usage: salt_strings.exe -mode <mode> <-extract/-import> <json/txt> <input game file> [input txt/json] [output file]");
            Console.WriteLine("");
            Console.WriteLine("Modes:");
            Console.WriteLine("  skilltree\tWork with skilltree.zsx file");
            Console.WriteLine("  dialog\tWork with dialog.zdx file");
            Console.WriteLine("  strings\tWork with strings.ztx file");
            Console.WriteLine("  missions\tWork with missions.zms file");
            Console.WriteLine("  loot\t\tWork with loot.zls file");
            Console.WriteLine("  monsters\tWork with monsters.zms file (dump to JSON only)");
            Console.WriteLine("");
            Console.WriteLine("Examples of extraction:");
            Console.WriteLine(@"  salt_strings.exe -mode loot -extract txt loot.zls");
            Console.WriteLine(@"  salt_strings.exe -mode loot -extract json loot.zls");
            Console.WriteLine("Examples of import:");
            Console.WriteLine(@"  salt_strings.exe -mode loot -import txt strings.ztx strings.ztx.txt new\strings.ztx");
            Console.WriteLine(@"  salt_strings.exe -mode loot -import txt loot.zls loot.zls.txt new\loot.zls");
            Console.WriteLine("");
        }

        static void Monsters(string[] args)
        {
            Monsters ms = new Monsters();
            if (args[0].ToLower() == "-extract")
            {
                if (args[1].ToLower() == "json")
                {
                    Console.WriteLine("Mode: Dump to JSON");
                    ms.inputFile = args[2];
                    ms.Serialize();
                    return;
                }
                else if (args[1].ToLower() == "txt")
                {
                    Console.WriteLine("Mode: Extraction tot TXT");
                    ms.inputFile = args[2];
                    ms.Read();
                    ms.GetLocaleById(localeId);
                    return;
                }
            }
            else if (args[0].ToLower() == "-import")
            {
                if (args[1].ToLower() == "json")
                {
                    Console.WriteLine("Import from JSON not implemented yet.");
                    return;
                }
                else if (args[1].ToLower() == "txt")
                {
                    Console.WriteLine("Import from TXT not implemented yet.");
                    return;
                }
            }
        }
        static void Loot(string[] args)
        {
            Loot loot = new Loot();
            if (args[0].ToLower() == "-extract")
            {
                if (args[1].ToLower() == "json")
                {
                    Console.WriteLine("Mode: Dump to JSON");
                    loot.inputFile = args[2];
                    loot.Serialize();
                    return;
                }
                else if (args[1].ToLower() == "txt")
                {
                    Console.WriteLine("Mode: Extraction to TXT");
                    loot.inputFile = args[2];
                    loot.Read();
                    loot.GetLocaleById(localeId, loot.inputFile + ".txt");
                    return;
                }
            }
            else if (args[0].ToLower() == "-import")
            {
                if (args[1].ToLower() == "json")
                {
                    Console.WriteLine("Mode: Import from JSON");
                    loot.inputFile = args[2];
                    loot.Deserialize(args[3]);
                    loot.Write(args[4]);
                    return;
                }
                else if (args[1].ToLower() == "txt")
                {
                    Console.WriteLine("Mode: Import from TXT");
                    loot.inputFile = args[2];
                    loot.Read();
                    loot.InsertLocaleById(localeId, args[3]);
                    loot.Write(args[4]);
                }
            }
        }

        static void Missions(string[] args)
        {
            Missions ms = new Missions();
            if (args[0].ToLower() == "-extract")
            {
                if (args[1].ToLower() == "json")
                {
                    Console.WriteLine("Mode: Dump to JSON");
                    ms.inputFile = args[2];
                    ms.Serialize();
                    return;
                }
                else if (args[1].ToLower() == "txt")
                {
                    Console.WriteLine("Mode: Extraction to TXT");
                    ms.inputFile = args[2];
                    ms.Read();
                    ms.GetLocaleById(localeId, ms.inputFile + ".txt");
                    return;
                }
            }
            else if (args[0].ToLower() == "-import")
            {
                if (args[1].ToLower() == "json")
                {
                    Console.WriteLine("Mode: Import from JSON");
                    ms.inputFile = args[2];
                    ms.Deserialize(args[3]);
                    ms.Write(args[4]);
                    return;
                }
                else if (args[1].ToLower() == "txt")
                {
                    Console.WriteLine("Mode: Import from TXT");
                    ms.inputFile = args[2];
                    ms.Read();
                    ms.InsertLocaleById(localeId, args[3]);
                    ms.Write(args[4]);
                    return;
                }
            }
        }

        static void SkillTree(string[] args)
        {
            SkillTree skill = new SkillTree();
            if (args[0].ToLower() == "-extract")
            {
                if (args[1].ToLower() == "json")
                {
                    Console.WriteLine("Mode: Dump to JSON");
                    skill.inputFile = args[2];
                    skill.Serialize();
                    return;
                }
                else if (args[1].ToLower() == "txt")
                {
                    Console.WriteLine("Mode: Extraction to TXT");
                    skill.inputFile = args[2];
                    skill.Read();
                    skill.GetLocaleById(localeId, skill.inputFile + ".txt");
                    return;
                }
            }
            else if (args[0].ToLower() == "-import")
            {
                if (args[1].ToLower() == "json")
                {
                    Console.WriteLine("Mode: Import from JSON");
                    skill.inputFile = args[2];
                    skill.DeserializeNodes(args[3]);
                    skill.Write(args[4]);
                    return;
                }
                else if (args[1].ToLower() == "txt")
                {
                    Console.WriteLine("Mode: Import from TXT");
                    skill.inputFile = args[2];
                    skill.Read();
                    skill.InsertLocaleById(localeId, args[3]);
                    skill.Write(args[4]);
                    return;
                }
            }
        }

        static void LocText(string[] args)
        {
            LocText locText = new LocText();
            if (args[0].ToLower() == "-extract")
            {
                if (args[1].ToLower() == "json")
                {
                    Console.WriteLine("Mode: Dump to JSON");
                    locText.inputFile = args[2];
                    locText.Serialize();
                    return;
                }
                else if (args[1].ToLower() == "txt")
                {
                    Console.WriteLine("Mode: Extraction to TXT");
                    locText.inputFile = args[2];
                    locText.GetLocaleById(localeId, locText.inputFile + ".txt");
                    return;
                }
            }
            else if (args[0].ToLower() == "-import")
            {
                if (args[1].ToLower() == "json")
                {
                    Console.WriteLine("Mode: Create from JSON");
                    locText.Deserialize(args[2]);
                    locText.Write(args[3]);
                    return;
                }
                else if (args[1].ToLower() == "txt")
                {
                    Console.WriteLine("Mode: Import from TXT");
                    locText.inputFile = args[2];
                    locText.WriteLocaleById(localeId, args[3], args[4]);
                    return;
                }
            }
        }
        static void Dialog(string[] args)
        {
            if (args.Length < 3)
                return;
            Dialog xs = new Dialog();
            if (args[0].ToLower() == "-extract")
            {
                if (args[1].ToLower() == "json")
                {
                    Console.WriteLine("Mode: Dump to JSON");
                    xs.filename = args[2];
                    xs.Serialize();
                    return;
                }
                else if (args[1].ToLower() == "txt")
                {
                    Console.WriteLine("Mode: Extraction to TXT");
                    xs.filename = args[2];
                    xs.GetLocaleById(localeId, xs.filename + ".txt");
                    return;
                }
            }
            else if (args[0].ToLower() == "-import")
            {
                if (args[1].ToLower() == "json")
                {
                    Console.WriteLine("Mode: Create from JSON");
                    xs.Deserialize(args[2]);
                    xs.Write(args[3]);
                    return;
                }
                else if (args[1].ToLower() == "txt")
                {
                    Console.WriteLine("Mode: Import from TXT");
                    xs.filename = args[2];
                    xs.WriteLocaleById(localeId, args[3], args[4]);
                    return;
                }
            }
        }
    }
}
