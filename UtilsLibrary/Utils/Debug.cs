﻿/*
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
using System.IO;
using System.Text;

namespace AdventureAuthor.Utils
{
	public static class DebugLog
	{	
		public static event EventHandler<DebugLogEventArgs> Message;
		
		private static void OnMessage(DebugLogEventArgs e)
		{
			EventHandler<DebugLogEventArgs> handler = Message;
			if (handler != null) {
				handler(null,e);
			}
		}
		
			
		public static void Write(string message)
		{
			OnMessage(new DebugLogEventArgs(message,null));
		}
		
			
		public static void Write(Exception e)
		{
			OnMessage(new DebugLogEventArgs(null,e));
		}				
	}
}
