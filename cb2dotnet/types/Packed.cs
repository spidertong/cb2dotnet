using System;
using System.Linq;
using System.Numerics;

namespace cb2dotnet
{

    public class Packed : Numeric {
        
        public override int LengthOfBytes { get{
            return (base.NumberOfDigits / 2) + 1;
        } }

        public override Element CloneInstance(){
            return new Packed(this.name, this.level, this.occurs, this.Picture);
        }

        public Packed(String name, int level, int occurs, String picture)
        : base(name, level, occurs, picture) 
        {}


        protected override Decimal getTypedValue(byte[] bytes){

            string input = ElementExtend.BytesToHex(bytes);

            if (input.EndsWith("D"))
            {
                input = "-" + input;
            }
            input = input.Replace("F","").Replace("D", "");

            return Decimal.Parse(input) / (decimal) BigInteger.Pow(10, this.DecimalPlaces);
        }


        protected override void setTypedValue(decimal value, byte[] bytes){

            // get integer part and fractional part
            var integal  = (UInt64) Math.Abs(Math.Truncate(value));
            var fraction = (UInt64) (Math.Abs(value) - integal).ShiftBy10(this.DecimalPlaces);

            // number only, fill leading zero and trailing zero
            var input = integal.ToString() 
                        + (this.DecimalPlaces > 0 
                            ? fraction.ToString().PadRight(this.DecimalPlaces, '0')
                            : "");
            input = input + (this.IsSigned && value < 0 ? "D" : "F");
            input = input.PadLeft(this.LengthOfBytes*2, '0');

            Buffer.BlockCopy(ElementExtend.HexToBytes(input), 0, bytes, 0, bytes.Length);
        }
    }
}