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
			this.category = category;
			Brush categoryBrush = MagnetControl.GetBrushForCategory(category);	
			Brush foregroundBrush = Brushes.White;
			
			try {
				TextBlock textBlock = new TextBlock();
				TextBlock tb0 = new TextBlock();
				TextBlock tb1 = new TextBlock();
				tb0.FontSize = 12;
				tb1.FontSize = 12;
				tb0.Foreground = foregroundBrush;
				tb1.Foreground = categoryBrush;
				tb0.Text = "Show ";
				tb1.Text = category.ToString();				
				textBlock.Inlines.Add(tb0);
				textBlock.Inlines.Add(tb1);				
				Content = textBlock;
			}
			catch (Exception e) {
				Say.Debug("Failed to give the category checkbox item a pretty colour.\n"+e);
				Content = "Show " + category.ToString() + " ideas";
			}	
		}
	}
}
