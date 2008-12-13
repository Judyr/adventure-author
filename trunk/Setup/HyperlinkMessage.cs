/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2008.
 *
 *   This copyright and licence apply to all source code, compiled code,
 *   documentation, graphics and auxiliary files, except where otherwise stated.
 *
 *   Adventure Author is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   Adventure Author is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 * 
 *   Adventure Author is a plugin for Atari's Neverwinter Nights 2, a COMMERCIAL
 *   product. Permission is given to link this GPL-covered plug-in with the 
 *   non-free main program. 
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Windows;

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// Description of HyperlinkMessage.
	/// </summary>
	public class HyperlinkMessage
	{
		#region Properties and fields
		
		private string messageText;
		public string MessageText {
			get { return messageText; }
			set { messageText = value; }
		}
		
		
		private string hyperlinkText;
		public string HyperlinkText {
			get { return hyperlinkText; }
			set { hyperlinkText = value; }
		}
		
		
//		public string MessageText {
//			get { return (string)GetValue(MessageTextProperty); }
//			set { SetValue(MessageTextProperty,value); }
//		}
//		
//		
//		public string HyperlinkText {
//			get { return (string)GetValue(HyperlinkTextProperty); }
//			set { SetValue(HyperlinkTextProperty,value); }
//		}
//		
//		
		protected Delegate hyperlinkMethod;
		public Delegate HyperlinkMethod {
			get { return hyperlinkMethod; }
		}
//				
//		
//		public static readonly DependencyProperty MessageTextProperty
//			= DependencyProperty.Register("MessageText",
//			                              typeof(string),
//			                              typeof(HyperlinkMessage));
//		
//		public static readonly DependencyProperty HyperlinkTextProperty
//			= DependencyProperty.Register("HyperlinkText",
//			                              typeof(string),
//			                              typeof(HyperlinkMessage));
		
		#endregion
		
		#region Constructors		
		
		public HyperlinkMessage(string messageText, string hyperlinkText, Delegate hyperlinkMethod)
		{
			MessageText = messageText;
			HyperlinkText = hyperlinkText;
			this.hyperlinkMethod = hyperlinkMethod;
		}
		
		
		public HyperlinkMessage(string messageText) : this(messageText,null,null)
		{			
		}
		
		#endregion
	}
}
