using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using com.ibm.as400.access;

namespace cb2dotnet
{
    public class AlphaNumeric : ElementOf<string> 
    {

        public override Element CloneInstance(){
            return new AlphaNumeric(this.name, this.level, this.occurs, this.Picture);
        }

        public String Picture { get; protected set;}

        public override int LengthOfBytes {get {
            return this.Picture.GetPictureLengths().Sum();
        } }

        protected override string TryParse(string value){
            return value;
        }

        public AlphaNumeric(string name, int level, int occurs, string picture)
        : base(name, level, occurs) 
        { 
            this.Picture = picture.ToUpper();
        }

        /*
        protected override string getTypedValue(byte[] bytes, Dictionary<string, string> settings){
            //
            // After adopting JT400 to .Net world :
            //  Encoding.RegisterProvider(JTEncodingProvider.Instance);
            // This solutio will be better.
            //
            var target_encoding = settings != null && settings.ContainsKey("TargetEncoding")
                                ? Encoding.GetEncoding(settings["TargetEncoding"])
                                : Settings.TargetEncoding;
            return target_encoding.ConvertToString(bytes);
        }
        protected override void setTypedValue(string value, byte[] bytes, Dictionary<string, string> settings){

            //
            // After adopting JT400 to .Net world :
            //  Encoding.RegisterProvider(JTEncodingProvider.Instance);
            // This solutio will be better.
            //
            
            var padded_value   = value.PadRight(bytes.Length);
            var target_encoding = settings != null && settings.ContainsKey("TargetEncoding") 
                                ? Encoding.GetEncoding(settings["TargetEncoding"])
                                : Settings.TargetEncoding;

            var content = target_encoding.ConvertToBytes(padded_value);
            Buffer.BlockCopy(content, 0, bytes, 0, content.Length < bytes.Length ? content.Length : bytes.Length);
        } */

        protected AS400Text AS400Text;
        protected override string getTypedValue(byte[] bytes, Dictionary<string, string> settings){
            /*if (AS400Text == null && (settings == null || !settings.ContainsKey("TargetEncoding")) ){
                return Settings.TargetEncoding.ConvertToString(bytes);
            } 
            else{
                AS400Text = AS400Text ?? new com.ibm.as400.access.AS400Text(bytes.Length, int.Parse(settings["TargetEncoding"]));
                return AS400Text.toObject(bytes).ToString();
            } */

            AS400Text = AS400Text ?? new com.ibm.as400.access.AS400Text(bytes.Length, Settings.Default().TargetEncoding);
            return AS400Text.toObject(bytes).ToString();
        }
        protected override void setTypedValue(string value, byte[] bytes, Dictionary<string, string> settings){
            /*
            var padded_value   = value.PadRight(bytes.Length);
            byte[] content;
            if (AS400Text == null && (settings == null || !settings.ContainsKey("TargetEncoding")) ){
                content = Settings.TargetEncoding.ConvertToBytes(padded_value);
            } 
            else{
                AS400Text = AS400Text ?? new com.ibm.as400.access.AS400Text(bytes.Length, int.Parse(settings["TargetEncoding"]));
                content = AS400Text.toBytes(value);
            }
            Buffer.BlockCopy(content, 0, bytes, 0, content.Length < bytes.Length ? content.Length : bytes.Length);
            */
            byte[] content;
            AS400Text = AS400Text ?? new com.ibm.as400.access.AS400Text(bytes.Length, Settings.Default().TargetEncoding);
            content = AS400Text.toBytes(value);
            Buffer.BlockCopy(content, 0, bytes, 0, content.Length < bytes.Length ? content.Length : bytes.Length);
        }
    }
}