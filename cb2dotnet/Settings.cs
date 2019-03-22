
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using cb2dotnet;

namespace cb2dotnet{

	public class Settings {

		public string Encoding {get;set;} = "IBM037";//("file.encoding");
		public bool isLittleEndian {get;set;} = false;
		//public String floatConversion {get;set;} = "net.sf.cb2java.copybook.floating.IEEE754";
		//public SignPosition signPosition {get;set;} = SignPosition.TRAILING;
		public int columnStart {get;set;} = 6;
		public int columnEnd {get;set;} = 72;
		//public Values values {get;set;} = new Values();

		static Settings DEFAULT;
		public static Settings Default(string path = "copybook.props") 
		{
			if(DEFAULT == null) {
				DEFAULT = new Settings();

				var dict = getSetting(path);

				DEFAULT.Encoding 		= dict.ContainsKey("encoding") ? dict["encoding"] : DEFAULT.Encoding;
				DEFAULT.isLittleEndian 	= dict.ContainsKey("little-endian") ? bool.Parse(dict["little-endian"]) : DEFAULT.isLittleEndian;
				//DEFAULT.floatConversion = dict.ContainsKey("float-conversion") ? dict["float-conversion"] : DEFAULT.floatConversion;
				/* 
				DEFAULT.signPosition	= dict.ContainsKey("default-sign-position") 
										? (dict["default-sign-position"] == "leading" ? SignPosition.LEADING : SignPosition.TRAILING)
										: DEFAULT.signPosition;//*/

				DEFAULT.columnStart 	= dict.ContainsKey("column.start") ? int.Parse(dict["column.start"]) : DEFAULT.columnStart;
				DEFAULT.columnEnd 		= dict.ContainsKey("column.end") ? int.Parse(dict["column.end"]) : DEFAULT.columnEnd;
			}
			return DEFAULT;
		}


		static private Dictionary<string, string> getSetting(string path) {
			var result = new Dictionary<string, string>();

			if (File.Exists(path)) {
				foreach (var row in File.ReadAllLines(path))
  					result.Add(row.Split('=')[0], string.Join("=",row.Split('=').Skip(1).ToArray()));
			}
			else{
				Console.WriteLine($"{path} not found.");
			}

			return result;
		}
	}

	public static class SettingsExt{
		/**
		* helper method for converting the given bytes to a string with encoding
		*/
		public static String GetString( this Settings  source, byte[] data) {
			/* Assume that data is encoded by the Encoding in Settings.
			 */
			///UNDONE use ICU4C
			var src_encoding = System.Text.Encoding.GetEncoding(source.Encoding);
			var sys_encoding = System.Text.Encoding.Default;
			var system_byte = Encoding.Convert(src_encoding, sys_encoding, data);
			return sys_encoding.GetString(system_byte);
		}
		
		/**
		* converts a String to a byte array based on the current encoding
		*/
		public static byte[] getBytes(this Settings source, String s) {

			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			var src_encoding = System.Text.Encoding.GetEncoding(source.Encoding);
			var sys_encoding = System.Text.Encoding.Default;
			var system_byte = sys_encoding.GetBytes(s);
			return Encoding.Convert(sys_encoding, src_encoding, system_byte);
		}
	}
	
}
