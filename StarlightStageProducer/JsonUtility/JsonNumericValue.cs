// Type: System.Net.Json.JsonNumericValue
// Assembly: System.Net.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Administrator\Desktop\System.Net.Json.dll

using System;
using System.IO;

namespace System.Net.Json {
	public class JsonNumericValue : JsonObject {
		private double _value;

		public double Value {
			get {
				return this._value;
			}
			set {
				this._value = value;
			}
		}

		public JsonNumericValue() {
		}

		public JsonNumericValue(double value) {
			this.Value = value;
		}

		public JsonNumericValue(int value) {
			this.Value = (double)value;
		}

		public JsonNumericValue(long value) {
			this.Value = (double)value;
		}

		public JsonNumericValue(float value) {
			this.Value = (double)value;
		}

		public JsonNumericValue(string name) {
			this.Name = name;
		}

		public JsonNumericValue(string name, double value) {
			this.Name = name;
			this.Value = value;
		}

		public JsonNumericValue(string name, int value) {
			this.Name = name;
			this.Value = (double)value;
		}

		public JsonNumericValue(string name, long value) {
			this.Name = name;
			this.Value = (double)value;
		}

		public JsonNumericValue(string name, float value) {
			this.Name = name;
			this.Value = (double)value;
		}

		public override bool Equals(object obj) {
			JsonNumericValue jsonNumericValue = obj as JsonNumericValue;
			if (jsonNumericValue == null)
				return false;
			else
				return this.Value == jsonNumericValue.Value;
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
			writer.Write(this.Value.ToString("g", (IFormatProvider)JsonUtility.CultureInfo));
		}
	}
}
