/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 26/07/2008
 * Time: 21:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using AdventureAuthor.Tasks;
using System.ComponentModel;

namespace AdventureAuthor.Tasks
{	
	[TypeConverter(typeof(CornerConverter))]
	public enum Corner
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}
}