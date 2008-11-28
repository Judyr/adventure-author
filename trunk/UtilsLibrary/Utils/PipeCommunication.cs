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

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Deals with communication across inter-process named pipes
	/// and generates events when a message is received.
	/// </summary>
	public static class PipeCommunication
	{		
		#region Constants
		
		public const string MYTASKSPIPE = "My Tasks to NWN2 pipe";
		public const string FRIDGEMAGNETSPIPE = "Fridge Magnets to NWN2 pipe";
				
		#endregion
		
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
    	/// Start a thread to listen for messages on a particular pipe.
    	/// </summary>
    	/// <param name="pipename">The name of the pipe to listen to.</param>
    	public static void ThreadedListen(string pipename)
    	{   
    		ParameterizedThreadStart start = new ParameterizedThreadStart(Listen);
    		Thread thread = new Thread(start);
    		thread.Name = "Listen to pipe '" + pipename + "'";
    		thread.IsBackground = true; // will not prevent the application from closing down
			thread.Priority = ThreadPriority.Normal;
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
		public static void Listen(string pipename)
    	{
    		using (NamedPipeClientStream client = new NamedPipeClientStream(".",
			                                                                pipename,
			                                                                PipeDirection.InOut))
    		{
    			try {
    				System.Diagnostics.Debug.WriteLine("Try to connect to server.");
    				client.Connect();    				
    				System.Diagnostics.Debug.WriteLine("Connected.");
    				
	    			using (StreamReader reader = new StreamReader(client))
	    			{	    				
	    				string message;
	    				
    					System.Diagnostics.Debug.WriteLine("Start listening for messages.");
	    				while ((message = reader.ReadToEnd()) != String.Empty) {
	    					if (message.Length > 0) {	    	
	    						OnMessageReceived(new MessageReceivedEventArgs(message,pipename,DateTime.Now));
	    					}
	    				}
    					System.Diagnostics.Debug.WriteLine("Stopped listening for messages.");
	    			}
	    		}
    			catch (Exception e) {
    				Say.Error("Failed to connect to pipe '" + pipename + "'.",e);
    			}    			
			} 
    		
			Listen(pipename);
    	}
		   		
    	    	
		/// <summary>
		/// Start a thread to send a message or object across a particular pipe.
		/// </summary>
		/// <param name="pipename">The name of the pipe to send across.</param>
		/// <param name="message">The message or object to send.</param>
		public static void ThreadedSendMessage(string pipename, object message)
		{			
			ParameterizedThreadStart start = new ParameterizedThreadStart(SendMessage);
			Thread thread = new Thread(start);
    		thread.Name = "Send message/object across pipe '" + pipename + "'";
			thread.IsBackground = true;
			thread.Priority = ThreadPriority.Normal;
			thread.Start(new object[]{pipename,message});
		}
		
		
		/// <summary>
		/// Send a message or object across a particular pipe.
		/// </summary>
		/// <param name="parameters">An object[] containing the name of the pipe
		/// to send across and the message to send. Use 'SendMessage(string pipename,
		/// object message)' instead.</param>
		private static void SendMessage(object parameters)
		{
			try {
				object[] parameterArray = (object[])parameters;
				string pipename = (string)parameterArray[0];
				object message = parameterArray[1];
				SendMessage(pipename,message);
			}
			catch (IndexOutOfRangeException) {
				throw new ArgumentException("parameters must be an object[] containing a string pipename " +
				                            "and an object message in that order.","parameters");
			}
			catch (InvalidCastException) {
				throw new ArgumentException("parameters must be an object[] containing a string pipename " +
				                            "and an object message in that order.","parameters");
			}
		}
		
		
		/// <summary>
		/// Start a thread to send a message or object across a particular pipe.
		/// </summary>
		/// <param name="pipename">The name of the pipe to send across.</param>
		/// <param name="message">The message or object to send.</param>
    	public static void SendMessage(string pipename, object message)
    	{    		    		
    		try {	    			 
		    	using (NamedPipeServerStream server = new NamedPipeServerStream(pipename,
					    			                                            PipeDirection.InOut,
					    			                                            1))
		    	{	
					System.Diagnostics.Debug.WriteLine("Waiting for connection.");
		    		server.WaitForConnection();		    		
					System.Diagnostics.Debug.WriteLine("Connected.");
		    		using (StreamWriter writer = new StreamWriter(server))
		    		{		    			
		    			writer.WriteLine(message);
		    			writer.Flush();
		    		}
		    	}
    		}
    		catch (IOException e) {
    			System.Diagnostics.Debug.WriteLine("Tried to send '" + message + "' across pipe '" + pipename + 
    			                                   "', but the pipe was busy: " + e);
    		}
    	}
    	
		#endregion
	}
}
