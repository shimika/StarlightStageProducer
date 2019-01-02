using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StarlightStageProducer {
	class FileSystem {
		public static string Root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Shimika\";
		public static string DataFolder = Root + @"\StarlightStageProducer\";
		public static string TempFolder = Root + @"\StarlightStageProducer\temp\";
		public static string DataPath = DataFolder + "data.json";
		public static string CountPath = DataFolder + "check2.txt";

		public static void CheckDirectory() {
			if (!Directory.Exists(Root)) { Directory.CreateDirectory(Root); }
			if (!Directory.Exists(DataFolder)) { Directory.CreateDirectory(DataFolder); }
			if (!Directory.Exists(TempFolder)) { Directory.CreateDirectory(TempFolder); }
		}

		private static String getString(JsonObject obj) {
			try {
				return obj.GetValue().ToString();
			}
			catch {
				return null;
			}
		}

		private static int getInt(JsonObject obj) {
			string r = getString(obj);
			if(r == null) { return 0; }
			return Convert.ToInt32(r);
		}

		public static List<Idol> GetIdols() {
			List<Idol> idols = new List<Idol>();
			if (!File.Exists(DataPath)) { return idols; }

			JsonTextParser parser = new JsonTextParser();
			JsonArrayCollection root = null;

			using (StreamReader sr = new StreamReader(DataPath)) {
				root = (JsonArrayCollection)parser.Parse(sr.ReadToEnd());
			}

			foreach (JsonObjectCollection obj in root) {
				int id = getInt(obj["Id"]);
				string rarity = getString(obj["Rarity"]);
				int rarityNumber = getInt(obj["RarityNumber"]);
				string type = getString(obj["Type"]);
				int cute = getInt(obj["Vocal"]);
				int cool = getInt(obj["Dance"]);
				int passion = getInt(obj["Visual"]);
				string name = getString(obj["Name"]);
				string originalName = getString(obj["OriginalName"]);
				string centerSkill = getString(obj["CenterSkill"]);
				string centerSkillType = getString(obj["CenterSkillType"]);
                string centerSkillCondition = getString(obj["CenterSkillCondition"]);
                string skill = getString(obj["Skill"]);

				idols.Add(new Idol(id, rarity, rarityNumber, type, cute, cool, passion, name, originalName, centerSkill, centerSkillType, centerSkillCondition, skill));
			}

			return idols;
		}

		public static void SaveDatabase(List<Idol> idols) {
			CheckDirectory();

			JsonArrayCollection root = new JsonArrayCollection();

			foreach (Idol idol in idols) {
				JsonObjectCollection obj = new JsonObjectCollection();
				obj.Add(new JsonNumericValue("Id", idol.Id));
				obj.Add(new JsonStringValue("Rarity", idol.Rarity.ToString()));
				obj.Add(new JsonNumericValue("RarityNumber", idol.RarityNumber));
				obj.Add(new JsonStringValue("Type", idol.Type.ToString()));
				obj.Add(new JsonNumericValue("Vocal", idol.Vocal));
				obj.Add(new JsonNumericValue("Dance", idol.Dance));
				obj.Add(new JsonNumericValue("Visual", idol.Visual));
				obj.Add(new JsonStringValue("Name", idol.Name));
				obj.Add(new JsonStringValue("OriginalName", idol.OriginalName));
				obj.Add(new JsonStringValue("CenterSkill", idol.CenterSkill.ToString()));
				obj.Add(new JsonStringValue("CenterSkillType", idol.CenterSkillType.ToString()));
                obj.Add(new JsonStringValue("CenterSkillCondition", idol.CenterSkillCondition.ToString()));
                obj.Add(new JsonStringValue("Skill", idol.Skill.ToString()));
				root.Add(obj);
			}

			using (StreamWriter sw = new StreamWriter(DataPath)) {
				sw.Write(root);
			}
		}

		public static Dictionary<int, int> GetCheck() {
			Dictionary<int, int> dict = Data.Idols.ToDictionary(i => i.Id, i => 0);
			if (!File.Exists(CountPath)) { return dict; }

			JsonTextParser parser = new JsonTextParser();
			JsonArrayCollection root = null;

			using (StreamReader sr = new StreamReader(CountPath)) {
				root = (JsonArrayCollection)parser.Parse(sr.ReadToEnd());
			}

			foreach (JsonObjectCollection obj in root) {
				try {
					int id = getInt(obj["Id"]);
					int count = getInt(obj["Count"]);

					dict[id] = count;
				}catch (Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}

			return dict;
		}

		public static void SaveCheck(Dictionary<int,int> dict) {
			CheckDirectory();

			JsonArrayCollection root = new JsonArrayCollection();

			foreach (KeyValuePair<int,int> kvp in dict) {
				JsonObjectCollection obj = new JsonObjectCollection();

				if (kvp.Value <= 0) { continue; }

				obj.Add(new JsonNumericValue("Id", kvp.Key));
				obj.Add(new JsonNumericValue("Count", kvp.Value));
				root.Add(obj);
			}

			using (StreamWriter sw = new StreamWriter(CountPath)) {
				sw.Write(root);
			}
		}

		public static string GetImagePath(int id) {
			return String.Format("{0}{1}.jpg", FileSystem.DataFolder, id);
		}
	}
}
