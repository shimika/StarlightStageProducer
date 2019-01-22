using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;

namespace StarlightStageProducer {
	class Parser {
		public static List<Idol> ParseHtml(string html, Network network) {
			if (html == null) { return null; }

			List<Idol> idols = new List<Idol>();

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);

			removeTag(doc, "//div[@class='ndata']");
			removeTag(doc, "//span[@class='cmtnum']");

			HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes("//div[@class='List']//tbody//tr");

			for (int i = 0; i < nodeList.Count; i++) {
				HtmlNode node = nodeList[i];

				string rarity = node.SelectSingleNode(".//td[@class='field1']").InnerText;
				string imageUrl = node.SelectSingleNode(".//img").GetAttributeValue("src", null);
				string type = node.SelectSingleNode(".//td[@class='field2']").InnerText;
				int vocal = Convert.ToInt32(node.SelectSingleNode(".//td[@class='etc6']//span[@class='t_cute']").InnerText);
				int dance = Convert.ToInt32(node.SelectSingleNode(".//td[@class='etc7']//span[@class='t_cool']").InnerText);
				int visual = Convert.ToInt32(node.SelectSingleNode(".//td[@class='etc8']//span[@class='t_passion']").InnerText);

				int id = extractLastNumber(imageUrl, "/");
				int infoId = extractLastNumber(node.SelectSingleNode(".//div[@class='cardName']//a").GetAttributeValue("onmouseover", "0"), "/");
				string[] names = splitByLine(node.SelectSingleNode(".//div[@class='cardName']//a").InnerHtml);
				string[] skills = splitByLine(node.SelectSingleNode(".//td[@class='field4']").InnerHtml);

				idols.Add(new Idol(id, rarity, imageUrl, infoId, type, vocal, dance, visual, names, skills));

				//Console.WriteLine(string.Format("{0} {1} {2} {3} {4}", rarity, type, vocal, dance, visual));
				Console.WriteLine(string.Format("{0}\n{1}\n{2}  {3} {4}", skills[0], skills[1], names[0], id, infoId));
				//Console.WriteLine();

				network.SendLoadingStatus(string.Format("Database downloading... {0} / {1}", i + 1, nodeList.Count));
			}

			return idols;
		}

		public static String GetLastestVersion(string html) {
			if (html == null) { return null; }

			List<Idol> idols = new List<Idol>();

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);

			removeTag(doc, "//div[@class='ndata']");
			removeTag(doc, "//span[@class='cmtnum']");

			HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes("//h1[@class='release-title']");

			for (int i = 0; i < nodeList.Count; i++) {
				HtmlNode node = nodeList[i];
				return node.InnerText.Trim();
			}

			return null;
		}

		private static void removeTag(HtmlDocument doc, string xPath) {
			HtmlNodeCollection nDataList = doc.DocumentNode.SelectNodes(xPath);
			if (nDataList != null) {
				foreach (HtmlNode node in nDataList) {
					node.Remove();
				}
			}
		}

		private static int extractLastNumber(string text, string divider) {
			string[] split = text.Split(new string[] { divider }, StringSplitOptions.RemoveEmptyEntries);
			return Convert.ToInt32(Regex.Match(split[split.Length - 1], @"\d+").Value);
		}

		private static string[] splitByLine(string html) {
			return html
				.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)
				.Select(t => getInnerText(t)).ToArray();
		}

		private static string getInnerText(string innerHtml) {
			StringBuilder sbTotal = new StringBuilder();
			StringBuilder sb = new StringBuilder();
			int inCount = 0;
			bool isPlus = false;

			foreach (char c in innerHtml) {
				switch (c) {
					case '<':
						if (sb.Length > 0) {
							sbTotal.AppendFormat("{0} ", sb.ToString());
							sb.Clear();
						}
						inCount++;
						break;

					case '【':
						inCount++;
						break;

					case '】':
						inCount--;
						break;

					case '>':
						inCount--;
						break;

					default:
						if (inCount == 0) {
							sb.Append(c);
						}
						else if (c == '＋') {
							isPlus = true;
						}
						break;
				}
			}

			if (sb.Length > 0) {
				sbTotal.AppendFormat("{0} ", sb.ToString());
			}

			string text = sbTotal.ToString()
				.Replace("［", "[")
				.Replace("］", "] ")
				.Replace("＋", "+")
				.Trim();
			if (isPlus) {
				return string.Format("{0}+", text);
			}
			return text;
		}

		public static string DivideKorean(string str) {
			if (str.Trim() == "") { return ""; }
			str = Regex.Replace(str.ToLower().Trim(), @"[^0-9a-z가-힣]+", "");

			string rtStr = "";
			for (int i = 0; i < str.Length; i++) {
				char origChar = str[i];
				if (origChar == ' ') { continue; }
				int unicode = Convert.ToInt32(origChar);

				uint jongCode = 0;
				uint jungCode = 0;
				uint choCode = 0;

				if (unicode < 44032 || unicode > 55203) {
					rtStr += origChar;
					continue;
				}
				else {
					uint uCode = Convert.ToUInt32(origChar - '\xAC00');
					jongCode = uCode % 28;
					jungCode = ((uCode - jongCode) / 28) % 21;
					choCode = ((uCode - jongCode) / 28) / 21;
				}
				char[] choChar = new char[] { 'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
				char[] jungChar = new char[] { 'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ' };
				char[] jongChar = new char[] { ' ', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
				rtStr += choChar[choCode].ToString() + jungChar[jungCode].ToString() + jongChar[jongCode].ToString();
				rtStr = rtStr.Replace(" ", "");
			}
			return rtStr;
		}
	}
}
