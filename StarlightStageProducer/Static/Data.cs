﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StarlightStageProducer {
	class Data {
		public static string[] RarityString = new string[] { "", "N", "N+", "R", "R+", "SR", "SR+", "SSR", "SSR+" };
        public static Skill[] SkillIndex = new Skill[] {
            Skill.None,
            Skill.Score,
            Skill.Combo,
            Skill.PerfectSupport,
            Skill.ComboSupport,
            Skill.Heal,
            Skill.Guard,
            Skill.Overload,
            Skill.Ignore
		};
        public static Type[] TypeIndex = new Type[] {
            Type.All,
            Type.Cute,
            Type.Cool,
            Type.Passion
        };
        public enum Burst { Vocal, Dance, Visual, None };

		private static List<Idol> idols = new List<Idol>();
		private static Dictionary<int, Idol> idolDict = new Dictionary<int, Idol>();
		public static List<Idol> Idols {
			get { return idols; }
			set {
				idols = value.OrderBy(i => i.Id / 100000)
					.ThenByDescending(i => i.RarityNumber)
					.ThenByDescending(i => i.Id).ToList();
				idols.Where(i => !CountMap.ContainsKey(i.Id))
					.ToList().ForEach(i => CountMap.Add(i.Id, 0));
				idolDict = idols.ToDictionary(i => i.Id);
			}
		}

		public static Idol GetIdol(int id) {
			return idolDict[id];
		}

		public static List<Idol> GetMyIdols() {
			return Idols
				.Where(i => CountMap.ContainsKey(i.Id))
				.SelectMany(i => Enumerable.Repeat(i, CountMap[i.Id])).ToList();
		}

		public static int[] SkillCount = new int[] { 0, 3, 2, 0, 0, 0, 0, 0 };
		public static Dictionary<int, int> CountMap = new Dictionary<int, int>();
		public static Burst BurstMode = Burst.None;
		public static bool CheckSkill = true;

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
			return Array.IndexOf(SkillIndex, skill);
		}

        private static int getTypeIndex(Type type)
        {
            return Array.IndexOf(TypeIndex, type);
        }

        public static Deck CalculateBest(Burst burstMode, Type musicType) {
			if (musicType == Type.All && burstMode != Burst.None) { return null; }

			List<Idol> guests = null;
			if (burstMode != Burst.None) {
				guests = new List<Idol>(new Idol[] { new Idol() });
			}
			else {
				guests = Idols.Where(i => i.RarityNumber == 8).ToList();
			}

			List<Idol> myIdols = Idols
				.Where(i => CountMap.ContainsKey(i.Id) && 
							CountMap[i.Id] > 0)
				.ToList();

			Deck bestDeck = null;
			List<Deck> deckRank = new List<Deck>();
			foreach (Idol guest in guests) {
				foreach (Idol leader in myIdols) {
					Idol[] effectIdols = new Idol[] { leader, guest };

					Bonus bonus = getBonus(musicType, burstMode, effectIdols);

					IdolSummary nGuest = applyBonus2(guest, bonus, false);
					IdolSummary nLeader = applyBonus2(leader, bonus, false);

					List<IdolSummary> members = new List<IdolSummary>();

                    int[] skillCount, typeCount;

                    if(leader.CenterSkillType == CenterSkillType.Fes || guest.CenterSkillType == CenterSkillType.Fes)
                    {
                        typeCount = new int[] { 1, 1, 1, 1 };
                        if(typeCount[getTypeIndex(leader.Type)] != 0)
                        {
                            typeCount[getTypeIndex(leader.Type)]--;
                            typeCount[getTypeIndex(Type.All)]++;
                        }
                        if (typeCount[getTypeIndex(guest.Type)] != 0)
                        {
                            typeCount[getTypeIndex(guest.Type)]--;
                            typeCount[getTypeIndex(Type.All)]++;
                        }
                    }
                    else typeCount = new int[] { 4, 0, 0, 0 };

                    skillCount = new int[SkillCount.Length + 1];
                    skillCount[getSkillIndex(Skill.Ignore)] = 5;
                    if (CheckSkill)
                    {
                        for (int i = 0; i < SkillCount.Length; i++)
                        {
                            skillCount[i] = SkillCount[i];
                            skillCount[getSkillIndex(Skill.Ignore)] -= SkillCount[i];
                        }
                    }
                    if (skillCount[getSkillIndex(leader.Skill)] != 0) skillCount[getSkillIndex(leader.Skill)]--;
                    else if (skillCount[getSkillIndex(Skill.Ignore)] == 0) continue;
                    else skillCount[getSkillIndex(Skill.Ignore)]--;
                    

                    members.AddRange(getRankedIdol(myIdols, leader.Id, skillCount, typeCount, bonus, 4));
                    
					Deck deck = new Deck(nGuest, nLeader, members);
                    
                    if (bestDeck == null) {
						bestDeck = deck;
					}
					else if (bestDeck.isBetter(deck)) {
						bestDeck = deck;
					}
				}
			}

			if (bestDeck == null) { return null; }

			Bonus supportBonus = getBonus(musicType, burstMode, null, true);

			List<IdolSummary> supporters = Idols
				.Where(i => getIdolLessCount(i.Id, bestDeck) > 0)
				.SelectMany(i => Enumerable.Repeat(i, getIdolLessCount(i.Id, bestDeck)))
				.Select(i => applyBonus2(i, supportBonus, true))
				.OrderByDescending(i => i.Appeal)
				.Take(10)
				.ToList();

			bestDeck.SetSupporters(supporters);

			return bestDeck;
		}

		public static Dictionary<string, List<IdolSummary>> CacheList = new Dictionary<string, List<IdolSummary>>();

        private static List<IdolSummary> getRankedIdol(List<Idol> idols, int leaderId, int[] skillCount, int[] typeCount, Bonus bonus, int count)
        {
            string key = string.Format("{0}:{1}:{2}:{3}", bonus, skillCount, typeCount, leaderId);
            List<IdolSummary> list = null;

            if (CacheList.ContainsKey(key))
            {
                list = CacheList[key];
            }
            else
            {
                list = new List<IdolSummary>();
                foreach (Idol idol in idols)
                {
                    list.Add(applyBonus2(idol, bonus, false));
                }
                list.Sort();

                List<IdolSummary> templist = new List<IdolSummary>();

                int i = 0;
                while(templist.Count < 6)
                {
                    if (i >= list.Count) break;
                    if (list[i].Id != leaderId)
                        if((skillCount[getSkillIndex(list[i].Skill)] != 0 || skillCount[getSkillIndex(Skill.Ignore)] != 0) &&
                            (typeCount[getTypeIndex(list[i].Type)] != 0 || typeCount[getTypeIndex(Type.All)] != 0))
                        {
                            templist.Add(list[i]);

                            if (skillCount[getSkillIndex(list[i].Skill)] != 0) skillCount[getSkillIndex(list[i].Skill)]--;
                            else skillCount[getSkillIndex(Skill.Ignore)]--;
                            if (typeCount[getTypeIndex(list[i].Type)] != 0) typeCount[getTypeIndex(list[i].Type)]--;
                            else typeCount[getTypeIndex(Type.All)]--;
                        }
                    i++;
                }

                list = templist;

                CacheList.Add(key, list);
            }

            return list.Where(idol => idol.Id != leaderId).Take(count).ToList();
        }

        private static Bonus getBonus(Type musicType, Burst burst, Idol[] effectIdols, bool isSupporter = false) {
			Bonus bonus = new Bonus();

			bonus.AddAppeal(Type.All, AppealType.Vocal, 100);
			bonus.AddAppeal(Type.All, AppealType.Dance, 100);
			bonus.AddAppeal(Type.All, AppealType.Visual, 100);

			if (!isSupporter) {
				bonus.AddAppeal(Type.All, AppealType.Vocal, 10);
				bonus.AddAppeal(Type.All, AppealType.Dance, 10);
				bonus.AddAppeal(Type.All, AppealType.Visual, 10);
			}

			switch (burst) {
				case Burst.Vocal:
					bonus.AddAppeal(Type.All, AppealType.Vocal, 150);
					break;
				case Burst.Dance:
					bonus.AddAppeal(Type.All, AppealType.Dance, 150);
					break;
				case Burst.Visual:
					bonus.AddAppeal(Type.All, AppealType.Visual, 150);
					break;
			}

			bonus.AddAppeal(musicType, AppealType.Vocal, 30);
			bonus.AddAppeal(musicType, AppealType.Dance, 30);
			bonus.AddAppeal(musicType, AppealType.Visual, 30);

			if (effectIdols != null) {
				foreach (Idol effectIdol in effectIdols) {
					float value = 1;
					switch (effectIdol.CenterSkillType) {
						case CenterSkillType.All:
							value = 8;
							break;
                        case CenterSkillType.Fes:
                            value = 100 / 9;
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

					if (effectIdol.CenterSkillType == CenterSkillType.All || effectIdol.CenterSkillType == CenterSkillType.Fes) {
						switch (effectIdol.CenterSkill) {
							case CenterSkill.All:
								bonus.AddAppeal(Type.All, AppealType.Vocal, (int)value);
								bonus.AddAppeal(Type.All, AppealType.Dance, (int)value);
								bonus.AddAppeal(Type.All, AppealType.Visual, (int)value);
								break;

							case CenterSkill.Vocal:
								bonus.AddAppeal(Type.All, AppealType.Vocal, (int)(value * 3));
								break;

							case CenterSkill.Dance:
								bonus.AddAppeal(Type.All, AppealType.Dance, (int)(value * 3));
								break;

							case CenterSkill.Visual:
								bonus.AddAppeal(Type.All, AppealType.Visual, (int)(value * 3));
								break;
						}
					}
					else {
						switch (effectIdol.CenterSkill) {
							case CenterSkill.All:
								bonus.AddAppeal(effectIdol.Type, AppealType.Vocal, (int)value);
								bonus.AddAppeal(effectIdol.Type, AppealType.Dance, (int)value);
								bonus.AddAppeal(effectIdol.Type, AppealType.Visual, (int)value);
								break;

							case CenterSkill.Vocal:
								bonus.AddAppeal(effectIdol.Type, AppealType.Vocal, (int)(value * 3));
								break;

							case CenterSkill.Dance:
								bonus.AddAppeal(effectIdol.Type, AppealType.Dance, (int)(value * 3));
								break;

							case CenterSkill.Visual:
								bonus.AddAppeal(effectIdol.Type, AppealType.Visual, (int)(value * 3));
								break;
						}
					}
				}
			}

			return bonus;
		}
		
		private static IdolSummary applyBonus2(Idol idol, Bonus bonus, bool isSupporter) {
			IdolSummary idolSummary = new IdolSummary();

			if (idol.Id < 0) { return idolSummary; }

			int vocal = bonus.GetAppeal(idol.Type, AppealType.Vocal);
			int dance = bonus.GetAppeal(idol.Type, AppealType.Dance);
			int visual = bonus.GetAppeal(idol.Type, AppealType.Visual);

			vocal = roundUp(idol.Vocal * vocal, 100);
			dance = roundUp(idol.Dance * dance, 100);
			visual = roundUp(idol.Visual * visual, 100);

			if (isSupporter) {
				vocal = roundUp(vocal * 5, 10);
				dance = roundUp(dance * 5, 10);
				visual = roundUp(visual * 5, 10);
			}

			idolSummary.Id = idol.Id;
			idolSummary.Skill = idol.Skill;
			idolSummary.Appeal = vocal + dance + visual;
            idolSummary.Type = idol.Type;

			return idolSummary;
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
