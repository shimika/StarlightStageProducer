using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarlightStageProducer {
	class Data {
		private static List<Idol> idols = new List<Idol>();
		public static List<Idol> Idols {
			get { return idols; }
			set {
				idols = value.OrderBy(i => i.Id / 100000)
					.ThenByDescending(i => i.RarityNumber)
					.ThenByDescending(i => i.Id).ToList();
				idols.Where(i => !CountMap.ContainsKey(i.Id))
					.ToList().ForEach(i => CountMap.Add(i.Id, 0));
			}
		}

		public static int[] SkillCount = new int[] { 3, 2, 0, 0, 0, 0, 0 };
		public static Skill[] SKillIndex = new Skill[] { Skill.Score, Skill.Combo, Skill.PerfectSupport, Skill.ComboSupport, Skill.Heal, Skill.Guard, Skill.None };

		public static Dictionary<int, int> CountMap = new Dictionary<int, int>();

		private static IdolCompare compare = new IdolCompare();

		public enum Burst { Vocal, Dance, Visual, None };
		public static Burst BurstMode = Burst.None;

		public static void SetBurstMode(int index) {
			switch (index) {
				case 1:
					BurstMode = Burst.Vocal;
					break;
				case 2:
					BurstMode = Burst.Dance;
					break;
				case 3:
					BurstMode = Burst.Visual;
					break;
				default:
					BurstMode = Burst.None;
					break;
			}
		}

		public static void ApplyCount(int id, int count) {
			if (CountMap.ContainsKey(id)) {
				CountMap[id] = count;
			}
		}

		public static int GetCount(int id) {
			if (CountMap.ContainsKey(id)) {
				return CountMap[id];
			}
			return 0;
		}

		public static string GetCheckCount() {
			return string.Format("Selected: {0} / {1}", CountMap.Count(i => i.Value > 0), Idols.Count);
		}

		public static List<Idol> GetMyIdols() {
			return Idols
				.Where(i => CountMap.ContainsKey(i.Id))
				.SelectMany(i => Enumerable.Repeat(i, CountMap[i.Id])).ToList();
		}

		public static List<Idol> GetSSR() {
			return Idols.Where(i => i.RarityNumber == 8).ToList();
		}

		private static int getSkillIndex(Skill skill) {
			return Array.IndexOf(SKillIndex, skill);
		}

		public static Deck CalculateBest(List<Idol> myIdols, List<Idol> guests, Type musicType) {
			if (musicType == Type.All && BurstMode != Burst.None) { return null; }

			List<Deck> deckRank = new List<Deck>();
			foreach (Idol guest in guests) {
				foreach (Idol leader in myIdols) {
					if (SkillCount[getSkillIndex(leader.Skill)] == 0) {
						continue;
					}

					Type[] bonusTypes = new Type[] { guest.Type, leader.Type };
					CenterSkill[] bonusSkills = new CenterSkill[] { guest.CenterSkill, leader.CenterSkill };
					Rarity[] rarities = new Rarity[] { Rarity.SSR, leader.Rarity };

					Idol nGuest = applyBonus(guest, musicType, BurstMode, bonusTypes, bonusSkills, rarities);
					Idol nLeader = applyBonus(leader, musicType, BurstMode, bonusTypes, bonusSkills, rarities);

					int[] skillCount = new int[SkillCount.Length];
					Array.Copy(SkillCount, skillCount, SkillCount.Length);
					skillCount[getSkillIndex(leader.Skill)]--;

					List<Idol> members = new List<Idol>();
					for (int i = 0; i < skillCount.Length; i++) {
						if (skillCount[i] > 0) {
							members.AddRange(myIdols
								.Where(idol => idol.Id != leader.Id)
								.Distinct(compare)
								.Where(idol => idol.Skill == SKillIndex[i])
								.Select(idol => applyBonus(idol, musicType, BurstMode, bonusTypes, bonusSkills, rarities))
								.OrderByDescending(idol => idol.Appeal)
								.Take(skillCount[i]));
						}
					}

					deckRank.Add(new Deck(nGuest, nLeader, members));
				}
			}

			if (deckRank.Count == 0) { return null; }

			Deck best = deckRank
				.OrderByDescending(d => d.MainAppeal)
				.ThenByDescending(d => d.Leader.Appeal)
				.First();

			List<Idol> supporters = Idols
				.Where(i => getIdolLessCount(i.Id, best) > 0)
				.SelectMany(i => Enumerable.Repeat(i, getIdolLessCount(i.Id, best)))
				.Select(i => applyBonus(i, musicType, BurstMode, null, null, null, true))
				.OrderByDescending(i => i.Appeal)
				.Take(10)
				.ToList();

			best.SetSupporters(supporters);

			return best;
		}

		private static Idol applyBonus(Idol idol, Type musicType, Burst burst, Type[] bonusTypes, CenterSkill[] centerSkills, Rarity[] raritys, bool isSupporter = false) {
			int vocal, dance, visual;
			vocal = dance = visual = 100;

			if (!isSupporter) {
				vocal += 10;
				dance += 10;
				visual += 10;
			}

			switch (burst) {
				case Burst.Vocal:
					vocal += 50;
					break;
				case Burst.Dance:
					dance += 50;
					break;
				case Burst.Visual:
					visual += 50;
					break;
			}

			if (musicType == Type.All || idol.Type == musicType) {
				vocal += 30;
				dance += 30;
				visual += 30;
			}

			if (bonusTypes != null) {
				for (int i = 0; i < 2; i++) {
					Type type = bonusTypes[i];
					CenterSkill skill = centerSkills[i];

					int value = 0;
					switch (raritys[i]) {
						case Rarity.R:
							value = 10;
							break;
						case Rarity.SR:
							value = 20;
							break;
						case Rarity.SSR:
							value = 30;
							break;
					}

					if (idol.Type == type) {
						switch (skill) {
							case CenterSkill.All:
								vocal += value;
								dance += value;
								visual += value;
								break;

							case CenterSkill.Vocal:
								vocal += value * 3;
								break;

							case CenterSkill.Dance:
								dance += value * 3;
								break;

							case CenterSkill.Visual:
								visual += value * 3;
								break;
						}
					}
				}
			}

			Idol nIdol = idol.Clone();

			nIdol.Vocal = roundUp(idol.Vocal * vocal, 100);
			nIdol.Dance = roundUp(idol.Dance * dance, 100);
			nIdol.Visual = roundUp(idol.Visual * visual, 100);

			if (isSupporter) {
				nIdol.Vocal = roundUp(nIdol.Vocal * 5, 10);
				nIdol.Dance = roundUp(nIdol.Dance * 5, 10);
				nIdol.Visual = roundUp(nIdol.Visual * 5, 10);
			}

			return nIdol;
		}
		private static int roundUp(int value, int divider) {
			return (value + divider - 1) / divider;
		}

		private static int getIdolLessCount(int id, Deck deck) {
			int count = deck.Leader.Id == id || deck.MemberIds.Contains(id) ? 1 : 0;
			return CountMap[id] - count;
		}

		private static string[] RarityString = new string[] { "", "N", "N+", "R", "R+", "SR", "SR+", "SSR", "SSR+" };
		public static string GetInfo(int id) {
			return GetInfo(Idols.Where(i => i.Id == id).First());
		}

		public static string GetInfo(Idol idol) {
			return string.Format("{0}\n{1}\n\n보컬: {2}\n댄스: {3}\n비쥬얼: {4}\n합: {5}\n\n{6}\n{7}",
				RarityString[idol.RarityNumber],
				idol.Name,
				idol.Vocal,
				idol.Dance,
				idol.Visual,
				idol.Appeal,
				idol.CenterSkill.ToString(),
				idol.Skill.ToString());
		}
	}
}
