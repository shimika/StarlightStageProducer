// Type: System.Net.Json.JsonUtility
// Assembly: System.Net.Json, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Administrator\Desktop\System.Net.Json.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace System.Net.Json {
	public static class JsonUtility {
		internal static readonly CultureInfo CultureInfo = new CultureInfo("en-US", false);
		public static bool GenerateIndentedJsonText = true;
		internal static readonly SortedDictionary<int, int> IndentDepthCollection = new SortedDictionary<int, int>();
		public static int MaxDepthNesting = -1;
		public static int MaxStringLength = 1024;
		public static int MaxTextLength = -1;
		internal const char begin_array = '[';
		internal const char begin_object = '{';
		internal const char end_array = ']';
		internal const char end_object = '}';
		internal const char indent = '\t';
		public const string MimeType = "application/json";
		internal const char name_separator = ':';
		internal const char quote = '"';
		internal const char space = ' ';
		internal const char value_separator = ',';

		internal static int IndentDepth {
			get {
				int threadId = JsonUtility.ThreadId;
				try {
					return JsonUtility.IndentDepthCollection[threadId];
				} catch (KeyNotFoundException ex) {
					return 0;
				}
			}
			set {
				JsonUtility.IndentDepthCollection[JsonUtility.ThreadId] = value;
			}
		}

		internal static int ThreadId {
			get {
				return Thread.CurrentThread.ManagedThreadId;
			}
		}

		static JsonUtility() {
		}

		internal static string EscapeNonPrintCharacter(char c) {
			return "\\u" + ((int)c).ToString("x4");
		}

		internal static string EscapeString(string text) {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('"');
			foreach (char c in text) {
				switch (c) {
					case '\b':
						stringBuilder.Append("\\b");
						break;
					case '\t':
						stringBuilder.Append("\\t");
						break;
					case '\n':
						stringBuilder.Append("\\n");
						break;
					case '\f':
						stringBuilder.Append("\\f");
						break;
					case '\r':
						stringBuilder.Append("\\r");
						break;
					case '"':
						stringBuilder.Append("\\\"");
						break;
					case '\\':
						stringBuilder.Append("\\\\");
						break;
					default:
						if (char.IsLetterOrDigit(c)) {
							stringBuilder.Append(c);
							break;
						} else if (char.IsPunctuation(c)) {
							stringBuilder.Append(c);
							break;
						} else if (char.IsSeparator(c)) {
							stringBuilder.Append(c);
							break;
						} else if (char.IsWhiteSpace(c)) {
							stringBuilder.Append(c);
							break;
						} else if (char.IsSymbol(c)) {
							stringBuilder.Append(c);
							break;
						} else {
							stringBuilder.Append(JsonUtility.EscapeNonPrintCharacter(c));
							break;
						}
				}
			}
			stringBuilder.Append('"');
			return ((object)stringBuilder).ToString();
		}

		internal static string GetIndentString() {
			int indentDepth = JsonUtility.IndentDepth;
			if (indentDepth <= 0)
				return string.Empty;
			else
				return new string('\t', indentDepth);
		}

		internal static string UnEscapeString(string text) {
			text = text.Trim();
			if (text.StartsWith("\""))
				text = text.Remove(0, 1);
			if (text.EndsWith("\"") && !text.EndsWith("\\\""))
				text = text.Remove(text.Length - 1, 1);
			StringBuilder stringBuilder = new StringBuilder();
			try {
				for (int index = 0; index < text.Length; ++index) {
					char ch1 = text[index];
					if ((int)ch1 == 92) {
						++index;
						if ((int)text[index] != 117 && (int)text[index] != 85) {
							if ((int)text[index] != 110) {
								if ((int)text[index] != 114) {
									if ((int)text[index] != 116) {
										if ((int)text[index] != 102) {
											if ((int)text[index] != 98) {
												if ((int)text[index] != 92) {
													if ((int)text[index] != 47) {
														if ((int)text[index] != 34)
															throw new FormatException("Unrecognized escape sequence '\\" + (object)text[index] + "' in position: " + (string)(object)index + ".");
														else
															stringBuilder.Append('"');
													} else
														stringBuilder.Append('/');
												} else
													stringBuilder.Append('\\');
											} else
												stringBuilder.Append('\b');
										} else
											stringBuilder.Append('\f');
									} else
										stringBuilder.Append('\t');
								} else
									stringBuilder.Append('\r');
							} else
								stringBuilder.Append('\n');
						} else {
							char ch2 = (char)int.Parse(text.Substring(index + 1, 4), NumberStyles.HexNumber);
							index += 4;
							stringBuilder.Append(ch2);
						}
					} else
						stringBuilder.Append(ch1);
				}
			} catch (Exception ex) {
				throw;
			}
			return ((object)stringBuilder).ToString();
		}

		internal static void WriteIndent(TextWriter writer) {
			if (!JsonUtility.GenerateIndentedJsonText)
				return;
			writer.Write(JsonUtility.GetIndentString());
		}

		internal static void WriteLine(TextWriter writer) {
			if (!JsonUtility.GenerateIndentedJsonText)
				return;
			writer.Write(Environment.NewLine);
		}

		internal static void WriteSpace(TextWriter writer) {
			if (!JsonUtility.GenerateIndentedJsonText)
				return;
			writer.Write(' ');
		}
	}
}
