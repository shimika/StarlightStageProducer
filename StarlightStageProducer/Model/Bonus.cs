using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarlightStageProducer {
	class Bonus {
		private Dictionary<String, int> dict;

		public Bonus() {
			dict = new Dictionary<string, int>();
		}

		private string getKey(Type type, AppealType appealType) {
			return string.Format("{0}:{1}", type, appealType);
		}

		public int GetAppeal(Type type, AppealType appealType) {
			string keyType = getKey(type, appealType);
			string keyAll = getKey(Type.All, appealType);

			int value = 0;

			if (dict.ContainsKey(keyType)) {
				value += dict[keyType];
			}
			if (dict.ContainsKey(keyAll)) {
				value += dict[keyAll];
			}

			return value;
		}

		public void AddAppeal(Type type, AppealType appealType, int value) {
			string key = getKey(type, appealType);

			if (dict.ContainsKey(key)) {
				dict[key] += value;
			}
			else {
				dict.Add(key, value);
			}
		}

		public override string ToString() {
			string data = "";
			foreach(KeyValuePair<String, int> kvp in dict) {
				data = string.Format("{0}|{1}:{2}", data, kvp.Key, kvp.Value);
			}
			return data;
		}

		public int Vocal { get; set; }
		public int Dance { get; set; }
		public int Visual { get; set; }
	}
}
