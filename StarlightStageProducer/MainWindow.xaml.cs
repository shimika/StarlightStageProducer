using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using StarlightStageProducer.Static;

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

			checkCute.Checked += CheckBox_ValueChanged;
			checkCute.Unchecked += CheckBox_ValueChanged;
			checkCool.Checked += CheckBox_ValueChanged;
			checkCool.Unchecked += CheckBox_ValueChanged;
			checkPassion.Checked += CheckBox_ValueChanged;
			checkPassion.Unchecked += CheckBox_ValueChanged;

			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += VersionSync;
			bw.RunWorkerCompleted += VersionSyncComplete;
			bw.RunWorkerAsync();

			refresh("");
			calculate();
		}

		private void ComboBurst_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (this.IsLoaded) {
				Data.SetBurstMode(comboBurst.SelectedIndex);
				calculate();
			}
		}

		Dictionary<int, IdolView> DictView = new Dictionary<int, IdolView>();

		private void refresh(string text) {
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
			refreshFilter(text);
		}

		private void refreshFilter(string text) {
			bool cute = checkCute.IsChecked.Value;
			bool cool = checkCool.IsChecked.Value;
			bool passion = checkPassion.IsChecked.Value;

			string filter = Parser.DivideKorean(text);

			int count = 0;
			for (int i = 0; i < gridContent.Children.Count; i++) {
				IdolView view = (IdolView)gridContent.Children[i];
				bool isShow = false;

				switch (view.getIdolType()) {
					case Type.Cute:
						isShow = view.getVisibility(cute, filter);
						break;

					case Type.Cool:
						isShow = view.getVisibility(cool, filter);
						break;

					case Type.Passion:
						isShow = view.getVisibility(passion, filter);
						break;
				}

				if (!isShow) {
					view.Margin = new Thickness(0, 0, 0, 0);
					view.Visibility = Visibility.Hidden;
				}
				else {
					view.Visibility = Visibility.Visible;
					view.Margin = new Thickness(70 * (count % 8), 70 * (count / 8), 0, 0);
					count++;
				}
			}
		}

		private void CheckBox_ValueChanged(object sender, RoutedEventArgs e) {
			if (this.IsLoaded) {
				refreshFilter(textboxFilter.Text);
			}
		}
		private void textboxFilter_TextChanged(object sender, TextChangedEventArgs e) {
			if (this.IsLoaded) {
				refreshFilter(textboxFilter.Text);
			}
		}

		private void CheckChanged(object sender, CheckEventArgs e) {
			DictView[e.Id].setSelection(e.Count);

			Data.ApplyCount(e.Id, e.Count);
			refreshCount();

			FileSystem.SaveCheck(Data.CountMap);
		}

		private void refreshCount() {
			textSelectCount.Text = Info.GetCheckCount();
		}
		private void Network_Loading(object sender, LoadingEventArgs e) {
			Dispatcher.BeginInvoke(new Action(() => {
				textLoading.Text = e.Status;
			}));
		}
		private void buttonDownload_Response(object sender, CustomButtonEventArgs e) {
			gridBlock.Opacity = 1;
			gridBlock.Visibility = Visibility.Visible;
			network.Download();
		}

		private void Network_Completed(object sender, DataEventArgs e) {
			gridBlock.Visibility = Visibility.Collapsed;

			if (e.Idols == null) { return; }
			
			Data.Idols = e.Idols;
			FileSystem.SaveDatabase(e.Idols);

			refresh(textboxFilter.Text);
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
			selectScore.SetIndex(Data.SkillCount[1]);
			selectCombo.SetIndex(Data.SkillCount[2]);
			selectPerfectSupport.SetIndex(Data.SkillCount[3]);
			selectComboSupport.SetIndex(Data.SkillCount[4]);
			selectHeal.SetIndex(Data.SkillCount[5]);
			selectGuard.SetIndex(Data.SkillCount[6]);
			selectOverload.SetIndex(Data.SkillCount[7]);
			selectNone.SetIndex(Data.SkillCount[0]);

			textOptionError.Visibility = Visibility.Collapsed;
			gridOption.Visibility = Visibility.Visible;
		}

		private void calculate() {
			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += Bw_DoWork;
			bw.RunWorkerCompleted += Bw_RunWorkerCompleted;

			Storyboard sb = new Storyboard();
			sb.Children.Add(Animation.GetDoubleAnimation(0, gridBlock, 0, 0));
			sb.Children.Add(Animation.GetDoubleAnimation(1, gridBlock, 200, 500));
			sb.Begin(this);

			gridBlock.Visibility = Visibility.Visible;
			textLoading.Text = "Calculating...";

			Console.WriteLine((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);
			Data.CacheList.Clear();

			deckAll.reset();
			deckCute.reset();
			deckCool.reset();
			deckPassion.reset();

			bw.RunWorkerAsync();
		}

		private void Bw_DoWork(object sender, DoWorkEventArgs e) {
			Dictionary<Type, Deck> decks = new Dictionary<Type, Deck>();

			if (Data.Idols.Count == 0) {
				e.Cancel = true;
				return;
			}

			foreach (Type musicType in Enum.GetValues(typeof(Type))) {
				Deck best = Data.CalculateBest(Data.BurstMode, musicType);
				decks.Add(musicType, best);
			}

			e.Result = decks;

			Console.WriteLine((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);
		}

		private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			gridBlock.Visibility = Visibility.Collapsed;
			if (e.Cancelled) { return; }

			Dictionary<Type, Deck> decks = (Dictionary<Type, Deck>)e.Result;

			foreach (KeyValuePair<Type, Deck> kvp in decks) {
				switch (kvp.Key) {
					case Type.All:
						deckAll.refresh(kvp.Value);
						break;
					case Type.Cute:
						deckCute.refresh(kvp.Value);
						break;
					case Type.Cool:
						deckCool.refresh(kvp.Value);
						break;
					case Type.Passion:
						deckPassion.refresh(kvp.Value);
						break;
				}
			}
		}

		private void buttonSaveOption_Response(object sender, CustomButtonEventArgs e) {
			int sum = selectScore.SelectedIndex + selectCombo.SelectedIndex +
				selectHeal.SelectedIndex + selectGuard.SelectedIndex +
				selectPerfectSupport.SelectedIndex + selectComboSupport.SelectedIndex +
				selectOverload.SelectedIndex + selectNone.SelectedIndex;

			if (sum > 5) {
				textOptionError.Visibility = Visibility.Visible;
				return;
			}

            Data.SkillCount[0] = selectNone.SelectedIndex;
            Data.SkillCount[1] = selectScore.SelectedIndex;
			Data.SkillCount[2] = selectCombo.SelectedIndex;
			Data.SkillCount[3] = selectPerfectSupport.SelectedIndex;
			Data.SkillCount[4] = selectComboSupport.SelectedIndex;
			Data.SkillCount[5] = selectHeal.SelectedIndex;
			Data.SkillCount[6] = selectGuard.SelectedIndex;
			Data.SkillCount[7] = selectOverload.SelectedIndex;

			gridOption.Visibility = Visibility.Collapsed;
			calculate();
		}

		private void IgnoreSkill_Changed(object sender, RoutedEventArgs e) {
			if (this.IsLoaded) {
				Data.CheckSkill = (checkIgnoreSkill.IsChecked == false);
				calculate();
			}
		}

		string LastestUrl = "https://github.com/shimika/StarlightStageProducer/releases/latest";

		private void buttonVersionSync_Response(object sender, CustomButtonEventArgs e) {
			System.Diagnostics.Process.Start(LastestUrl);
		}

		private void VersionSync(object sender, DoWorkEventArgs e) {
			string html = Network.GET(LastestUrl);
			e.Result = Parser.GetLastestVersion(html);
		}

		private void VersionSyncComplete(object sender, RunWorkerCompletedEventArgs e) {
			if (e.Result != null) {
				if (!e.Result.ToString().Equals(Version.version)) {
					buttonVersionSync.ViewMode = ImageButton.Mode.Visible;
					buttonVersionSync.ToolTip = string.Format("{0} released", e.Result);
					buttonVersionSync.StartAnimateImage();
				}
			}
		}
	}
}
