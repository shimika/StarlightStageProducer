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
	/// IdolDeckView.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class IdolDeckView : UserControl {
		public IdolDeckView() {
			InitializeComponent();
		}

		public void Hide() {
			gridContent.Visibility = Visibility.Collapsed;
		}

		public void SetIdol(Idol idol, bool showSkill = true) {
			gridContent.Visibility = Visibility.Visible;
			ToolTip = Data.GetInfo(idol.Id);
			//ToolTip = Data.GetInfo(idol);

			try { image.Source = new BitmapImage(new Uri(FileSystem.GetImagePath(idol.Id))); }
			catch { }

			gridSkill.Visibility = Visibility.Visible;
			circle.Fill = FindResource(string.Format("{0}Brush", idol.Skill)) as SolidColorBrush;

			string imageName = "";
			switch (idol.Skill) {
				case Skill.Score:
				case Skill.Combo:
					imageName = "bonus.png";
					break;
				case Skill.PerfectSupport:
				case Skill.ComboSupport:
					imageName = "support.png";
					break;
				case Skill.Heal:
					imageName = "heal.png";
					break;
				case Skill.Guard:
					imageName = "guard.png";
					break;
				default:
					gridSkill.Visibility = Visibility.Collapsed;
					return;
			}

			if (!showSkill) {
				gridSkill.Visibility = Visibility.Collapsed;
			}

			string uri = string.Format("pack://application:,,,/StarlightStageProducer;component/Resources/{0}", imageName);
			skillImage.Source = new BitmapImage(new Uri(uri));
		}
	}
}
