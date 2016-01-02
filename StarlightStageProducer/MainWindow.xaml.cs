using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StarlightStageProducer {
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();

			//comboScoreCount.SelectedIndex = 2;
			comboBurst.SelectedIndex = 0;
		}

		Network network;

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			Data.Idols = FileSystem.GetIdols();
			Data.CountMap = FileSystem.GetCheck();

			network = new Network();
			network.Completed += Network_Completed;
			network.Loading += Network_Loading;

			refresh();

			comboBurst.SelectionChanged += ComboBurst_SelectionChanged;
			calculate();
		}
		
		private void ComboBurst_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			Data.SetBurstMode(comboBurst.SelectedIndex);
			calculate();
		}

		Dictionary<int, IdolView> DictView = new Dictionary<int, IdolView>();

		private void refresh() {
			gridContent.Children.Clear();
			DictView.Clear();

			for (int i = 0; i < Data.Idols.Count; i++) {
				Idol idol = Data.Idols[i];

				IdolView view = new IdolView(idol, Data.GetCount(idol.Id));
				view.Margin = new Thickness(70 * (i % 8), 70 * (i / 8), 0, 0);
				view.CheckChanged += CheckChanged;

				gridContent.Children.Add(view);
				DictView.Add(idol.Id, view);
			}
			
			refreshCount();
		}

		private void CheckChanged(object sender, CheckEventArgs e) {
			DictView[e.Id].setSelection(e.Count);

			Data.ApplyCount(e.Id, e.Count);
			refreshCount();

			FileSystem.SaveCheck(Data.CountMap);
		}

		private void refreshCount() {
			textSelectCount.Text = Data.GetCheckCount();
		}
		private void Network_Loading(object sender, LoadingEventArgs e) {
			Dispatcher.BeginInvoke(new Action(() => {
				textLoading.Text = e.Status;
			}));
		}
		private void buttonDownload_Response(object sender, CustomButtonEventArgs e) {
			gridBlock.Visibility = Visibility.Visible;
			network.Download();
		}
		private void Network_Completed(object sender, DataEventArgs e) {
			gridBlock.Visibility = Visibility.Collapsed;

			if (e.Idols == null) { return; }
			
			Data.Idols = e.Idols;
			FileSystem.SaveDatabase(e.Idols);

			refresh();
			calculate();
		}
		private void buttonAlbum_Click(object sender, CustomButtonEventArgs e) {
			gridAlbum.Visibility = Visibility.Visible;
		}
		private void buttonBack_Response(object sender, CustomButtonEventArgs e) {
			gridAlbum.Visibility = Visibility.Collapsed;
			calculate();
		}
		private void buttonBackOption_Response(object sender, CustomButtonEventArgs e) { gridOption.Visibility = Visibility.Collapsed; }
		private void buttonOption_Response(object sender, CustomButtonEventArgs e) {
			selectScore.SetIndex(Data.SkillCount[0]);
			selectCombo.SetIndex(Data.SkillCount[1]);
			selectPerfectSupport.SetIndex(Data.SkillCount[2]);
			selectComboSupport.SetIndex(Data.SkillCount[3]);
			selectHeal.SetIndex(Data.SkillCount[4]);
			selectGuard.SetIndex(Data.SkillCount[5]);
			selectNone.SetIndex(Data.SkillCount[6]);

			textOptionError.Visibility = Visibility.Collapsed;
			gridOption.Visibility = Visibility.Visible;
		}

		private void calculate() {
			deckAll.reset();
			deckCute.reset();
			deckCool.reset();
			deckPassion.reset();

			List<Idol> myIdols = Data.GetMyIdols();
			List<Idol> guests = Data.GetSSR();

			if (myIdols.Count == 0) { return; }

			foreach (Type musicType in Enum.GetValues(typeof(Type))) {
				Deck best = Data.CalculateBest(myIdols, guests, Data.BurstMode, musicType);

				switch (musicType) {
					case Type.All:
						deckAll.refresh(best);
						break;
					case Type.Cute:
						deckCute.refresh(best);
						break;
					case Type.Cool:
						deckCool.refresh(best);
						break;
					case Type.Passion:
						deckPassion.refresh(best);
						break;
				}
			}
		}

		private void buttonSaveOption_Response(object sender, CustomButtonEventArgs e) {
			int sum = selectScore.SelectedIndex + selectCombo.SelectedIndex +
				selectHeal.SelectedIndex + selectGuard.SelectedIndex +
				selectPerfectSupport.SelectedIndex + selectComboSupport.SelectedIndex + 
				selectNone.SelectedIndex;

			if (sum != 5) {
				textOptionError.Visibility = Visibility.Visible;
				return;
			}

			Data.SkillCount[0] = selectScore.SelectedIndex;
			Data.SkillCount[1] = selectCombo.SelectedIndex;
			Data.SkillCount[2] = selectPerfectSupport.SelectedIndex;
			Data.SkillCount[3] = selectComboSupport.SelectedIndex;
			Data.SkillCount[4] = selectHeal.SelectedIndex;
			Data.SkillCount[5] = selectGuard.SelectedIndex;
			Data.SkillCount[6] = selectNone.SelectedIndex;

			gridOption.Visibility = Visibility.Collapsed;
			calculate();
		}
	}
}
