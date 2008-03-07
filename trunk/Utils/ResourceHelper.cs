/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 05/03/2008
 * Time: 11:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using AdventureAuthor.Core;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Class to fetch Adventure Author resources in various formats, e.g. images, scripts.
	/// </summary>
	public static class ResourceHelper
	{
		/// <summary>
		/// Fetch an image (.NET 3.0) from Adventure Author's images folder.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static Image GetImage(string filename)
		{			
	        Image image = new Image();
			string path = Path.Combine(ModuleHelper.ImagesDir,filename);
	        ImageSourceConverter sourceConverter = new ImageSourceConverter();
	        image.Source = (ImageSource)sourceConverter.ConvertFromString(path);            
	        return image;
		}
		
		
		/// <summary>
		/// Fetch a bitmap (.NET 2.0) from Adventure Author's images folder.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static System.Drawing.Bitmap GetBitmap(string filename)
		{
			string path = Path.Combine(ModuleHelper.ImagesDir,filename);
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(path);
            return bitmap;
		}
	}
}
