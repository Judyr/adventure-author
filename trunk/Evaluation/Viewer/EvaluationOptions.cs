/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 16/01/2008
 * Time: 17:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Evaluation.Viewer
{
	/// <summary>
	/// Description of EvaluationOptions.
	/// </summary>
	public static class EvaluationOptions
	{		
		#region Events
		
		public static event EventHandler ChangedDefaultImageApplication;
		
		private static void OnChangedDefaultImageApplication(EventArgs e) 
		{
			EventHandler handler = ChangedDefaultImageApplication;
			if (handler != null) {
				handler(null,e);
			}
		}
		
		#endregion 
		
		
		public enum ImageApps {
			MicrosoftPaint,
			Default
		}
		
		private static ImageApps applicationToOpenImages = ImageApps.MicrosoftPaint;		
		public static ImageApps ApplicationToOpenImages {
			get { return applicationToOpenImages; }
			set { 
				applicationToOpenImages = value; 
				OnChangedDefaultImageApplication(new EventArgs());
			}
		}
	}
}
