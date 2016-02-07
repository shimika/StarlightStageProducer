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

		public IdolSummary Leader { get; set; }
		public IdolSummary Guest { get; set; }
		public List<IdolSummary> Members { get; set; }
		public HashSet<int> MemberIds { get; set; }
		public List<IdolSummary> Supporters { get; set; }

		public Deck(IdolSummary guest, IdolSummary leader, List<IdolSummary> members) {
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

		public void SetSupporters(List<IdolSummary> supporters) {
			this.Supporters = supporters;
			supporters.ForEach(i => SupportAppeal += i.Appeal);
		}

		public bool isBetter(Deck deck) {
			if (MainAppeal < deck.MainAppeal) {
				return true;
			}
			if (MainAppeal == deck.MainAppeal && Leader.Appeal < deck.Leader.Appeal) {
				return true;
			}
			return false;
		}
	}
}
