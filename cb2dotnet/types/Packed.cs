using System;
using System.Linq;
using System.Numerics;

namespace cb2dotnet
{

    public class Packed : Numeric {
        
        public override int LengthOfBytes { get{
            return (base.NumberOfDigits / 2) + 1;
        } }

        public Packed(String name, int level, int occurs, String picture, SignPosition signPosition)
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
        /* Tested throw
        protected override Decimal getTypedValue(byte[] input){
            byte lastByte = input[input.Length - 1];
            bool negative = this.IsSigned && (lastByte & 0x0F) == 0x0D;
            BigInteger iresult = BigInteger.Zero;

            for (int i = 0; i < this.LengthOfBytes * 2 - 1; i++){
                byte current = input[i / 2];
                if (i % 2 == 0){
                    current = (byte) ((current & 0xF0) >> 4);
                } else {
                    current = (byte) (current & 0x0F);
                }

                //*
                //* magnitude is the one more than the reverse of the index so if
                //* there are 9 digits, the magnitude at i = 0, is 8, the magnitude
                //* at i = 8 is 0
                ///
                int magnitude = this.NumberOfDigits - (i + 1);
                var digits_ten = BigInteger.Pow(10, magnitude);
                int icurrent = (int)current;
                iresult = iresult + icurrent * digits_ten;
            }

            decimal result = (decimal) (negative ? iresult * -1 : iresult);

            return result / (decimal) BigInteger.Pow(10, this.DecimalPlaces);
        }// */

        /*
        @Override
        public Data parse(byte[] input) {
            byte lastByte = input[input.length - 1];
            boolean negative = signed() && (lastByte & 0x0F) == 0x0D;
            BigInteger bigI = BigInteger.ZERO;
            int numberLength = (length * 2) - 1;

            for (int i = 0; i < numberLength; i++) {
                byte current = input[i / 2];
                // if the index is even, use the left nibble odd, use the right nibble
                if (i % 2 == 0) {
                    current = (byte) ((current & 0xF0) >> 4);
                } else {
                    current = (byte) (current & 0x0F);
                }

                //
                // magnitude is the one more than the reverse of the index so if
                // there are 9 digits, the magnitude at i = 0, is 8, the magnitude
                // at i = 8 is 0
                //
                int magnitude = numberLength - (i + 1);
                BigInteger temp = BigInteger.TEN.pow(magnitude);
                bigI = bigI.add(temp.multiply(BigInteger.valueOf(current)));
            }

            if (negative) {
                bigI = bigI.negate();
            }

            Data data = create();

            if (data instanceof DecimalData) {
                DecimalData dData = (DecimalData) data;
                BigDecimal bigD = new BigDecimal(bigI, decimalPlaces());
                dData.setValue(bigD);
            } else {
                IntegerData iData = (IntegerData) data;
                iData.setValue(bigI);
            }
            return data;
        } */


        //01234F
        protected override void setTypedValue(decimal value, byte[] bytes){
            var input = value.ToString().Replace(".","")
                                        .Replace("-","");
            input = input + "F";

            input = input.PadLeft(this.LengthOfBytes*2, '0');

            Buffer.BlockCopy(ElementExtend.HexToBytes(input), 0, bytes, 0, bytes.Length);
        }

        /* Tested incorrect.
        protected override void setTypedValue(decimal value, byte[] bytes)
        {
            byte signNibble = this.IsSigned
                            ? (byte) ( value < decimal.Zero ? 0x0D : 0x0C)
                            : (byte) 0x0F;

            for (int i = this.LengthOfBytes; i > 0; i--) {
                int mod = Decimal.ToInt32(value % 10);
                int index = (i - 1) / 2;

                if (i % 2 == 0) {
                    bytes[index] = (byte) (bytes[index] | mod);
                } else {
                    bytes[index] = (byte) (bytes[index] | (mod << 4));
                }

                value = value / 10;
            }

            int signByte = bytes.Length - 1;
            bytes[signByte] = (byte) (bytes[signByte] | signNibble);            
        }//*/

        /*
        @Override
        public byte[] toBytes(Object data) {
            BigInteger bigI = (data == null) ? BigInteger.ZERO : getUnscaled(data);
            byte signNibble = signed() ? (byte) (bigI.compareTo(BigInteger.ZERO) < 0 ? 0x0D : 0x0C) : 0x0F;
            byte[] bytes = new byte[getLength()];
            int numberLength = (length * 2) - 1;

            for (int i = numberLength; i > 0; i--) {
                int value = bigI.mod(BigInteger.TEN).intValue();
                int index = (i - 1) / 2;

                if (i % 2 == 0) {
                    bytes[index] = (byte) (bytes[index] | value);
                } else {
                    bytes[index] = (byte) (bytes[index] | (value << 4));
                }

                bigI = bigI.divide(BigInteger.TEN);
            }

            int signByte = bytes.length - 1;
            bytes[signByte] = (byte) (bytes[signByte] | signNibble);
            return bytes;
        } */
    }
}