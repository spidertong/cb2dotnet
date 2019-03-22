using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Text;

namespace cb2dotnet
{
    public class Zoned : Numeric{
        

        public override int LengthOfBytes { get{   
            return this.NumberOfDigits;
        } }

        public override Element CloneInstance(){
            return new Zoned(this.name, this.level, this.occurs, this.Picture);
        }

        public Zoned(String name, int level, int occurs, String picture)  // :base(name, level, occurs, picture, signPosition)
        :base(name, level, occurs, picture)  
        {}
        

        /*
         * We parse this Zoned-Decimal format by Hex Value
         */
        protected override Decimal getTypedValue(byte[] bytes){

            string input = ElementExtend.BytesToHex(bytes);

            var numbers = input.ToUpper().Replace("F", "").Replace("D", "");
            bool negative = this.IsSigned && input.ToUpper().Contains("D");

            return Decimal.Parse((negative ? "-" : "") + numbers)  /  (decimal) BigInteger.Pow(10, this.DecimalPlaces) ;
        }


        


        protected override void setTypedValue(Decimal value, byte[] bytes){
            
            // get integer part and fractional part
            var integal  = (UInt64) Math.Abs(Math.Truncate(value));
            var fraction = (UInt64) (Math.Abs(value) - integal).ShiftBy10(this.DecimalPlaces);

            // fill the trailing zero of fraction, and leading zero for the whole
            var numbers = (integal.ToString() 
                          + (this.DecimalPlaces > 0 ? fraction.ToString().PadRight(this.DecimalPlaces, '0') : "")
                          )
                          .PadLeft(this.NumberOfDigits, '0');
            
            // translate
            numbers = "F" + String.Join("F", numbers.ToArray());
            if (this.IsSigned && value < 0) {
                numbers = numbers.ReplaceAt(numbers.LastIndexOf('F'), 'D');
            }

            Buffer.BlockCopy(ElementExtend.HexToBytes(numbers), 0, bytes, 0, bytes.Length);
        }
    }
}