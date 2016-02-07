using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace StarlightStageProducer {
	class Animation {
		public static DoubleAnimation GetDoubleAnimation(double opacity, FrameworkElement fe, double duration = 250, double delay = 0) {
			DoubleAnimation da = new DoubleAnimation(opacity, TimeSpan.FromMilliseconds(duration)) {
				BeginTime = TimeSpan.FromMilliseconds(delay),
			};
			Storyboard.SetTarget(da, fe);
			Storyboard.SetTargetProperty(da, new PropertyPath(UIElement.OpacityProperty));

			return da;
		}
	}
}
