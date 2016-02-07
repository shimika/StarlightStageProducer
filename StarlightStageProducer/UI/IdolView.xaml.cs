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
using StarlightStageProducer.Static;

namespace StarlightStageProducer {
	/// <summary>
	/// IdolView.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class IdolView : UserControl {
		private Idol idol;
		private int count;
		public EventHandler<CheckEventArgs> CheckChanged;

		public IdolView(Idol idol, int count) {
			InitializeComponent();

			this.idol = idol;
			setSelection(count);

			string path = FileSystem.GetImagePath(idol.Id);
			try { image.Source = new BitmapImage(new Uri(path)); }
			catch { }

			circle.Fill = FindResource(idol.Type + "Brush") as SolidColorBrush;
			ToolTip = Info.GetInfo(idol.Id);
		}

		public Type getIdolType() {
			return idol.Type;
		}

		public bool getVisibility(bool type, string filter) {
			if (filter == "") {
				return type;
			}
			return type && idol.ParsedName.IndexOf(filter) >= 0;
		}

		public void setSelection(int count) {
			this.count = count;
			image.Opacity = count > 0 ? 1 : 0.4;
			gridSelection.Visibility = count > 0 ? Visibility.Visible : Visibility.Collapsed;

			if (count == 1) {
				imageCheck.Visibility = Visibility.Visible;
				textCount.Visibility = Visibility.Collapsed;
			}
			else {
				imageCheck.Visibility = Visibility.Collapsed;
				textCount.Visibility = Visibility.Visible;
				textCount.Text = count.ToString();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			if (count == 0) {
				count = 1;

				if (CheckChanged != null) {
					CheckChanged(this, new CheckEventArgs(this.idol.Id, count));
				}
				
				gridModify.Visibility = Visibility.Visible;
			}
		}

		private void Grid_MouseEnter(object sender, MouseEventArgs e) {
			if (count > 0) {
				gridModify.Visibility = Visibility.Visible;
			}
		}

		private void Grid_MouseLeave(object sender, MouseEventArgs e) {
			gridModify.Visibility = Visibility.Collapsed;
		}

		private void buttonUp_Click(object sender, RoutedEventArgs e) {
			this.count = Math.Min(this.count + 1, 11);
			if (CheckChanged != null) {
				CheckChanged(this, new CheckEventArgs(this.idol.Id, count));
			}
		}

		private void buttonDown_Click(object sender, RoutedEventArgs e) {
			this.count = Math.Max(this.count - 1, 0);
			if (CheckChanged != null) {
				CheckChanged(this, new CheckEventArgs(this.idol.Id, count));
			}
		}
	}

	public class CheckEventArgs : EventArgs {
		public int Id { get; internal set; }
		public int Count { get; internal set; }

		public CheckEventArgs(int id, int count) {
			this.Id = id;
			this.Count = count;
		}
	}
}
