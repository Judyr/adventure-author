/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 15/02/2008
 * Time: 14:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Controls;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Intended for XAML controls which cannot be easily serialized, 
	/// to return a serializable object containing the essential data about that control.
	/// </summary>
	public abstract class UnserializableControl : UserControl
	{
		/// <summary>
		/// Get a serializable object representing serializable data in an unserializable control.
		/// </summary>
		/// <returns>A serializable object</returns>
		public abstract ISerializableData GetSerializable();
	}
}
