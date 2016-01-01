using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarlightStageProducer {
	public class Deck {
		public int MainAppeal { get; set; }
		public int SupportAppeal { get; set; }
		public int Appeal { get { return MainAppeal + SupportAppeal; } }

		public Idol Leader { get; set; }
		public Idol Guest { get; set; }
		public List<Idol> Members { get; set; }
		public HashSet<int> MemberIds { get; set; }
		public List<Idol> Supporters { get; set; }

		public Deck(Idol guest, Idol leader, List<Idol> members) {
			this.Guest = guest;
			this.Leader = leader;
			this.Members = members;
			this.MainAppeal = 0;

			MemberIds = new HashSet<int>(members.Select(i => i.Id));
			MemberIds.Add(leader.Id);

			MainAppeal += guest.Appeal;
			MainAppeal += leader.Appeal;
			members.ForEach(i => MainAppeal += i.Appeal);
		}

		public void SetSupporters(List<Idol> supporters) {
			this.Supporters = supporters;
			supporters.ForEach(i => SupportAppeal += i.Appeal);
		}
	}
}
