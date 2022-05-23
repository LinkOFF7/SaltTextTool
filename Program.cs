﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using salt_strings.Strings;

namespace salt_strings
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
                    default: break;
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
                    loot.GetLocaleById(0, loot.inputFile + ".txt");
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
                    loot.InsertLocaleById(0, args[3]);
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
                    ms.GetLocaleById(0, ms.inputFile + ".txt");
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
                    ms.InsertLocaleById(0, args[3]);
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
                    skill.GetLocaleById(0, skill.inputFile + ".txt");
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
                    skill.InsertLocaleById(0, args[3]);
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
                    locText.GetLocaleById(0, locText.inputFile + ".txt");
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
                    locText.WriteLocaleById(0, args[3], args[4]);
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
                    xs.GetLocaleById(0, xs.filename + ".txt");
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
                    xs.WriteLocaleById(0, args[3], args[4]);
                    return;
                }
            }
        }
    }
}
