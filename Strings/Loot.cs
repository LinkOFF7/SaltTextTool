using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace salt_strings.Strings
{
    internal class Loot
    {
        public string inputFile;
        public List<LootDef> lootCatalog;

        public struct LootField
        {
            public int ID;
            public int dataType;
            public float fData;
            public int iData;
            public string strData;
            public bool bData;
        }
        public struct LootDef
        {
            public string name;
            public string[] title;
            public string[] description;
            public int type;
            public int subType;
            public float cost;
            public int img;
            public int altImg;
            public string texture;
            public List<LootField> lootField;
            public List<int> flags;
            public string tokenLoot;
            public int tokenCost;
        }

        public void InsertLocaleById(int id, string inputTextFile)
        {
            List<string> localeText = File.ReadAllLines(inputTextFile).ToList();
            int line = 0;
            for(int i = 0; i < lootCatalog.Count; i++)
            {
                lootCatalog[i].title[id] = localeText[line].Replace("{CRLF}", "\r\n\r\n");
                line++;
                lootCatalog[i].description[id] = localeText[line].Replace("{CRLF}", "\r\n\r\n");
                line += 2;
            }
        }

        public void GetLocaleById(int id, string outputTextfile)
        {
            List<string> localeText = new List<string>();
            foreach(var loot in lootCatalog)
            {
                localeText.Add(loot.title[id].Replace("\r\n\r\n", "{CRLF}"));
                localeText.Add(loot.description[id].Replace("\r\n\r\n", "{CRLF}"));
                localeText.Add("");
            }
            File.WriteAllLines(outputTextfile, localeText);
        }

        public void Deserialize(string inputJson)
        {
            Read();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.NullValueHandling = NullValueHandling.Include;
            lootCatalog = JsonConvert.DeserializeObject<List<LootDef>>(new StreamReader(inputJson).ReadToEnd(), settings);
        }
        public void Serialize()
        {
            Read();
            var json = JsonConvert.SerializeObject(lootCatalog, Formatting.Indented);
            File.WriteAllText(inputFile + ".json", json);
        }
        private LootField ReadLootField(BinaryReader reader)
        {
            LootField lf = new LootField();
            lf.ID = reader.ReadInt32();
            lf.dataType = reader.ReadInt32();
            switch (lf.dataType)
            {
                case 0:
                    lf.fData = reader.ReadSingle();
                    break;
                case 2:
                case 6:
                    lf.iData = reader.ReadInt32();
                    break;
                case 1:
                case 4:
                case 5:
                case 7:
                    lf.strData = reader.ReadString();
                    break;
                case 3:
                    lf.iData = (reader.ReadBoolean() ? 1 : 0);
                    lf.bData = lf.iData == 1;
                    break;
            }
            return lf;
        }

        public void Write(string outputLootFile)
        {
            BinaryWriter writer = new BinaryWriter(File.Create(outputLootFile));
            writer.Write(lootCatalog.Count);
            for (int i = 0; i < lootCatalog.Count; i++)
            {
                writer.Write(lootCatalog[i].name);
                for (int j = 0; j < lootCatalog[i].title.Length; j++)
                    writer.Write(lootCatalog[i].title[j]);
                for (int j = 0; j < lootCatalog[i].description.Length; j++)
                    writer.Write(lootCatalog[i].description[j]);
                writer.Write(lootCatalog[i].type);
                writer.Write(lootCatalog[i].subType);
                writer.Write(lootCatalog[i].cost);
                writer.Write(lootCatalog[i].img);
                writer.Write(lootCatalog[i].altImg);
                writer.Write(lootCatalog[i].texture);
                //lootField
                writer.Write(lootCatalog[i].lootField.Count);
                for (int j = 0; j < lootCatalog[i].lootField.Count; j++)
                {
                    writer.Write(lootCatalog[i].lootField[j].ID);
                    writer.Write(lootCatalog[i].lootField[j].dataType);
                    switch (lootCatalog[i].lootField[j].dataType)
                    {
                        case 0:
                            writer.Write(lootCatalog[i].lootField[j].fData);
                            break;
                        case 2:
                        case 6:
                            writer.Write(lootCatalog[i].lootField[j].iData);
                            break;
                        case 1:
                        case 4:
                        case 5:
                        case 7:
                            writer.Write(lootCatalog[i].lootField[j].strData);
                            break;
                        case 3:
                            writer.Write(lootCatalog[i].lootField[j].bData);
                            break;
                    }
                }
                //flags
                writer.Write(lootCatalog[i].flags.Count);
                for(int j = 0; j < lootCatalog[i].flags.Count; j++)
                    writer.Write(lootCatalog[i].flags[j]);

                writer.Write(lootCatalog[i].tokenLoot);
                writer.Write(lootCatalog[i].tokenCost);
            }
            writer.Write(512);
            writer.Write(CreateEmptyByteArray(0xE));
        }
        public void Read()
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(inputFile));
            lootCatalog = new List<LootDef>();
            int num = reader.ReadInt32();
            for(int i = 0; i < num; i++)
            {
                LootDef loot = new LootDef();
                loot.name = reader.ReadString();
                loot.title = new string[20];
                loot.description = new string[20];
                for (int j = 0; j < loot.title.Length; j++)
                    loot.title[j] = reader.ReadString();
                for (int j = 0; j < loot.description.Length; j++)
                    loot.description[j] = reader.ReadString();
                loot.type = reader.ReadInt32();
                loot.subType = reader.ReadInt32();
                loot.cost = reader.ReadSingle();
                loot.img = reader.ReadInt32();
                loot.altImg = reader.ReadInt32();
                loot.texture = reader.ReadString();

                //lootField
                int num2 = reader.ReadInt32();
                loot.lootField = new List<LootField>();
                for(int j = 0; j < num2; j++)
                {
                    loot.lootField.Add(ReadLootField(reader));
                }
                //flags
                loot.flags = new List<int>();
                int num3 = reader.ReadInt32();
                for (int j = 0; j < num3; j++)
                {
                    loot.flags.Add(reader.ReadInt32());
                }
                loot.tokenLoot = reader.ReadString();
                loot.tokenCost = reader.ReadInt32();
                lootCatalog.Add(loot);
            }
        }
        private byte[] CreateEmptyByteArray(int length)
        {
            byte[] buffer = new byte[length];
            return buffer;
        }
    }
}
