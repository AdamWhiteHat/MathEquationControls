using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MathEquationControls
{
	public static class WPFHelper
	{
		public static Rect GetClientRectangle(FrameworkElement element)
		{
			Point topLeft = element.PointToScreen(element.TransformToVisual(element).Transform(new Point(0, 0)));
			return new Rect(topLeft.X, topLeft.Y, element.ActualWidth, element.ActualHeight);
		}

		public static bool IsPointInRect(Point point, Rect rect)
		{
			if (point.X >= rect.Left && point.X <= rect.Right)
			{
				if (point.Y >= rect.Top && point.Y <= rect.Bottom)
				{
					return true;
				}
			}
			return false;
		}

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

		public static Size MeasureString(string candidate, Visual visual, Control textBox)
		{
			DpiScale dpiScale = VisualTreeHelper.GetDpi(visual);

			var formattedText = new FormattedText(
				candidate,			
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch),
				textBox.FontSize,
				Brushes.Black,
				new NumberSubstitution(),
				TextFormattingMode.Display,
				dpiScale.PixelsPerDip);

			formattedText.TextAlignment = TextAlignment.Center;
			formattedText.Trimming = TextTrimming.None;

			var textBlock = new TextBlock
			{
				Text = candidate,
				FontFamily = textBox.FontFamily,
				FontSize = textBox.FontSize,
				FontStyle = textBox.FontStyle,
				FontWeight = textBox.FontWeight,
				FontStretch = textBox.FontStretch,
				TextWrapping = TextWrapping.NoWrap,
				TextAlignment = TextAlignment.Center,
				TextTrimming = TextTrimming.None,
			};

			// auto sized
			textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
			textBlock.Arrange(new Rect(textBlock.DesiredSize));

			return new Size(formattedText.Width + textBox.Padding.Left + textBox.Padding.Right, formattedText.Height);
		}

		public static string ExtractPlainTextContent(RichTextBox rtb)
		{
			TextRange textRange = new TextRange(
				// TextPointer to the start of content in the RichTextBox.
				rtb.Document.ContentStart,
				// TextPointer to the end of content in the RichTextBox.
				rtb.Document.ContentEnd
			);

			// The Text property on a TextRange object returns a string
			// representing the plain text content of the TextRange.
			return textRange.Text;
		}
	}
}
