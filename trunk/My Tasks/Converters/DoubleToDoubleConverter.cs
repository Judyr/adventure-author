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
using System.Windows.Data;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// When binding to a property of type double, using this converter allows 
	/// some other value of type double to be added to the bound value.
	/// </summary>
	/// <remarks>Mainly useful when you want to bind a control's height or width
	/// to the height or width of a parent control, but slightly reduce the value
	/// to allow the child control to sit neatly within the bounds of the parent.
	public class DoubleToDoubleConverter : IValueConverter
	{
		public DoubleToDoubleConverter()
		{
		}
		
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(double)) {
				throw new ArgumentException("targetType must be double");
			}
			
			double a = (double)value;
			double b = (double)parameter;
			return a + b;
		}
		
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(double)) {
				throw new ArgumentException("targetType must be double");
			}
			
			double a = (double)value;
			double b = (double)parameter;
			return a + b;
		}
	}
}
