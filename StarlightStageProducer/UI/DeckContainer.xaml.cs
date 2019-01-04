using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StarlightStageProducer {
	/// <summary>
	/// Deck.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class DeckContainer : UserControl {
		public DeckContainer() {
			InitializeComponent();
		}

		public void reset() {
			imageLeader.Hide();
			imageGuest.Hide();
			for (int i = 0; i < 4; i++) {
				(FindName(string.Format("image{0}", i)) as IdolDeckView).Hide();
			}
			textAppeal.Text = "";
			textAppeal.ToolTip = null;
		}

		public void refresh(Deck deck) {
			if (deck == null) {
				this.Visibility = Visibility.Collapsed;
				return;
			}
			this.Visibility = Visibility.Visible;

			try {
				reset();
				textAppeal.Text = string.Format("{0} + {1} = {2}", deck.MainAppeal, deck.SupportAppeal, deck.Appeal);

				imageLeader.SetIdol(deck.Leader.Id);
				if (deck.Guest.Id > 0) {
					imageGuest.SetIdol(deck.Guest.Id, false);
				}

				for (int i = 0; i < deck.Members.Count; i++) {
					IdolDeckView idolDeckView = (FindName(string.Format("image{0}", i)) as IdolDeckView);
					idolDeckView.SetIdol(deck.Members[i].Id);
				}

                textAppeal.ToolTip = string.Format("서포터 목록 : \n{0}", 
					string.Join("\n", deck.Supporters.Select(i => Data.GetIdol(i.Id).Name)));
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
	}
}
