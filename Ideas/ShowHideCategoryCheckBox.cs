/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 25/02/2008
 * Time: 14:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Controls;

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
		
		
		public ShowHideCategoryCheckBox(IdeaCategory category)
		{
			this.category = category;
		}
	}
}
