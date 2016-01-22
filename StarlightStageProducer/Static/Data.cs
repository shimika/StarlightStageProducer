using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarlightStageProducer {
	class Data {
		public static string[] RarityString = new string[] { "", "N", "N+", "R", "R+", "SR", "SR+", "SSR", "SSR+" };
		public static Skill[] SKillIndex = new Skill[] { Skill.Score, Skill.Combo, Skill.PerfectSupport, Skill.ComboSupport, Skill.Heal, Skill.Guard, Skill.None };
		public enum Burst { Vocal, Dance, Visual, None };

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

		public static List<Idol> GetMyIdols() {
			return Idols
				.Where(i => CountMap.ContainsKey(i.Id))
				.SelectMany(i => Enumerable.Repeat(i, CountMap[i.Id])).ToList();
		}

		public static List<Idol> GetSSR() {
			return Idols.Where(i => i.RarityNumber == 8).ToList();
		}

		public static int[] SkillCount = new int[] { 3, 2, 0, 0, 0, 0, 0 };
		public static Dictionary<int, int> CountMap = new Dictionary<int, int>();
		private static IdolCompare compare = new IdolCompare();
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

		private static int getSkillIndex(Skill skill) {
			return Array.IndexOf(SKillIndex, skill);
		}

		public static Deck CalculateBest(List<Idol> myIdols, List<Idol> guests, Burst burstMode, Type musicType) {
			if (musicType == Type.All && burstMode != Burst.None) { return null; }
			if (burstMode != Burst.None) { guests = new List<Idol>(new Idol[] { new Idol() }); }

			List<Deck> deckRank = new List<Deck>();
			foreach (Idol guest in guests) {
				foreach (Idol leader in myIdols) {
					if (SkillCount[getSkillIndex(leader.Skill)] == 0) {
						continue;
					}

					Idol[] effectIdols = new Idol[] { leader, guest };

					Idol nGuest = applyBonus(guest, musicType, burstMode, effectIdols);
					Idol nLeader = applyBonus(leader, musicType, burstMode, effectIdols);

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
								.Select(idol => applyBonus(idol, musicType, burstMode, effectIdols))
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
				.Select(i => applyBonus(i, musicType, burstMode, null, true))
				.OrderByDescending(i => i.Appeal)
				.Take(10)
				.ToList();

			best.SetSupporters(supporters);

			return best;
		}

		private static Idol applyBonus(Idol idol, Type musicType, Burst burst, Idol[] effectIdols, bool isSupporter = false) {
			int vocal, dance, visual;
			vocal = dance = visual = 100;

			if (!isSupporter) {
				vocal += 10;
				dance += 10;
				visual += 10;
			}

			switch (burst) {
				case Burst.Vocal:
					vocal += 150;
					break;
				case Burst.Dance:
					dance += 150;
					break;
				case Burst.Visual:
					visual += 150;
					break;
			}

			if (musicType == Type.All || idol.Type == musicType) {
				vocal += 30;
				dance += 30;
				visual += 30;
			}

			if (effectIdols != null) {
				foreach (Idol effectIdol in effectIdols) {
					int value = 1;
					switch (effectIdol.CenterSkillType) {
						case Type.All:
							value = 8;
							break;
						default:
							value = 10;
							break;
					}

					switch (effectIdol.Rarity) {
						case Rarity.R:
							break;
						case Rarity.SR:
							value *= 2;
							break;
						case Rarity.SSR:
							value *= 3;
							break;
						default:
							value = 0;
							break;
					}

					if (idol.Type == effectIdol.Type || effectIdol.CenterSkillType == Type.All) {
						switch (effectIdol.CenterSkill) {
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
	}
}
