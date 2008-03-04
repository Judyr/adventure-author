/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 25/02/2008
 * Time: 14:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of ShowHideCategoryCheckBox.
	/// </summary>
	public class ShowHideCategoryCheckBox : CheckBox
	{
		private IdeaCategory category;		
		public IdeaCategory Category {
			get { return category; }
			set { category = value; }
		}
		
		
		public ShowHideCategoryCheckBox(IdeaCategory category) : base()
		{
			Brush background = Brushes.Black;
			Brush foreground = Brushes.White;
			
			this.category = category;
			try {
				TextBlock textBlock = new TextBlock();
				TextBlock tb0 = new TextBlock();
				TextBlock tb1 = new TextBlock();
				TextBlock tb2 = new TextBlock();
				tb0.FontSize = 12;
				tb1.FontSize = 12;
				tb2.FontSize = 12;
				tb0.Foreground = foreground;
				tb1.Foreground = MagnetControl.GetColourForCategory(category);
				tb2.Foreground = foreground;
				tb0.Background = background;
				tb1.Background = background;
				tb2.Background = background;
				tb0.Text = "Show ";
				tb1.Text = category.ToString();
				tb2.Text = " ideas";
				
				textBlock.Inlines.Add(tb0);
				textBlock.Inlines.Add(tb1);
				textBlock.Inlines.Add(tb2);
				textBlock.Background = background;
				
				this.Margin = new Thickness(5);
				
				Content = textBlock;
			}
			catch (Exception e) {
				Say.Error("Failed to give the category checkbox item a pretty colour.",e);
				Content = "Show " + category.ToString() + " ideas";
			}	
		}
	}
}
