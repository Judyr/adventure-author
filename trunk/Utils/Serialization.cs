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
    /// <summary>
    /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
    /// </summary>
    /// <param name="characters">Unicode Byte Array to be converted to String</param>
    /// <returns>String converted from Unicode Byte Array</returns>
    private static String UTF8ByteArrayToString ( Byte[ ] characters )
    {
        UTF8Encoding encoding = new UTF8Encoding ( );
        String constructedString = encoding.GetString ( characters );
        return ( constructedString );
    }

 

    /// <summary>
    /// Converts the String to UTF8 Byte array and is used in De serialization
    /// </summary>
    /// <param name="pXmlString"></param>
    /// <returns></returns>
    private static Byte[ ] StringToUTF8ByteArray ( String pXmlString )
    {
        UTF8Encoding encoding = new UTF8Encoding ( );
        Byte[ ] byteArray = encoding.GetBytes ( pXmlString );
        return byteArray;
    }
    
    
    
    /// <summary>
    /// Method to convert a custom Object to XML string
    /// </summary>
    /// <param name="pObject">Object that is to be serialized to XML</param>
    /// <returns>XML string</returns>
    public static string SerializeObject (object pObject)
    {
		try {
            String XmlizedString = null;
            MemoryStream memoryStream = new MemoryStream ();
            Type type = pObject.GetType();
            XmlSerializer xs = new XmlSerializer ( type );
            XmlTextWriter xmlTextWriter = new XmlTextWriter ( memoryStream, Encoding.UTF8 ); 

            xs.Serialize ( xmlTextWriter, pObject );
            memoryStream = ( MemoryStream ) xmlTextWriter.BaseStream;
            XmlizedString = UTF8ByteArrayToString ( memoryStream.ToArray ( ) );
            return XmlizedString;
        }
		catch (Exception e) {
			Say.Error(e);
			return null;
		}
    }
    
    
    
    
    /// <summary>
    /// Method to reconstruct an Object from XML string
    /// </summary>
    /// <param name="pXmlizedString"></param>
    /// <returns></returns>
    public static object DeserializeObject ( String pXmlizedString, Type type )
    {
        XmlSerializer xs = new XmlSerializer ( type );
        MemoryStream memoryStream = new MemoryStream ( StringToUTF8ByteArray ( pXmlizedString ) );
        XmlTextWriter xmlTextWriter = new XmlTextWriter ( memoryStream, Encoding.UTF8 );
        return xs.Deserialize ( memoryStream );
    }

	
	
	
	
	}
	
	
}
