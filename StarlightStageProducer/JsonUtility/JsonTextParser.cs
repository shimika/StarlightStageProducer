// Type: System.Net.Json.JsonTextParser
// Assembly: System.Net.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Administrator\Desktop\System.Net.Json.dll

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Net.Json {
	public sealed class JsonTextParser {
		private static readonly Regex _regexLiteral = new Regex("(?<value>false|true|null)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex _regexNumber = new Regex("(?<minus>[-])?(?<int>(0)|([1-9])[0-9]*)(?<frac>\\.[0-9]+)?(?<exp>(e|E)([-]|[+])?[0-9]+)?", RegexOptions.Compiled);
		private string s = string.Empty;
		private object SyncObject = new object();
		private int c;

		private char cur {
			get {
				return this.s[this.c];
			}
		}

		private bool IsEOS {
			get {
				return this.c >= this.s.Length;
			}
		}

		static JsonTextParser() {
		}

		public JsonObject Parse(string text) {
			lock (this.SyncObject) {
				this.c = 0;
				if (text == null)
					throw new FormatException();
				this.s = text.Trim();
				if (this.s == string.Empty)
					throw new FormatException();
				try {
					return this.ParseSomethingWithoutName();
				} catch (Exception exception_0) {
					throw;
				}
			}
		}

		private JsonCollection ParseCollection() {
			this.SkipWhiteSpace();
			bool flag = false;
			JsonCollection jsonCollection;
			if ((int)this.s[this.c] == 123) {
				jsonCollection = (JsonCollection)new JsonObjectCollection();
			} else {
				if ((int)this.s[this.c] != 91)
					throw new FormatException();
				flag = true;
				jsonCollection = (JsonCollection)new JsonArrayCollection();
			}
			++this.c;
			this.SkipWhiteSpace();
			if ((int)this.s[this.c] != 125 && (int)this.s[this.c] != 93) {
				while (true) {
					string str = string.Empty;
					if (!flag)
						str = this.ParseName();
					JsonObject jsonObject = this.ParseSomethingWithoutName();
					if (jsonObject != null) {
						if (!flag)
							jsonObject.Name = str;
						jsonCollection.Add(jsonObject);
						this.SkipWhiteSpace();
						if ((int)this.s[this.c] == 44) {
							++this.c;
							this.SkipWhiteSpace();
						} else
							goto label_14;
					} else
						break;
				}
				throw new Exception();
			label_14:
				this.SkipWhiteSpace();
			}
			if (flag) {
				if ((int)this.s[this.c] != 93)
					throw new FormatException();
			} else if ((int)this.s[this.c] != 125)
				throw new FormatException();
			++this.c;
			return jsonCollection;
		}

		private JsonBooleanValue ParseLiteralValue() {
			Match match = JsonTextParser._regexLiteral.Match(this.s, this.c);
			if (!match.Success)
				throw new FormatException("Cannot parse a literal value.");
			string str = match.Captures[0].Value;
			this.c += str.Length;
			return new JsonBooleanValue((string)null, str);
		}

		private string ParseName() {
			if (this.IsEOS)
				throw new FormatException("Cannot find object item's name.");
			if ((int)this.s[this.c] != 34)
				throw new FormatException();
			++this.c;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			while (!this.IsEOS) {
				char ch = this.s[this.c];
				if ((int)ch == 92 && !flag) {
					stringBuilder.Append(ch);
					++this.c;
					flag = true;
				} else if ((int)ch != 34 || flag) {
					stringBuilder.Append(ch);
					++this.c;
					flag = false;
				} else {
					++this.c;
					this.SkipWhiteSpace();
					if (this.IsEOS)
						throw new FormatException();
					if ((int)this.s[this.c] != 58)
						throw new FormatException();
					++this.c;
					return JsonUtility.UnEscapeString(((object)stringBuilder).ToString());
				}
			}
			throw new FormatException();
		}

		private JsonNumericValue ParseNumericValue() {
			Match match = JsonTextParser._regexNumber.Match(this.s, this.c);
			if (!match.Success)
				throw new FormatException("Cannot parse a number value.");
			string s = match.Captures[0].Value;
			this.c += s.Length;
			return new JsonNumericValue(double.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, (IFormatProvider)JsonUtility.CultureInfo));
		}

		private JsonObject ParseSomethingWithoutName() {
			this.SkipWhiteSpace();
			if ((int)this.s[this.c] == 123 | (int)this.s[this.c] == 91)
				return (JsonObject)this.ParseCollection();
			if ((int)this.s[this.c] == 34)
				return (JsonObject)this.ParseStringValue();
			if (char.IsDigit(this.s[this.c]) || (int)this.s[this.c] == 45)
				return (JsonObject)this.ParseNumericValue();
			if ((int)this.s[this.c] != 116 && (int)this.s[this.c] != 102 && (int)this.s[this.c] != 110)
				throw new FormatException("Cannot parse a value.");
			else
				return (JsonObject)this.ParseLiteralValue();
		}

		private JsonStringValue ParseStringValue() {
			if ((int)this.s[this.c] != 34)
				throw new FormatException();
			++this.c;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			while (!this.IsEOS) {
				char ch = this.s[this.c];
				if ((int)ch == 92 && !flag) {
					stringBuilder.Append(ch);
					++this.c;
					flag = true;
				} else if ((int)ch != 34 || flag) {
					stringBuilder.Append(ch);
					++this.c;
					flag = false;
				} else {
					if ((int)this.s[this.c] != 34)
						throw new FormatException();
					++this.c;
					return new JsonStringValue() {
						Value = JsonUtility.UnEscapeString(((object)stringBuilder).ToString())
					};
				}
			}
			throw new FormatException();
		}

		private void SkipWhiteSpace() {
			while (!this.IsEOS && char.IsWhiteSpace(this.s[this.c]))
				++this.c;
			if (this.IsEOS)
				throw new FormatException();
		}
	}
}
