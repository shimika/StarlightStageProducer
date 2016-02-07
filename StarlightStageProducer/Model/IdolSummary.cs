using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarlightStageProducer {
	public class IdolSummary : IComparable<IdolSummary> {
		public int Id { get; set; }
		public int Appeal { get; set; }
		public Skill Skill { get; set; }

		public IdolSummary() {
			this.Id = -1;
			this.Appeal = 0;
			this.Skill = Skill.None;
		}

		public int CompareTo(IdolSummary other) {
			if (other == null) { return -1; }
			return other.Appeal.CompareTo(Appeal);
		}
	}
}
