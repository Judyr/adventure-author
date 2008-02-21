/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 21/02/2008
 * Time: 17:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Arguments to accompany events relating to magnet controls being removed or deleted.
	/// </summary>
	public class MagnetRemovedEventArgs : MagnetEventArgs
	{
		/// <summary>
		/// True if it is appropriate for this magnet to be transferred somewhere else in the interface
		/// (usually the magnet board); false if it should be removed completely
		/// </summary>
		private bool transfer;		
		public bool Transfer {
			get { return transfer; }
		}
		
		
		/// <summary>
		/// Create a new MagnetRemovedEventArgs.
		/// </summary>
		/// <param name="magnet">The magnet being removed</param>
		/// <param name="transfer">True if it is appropriate for this magnet to be transferred somewhere else in the interface
		/// (usually the magnet board); false if it should be removed completely</param>
		public MagnetRemovedEventArgs(MagnetControl magnet, bool transfer) : base(magnet)
		{
			this.transfer = transfer;
		}
	}
}
