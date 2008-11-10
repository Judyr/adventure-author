/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 20/06/2008
 * Time: 20:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Timers;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// A timer that automatically counts down to 0, whenever it is given a non-zero
	/// period of time to count down from.
	/// </summary>
	public class CountdownTimer : Timer
	{
		#region Fields
		
		private object padlock = new object();
		
		
		/// <summary>
		/// The time remaining to count down in milliseconds. 
		/// </summary>
		/// <remarks>To start the timer, provide this property with a value greater than zero.</remarks>
		private int countdownTime;
		public int CountdownTime {
			get { 
				lock (padlock) {
					return countdownTime; 
				}
			}
			set { 
				lock (padlock) {					
					countdownTime = value;					
					if (!Enabled && countdownTime > 0) { // switching on the bulb
						Start();
						OnStartedCountdown(new EventArgs());
					}
				}
			}
		}
		
		#endregion
		
		#region Events
		
		public event EventHandler StartedCountdown;
		protected virtual void OnStartedCountdown(EventArgs e)
		{
			EventHandler handler = StartedCountdown;
			if (handler != null) {
				handler(this, e);
			}
		}
		
		
		public event EventHandler ReachedZero;
		protected virtual void OnReachedZero(EventArgs e)
		{
			EventHandler handler = ReachedZero;
			if (handler != null) {
				handler(this, e);
			}
		}
		
		#endregion
		
		#region Constructors
		
		public CountdownTimer() : this(0) {}
		
		
		public CountdownTimer(int time) : base()
		{
			AutoReset = true;
			Interval = 100; // check whether the illumination time has been updated every 0.1 seconds
			Elapsed += new ElapsedEventHandler(timerElapsed);
			CountdownTime = countdownTime;
		}

		
		#endregion
		
		#region Event handlers
		
		private void timerElapsed(object sender, ElapsedEventArgs e)
		{
			CountdownTime = Math.Max(0,CountdownTime - (int)Interval);
			
			if (CountdownTime == 0) {
				Stop();
				OnReachedZero(new EventArgs());
			}
		}
		
		#endregion
	}
}
