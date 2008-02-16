/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 13/02/2008
 * Time: 17:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Description of Filters.
	/// </summary>
	public static class Filters
	{
    	#region Constants
    	    	
    	public const string TXT = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
    	public const string XML = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
    	public const string PICTURES = "Pictures (*.jpg;*.jpeg;*.bmp;*.png;*.gif)|*.jpg;*.jpeg;*.bmp;*.png;*.gif";
    	public const string ALL = "All files (*.*)|*.*";
    	public const string WORKSHEET_EVIDENCE = PICTURES + "|" + ALL;
    	
    	#endregion
	}
}
