using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Text;

namespace cb2dotnet
{
    public class Zoned : Numeric{
        
        SignPosition SignPosition;

        public override int LengthOfBytes { get{   
            return this.NumberOfDigits;
        } }

        public Zoned(String name, int level, int occurs, String picture, SignPosition signPosition)  // :base(name, level, occurs, picture, signPosition)
        :base(name, level, occurs, picture)  {
            this.SignPosition = signPosition;
        }
        
        /**
        * returns the character for the given char representing a digit
        * in order to create an overpunched digit in a zoned number
        * 
        * @param positive whether the value is positive i.e. false is negative
        * @param overpunch the char to overpunch
        * @return the overpunched char
        */
        private byte getChar(bool positive, byte overpunch) {

            if (!this.IsSigned)
                return overpunch;

            if (positive) {

                var  sysNumbers = new List<byte>( this.settings.getBytes("0123456789") );
                var  sysSigned  = new List<byte>( this.settings.getBytes("{ABCDEFGHI") );

                //Debug
                //Console.WriteLine("sysNumbers:" + bytesToHex(sysNumbers) );
                //Console.WriteLine("sysSigned:"  + bytesToHex(sysSigned) );

                return sysSigned.ElementAt(sysNumbers.IndexOf(overpunch));
            }
            else {

                var  sysNumbers = new List<byte>( this.settings.getBytes("0123456789") );
                var  sysSigned  = new List<byte>( this.settings.getBytes("}JKLMNOPQR") );

                //Debug
                //Console.WriteLine("sysNumbers:" + bytesToHex(sysNumbers) );
                //Console.WriteLine("sysSigned:"  + bytesToHex(sysSigned) );

                return sysSigned.ElementAt(sysNumbers.IndexOf(overpunch));
            }
        }
        
        /**
        * whether the given overpunched char is positive
        * 
        * @param overpunched the char to check
        * @return whether the given overpunched char is positive
        */
        private bool isPositive(char overpunched) {
            if (!this.IsSigned) {
                return true;
            } else {
                switch(overpunched) {
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '{':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case '0':
                        return true;
                    case '}':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R': 
                        return false;
                }
            }
            
            throw new ArgumentOutOfRangeException("invalid char: " + overpunched);
        }
        
        /**
        * The digit char for an overpunched char
        * 
        * @param overpunched
        * @return the digit char
        */
        private string getNumber(char overpunched) {
            if (!this.IsSigned) {
                return overpunched.ToString();
            } else {
                switch(overpunched) {
                    case '9':
                    case 'R': 
                    case 'I': 
                        return '9'.ToString();
                    case '8':
                    case 'Q':
                    case 'H': 
                        return '8'.ToString();
                    case '7':
                    case 'P': 
                    case 'G': 
                        return '7'.ToString();
                    case '6':
                    case 'O': 
                    case 'F': 
                        return '6'.ToString();
                    case '5':
                    case 'N': 
                    case 'E': 
                        return '5'.ToString();
                    case '4':
                    case 'M': 
                    case 'D': 
                        return '4'.ToString();
                    case '3':
                    case 'L': 
                    case 'C': 
                        return '3'.ToString();
                    case '2':
                    case 'K': 
                    case 'B': 
                        return '2'.ToString();
                    case '1':
                    case 'J': 
                    case 'A': 
                        return '1'.ToString();
                    case '0': 
                    case '}': 
                    case '{': 
                        return '0'.ToString();
                }
            }
            
            throw new ArgumentOutOfRangeException("invalid char: " + overpunched);
        }


        /*
         * Instead of using the Setting.Encoding to parse the value
         *
         * We parse this Zoned-Decimal format by Hex Value
         */
        protected override Decimal getTypedValue(byte[] bytes){

            string input = ElementExtend.BytesToHex(bytes);

            var numbers = new string (input.Select((ch, idx) => idx % 2 == 0 ? ' ' : ch).ToArray() );
            var signs   = new string (input.Select((ch, idx) => idx % 2 == 1 ? ' ' : ch).ToArray() );

            return Decimal.Parse((signs.Contains("D") ? "-" : "") + numbers.Replace(" ", ""))
                    / (decimal) BigInteger.Pow(10, this.DecimalPlaces) ;
        }

        /* Tested work.
        protected override Decimal getTypedValue(byte[] bytes){

            string input = this.settings.GetString(bytes).Trim();;

            if (String.IsNullOrEmpty(input)){
                throw new ArgumentNullException("Invalid value for decimal value");
            }

            if(this.IsSigned){
                char c  = this.SignPosition == SignPosition.LEADING  ? input.First()
                        : this.SignPosition == SignPosition.TRAILING ? input.Last()
                        : input.First();
                
                input   = this.SignPosition == SignPosition.LEADING  ? getNumber(c) + String.Join("", input.Skip(1))
                        : this.SignPosition == SignPosition.TRAILING ? String.Join("", input.SkipLast(1)) + getNumber(c)
                        : input;

                input = (isPositive(c) ? "" : "-") + input;
            }

            var result = (decimal) BigInteger.Parse(input);

            return result / (decimal) BigInteger.Pow(10, this.DecimalPlaces);
        }// */

        /*
        @Override
        public Data parse(byte[] bytes) {
            String input = getString(bytes).trim();
            String s = input;
            
            if (input.length() < 1) {
                s = null;
            } else if (signed()) {
                if (getSignPosition() == SignPosition.LEADING) {
                    char c = input.charAt(0); 
                    s = (isPositive(c) ? "" : "-") + getNumber(c) 
                        + (input.length() > 1 ? input.substring(1) : ""); 
                } else if (getSignPosition() == SignPosition.TRAILING) {
                    int last = input.length() - 1; 
                    char c = input.charAt(last); 
                    s = (isPositive(c) ? "" : "-") 
                        + (input.length() > 1 ? input.substring(0, last) : "") + getNumber(c);
                } else {
                    throw new IllegalStateException("undefined sign position");
                }
            }        
            BigInteger big = s == null ? null : new BigInteger(s);
            Data data = create();
            
            if (data instanceof DecimalData) {
                DecimalData dData = (DecimalData) data;
                BigDecimal bigD = big == null ? null : new BigDecimal(big, decimalPlaces());
                
                dData.setValue(bigD);
                
                return data;
            } else {
                IntegerData iData = (IntegerData) data;
                
                iData.setValue(big);
                
                return data;
            }
        }  */   //MCH1202 error for decimal fields.
        


        protected override void setTypedValue(Decimal value, byte[] bytes){
            
            var numbers = value.ToString()  .Replace("-", "")
                                            .Replace(".", "");

            /*
            var builder = new StringBuilder();
            for (int i = 0; i < numbers.Length; i++){
                builder.Append("F").Append(numbers[i]);
            } */
            numbers = "F" + String.Join("F", numbers.ToArray());
            Buffer.BlockCopy(ElementExtend.HexToBytes(numbers), 0, bytes, 0, bytes.Length);

            Func<byte, byte> Sign = b => (byte) (b & 0xDF);
            if (value < 0) {
                if (this.SignPosition == SignPosition.LEADING)
                    bytes[0] = Sign(bytes[0]);
                else 
                    bytes[bytes.Length - 1] = Sign(bytes[bytes.Length - 1]);
            }
        }
        /* Tested work
        protected override void setTypedValue(Decimal value, byte[] bytes){

            //
            // Pre-filled with zero
            //
            var defualt = string.Join("", Enumerable.Repeat("0", bytes.Count()));
            Buffer.BlockCopy(this.settings.getBytes(defualt), 0, bytes,0, bytes.Count());

            //
            //  Fill with encoded value
            //
            BigInteger ivalue = (BigInteger) ( Math.Abs(value)  * (Decimal) BigInteger.Pow(10, this.DecimalPlaces) );  //Fractional digits are truncated.
            byte[] bvalue = this.settings.getBytes(ivalue.ToString());
            Buffer.BlockCopy(bvalue, 0, bytes, bytes.Length - bvalue.Length, bvalue.Length);  //Using numeric algnment.


            //
            //  Handle sign
            //
            if (this.SignPosition == SignPosition.LEADING){
                bytes[0] = getChar(value > 0, bytes[0]);
            } else 
            if (this.SignPosition == SignPosition.TRAILING){
                bytes[bytes.Length - 1] = getChar(value > 0, bytes[bytes.Length - 1]);
            }
        } // */


        /*
        @Override
        public byte[] toBytes(Object data) {
            if (data == null) {
                return getValue().fill(getLength());
            } 

            BigInteger bigI = getUnscaled(data);
            boolean positive;
            
            if (BigDecimal.ZERO.unscaledValue().compareTo(bigI) > 0) {
                bigI = bigI.abs();
                positive = false;
            } else {
                positive = true;
            }
            
            byte[] output = getValue().fill(getBytes(bigI.toString()), getLength(), Value.LEFT);
            
            if (getSignPosition() == SignPosition.LEADING) {
                output[0] = (byte) getChar(positive, (char) output[0]);
            } else if (getSignPosition() == SignPosition.TRAILING) {
                int last = output.length - 1;
                output[last] = (byte) getChar(positive, (char) output[last]);
            } else {
                throw new IllegalStateException("undefined sign position");
            }
            
            return output;
        }  */

        /* Tested work
        static string BytesToHex(byte[] bytes){
            char[] hexArray = "0123456789ABCDEF".ToCharArray();
            char[] hexChars = new char[bytes.Length*2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int v = bytes[i] & 0xFF;
                hexChars[i*2] = hexArray[v >> 4];
                hexChars[i*2+1] = hexArray[v & 0x0F];
            }
            return new string(hexChars);
        }// */
    }
}