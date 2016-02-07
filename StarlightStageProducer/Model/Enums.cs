using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarlightStageProducer {
	public enum Rarity { N, R, SR, SSR }
	public enum Type { All, Cute, Cool, Passion };
	public enum Skill { None, Score, Combo, PerfectSupport, ComboSupport, Heal, Guard };

	public enum CenterSkill { None = -1, Vocal = 0, Dance = 1, Visual = 2, All = 100 };
	public enum AppealType { Vocal = 0, Dance = 1, Visual = 2, Invalid = -1 };
}
