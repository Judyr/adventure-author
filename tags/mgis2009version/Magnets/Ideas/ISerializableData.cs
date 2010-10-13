/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 15/02/2008
 * Time: 14:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Intended for objects which represent serializable data from XAML controls which cannot be easily serialized.
	/// The GetControl() method returns a new instance of a XAML control constructed from this information.
	/// to return a serializable object containing the essential data about that control.
	/// </summary>
	public interface ISerializableData
	{
		/// <summary>
		/// Get a control based on the data in this object.
		/// </summary>
		/// <returns>A control based on this information</returns>
		UnserializableControl GetControl();
	}
}
