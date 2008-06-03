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
    	
    	// TODO separate all filters into TXT and TXT_ALL, where TXT_ALL is TXT + "|" + ALL
    	// TODO check every use of a filter to set it to the correct one (TXT versus TXT_ALL)
    	    	
    	public const string TXT = "Text files (*.txt)|*.txt";    	
    	public const string XML = "XML files (*.xml)|*.xml";    	
    	public const string PICTURES = "Pictures (*.jpg;*.jpeg;*.bmp;*.png;*.gif)|*.jpg;*.jpeg;*.bmp;*.png;*.gif";    	
    	public const string WORKSHEETS = "Worksheet files (*.worksheet)|*.worksheet";    	
    	public const string COMPILEDSCRIPTS = "Compiled scripts (*.ncs)|*.ncs";    	
    	public const string ALL = "All files (*.*)|*.*";    	    	
    	
    	public const string TXT_ALL = TXT + "|" + ALL;  
    	public const string XML_ALL = XML + "|" + ALL;  
    	public const string PICTURES_ALL = PICTURES + "|" + ALL;  
    	public const string WORKSHEETS_ALL = WORKSHEETS + "|" + ALL;  
    	public const string COMPILEDSCRIPTS_ALL = COMPILEDSCRIPTS + "|" + ALL; 
    	
    	#endregion
	}
}
