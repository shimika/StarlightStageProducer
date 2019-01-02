using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarlightStageProducer {
	public enum Rarity { N, R, SR, SSR }
	public enum Type { All, Cute, Cool, Passion };
    public enum CenterSkillType { All, Cute, Cool, Passion, Unknown };
    public enum CenterSkillCondition { None, Cute, Cool, Passion, All};
    public enum Skill { None, Score, Combo, PerfectSupport, ComboSupport, Heal, Guard, Overload, Ignore, Unknown };

	public enum CenterSkill { None, Vocal, Dance, Visual, All, Skill, Life, Present, Charm };
	public enum AppealType { Vocal = 0, Dance = 1, Visual = 2, Invalid = -1 };
}
