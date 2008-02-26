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
			this.category = category;
		}
	}
}
