/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 21/03/2008
 * Time: 09:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Ideas
{
	public class MagnetCategoryEventArgs : EventArgs
	{
		private IdeaCategory category;
		public IdeaCategory Category {
			get { return category; }
		}
		
		
		public MagnetCategoryEventArgs(IdeaCategory category)
		{
			this.category = category;
		}
	}
}
