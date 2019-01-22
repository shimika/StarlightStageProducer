using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace StarlightStageProducer {
	public class Idol {
		public int Id { get; internal set; }
		public Rarity Rarity { get; internal set; }
		public int RarityNumber { get; internal set; }
		public string ImageUrl { get; internal set; }
		public int InfoId { get; internal set; }
		public Type Type { get; internal set; }
		public int Vocal { get; set; }
		public int Dance { get; set; }
		public int Visual { get; set; }
		public string OriginalName { get; internal set; }
		public CenterSkill CenterSkill { get; internal set; }
		public CenterSkillType CenterSkillType { get; internal set; }
        public CenterSkillCondition CenterSkillCondition { get; internal set; }
        public Skill Skill { get; internal set; }

		public string ParsedName { get; internal set; }
		private string name;
		public string Name {
			get { return name; }
			internal set {
				name = value;
				ParsedName = Parser.DivideKorean(name);
			}
		}

		public int Appeal {
			get { return Vocal + Dance + Visual; }
		}

		public Idol() {
			this.Id = -1;
			this.Type = Type.All;
			this.Rarity = Rarity.N;
			this.CenterSkill = CenterSkill.None;
			this.CenterSkillType = CenterSkillType.Unknown;
            this.CenterSkillCondition = CenterSkillCondition.None;
        }

		private T parseEnum<T>(string str) where T : struct, IConvertible {
			if (!typeof(T).IsEnum) {
				throw new ArgumentException("T must be an enumerated type");
			}
			try {
				return (T)Enum.Parse(typeof(T), str);
			}
			catch { }
			return (T)Enum.GetValues(typeof(T)).GetValue(0);
		}

		public Idol(int id, string rarity, int rarityNumber, string type, int vocal, int dance, int visual, string name, string originalName, string centerSkill, string centerSkillType, string centerSkillCondition, string skill) {
			this.Id = id;
			this.Rarity = parseEnum<Rarity>(rarity);
			this.RarityNumber = rarityNumber;
			this.Type = parseEnum<Type>(type);
			//this.Type = (Type)Enum.Parse(typeof(Type), type);
			this.Vocal = vocal;
			this.Dance = dance;
			this.Visual = visual;
			this.Name = name;
			this.OriginalName = originalName;
			this.CenterSkill = parseEnum<CenterSkill>(centerSkill);
			this.Skill = parseEnum<Skill>(skill);
			this.CenterSkillType = parseEnum<CenterSkillType>(centerSkillType);
            this.CenterSkillCondition = parseEnum<CenterSkillCondition>(centerSkillCondition);
        }

		public Idol(int id, Rarity rarity, int rarityNumber, string imageUrl, int infoId, Type type, int vocal, int dance, int visual, string name, string originalName, CenterSkill centerSkill, CenterSkillType centerSkillType, CenterSkillCondition centerSkillCondition, Skill skill) {
			this.Id = id;
			this.Rarity = rarity;
			this.RarityNumber = rarityNumber;
			this.ImageUrl = imageUrl;
			this.InfoId = infoId;
			this.Type = type;
			this.Vocal = vocal;
			this.Dance = dance;
			this.Visual = visual;
			this.Name = name;
			this.OriginalName = originalName;
			this.CenterSkill = centerSkill;
			this.CenterSkillType = centerSkillType;
            this.CenterSkillCondition = centerSkillCondition;
            this.Skill = skill;
		}

		public Idol(int id, string rarity, string imageUrl, int infoId, string type, int vocal, int dance, int visual, string[] names, string[] skills) {
			this.Id = id;

			switch (rarity) {
				case "N":
					this.RarityNumber = 1;
					this.Rarity = Rarity.N;
					break;
				case "N+":
					this.RarityNumber = 2;
					this.Rarity = Rarity.N;
					break;
				case "R":
					this.RarityNumber = 3;
					this.Rarity = Rarity.R;
					break;
				case "R+":
					this.RarityNumber = 4;
					this.Rarity = Rarity.R;
					break;
				case "SR":
					this.RarityNumber = 5;
					this.Rarity = Rarity.SR;
					break;
				case "SR+":
					this.RarityNumber = 6;
					this.Rarity = Rarity.SR;
					break;
				case "SSR":
					this.RarityNumber = 7;
					this.Rarity = Rarity.SSR;
					break;
				case "SSR+":
					this.RarityNumber = 8;
					this.Rarity = Rarity.SSR;
					break;
			}
			
			this.ImageUrl = imageUrl;
			this.InfoId = infoId;

			switch (type) {
				case "큐트":
					this.Type = Type.Cute;
					break;
				case "쿨":
					this.Type = Type.Cool;
					break;
				case "패션":
					this.Type = Type.Passion;
					break;
			}

			this.Vocal = vocal;
			this.Dance = dance;
			this.Visual = visual;

			this.Name = names[0];
			if (names.Length > 1) { this.OriginalName = names[1]; }

			//Console.WriteLine("{0}", skills.Length);

			foreach (string skill in skills) {
				string[] split = skill.Split(':');
                //Console.WriteLine("{0} {1}", split[0], split[1]);

                string[] centerSkillSplit = { "센터 효과" };
                string infoScore = Network.GET(string.Format("{0}={1}", Network.InfoEndPoint, infoId));

				this.CenterSkillType = CenterSkillType.Unknown;
                this.CenterSkillCondition = CenterSkillCondition.None;

                if (infoScore.IndexOf("큐트 아이돌만 편성") >= 0)
                {
                    this.CenterSkillCondition = CenterSkillCondition.Cute;
                }
                else if (infoScore.IndexOf("쿨 아이돌만 편성") >= 0)
                {
                    this.CenterSkillCondition = CenterSkillCondition.Cool;
                }
                else if (infoScore.IndexOf("패션 아이돌만 편성") >= 0)
                {
                    this.CenterSkillCondition = CenterSkillCondition.Passion;
                }
                else if (infoScore.IndexOf("3 타입 아이돌이 전부 편성") >= 0)
                {
                    this.CenterSkillCondition = CenterSkillCondition.All;
                }

                if (infoScore.IndexOf("큐트 아이돌의") >= 0) {
					this.CenterSkillType = CenterSkillType.Cute;
				}
                else if(infoScore.IndexOf("쿨 아이돌의") >= 0) {
					this.CenterSkillType = CenterSkillType.Cool;
				}
				else if(infoScore.IndexOf("패션 아이돌의") >= 0) {
					this.CenterSkillType = CenterSkillType.Passion;
                }
                else if (infoScore.IndexOf("모두의") >= 0)
                {
                    this.CenterSkillType = CenterSkillType.All;
                }

                if (split[0] == "C") {
					switch (split[1]) {
						case "보컬어필":
							this.CenterSkill = CenterSkill.Vocal;
							break;
						case "댄스어필":
							this.CenterSkill = CenterSkill.Dance;
							break;
						case "비쥬얼어필":
							this.CenterSkill = CenterSkill.Visual;
							break;
						case "전어필":
							this.CenterSkill = CenterSkill.All;
							break;
                        case "라이프UP":
                            this.CenterSkill = CenterSkill.Life;
                            break;
                        case "포춘 프레젠트":
                            this.CenterSkill = CenterSkill.Present;
                            break;
                        case "신데렐라 챰":
                            this.CenterSkill = CenterSkill.Charm;
                            break;
                        default:
							this.CenterSkill = CenterSkill.None;
							break;
					}
				} else if (split[0] == "S") {
					switch (split[1]) {
						case "스코어":
							this.Skill = Skill.Score;
							break;

						case "콤보보너스":
							this.Skill = Skill.Combo;
							break;

						case "회복":
							this.Skill = Skill.Heal;
							break;

						case "판강":
							string infoJudge = Network.GET(string.Format("{0}={1}", Network.InfoEndPoint, infoId));
							if (infoJudge == null) {
								this.Skill = Skill.None;
							}
							else {
								if (infoJudge.IndexOf("GREAT를") >= 0) {
									this.Skill = Skill.PerfectSupport;
								}
								else if (infoJudge.IndexOf("GREAT/NICE") >= 0) { 
									this.Skill = Skill.PerfectSupport;
								}
								else if (infoJudge.IndexOf("NICE여도") >= 0) {
									this.Skill = Skill.ComboSupport;
								}
								else {
									this.Skill = Skill.None;
								}
							}

							break;

						case "오버로드":
							this.Skill = Skill.Overload;
							break;

						case "무적":
							this.Skill = Skill.Guard;
							break;

						default:
							this.Skill = Skill.None;
							break;
					}
				}
			}
		}
	}
}
