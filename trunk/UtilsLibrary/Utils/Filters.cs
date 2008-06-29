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
	/// Various string constants representing file type filters.
	/// </summary>
	public static class Filters
	{    	    	    
    	public const string TXT = "Text files (*.txt)|*.txt";    	
    	public const string XML = "XML files (*.xml)|*.xml";    	
    	public const string PICTURES = "Pictures (*.jpg;*.jpeg;*.bmp;*.png;*.gif)|*.jpg;*.jpeg;*.bmp;*.png;*.gif";    	
    	public const string COMMENTCARDS = "Comment Cards (*.cmt)|*.cmt";    	  	
    	public const string MAGNETBOARDS = "Magnet board files (*.brd)|*.brd";    	  	
    	public const string MAGNETBOXES = "Magnet box files (*.box)|*.box";    	
    	public const string COMPILEDSCRIPTS = "Compiled scripts (*.ncs)|*.ncs";    	
    	public const string ALL = "All files (*.*)|*.*";    
    	
    	public const string TXT_ALL = TXT + "|" + ALL;
    	public const string XML_ALL = XML + "|" + ALL;
    	public const string PICTURES_ALL = PICTURES + "|" + ALL;
    	public const string COMMENTCARDS_ALL = COMMENTCARDS + "|" + ALL;
    	public const string MAGNETBOARDS_ALL = MAGNETBOARDS + "|" + ALL;
    	public const string MAGNETBOXES_ALL = MAGNETBOXES + "|" + ALL;
    	public const string COMPILEDSCRIPTS_ALL = COMPILEDSCRIPTS + "|" + ALL;
	}
}
