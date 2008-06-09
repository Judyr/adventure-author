/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 05/04/2008
 * Time: 22:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of MagnetEditedEventArgs.
	/// </summary>
	public class MagnetEditedEventArgs : MagnetEventArgs
	{
		private string originalText;
		public string OriginalText {
			get { return originalText; }
		}
		
		
		private IdeaCategory originalCategory;
		public IdeaCategory OriginalCategory {
			get { return originalCategory; }
		}
		
		
		public MagnetEditedEventArgs(MagnetControl magnet, string originalText, IdeaCategory originalCategory) : base(magnet)
		{
			this.originalCategory = originalCategory;
			this.originalText = originalText;
		}
	}
}
