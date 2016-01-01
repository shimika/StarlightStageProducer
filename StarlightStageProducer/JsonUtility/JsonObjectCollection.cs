// Type: System.Net.Json.JsonObjectCollection
// Assembly: System.Net.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Administrator\Desktop\System.Net.Json.dll

using System.Collections.Generic;

namespace System.Net.Json {
	public class JsonObjectCollection : JsonCollection {
		protected override char BeginCollection {
			get {
				return '{';
			}
		}

		protected override char EndCollection {
			get {
				return '}';
			}
		}

		public JsonObject this[string name] {
			get {
				for (int index = 0; index < this.Count; ++index) {
					if (base[index].Name == name)
						return base[index];
				}
				return (JsonObject)null;
			}
		}

		public JsonObjectCollection() {
		}

		public JsonObjectCollection(IEnumerable<JsonObject> collection)
			: base(collection) {
		}

		public JsonObjectCollection(string name)
			: base(name) {
		}

		public JsonObjectCollection(string name, IEnumerable<JsonObject> collection)
			: base(name, collection) {
		}
	}
}
