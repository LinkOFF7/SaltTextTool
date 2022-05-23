using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;
using Newtonsoft.Json;

namespace salt_strings.Strings
{
    internal class Missions
    {
        public string inputFile;
        public List<Mission> missionCatalog;

        public struct Mission
        {
            public string name;
            public string[] _title;
            public string[] _description;
            public List<MissionTarget> target;
            public List<int> flags;
            public int area;
            public int value;
            public int challengeTier;
            public int[] runes;
            public List<string> unlocks;
            public MissionFields fields;
            public int areaTier;
        }

        public struct MissionTarget
        {
            public string[] title;
            public string[] name;
            public string[] description;
            public string target;
            public string altMonster;
            public int targetCount;
            public int targetChallengeAdjust;
            public List<int> targetFlags;
            public List<Vector2> customPath;
        }

        public struct MissionField
        {
            public int ID;
            public int dataType;
            public float fData;
            public int iData;
            public string strData;
        }

        public void InsertLocaleById(int id, string inputTextFile)
        {
            List<string> localeText = File.ReadAllLines(inputTextFile).ToList();
            int line = 0;
            for(int i = 0; i < missionCatalog.Count; i++)
            {
                missionCatalog[i]._title[id] = localeText[line].Substring(11).Replace("{CL}", "\n").Replace("{RF}", "\r");
                line++;
                missionCatalog[i]._description[id] = localeText[line].Substring(11).Replace("{CL}", "\n").Replace("{RF}", "\r");
                line++;
                for(int j = 0; j < missionCatalog[i].target.Count; j++)
                {
                    if (localeText[line].Length > 6)
                        missionCatalog[i].target[j].title[id] = localeText[line].Substring(6).Replace("{CL}", "\n").Replace("{RF}", "\r");
                    line++;
                    if (localeText[line].Length > 5)
                        missionCatalog[i].target[j].name[id] = localeText[line].Substring(5).Replace("{CL}", "\n").Replace("{RF}", "\r");
                    line++;
                    if (localeText[line].Length > 6)
                        missionCatalog[i].target[j].description[id] = localeText[line].Substring(6).Replace("{CL}", "\n").Replace("{RF}", "\r");
                    line++;
                }
                line++;
            }
        }

        public void GetLocaleById(int id, string outputTextFile)
        {
            List<string> localeText = new List<string>();
            foreach(var mission in missionCatalog)
            {
                localeText.Add("BASE_TITLE@" + mission._title[id].Replace("\n", "{CL}").Replace("\r", "{RF}"));
                localeText.Add("BASE_DESCR@" + mission._description[id].Replace("\n", "{CL}").Replace("\r", "{RF}"));
                foreach(var target in mission.target)
                {
                    localeText.Add("TITLE@" + target.title[id].Replace("\n", "{CL}").Replace("\r", "{RF}"));
                    localeText.Add("NAME@" + target.name[id].Replace("\n", "{CL}").Replace("\r", "{RF}"));
                    localeText.Add("DESCR@" + target.description[id].Replace("\n", "{CL}").Replace("\r", "{RF}"));
                }
                localeText.Add("");
            }
            File.WriteAllLines(outputTextFile, localeText);
        }

        public void Deserialize(string inputJson)
        {
            Read();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.NullValueHandling = NullValueHandling.Include;
            missionCatalog = JsonConvert.DeserializeObject<List<Mission>>(new StreamReader(inputJson).ReadToEnd(), settings);
        }

        public void Serialize()
        {
            Read();
            var outJson = JsonConvert.SerializeObject(missionCatalog, Formatting.Indented);
            File.WriteAllText(inputFile + ".json", outJson);
        }
        public struct MissionFields
        {
            public List<MissionField> fields;
        }

        private MissionField CMF(int ID, int dataType)
        {
            MissionField mf = new MissionField();
            mf.ID = ID;
            mf.dataType = dataType;
            return mf;
        }

        private MissionFields ReadFields(BinaryReader reader)
        {
            int num = reader.ReadInt32();
            MissionFields mfs = new MissionFields();
            mfs.fields = new List<MissionField>();
            for (int i = 0; i < num; i++)
            {
                MissionField field = ReadMissionField(reader);
                mfs.fields.Add(field);
            }
            return mfs;
        }
        private MissionField ReadMissionField(BinaryReader reader)
        {
            MissionField mf = new MissionField();
            mf.ID = reader.ReadInt32();
            mf.dataType = reader.ReadInt32();
            switch (mf.dataType)
            {
                case 0: 
                    mf.fData = reader.ReadSingle(); 
                    break;
                case 2:
                case 5:
                    mf.iData = reader.ReadInt32(); 
                    break;
                case 1:
                case 3:
                case 4:
                    mf.strData = reader.ReadString();
                    break;
            }
            return mf;
        }

        private MissionTarget ReadMissionTarget(BinaryReader reader)
        {
            MissionTarget mt = new MissionTarget();
            mt.title = new string[13];
            mt.name = new string[13];
            mt.description = new string[13];
            for (int i = 0; i < 13; i++)
                mt.description[i] = "";
            for (int i = 0; i < 13; i++)
                mt.title[i] = reader.ReadString();
            for (int i = 0; i < 13; i++)
                mt.name[i] = reader.ReadString();
            mt.target = reader.ReadString();
            mt.altMonster = reader.ReadString();
            mt.targetCount = reader.ReadInt32();
            mt.targetChallengeAdjust = reader.ReadInt32();
            mt.targetFlags = new List<int>();
            int num = reader.ReadInt32();
            for (int i = 0; i < num; i++)
            {
                mt.targetFlags.Add(reader.ReadInt32());
            }
            mt.customPath = new List<Vector2>();
            if (HasCustomPath(mt.targetFlags))
            {
                int num2 = reader.ReadInt32();
                for(int i = 0; i < num2; i++)
                {
                    mt.customPath.Add(new Vector2(reader.ReadSingle(), reader.ReadSingle()));
                }
            }
            for(int i = 0; i < 13; i++)
            {
                mt.description[i] = reader.ReadString();
            }
            return mt;
        }

        internal bool HasCustomPath(List<int> targetFlags)
        {
            if (targetFlags.Contains(23) || targetFlags.Contains(24) || targetFlags.Contains(25) || targetFlags.Contains(26) || targetFlags.Contains(27) || targetFlags.Contains(28) || targetFlags.Contains(29) || targetFlags.Contains(30) || targetFlags.Contains(31) || targetFlags.Contains(32))
            {
                return true;
            }
            return false;
        }

        private Mission ReadMission(BinaryReader reader)
        {
            Mission mission = new Mission();
            mission._title = new string[13];
            mission._description = new string[13];
            mission.name = reader.ReadString();
            mission.target = new List<MissionTarget>();
            for(int i = 0; i < 13; i++)
            {
                mission._title[i] = reader.ReadString();
            }
            for (int i = 0; i < 13; i++)
            {
                mission._description[i] = reader.ReadString();
            }
            int num = reader.ReadInt32();
            for (int i = 0; i < num; i++)
            {
                mission.target.Add(ReadMissionTarget(reader));
            }
            mission.flags = new List<int>();
            int num2 = reader.ReadInt32();
            for (int i = 0; i < num2; i++)
            {
                mission.flags.Add(reader.ReadInt32());
            }
            mission.area = reader.ReadInt32();
            mission.value = reader.ReadInt32();
            mission.challengeTier = reader.ReadInt32();
            mission.runes = new int[5];
            for (int i = 0; i < 5; i++)
                mission.runes[i] = reader.ReadInt32();
            mission.unlocks = new List<string>();
            int num3 = reader.ReadInt32();
            for (int i = 0; i < num3; i++)
            {
                mission.unlocks.Add(reader.ReadString());
            }
            mission.fields = ReadFields(reader);
            mission.areaTier = mission.challengeTier;
            return mission;
        }

        public void Write(string outputMissionFile)
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(inputFile));
            BinaryWriter writer = new BinaryWriter(File.Create(outputMissionFile));
            writer.Write(reader.ReadBytes((int)reader.BaseStream.Length));
            reader.Close();
            writer.BaseStream.Position = 0;
            writer.Write(missionCatalog.Count);
            for(int i = 0; i < missionCatalog.Count; i++)
            {
                writer.Write(missionCatalog[i].name);
                for(int j = 0; j < 13; j++)
                    writer.Write(missionCatalog[i]._title[j]);
                for (int j = 0; j < 13; j++)
                    writer.Write(missionCatalog[i]._description[j]);
                writer.Write(missionCatalog[i].target.Count);
                for(int j = 0; j < missionCatalog[i].target.Count; j++)
                {
                    for(int k = 0; k < missionCatalog[i].target[j].title.Length; k++)
                    {
                        writer.Write(missionCatalog[i].target[j].title[k]);
                    }
                    for (int k = 0; k < missionCatalog[i].target[j].name.Length; k++)
                    {
                        writer.Write(missionCatalog[i].target[j].name[k]);
                    }
                    writer.Write(missionCatalog[i].target[j].target);
                    writer.Write(missionCatalog[i].target[j].altMonster);
                    writer.Write(missionCatalog[i].target[j].targetCount);
                    writer.Write(missionCatalog[i].target[j].targetChallengeAdjust);
                    writer.Write(missionCatalog[i].target[j].targetFlags.Count);
                    for (int k = 0; k < missionCatalog[i].target[j].targetFlags.Count; k++)
                    {
                        writer.Write(missionCatalog[i].target[j].targetFlags[k]);
                    }
                    if (HasCustomPath(missionCatalog[i].target[j].targetFlags))
                    {
                        writer.Write(missionCatalog[i].target[j].customPath.Count);
                        for(int k = 0; k < missionCatalog[i].target[j].customPath.Count; k++)
                        {
                            writer.Write(missionCatalog[i].target[j].customPath[k].X);
                            writer.Write(missionCatalog[i].target[j].customPath[k].Y);
                        }
                    }
                    for (int k = 0; k < missionCatalog[i].target[j].description.Length; k++)
                    {
                        writer.Write(missionCatalog[i].target[j].description[k]);
                    }
                }
                writer.Write(missionCatalog[i].flags.Count);
                for(int j = 0; j < missionCatalog[i].flags.Count; j++)
                {
                    writer.Write(missionCatalog[i].flags[j]);
                }
                writer.Write(missionCatalog[i].area);
                writer.Write(missionCatalog[i].value);
                writer.Write(missionCatalog[i].challengeTier);
                for (int j = 0; j < missionCatalog[i].runes.Length; j++)
                    writer.Write(missionCatalog[i].runes[j]);
                writer.Write(missionCatalog[i].unlocks.Count);
                for (int j = 0; j < missionCatalog[i].unlocks.Count; j++)
                {
                    writer.Write(missionCatalog[i].unlocks[j]);
                }
                //fields
                writer.Write(missionCatalog[i].fields.fields.Count);
                for(int j = 0; j < missionCatalog[i].fields.fields.Count; j++)
                {
                    writer.Write(missionCatalog[i].fields.fields[j].ID);
                    writer.Write(missionCatalog[i].fields.fields[j].dataType);
                    switch (missionCatalog[i].fields.fields[j].dataType)
                    {
                        case 0:
                            writer.Write(missionCatalog[i].fields.fields[j].fData);
                            break;
                        case 2:
                        case 5:
                            writer.Write(missionCatalog[i].fields.fields[j].iData);
                            break;
                        case 1:
                        case 3:
                        case 4:
                            writer.Write(missionCatalog[i].fields.fields[j].strData);
                            break;
                    }
                }
            }
        }
        public void Read()
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(inputFile));
            int num = reader.ReadInt32();
            missionCatalog = new List<Mission>();
            for (int i = 0; i < num; i++)
            {
                missionCatalog.Add(ReadMission(reader));
            }
            reader.Close();
        }
    }
}
