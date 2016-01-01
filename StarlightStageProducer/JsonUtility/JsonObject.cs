// Type: System.Net.Json.JsonObject
// Assembly: System.Net.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Administrator\Desktop\System.Net.Json.dll

using System.IO;

namespace System.Net.Json {
	public abstract class JsonObject {
		private string _name;

		public string Name {
			get {
				if (this._name == null)
					return string.Empty;
				else
					return this._name;
			}
			set {
				if (value == null)
					this._name = string.Empty;
				else
					this._name = value.Trim();
			}
		}

		public abstract override bool Equals(object obj);

		public abstract override int GetHashCode();

		public abstract object GetValue();

		public override string ToString() {
			StringWriter stringWriter = new StringWriter();
			this.WriteTo((TextWriter)stringWriter);
			return stringWriter.ToString();
		}

		public abstract void WriteTo(TextWriter writer);
	}
}
