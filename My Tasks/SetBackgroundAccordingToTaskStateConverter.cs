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
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// </summary>
	public class SetBackgroundAccordingToTaskStateConverter : IValueConverter
	{
		private static ImageBrush linedPaperBrush;
		
		
		public SetBackgroundAccordingToTaskStateConverter()
		{
			linedPaperBrush = new ImageBrush();
			ResourceManager manager = new ResourceManager("AdventureAuthor.Tasks.Images",Assembly.GetExecutingAssembly());
			
			System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)manager.GetObject("linedpaper");
			BitmapSource source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
																		 bitmap.GetHbitmap(),
			                                                             IntPtr.Zero,
			                                                             Int32Rect.Empty,
			                                                             BitmapSizeOptions.FromEmptyOptions());			
			linedPaperBrush.ImageSource = source;
			linedPaperBrush.TileMode = TileMode.Tile;
			linedPaperBrush.Stretch = Stretch.None;
			linedPaperBrush.AlignmentX = AlignmentX.Left;
			linedPaperBrush.Viewport = new Rect(0,0.5,1,0.04); //these values seem to work but don't really understand what they're doing
			linedPaperBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
		}
		
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Brush)) {
				throw new ArgumentException("targetType must be Brush");
			}
			
			return linedPaperBrush;
				
//			TaskState state = (TaskState)value;
//			if (state == TaskState.Completed) {
//				return Brushes.LightSteelBlue;
//			}
//			else if (state == TaskState.NotCompleted) {
//				return linedPaperBrush;
//			}
//			else {
//				return linedPaperBrush;
//			}
		}
		

		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
