using System;
using System.Collections.Generic;
using System.IO;
using salt_strings.Chronicler;
using Newtonsoft.Json;
using System.Linq;

namespace salt_strings.Strings
{
	public class LocText
    {
        public string inputFile;
        private List<LocPair> locStrings;

        public void Deserialize(string inputJson)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.NullValueHandling = NullValueHandling.Include;
            locStrings = JsonConvert.DeserializeObject<List<LocPair>>(new StreamReader(inputJson).ReadToEnd(), settings);
        }

        public void WriteLocaleById(int id, string inputTextFile, string outputLocTextFile)
        {
            Read();
            List<string> localeText = File.ReadAllLines(inputTextFile).ToList();
            for (int i = 0; i < locStrings.Count; i++)
            {
                locStrings[i].locStr[id] = localeText[i];
            }
            Write(outputLocTextFile);
        }

        public void GetLocaleById(int id, string outputTextFile)
        {
            Read();
            List<string> localeText = new List<string>();
            for(int i = 0; i < locStrings.Count; i++)
            {
                localeText.Add(locStrings[i].locStr[id].Replace("\r", "{RF}").Replace("\n", "{CL}"));
            }
            File.WriteAllLines(outputTextFile, localeText);
        }
        public void Serialize()
        {
            Read();
            var outJson = JsonConvert.SerializeObject(locStrings, Formatting.Indented);
            File.WriteAllText(inputFile + ".json", outJson);
        }

        public void Write(string outputStringfile)
        {
            if (locStrings == null) return;
            BinaryWriter writer = new BinaryWriter(File.Create(outputStringfile));
            writer.Write(locStrings.Count);
            for(int i = 0; i < locStrings.Count; i++)
            {
                writer.Write(locStrings[i].orig);
                for (int j = 0; j < 13; j++)
                {
                    writer.Write(locStrings[i].locStr[j]);
                }
                writer.Write(locStrings[i].unk);
            }
        }
        private void Read()
        {
            if (inputFile == null) return;
            BinaryReader reader = new BinaryReader(File.OpenRead(inputFile));
            locStrings = new List<LocPair>();
            int num = reader.ReadInt32();
            for(int i = 0; i < num; i++)
            {
                LocPair locPair = new LocPair(reader.ReadString());
                for (int j = 0; j < 13; j++)
                {
                    locPair.locStr[j] = reader.ReadString();
                }
                locPair.unk = reader.ReadString();
                locStrings.Add(locPair);
            }
            reader.Close();
        }
    }
}
