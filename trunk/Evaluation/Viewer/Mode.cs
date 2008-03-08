/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 11/02/2008
 * Time: 19:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Evaluation.Viewer
{
	/// <summary>
	/// The mode to run the evaluation application in, allowing users to either 
	/// build worksheets, fill them in or discuss completed worksheets.
	/// </summary>
	public enum Mode
	{		
    	Design, // design and build a worksheet from scratch, or edit an existing worksheet
    	Complete, // fill in the answer fields in an existing worksheet
    	Discuss // add comments on a worksheet which has been filled in
	}
}
