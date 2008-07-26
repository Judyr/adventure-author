/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 26/07/2008
 * Time: 22:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// Description of CornerConverter.
	/// </summary>
	public class CornerConverter : TypeConverter
	{
		public CornerConverter()
		{
		}
		
		
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string)) {
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		
		
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string) {
				string str = (string)value;
				return Enum.Parse(typeof(Corner),str);
			}
			return base.ConvertFrom(context, culture, value);
		}
		
		
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is Corner) {
				return ((Corner)value).ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
