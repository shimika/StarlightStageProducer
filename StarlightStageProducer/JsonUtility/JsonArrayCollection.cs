// Type: System.Net.Json.JsonArrayCollection
// Assembly: System.Net.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Administrator\Desktop\System.Net.Json.dll

using System.Collections.Generic;

namespace System.Net.Json {
	public class JsonArrayCollection : JsonCollection {
		protected override char BeginCollection {
			get {
				return '[';
			}
		}

		protected override char EndCollection {
			get {
				return ']';
			}
		}

		public JsonArrayCollection() {
		}

		public JsonArrayCollection(IEnumerable<JsonObject> collection)
			: base(collection) {
		}

		public JsonArrayCollection(string name)
			: base(name) {
		}

		public JsonArrayCollection(string name, IEnumerable<JsonObject> collection)
			: base(name, collection) {
		}
	}
}
