// Type: System.Net.Json.JsonBooleanValue
// Assembly: System.Net.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Administrator\Desktop\System.Net.Json.dll

using System;
using System.IO;

namespace System.Net.Json {
	public class JsonBooleanValue : JsonObject {
		private bool? _value;

		public bool? Value {
			get {
				return this._value;
			}
			set {
				this._value = value;
			}
		}

		public JsonBooleanValue() {
			this._value = new bool?();
		}

		public JsonBooleanValue(bool? value) {
			this._value = new bool?();
			this.Value = value;
		}

		public JsonBooleanValue(string name) {
			this._value = new bool?();
		}

		public JsonBooleanValue(string name, bool? value) {
			this._value = new bool?();
			this.Name = name;
			this.Value = value;
		}

		public JsonBooleanValue(string name, string value) {
			this._value = new bool?();
			this.Name = name;
			this._value = new bool?();
			if (value == null)
				return;
			value = value.Trim().ToLower();
			if (!(value != string.Empty))
				return;
			switch (value.Trim().ToLower()) {
				case "null":
					this._value = new bool?();
					break;
				case "true":
					this._value = new bool?(true);
					break;
				case "false":
					this._value = new bool?(false);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public override bool Equals(object obj) {
			JsonBooleanValue jsonBooleanValue = obj as JsonBooleanValue;
			if (jsonBooleanValue == null)
				return false;
			bool? nullable1 = this.Value;
			bool? nullable2 = jsonBooleanValue.Value;
			if (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault())
				return nullable1.HasValue == nullable2.HasValue;
			else
				return false;
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
			if (this.Value.HasValue)
				writer.Write(this.Value.ToString().ToLower());
			else
				writer.Write("null");
		}
	}
}
