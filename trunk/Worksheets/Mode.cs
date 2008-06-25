/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 11/02/2008
 * Time: 19:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// The mode to run the evaluation application in, allowing users to either 
	/// design their own Comment Cards, or fill in and discuss existing Comment Cards.
	/// </summary>
	public enum Mode
	{		
    	Design, // design and build a Comment Card from scratch, or edit an existing Comment Card
    	Complete, // fill in the answer fields in an existing Comment Card
    	Discuss // add replies to a Comment Card which has been filled in
	}
}
