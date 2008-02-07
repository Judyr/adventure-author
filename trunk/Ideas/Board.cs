/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 04/02/2008
 * Time: 10:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Controls;
using System.Collections.Generic;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of IBoard.
	/// </summary>
	public abstract class Board : UserControl
	{
		protected abstract List<BoardObject> BoardObjects {
			get;
		}
	}
}
