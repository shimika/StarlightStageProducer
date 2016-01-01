// Type: System.Net.Json.JsonStringValue
// Assembly: System.Net.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Administrator\Desktop\System.Net.Json.dll

using System.IO;

namespace System.Net.Json {
	public class JsonStringValue : JsonObject {
		private string _value;

		public string Value {
			get {
				if (this._value != null)
					return this._value;
				else
					return string.Empty;
			}
			set {
				this._value = value;
			}
		}

		public JsonStringValue() {
		}

		public JsonStringValue(string name) {
			this.Name = name;
		}

		public JsonStringValue(string name, string value) {
			this.Name = name;
			this.Value = value;
		}

		public override bool Equals(object obj) {
			JsonStringValue jsonStringValue = obj as JsonStringValue;
			if (jsonStringValue == null)
				return false;
			else
				return this.Value == jsonStringValue.Value;
		}

		public override int GetHashCode() {
			return this.Value.GetHashCode();
		}

		public override object GetValue() {
			return (object)this.Value;
		}

		public override string ToString() {
			return base.ToString();
		}

		public override void WriteTo(TextWriter writer) {
			if (this.Name != string.Empty) {
				writer.Write('"');
				writer.Write(this.Name);
				writer.Write('"');
				writer.Write(':');
				JsonUtility.WriteSpace(writer);
			}
			writer.Write(JsonUtility.EscapeString(this.Value));
		}
	}
}
