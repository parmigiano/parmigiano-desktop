using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Parmigiano.Utilities
{
    public class MouseUtilities
    {
        /// <summary>
        /// Медленный скрол
        /// </summary>
        public void PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = FindScrollViewer(sender as DependencyObject);
            if (scrollViewer != null)
            {
                double scrollAmount = 1;
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - Math.Sign(e.Delta) * scrollAmount);
                e.Handled = true;
            }
        }

        private ScrollViewer FindScrollViewer(DependencyObject d)
        {
            if (d is ScrollViewer viewer) return viewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);
                var result = FindScrollViewer(child);
                if (result != null) return result;
            }

            return null;
        }
    }
}
