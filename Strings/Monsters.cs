using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace salt_strings.Strings
{
    internal class Monsters
    {
        public string inputFile;
        public List<MonsterDef> monsterCatalog;

        public struct MonsterField
        {
            public int ID;
            public int dataType;
            public float fData;
            public int iData;
            public string strData;
        }
        public struct MonsterDef
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
            public string def;
            public int boxWidth;
            public int boxHeight;
            public int boxSubHeight;
            public int shadowWidth;
            public int shadowHeight;
            public List<MonsterField> monsterFields;
            public int[] flags;
        }

        public void GetLocaleById(int id)
        {
            Read();
            List<string> localeText = new List<string>();
            foreach(var monst in monsterCatalog)
            {
                if(monst.title[id] != "New Monster" && monst.title[id] != "")
                    localeText.Add(monst.title[id].Replace("\n", "{CL}").Replace("\r", "{RF}"));
                if (monst.description[id] != "New Description" && monst.title[id] != "")
                    localeText.Add(monst.description[id].Replace("\n", "{CL}").Replace("\r", "{RF}") + "\n");
            }
            File.WriteAllLines(inputFile + ".txt", localeText);
        }

        public void Serialize()
        {
            Read();
            var json = JsonConvert.SerializeObject(monsterCatalog, Formatting.Indented);
            File.WriteAllText(inputFile + ".json", json);
        }

        private MonsterField ReadMonsterField(BinaryReader reader)
        {
            MonsterField mf = new MonsterField();
            mf.ID = reader.ReadInt32();
            mf.dataType = reader.ReadInt32();
            switch (mf.dataType)
            {
                case 0:
                    mf.fData = reader.ReadSingle();
                    break;
                case 2:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 13:
                    mf.iData = reader.ReadInt32();
                    break;
                case 1:
                case 3:
                case 4:
                case 12:
                    mf.strData = reader.ReadString();
                    break;
            }
            return mf;
        }
        public void Read()
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(inputFile));
            monsterCatalog = new List<MonsterDef>();
            int num = reader.ReadInt32();
            for(int i = 0; i < num; i++)
            {
                MonsterDef md = new MonsterDef();
                md.name = reader.ReadString();
                md.title = new string[20];
                md.description = new string[20];
                for (int j = 0; j < 20; j++)
                    md.title[j] = reader.ReadString();
                for (int j = 0; j < 20; j++)
                    md.description[j] = reader.ReadString();
                md.type = reader.ReadInt32();
                md.subType = reader.ReadInt32();
                md.cost = reader.ReadSingle();
                md.img = reader.ReadInt32();
                md.altImg = reader.ReadInt32();
                md.texture = reader.ReadString();
                md.def = reader.ReadString();
                md.boxWidth = reader.ReadInt32();
                md.boxHeight = reader.ReadInt32();
                md.boxSubHeight = reader.ReadInt32();
                md.shadowWidth = reader.ReadInt32();
                md.shadowHeight = reader.ReadInt32();

                md.monsterFields = new List<MonsterField>();
                int num2 = reader.ReadInt32();
                for(int j = 0; j < num2; j++)
                {
                    md.monsterFields.Add(ReadMonsterField(reader));
                }
                int num3 = reader.ReadInt32();
                md.flags = new int[num3];
                for (int j = 0; j < num3; j++)
                    md.flags[j] = reader.ReadInt32();
                monsterCatalog.Add(md);
            }
        }
    }
}
