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
using System.Threading;
using System.IO;
using System.IO.Pipes;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// Listens for communication across inter-process named pipes
	/// and generates events when a message is received.
	/// </summary>
	public static class PipeCommunication
	{		
		#region Events
		
		/// <summary>
		/// Raised when a message is received across a pipe.
		/// </summary>
		public static event EventHandler<MessageReceivedEventArgs> MessageReceived;
		private static void OnMessageReceived(MessageReceivedEventArgs e)
		{
			EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
			if (handler != null) {
				handler(null,e);
			}
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Start a separate thread to listen for messages on all 
		/// Adventure Author-related named pipes.
		/// </summary>
		public static void StartListeningOnAllPipes()
		{
			
		}
		
		
    	/// <summary>
    	/// Start a thread to listen for messages on a particular pipe.
    	/// </summary>
    	/// <param name="pipename">The name of the pipe to listen to.</param>
    	public static void StartListening(string pipename)
    	{   
    		ParameterizedThreadStart start = new ParameterizedThreadStart(Listen);
    		Thread thread = new Thread(start);
    		thread.Name = "Listen to pipe '" + pipename + "'";
    		thread.IsBackground = true; // will not prevent the application from closing down
			thread.Priority = ThreadPriority.BelowNormal;
			thread.Start(pipename);
    	}
    	
    	
    	/// <summary>
    	/// Start listening for messages on a particular pipe.
    	/// </summary>
    	/// <param name="pipename">The name of the pipe to listen to.</param>
    	private static void Listen(object pipename)
    	{
    		if (!(pipename is string)) {
    			throw new ArgumentException("pipename must be a string.","pipename");
    		}
    		Listen((string)pipename);
    	}
    	
		
    	/// <summary>
    	/// Start listening for messages on a particular pipe.
    	/// </summary>
    	/// <param name="pipename">The name of the pipe to listen to.</param>
		private static void Listen(string pipename)
    	{
    		using (NamedPipeClientStream client = new NamedPipeClientStream(".",
			                                                                pipename,
			                                                                PipeDirection.In))
    		{
    			try {
    				client.Connect();
    				
	    			using (StreamReader reader = new StreamReader(client))
	    			{
	    				string message;
	    				while ((message = reader.ReadToEnd()) != null) {
	    					if (message.Length > 0) {	    	
	    						OnMessageReceived(new MessageReceivedEventArgs(message,pipename,DateTime.Now));
	    					}
	    				}
	    			}
	    		}
    			catch (Exception e) {
    				Say.Error("Failed to connect to pipe '" + pipename + "'.",e);
    			}    			
			}    		
			Listen(pipename);
    	}
		
		#endregion
	}
}
