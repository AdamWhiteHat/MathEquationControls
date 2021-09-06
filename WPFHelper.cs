using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MathEquationControl
{
	public static class WPFHelper
	{
		public static T GetParentOfType<T>(DependencyObject element) where T : DependencyObject
		{
			return GetParents(element).OfType<T>().FirstOrDefault();
		}

		public static IEnumerable<T> ChildrenOfType<T>(DependencyObject element) where T : DependencyObject
		{
			return GetChildren(element).OfType<T>();
		}


		public static IEnumerable<DependencyObject> GetParents(DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			DependencyObject parent = GetParent(element);
			while (parent != null)
			{
				yield return parent;
				parent = GetParent(parent);
			}

			yield break;
		}

		public static DependencyObject GetParent(DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			DependencyObject result = null;
			try
			{
				result = VisualTreeHelper.GetParent(element);
			}
			catch (InvalidOperationException)
			{
				result = null;
			}

			if (result == null)
			{
				if (element is FrameworkContentElement)
				{
					result = ((FrameworkContentElement)element).Parent;
				}
				else if (element is FrameworkElement)
				{
					FrameworkElement frameworkElement = element as FrameworkElement;

					if (frameworkElement.Parent != null)
					{
						result = frameworkElement.Parent;
					}
					else
					{
						result = frameworkElement.TemplatedParent;
					}
				}
			}

			return result;
		}

		public static IEnumerable<DependencyObject> GetChildren(DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			int maxIndex = VisualTreeHelper.GetChildrenCount(element);
			for (int i = 0; i < maxIndex; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(element, i);
				if (child != null)
				{
					yield return child;
					foreach (DependencyObject item in GetChildren(child))
					{
						yield return item;
					}
				}
			}
		}

	}
}
