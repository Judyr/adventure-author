/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 07/03/2008
 * Time: 18:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// The status of the evidence in an evidence control - either there is a valid link to
	/// a piece of evidence, a broken link to a piece of evidence, or no link at all.
	/// </summary>
    public enum EvidenceControlStatus {
    	NoLink,
    	Link,
    	BrokenLink
    }
}
