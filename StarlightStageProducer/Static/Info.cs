using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarlightStageProducer.Static {
	class Info {
		public static string GetCheckCount() {
			return string.Format("Selected: {0} / {1}", Data.CountMap.Count(i => i.Value > 0), Data.Idols.Count);
		}

		public static string GetInfo(int id) {
			return GetInfo(Data.GetIdol(id));
		}

		public static string GetInfo(Idol idol) {
			string basic = string.Format("{0}\n{1}\n\n보컬: {2}\n댄스: {3}\n비쥬얼: {4}\n합: {5}\n\n",
				Data.RarityString[idol.RarityNumber],
				idol.Name,
				idol.Vocal,
				idol.Dance,
				idol.Visual,
				idol.Appeal);

			string target = "", condition = "";
			float skillBonus = 0, rarityBonus = 0;
            switch (idol.CenterSkillCondition)
            {
                case CenterSkillCondition.All:
                    condition = "3 타입 아이돌이 모두 편성되어 있을 경우, ";
                    break;
                case CenterSkillCondition.Cute:
                    condition = "큐트 아이돌만 편성되어 있을 경우, ";
                    break;
                case CenterSkillCondition.Cool:
                    condition = "쿨 아이돌만 편성되어 있을 경우, ";
                    break;
                case CenterSkillCondition.Passion:
                    condition = "패션 아이돌만 편성되어 있을 경우, ";
                    break;
            }

			switch (idol.CenterSkillType) {
				case CenterSkillType.All:
					skillBonus = 8;
					target = "모든 아이돌의 ";
					break;
				case CenterSkillType.Cute:
					skillBonus = 10;
					target = "큐트 아이돌의 ";
					break;
				case CenterSkillType.Cool:
					skillBonus = 10;
					target = "쿨 아이돌의 ";
					break;
				case CenterSkillType.Passion:
					skillBonus = 10;
					target = "패션 아이돌의 ";
					break;
            }

			switch (idol.Rarity) {
				case Rarity.R:
					rarityBonus = 1;
					break;
				case Rarity.SR:
					rarityBonus = 2;
					break;
				case Rarity.SSR:
					rarityBonus = 3;
					break;
				case Rarity.N:
					rarityBonus = 0;
					break;
			}

			string centerSkill = "";
			switch (idol.CenterSkill) {
				case CenterSkill.All:
					centerSkill = string.Format("{0}{1}모든 어필 {2}%", condition, target, skillBonus * rarityBonus);
					break;
				case CenterSkill.Vocal:
					centerSkill = string.Format("{0}{1}보컬 어필 {2}%", condition, target, skillBonus * rarityBonus * 3);
					break;
				case CenterSkill.Dance:
					centerSkill = string.Format("{0}{1}댄스 어필 {2}%", condition, target, skillBonus * rarityBonus * 3);
					break;
				case CenterSkill.Visual:
					centerSkill = string.Format("{0}{1}비쥬얼 어필 {2}%", condition, target, skillBonus * rarityBonus * 3);
					break;
				case CenterSkill.None:
					centerSkill = "기타 센터 스킬";
					break;
			}

			string skill = "";
			switch (idol.Skill) {
				case Skill.Score:
					switch (idol.Rarity) {
						case Rarity.R:
							skill = "Perfect 스코어 보너스 10%";
							break;

						case Rarity.SR:
							skill = "Perfect 스코어 보너스 15%";
							break;

						case Rarity.SSR:
							skill = "Perfect/Great 스코어 보너스 17%";
							break;
					}
					break;

				case Skill.Combo:
					switch (idol.Rarity) {
						case Rarity.R:
							skill = "콤보 보너스 8%";
							break;

						case Rarity.SR:
							skill = "콤보 보너스 12%";
							break;

						case Rarity.SSR:
							skill = "콤보 보너스 15%";
							break;
					}
					break;

				case Skill.PerfectSupport:
					switch (idol.Rarity) {
						case Rarity.R:
							skill = "Great를 Perfect로";
							break;

						case Rarity.SR:
							skill = "Great/Nice를 Perfect로";
							break;

						case Rarity.SSR:
							skill = "Great/Nice/Bad를 Perfect로";
							break;
					}
					break;

				case Skill.ComboSupport:
					skill = "Nice 콤보 유지";
					break;

				case Skill.Guard:
					skill = "무적";
					break;

				case Skill.Overload:
					switch (idol.Rarity) {
						case Rarity.R:
							skill = "오버로드";
							break;

						case Rarity.SR:
							skill = "라이프를 15 소모\nPERFECT 스코어가 보너스 16%\nNice/Bad 콤보 유지";
							break;

						case Rarity.SSR:
							skill = "오버로드";
							break;
					}
					break;

				case Skill.Heal:
					switch (idol.Rarity) {
						case Rarity.R:
							skill = "Perfect로 라이프 2회복";
							break;

						case Rarity.SR:
						case Rarity.SSR:
							skill = "Perfect로 라이프 3회복";
							break;
					}

					break;
			}

			return string.Format("{0}{1}\n{2}", basic, centerSkill, skill);
		}
	}
}
