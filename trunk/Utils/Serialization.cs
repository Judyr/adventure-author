/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 29/11/2007
 * Time: 13:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Utils
{
	public static class Serialization
	{
		public static void Serialize(string path, object o)
		{
			Stream s = null;
			try {
				FileInfo f = new FileInfo(path);
				s = f.Open(FileMode.Create);
				XmlSerializer xml = new XmlSerializer(o.GetType());
				xml.Serialize(s,o);
				s.Close();
			}
			catch (Exception e) {
				if (s != null) {
					s.Dispose();
				}
				throw new Exception("Error in serialization",e);
			}
		}
		
		
		public static object Deserialize(string path, Type type)
		{
			Stream s = null;
			try {
				FileInfo f = new FileInfo(path);
				s = f.Open(FileMode.Open);
				XmlSerializer xml = new XmlSerializer(type);
				object o = xml.Deserialize(s);				
				s.Close();
				return o;
			}
			catch (Exception e) {
				if (s != null) {
					s.Dispose();
				}
				throw new Exception("Error in deserialization",e);
			}
		}
		
		
		// TODO : Broken? Was returning true on a sabotaged .xml file of the correct type.
		/// <summary>
		/// Check whether a given file is a serialised object of a given type.
		/// </summary>
		/// <param name="filename">The filename of the file to check</param>
		/// <param name="type">The type of serialised object to check for</param>
		/// <returns>True if the file is a serialised object of the given type; false otherwise.</returns>
		public static bool IsSerialisedObjectOfType(string filename, Type type)
		{
			if (!File.Exists(filename)) {
				throw new ArgumentException(filename + " does not exist.");
			}
			
			XmlTextReader reader = new XmlTextReader(filename);
			XmlSerializer serialiser = new XmlSerializer(type);
			bool result = serialiser.CanDeserialize(reader);
			reader.Close();
			return result;
		}
	}
}
