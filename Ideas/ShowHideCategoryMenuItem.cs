/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 25/02/2008
 * Time: 14:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Controls;
using System.Windows.Media;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of ShowHideCategoryMenuItem.
	/// </summary>
	public class ShowHideCategoryMenuItem : MenuItem
	{
		private IdeaCategory category;		
		public IdeaCategory Category {
			get { return category; }
			set { category = value; }
		}
		
		
		public ShowHideCategoryMenuItem(IdeaCategory category)
		{
			IsCheckable = true;
			this.category = category;

//			try {
//				TextBlock textBlock = new TextBlock();
//				TextBlock tb0 = new TextBlock();
//				TextBlock tb1 = new TextBlock();
//				TextBlock tb2 = new TextBlock();
//				tb0.Foreground = Brushes.White;
//				tb1.Foreground = MagnetControl.GetColourForCategory(category);
//				tb2.Foreground = Brushes.White;
//				tb0.Background = Brushes.SlateGray;
//				tb1.Background = Brushes.SlateGray;
//				tb2.Background = Brushes.SlateGray;
//				tb0.Text = "Show ";
//				tb1.Text = category.ToString();
//				tb2.Text = " ideas";
//				
//				textBlock.Inlines.Add(tb0);
//				textBlock.Inlines.Add(tb1);
//				textBlock.Inlines.Add(tb2);
//				textBlock.Background = Brushes.SlateGray;
//				
//				this.Header = textBlock;
//			}
//			catch (Exception e) {
//				Say.Error("Failed to give the category menu item a pretty colour.",e);
				this.Header = "Show " + category.ToString() + " ideas";
//			}			
		}
	}
}
