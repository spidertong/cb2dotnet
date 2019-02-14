using System;
using System.Linq;
using System.Collections.Generic;

namespace cb2dotnet
{
    public class AlphaNumeric : ElementOf<string> 
    {
        public String Picture { get; protected set;}

        public override int LengthOfBytes {get {
            return this.Picture.GetPictureLengths().Sum();
        } }

        protected override string TryParse(string value){
            return value;
        }

        public AlphaNumeric(string name, int level, int occurs, string picture)//(string name, int length, int level, int occurs) 
        : base(name, level, occurs) 
        { 
            this.Picture = picture.ToUpper();
        }
        
        protected override string getTypedValue(byte[] bytes){
            return this.settings.GetString(bytes);
        }
        protected override void setTypedValue(string value, byte[] bytes){
            var content = this.settings.getBytes(value);

            // Prefill space
            var spaces = this.settings.getBytes(new string(' ', bytes.Length) );
            Buffer.BlockCopy(spaces, 0, bytes, 0, bytes.Length);

            // Fill string value
            Buffer.BlockCopy(content, 0, bytes, 0, content.Length < bytes.Length ? content.Length : bytes.Length);
        }
    }
}