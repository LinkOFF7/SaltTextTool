using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace salt_strings.Chronicler
{
    public class LocPair
	{
		public string orig;
		public string[] locStr;
		public StringBuilder[] locString;
		public string notes;
		public string unk;

		public LocPair(int ID)
		{
			orig = "NEW_" + ID;
			locStr = new string[13];
			for (int i = 0; i < locStr.Length; i++)
			{
				locStr[i] = "";
			}
			locString = new StringBuilder[13];
			notes = "";
		}
		public LocPair(string p)
		{
			orig = p;
			locStr = new string[13];
			for (int i = 0; i < locStr.Length; i++)
			{
				locStr[i] = "";
			}
			locString = new StringBuilder[13];
			notes = "";
		}
		[JsonConstructor]
		public LocPair() { }
	}
}
