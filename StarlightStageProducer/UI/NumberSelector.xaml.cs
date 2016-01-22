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
	/// NumberSelector.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class NumberSelector : UserControl {
		public NumberSelector() {
			InitializeComponent();
		}

		public int SelectedIndex { get; internal set; }

		#region Title
		public string Title {
			get { return (string)GetValue(TitleProperty); }
			set {
				SetValue(TitleProperty, value);
			}
		}
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
			"Title",
			typeof(string),
			typeof(NumberSelector),
			new FrameworkPropertyMetadata(
				"",
				FrameworkPropertyMetadataOptions.AffectsRender,
				TitlePropertyChanged));

		private static void TitlePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			(obj as NumberSelector).textTitle.Text = (string)e.NewValue;
		}
		#endregion

		private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e) {
			SetIndex(Convert.ToInt32((sender as Ellipse).Tag));
		}

		public void SetIndex(int index) {
			SelectedIndex = Convert.ToInt32(index);
			textTitle.Text = string.Format("{0} ({1})", Title, SelectedIndex);
			selector.Margin = new Thickness(50 * SelectedIndex, 0, 0, 0);
		}
	}
}
