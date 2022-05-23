using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace salt_strings
{
    internal class Dialog
    {
        public string filename;
        List<NPCDialog> npcDialogs;
        byte[] footer = { 0x8D, 0xBB, 0xE4, 0xB8, 0x8A, 0xE7, 0xA5, 0xAD, 0xE5, 0x93, 0x81, 0xE3, 0x80, 0x82, 0x7D };
        struct NPCDialog
        {
            public string name;
            public List<DialogNode> nodes;
        }
        struct DialogNode
        {
            public string name;
            public TextSeries[] text;
            public List<NodeOption> nodeOption;
            public List<NodePrecheck> nodePrecheck;
            public string postSetFlagStr;
            public string postGoto;
            public bool coopGive;
            public string[] giveScript;
            public string[] storeScript;
            public bool postSetCoop;
            public bool bLeave;
        }
        struct TextSeries
        {
            public string[] text;
        }

        struct NodeOption
        {
            public string[] text;
            public string action;
            public string coopAction;
        }

        struct NodePrecheck
        {
            public string precheckFlag;
            public string precheckGoto;
        }

        public void WriteLocaleById(int id, string inputTextFile, string outputDialogfile)
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(filename));
            Read(reader);
            reader.Close();
            int count = 0;
            List<string> localeText = File.ReadAllLines(inputTextFile).ToList();
            for(int i = 0; i < npcDialogs.Count; i++)
            {
                for(int j = 0; j < npcDialogs[i].nodes.Count; j++)
                {
                    TextSeries ts_copy = npcDialogs[i].nodes[j].text[id];
                    if (ts_copy.text == null)
                        continue;
                    for (int q = 0; q < ts_copy.text.Length; q++)
                    {
                        ts_copy.text[q] = localeText[count].Replace("{RF}", "\r").Replace("{CL}", "\n");
                        count++;
                    }
                    npcDialogs[i].nodes[j].text[id] = ts_copy;
                }
            }
            Write(outputDialogfile);
        }
        public void GetLocaleById(int id, string outputTextfile)
        {
            //0 - english
            BinaryReader reader = new BinaryReader(File.OpenRead(filename));
            Read(reader);
            List<string> localeText = new List<string>();
            foreach (NPCDialog dialog in npcDialogs)
            {
                foreach(var node in dialog.nodes)
                {
                    if (node.text[id].text != null)
                    {
                        for (int i = 0; i < node.text[id].text.Length; i++)
                            localeText.Add(node.text[id].text[i].Replace("\r", "{RF}").Replace("\n", "{CL}"));
                    }   
                }
            }
            File.WriteAllLines(outputTextfile, localeText);
        }

        public void Write(string outputDialogFile)
        {
            BinaryWriter writer = new BinaryWriter(File.Create(outputDialogFile));
            //npcDialogs
            writer.Write(npcDialogs.Count);
            for(int i = 0; i < npcDialogs.Count; i++)
            {
                writer.Write(npcDialogs[i].name);

                //nodes
                writer.Write(npcDialogs[i].nodes.Count);
                for(int j = 0; j < npcDialogs[i].nodes.Count; j++)
                {
                    writer.Write(npcDialogs[i].nodes[j].name);

                    //TextSeries
                    for(int k = 0; k < npcDialogs[i].nodes[j].text.Length; k++)
                    {
                        if (npcDialogs[i].nodes[j].text[k].text == null)
                            writer.Write(new byte());
                        else if (k == 6)
                        {
                            //Japan text
                            string text = Flatten(npcDialogs[i].nodes[j].text[k].text, ">");
                            writer.Write(text);
                        }
                        else
                        {
                            string text = Flatten(npcDialogs[i].nodes[j].text[k].text, "\n");
                            writer.Write(text);
                        }
                    }
                    //nodeOptions
                    int num3 = npcDialogs[i].nodes[j].nodeOption.Count;
                    writer.Write(num3);
                    for(int k = 0; k < num3; k++)
                    {
                        for(int q = 0; q < 13; q++)
                        {
                            writer.Write(npcDialogs[i].nodes[j].nodeOption[k].text[q]);
                        }
                        writer.Write(npcDialogs[i].nodes[j].nodeOption[k].action);
                        writer.Write(npcDialogs[i].nodes[j].nodeOption[k].coopAction);
                    }

                    //nodePrecheck
                    int num4 = npcDialogs[i].nodes[j].nodePrecheck.Count;
                    writer.Write(num4);
                    for(int k = 0; k < num4; k++)
                    {
                        writer.Write(npcDialogs[i].nodes[j].nodePrecheck[k].precheckFlag);
                        writer.Write(npcDialogs[i].nodes[j].nodePrecheck[k].precheckGoto);
                    }

                    //node
                    writer.Write(npcDialogs[i].nodes[j].postSetFlagStr);
                    writer.Write(npcDialogs[i].nodes[j].postGoto);
                    writer.Write(npcDialogs[i].nodes[j].coopGive);
                    if (npcDialogs[i].nodes[j].giveScript == null)
                        writer.Write(new byte());
                    else
                    {
                        string text = Flatten(npcDialogs[i].nodes[j].giveScript, "\r\n");
                        writer.Write(text);
                    }
                    if (npcDialogs[i].nodes[j].storeScript == null)
                        writer.Write(new byte());
                    else
                    {
                        string text = Flatten(npcDialogs[i].nodes[j].storeScript, "\r\n");
                        writer.Write(text);
                    }
                    writer.Write(npcDialogs[i].nodes[j].postSetCoop);
                    writer.Write(npcDialogs[i].nodes[j].bLeave);
                }
            }
            writer.Write(footer);
            writer.Write(CreateEmptyByteArray(0x10));
        }

        private byte[] CreateEmptyByteArray(int length)
        {
            byte[] buffer = new byte[length];
            return buffer;
        }

        public static string Flatten(string[] elems, string separator)
        {
            if (elems == null)
                return null;
            StringBuilder sb = new StringBuilder();
            foreach (string elem in elems)
            {
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(elem);
            }
            return sb.ToString();
        }

        public void Deserialize(string inputJson)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.NullValueHandling = NullValueHandling.Include;
            npcDialogs = JsonConvert.DeserializeObject<List<NPCDialog>>(new StreamReader(inputJson).ReadToEnd(), settings);
        }
        public void Serialize()
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(filename));
            Read(reader);
            var json = JsonConvert.SerializeObject(npcDialogs, Formatting.Indented);
            File.WriteAllText(filename + ".json", json);
        }
        public void Read(BinaryReader reader)
        {
            //NPCDialog
            int num = reader.ReadInt32();
            npcDialogs = new List<NPCDialog>();
            for(int i = 0; i < num; i++)
            {
                NPCDialog npcDialog = new NPCDialog();
                npcDialog.name = reader.ReadString();
                int num2 = reader.ReadInt32();
                npcDialog.nodes = new List<DialogNode>();
                for(int j = 0; j < num2; j++)
                {
                    DialogNode node = new DialogNode();
                    node.name = reader.ReadString();
                    node.text = new TextSeries[13];
                    for(int k = 0; k < 13; k++)
                    {
                        node.text[k] = new TextSeries();
                        string text = reader.ReadString();
                        if (text == "")
                            node.text[k].text = null;
                        else if (k == 6)
                        {
                            if (text.StartsWith(">") && text.Length > 1)
                                text = text.Substring(1);
                            node.text[k].text = text.Split('>');
                        }
                        else
                            node.text[k].text = text.Split('\n');
                    }

                    //nodeOption
                    node.nodeOption = new List<NodeOption>();
                    int num3 = reader.ReadInt32();
                    for (int k = 0; k < num3; k++)
                    {
                        NodeOption nodeOption = new NodeOption();
                        nodeOption.text = new string[13];
                        for(int q = 0; q < nodeOption.text.Length; q++)
                        {
                            nodeOption.text[q] = reader.ReadString();
                        }
                        nodeOption.action = reader.ReadString();
                        nodeOption.coopAction = reader.ReadString();
                        node.nodeOption.Add(nodeOption);
                    }

                    //nodePrecheck
                    node.nodePrecheck = new List<NodePrecheck>();
                    int num4 = reader.ReadInt32();
                    for(int k = 0; k < num4; k++)
                    {
                        NodePrecheck nodePrecheck = new NodePrecheck();
                        nodePrecheck.precheckFlag = reader.ReadString();
                        nodePrecheck.precheckGoto = reader.ReadString();
                        node.nodePrecheck.Add(nodePrecheck);
                    }

                    node.postSetFlagStr = reader.ReadString();
                    node.postGoto = reader.ReadString();
                    node.coopGive = reader.ReadBoolean();
                    string text2 = reader.ReadString();
                    if (text2 != "")
                    {
                        node.giveScript = text2.Split('\r');
                        text2 = "";
                        string[] array = node.giveScript;
                        foreach (string text3 in array)
                        {
                            text2 += text3;
                        }
                        node.giveScript = text2.Split('\n');
                    }
                    else
                        node.giveScript = null;
                    text2 = reader.ReadString();
                    if (text2 != "")
                    {
                        node.storeScript = text2.Split('\r');
                        text2 = "";
                        string[] array = node.storeScript;
                        foreach (string text4 in array)
                        {
                            text2 += text4;
                        }
                        node.storeScript = text2.Split('\n');
                    }
                    else
                        node.storeScript = null;
                    node.postSetCoop = reader.ReadBoolean();
                    node.bLeave = reader.ReadBoolean();
                    npcDialog.nodes.Add(node);
                }

                npcDialogs.Add(npcDialog);
            }
        }
    }
}
