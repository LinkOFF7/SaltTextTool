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
    internal class SkillTree
    {
        public string inputFile;
        public SkillNode[] nodes;
        public SkillImg[] imgs;
        public struct SkillNode
        {
            public string name;
            public string[] title;
            public string[] desc;
            public string[] baseDesc;
            public int ID;
            public int type;
            public int value;
            public int cost;
            public int[] parent;
            public Vector2 loc;
            public int max;
        }

        public struct SkillImg
        {
            public int img;
            public Vector2 loc;
            public float rotation;
            public Vector2 scale;
        }

        public void InsertLocaleById(int id, string inputTextfile)
        {
            List<string> localeText = File.ReadAllLines(inputTextfile).ToList();
            int currentLine = 0;
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].title[id] = localeText[currentLine].Substring(6).Replace("{CL}", "\n").Replace("{RF}", "\r");
                currentLine++;
                nodes[i].desc[id] = localeText[currentLine].Substring(12).Replace("{CL}", "\n").Replace("{RF}", "\r");
                currentLine++;
                nodes[i].baseDesc[id] = localeText[currentLine].Substring(17).Replace("{CL}", "\n").Replace("{RF}", "\r");
                currentLine+=2;
            }
        }
        public void GetLocaleById(int id, string outputTextfile)
        {
            List<string> localeText = new List<string>();
            for(int i = 0; i < nodes.Length; i++)
            {
                localeText.Add("TITLE@"+nodes[i].title[id].Replace("\n", "{CL}").Replace("\r", "{RF}"));
                localeText.Add("DESCRIPTION@" + nodes[i].desc[id].Replace("\n", "{CL}").Replace("\r", "{RF}"));
                localeText.Add("BASE_DESCRIPTION@" + nodes[i].baseDesc[id].Replace("\n", "{CL}").Replace("\r", "{RF}"));
                localeText.Add("");
            }
            File.WriteAllLines(outputTextfile, localeText);
        }
        public void DeserializeNodes(string inputJson)
        {
            Read();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            nodes = JsonConvert.DeserializeObject<SkillNode[]>(new StreamReader(inputJson).ReadToEnd(), settings);
        }

        public void Write(string outputSkillFile)
        {
            if (nodes == null) return;
            BinaryReader reader = new BinaryReader(File.OpenRead(inputFile));
            BinaryWriter writer = new BinaryWriter(File.Create(outputSkillFile));
            writer.Write(reader.ReadBytes((int)reader.BaseStream.Length));
            reader.Close();
            writer.BaseStream.Position = 0;
            writer.Write(nodes.Length);
            for(int i = 0; i < nodes.Length; i++)
            {
                WriteSkillNode(writer, i);
            }
            writer.Write(imgs.Length);
            if(imgs.Length > 0)
            {
                for (int i = 0; i < imgs.Length; i++)
                {
                    WriteSkillImg(writer, i);
                }
            }
            writer.Close();
        }

        public void Serialize()
        {
            Read();
            var outJson = JsonConvert.SerializeObject(nodes, Formatting.Indented);
            File.WriteAllText(inputFile + ".json", outJson);
        }

        private void WriteSkillImg(BinaryWriter writer, int id)
        {
            SkillImg img = imgs[id];
            writer.Write(img.img);
            writer.Write(img.loc.X);
            writer.Write(img.loc.Y);
            writer.Write(img.rotation);
            writer.Write(img.scale.X);
            writer.Write(img.scale.Y);
        }
        private SkillImg ReadSkillImg(BinaryReader reader)
        {
            SkillImg skillImg = new SkillImg();
            skillImg.img = reader.ReadInt32();
            skillImg.loc = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            skillImg.rotation = reader.ReadSingle();
            skillImg.scale = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            return skillImg;
        }

        private void WriteSkillNode(BinaryWriter writer, int id)
        {
            SkillNode node = nodes[id];
            writer.Write(node.name);
            for (int i = 0; i < 13; i++)
                writer.Write(node.title[i]);
            for (int i = 0; i < 13; i++)
                writer.Write(node.desc[i]);
            for (int i = 0; i < 13; i++)
                writer.Write(node.baseDesc[i]);
            writer.Write(node.type);
            writer.Write(node.value);
            writer.Write(node.cost);
            for (int i = 0; i < 2; i++)
                writer.Write(node.parent[i]);
            writer.Write(node.loc.X);
            writer.Write(node.loc.Y);
        }
        private SkillNode ReadSkillNode(BinaryReader reader, int id)
        {
            SkillNode node = new SkillNode();
            node.name = reader.ReadString();
            node.title = new string[13];
            node.desc = new string[13];
            node.baseDesc = new string[13];
            node.ID = id;
            for (int i = 0; i < 13; i++)
            {
                node.title[i] = reader.ReadString();
            }
            for (int i = 0; i < 13; i++)
            {
                node.desc[i] = reader.ReadString();
            }
            for (int i = 0; i < 13; i++)
            {
                node.baseDesc[i] = reader.ReadString();
            }
            node.type = reader.ReadInt32();
            node.value = reader.ReadInt32();
            node.cost = reader.ReadInt32();
            node.parent = new int[2];
            for (int l = 0; l < 2; l++)
            {
                node.parent[l] = reader.ReadInt32();
            }
            node.loc = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            node.max = GetMaxTreeUnlock(node.cost, node.type);
            return node;
        }

        internal int GetMaxTreeUnlock(int cost, int type)
        {
            if (cost > 1)
            {
                return 1;
            }
            int num = type;
            if ((uint)num <= 8u)
            {
                return 5;
            }
            return 1;
        }
        public void Read()
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(inputFile));
            int num = reader.ReadInt32();
            nodes = new SkillNode[num];
            for(int i = 0; i < num; i++)
            {
                nodes[i] = ReadSkillNode(reader, i);
            }
            int num2 = reader.ReadInt32();
            imgs = new SkillImg[num2];
            for (int i = 0; i < num2; i++)
            {
                imgs[i] = ReadSkillImg(reader);
            }
            reader.Close();
        }
    }
}
