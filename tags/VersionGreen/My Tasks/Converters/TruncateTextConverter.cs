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
	public class TruncateTextConverter : IValueConverter
	{
		public TruncateTextConverter()
		{
		}
		
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(string)) {
				throw new ArgumentException("targetType must be string");
			}
			
			string text = (string)value;
			Int32 maxCharacters = (Int32)parameter;
			
			if (text.Length > 0 && text.Length > maxCharacters) {
				string truncatedText = text.Substring(0,maxCharacters-1);
				int lastSpace = truncatedText.LastIndexOf(' ');
				int cutOff;
				if (lastSpace == -1) {
					cutOff = truncatedText.Length-1;
				}
				else {
					cutOff = lastSpace;
				}
				return truncatedText.Substring(0,cutOff) + "...";
			}
			else {
				return text;
			}
		}
		
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
