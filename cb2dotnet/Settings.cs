
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using cb2dotnet;

namespace cb2dotnet{

	public class Settings {

		public Encoding TargetEncoding {get;set;} = System.Text.Encoding.GetEncoding("IBM037");
		public bool IsLittleEndian {get;set;} = false;

		public int ColumnStart {get;set;} = 6;
		public int ColumnEnd {get;set;} = 72;

		static Settings DEFAULT;
		public static Settings Default(string path = "copybook.props") 
		{
			if(DEFAULT == null) {
				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

				DEFAULT = new Settings();

				var dict = getSetting(path);

				DEFAULT.TargetEncoding 		= dict.ContainsKey("target-encoding") ? Encoding.GetEncoding(dict["target-encoding"]) : DEFAULT.TargetEncoding;
				DEFAULT.IsLittleEndian 	= dict.ContainsKey("little-endian") ? bool.Parse(dict["little-endian"]) : DEFAULT.IsLittleEndian;

				DEFAULT.ColumnStart 	= dict.ContainsKey("column.start") ? int.Parse(dict["column.start"]) : DEFAULT.ColumnStart;
				DEFAULT.ColumnEnd 		= dict.ContainsKey("column.end") ? int.Parse(dict["column.end"]) : DEFAULT.ColumnEnd;
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

		public static byte[] ConvertToBytes(this Encoding target, string s){
			return target.ConvertToBytes(s, System.Text.Encoding.Default);
		}
		public static byte[] ConvertToBytes(this Encoding targetEncoding, String s, Encoding dotnetEncoding ) {
			return Encoding.Convert(dotnetEncoding, targetEncoding, dotnetEncoding.GetBytes(s));
		}
		public static string ConvertToString(this Encoding targetEncoding, byte[] data){
			return targetEncoding.ConvertToString(data, System.Text.Encoding.Default);
		}
		public static string ConvertToString(this Encoding targetEncoding, byte[] data, Encoding dotnetEncoding ){
			var system_byte = Encoding.Convert(targetEncoding, dotnetEncoding, data);
			return dotnetEncoding.GetString(system_byte);
		}
	}
	
}
