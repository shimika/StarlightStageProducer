// Type: System.Net.Json.JsonCollection
// Assembly: System.Net.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Administrator\Desktop\System.Net.Json.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace System.Net.Json {
	public abstract class JsonCollection : JsonObject, IList<JsonObject>, ICollection<JsonObject>, IEnumerable<JsonObject>, IEnumerable {
		private bool? _isArray;
		private List<JsonObject> _list;

		protected abstract char BeginCollection { get; }

		public int Count {
			get {
				return this._list.Count;
			}
		}

		protected abstract char EndCollection { get; }

		private bool IsArray {
			get {
				if (!this._isArray.HasValue)
					this._isArray = new bool?(this.GetType() == typeof(JsonArrayCollection));
				return this._isArray.Value;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}

		public JsonObject this[int index] {
			get {
				return this._list[index];
			}
			set {
				this._list[index] = value;
			}
		}

		public JsonCollection() {
			this._isArray = new bool?();
			this._list = new List<JsonObject>();
		}

		public JsonCollection(IEnumerable<JsonObject> collection) {
			this._isArray = new bool?();
			this._list = new List<JsonObject>();
			this._list.AddRange(collection);
		}

		public JsonCollection(string name) {
			this._isArray = new bool?();
			this._list = new List<JsonObject>();
			this.Name = name;
		}

		public JsonCollection(string name, IEnumerable<JsonObject> collection) {
			this._isArray = new bool?();
			this._list = new List<JsonObject>();
			this.Name = name;
			this._list.AddRange(collection);
		}

		public void Add(JsonObject item) {
			this._list.Add(item);
		}

		public void Clear() {
			this._list.Clear();
		}

		public bool Contains(JsonObject item) {
			return this._list.Contains(item);
		}

		public void CopyTo(JsonObject[] array, int arrayIndex) {
			this._list.CopyTo(array, arrayIndex);
		}

		public override bool Equals(object obj) {
			throw new NotImplementedException();
		}

		public IEnumerator<JsonObject> GetEnumerator() {
			return (IEnumerator<JsonObject>)this._list.GetEnumerator();
		}

		public override int GetHashCode() {
			throw new NotImplementedException();
		}

		public override object GetValue() {
			return (object)this._list;
		}

		public int IndexOf(JsonObject item) {
			return this._list.IndexOf(item);
		}

		public void Insert(int index, JsonObject item) {
			this._list.Insert(index, item);
		}

		public bool Remove(JsonObject item) {
			return this._list.Remove(item);
		}

		public void RemoveAt(int index) {
			this._list.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return (IEnumerator)this._list.GetEnumerator();
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
			writer.Write(this.BeginCollection);
			JsonUtility.WriteLine(writer);
			++JsonUtility.IndentDepth;
			JsonUtility.WriteIndent(writer);
			for (int index = 0; index < this.Count; ++index) {
				if (index > 0) {
					writer.Write(',');
					JsonUtility.WriteLine(writer);
					JsonUtility.WriteIndent(writer);
				}
				this[index].WriteTo(writer);
			}
			JsonUtility.WriteLine(writer);
			--JsonUtility.IndentDepth;
			JsonUtility.WriteIndent(writer);
			writer.Write(this.EndCollection);
		}
	}
}
