using System;
using System.Linq;

namespace cb2dotnet
{
/**
 * Base type for numeric elements.
 *
 */
    public abstract class Numeric : ElementOf<Decimal> {
        
        public bool IsSigned { get{return this.Picture.StartsWith("S");} }
        public String Picture { get; protected set;}

        public int DecimalPlaces { get{return this.Picture.GetPicFractionalLength();} }

        // This should be implement by child class, such as Packed/Zone
        //public override int length {get{ return this.Picture.GetPictureLengths().Sum(); } }

        public int NumberOfDigits { get {return this.Picture.GetPictureLengths().Sum();} }
        
        protected override decimal TryParse(string value){
            return decimal.Parse(value);
        }
        
        protected Numeric(String name, int level, int occurs, String picture) 
        : base( name, level, occurs) 
        {
            this.Picture = picture.ToUpper();
        }
        

        /* 
        public DecimalFormat getFormatObject() {
            StringBuffer buffer = new StringBuffer("#");
                    
            for (int i = 0; i < digits(); i++) {
                if (i + decimalPlaces() == digits()) {
                    buffer.append('.');
                }
                buffer.append('0');
            }
            
            if (decimalPlaces() < 1) {
                buffer.append('.');
            }
            
            buffer.append('#');
                    
            return new DecimalFormat(buffer.toString());
        } */
        
        /**
        * validates the data with the given decimal (printed) length
        * constraint.  This is useful for types where the length in
        * the application data is not the same as the logical length
        * e.g. SignedSeparate.
        * 
        * @param data
        */
        /*
        @Override
        public void validate(Object data) {
            if (data == null) {
                return;
            }
            
            BigDecimal bigD = (data instanceof BigInteger) ? new BigDecimal((BigInteger) data) : (BigDecimal) data;
            boolean negative = BigDecimal.ZERO.compareTo(bigD) > 0;
            
            if (negative && !signed()) {
                throw (IllegalArgumentException) createEx(bigD, getName() 
                    + " is not signed").fillInStackTrace();
            }
            
            int scale = bigD.scale();
            
            if (decimalPlaces() > 0 && scale > decimalPlaces()) {
                throw (IllegalArgumentException) createEx(bigD, "must have " 
                    + decimalPlaces() + " decimal places").fillInStackTrace();
            }
            
            bigD = bigD.setScale(decimalPlaces());
            String s = bigD.unscaledValue().toString();
            
            if (negative) {
                s = s.substring(1);
            }
                
            if (s.length() > digits()) {
                throw (IllegalArgumentException) createEx(bigD, "must be no longer than " 
                    + digits() + " digits").fillInStackTrace();
            }
        }  */
        
        /*
        public Value getValue() {
            Value result = super.getValue();
            return result == null ? getSettings().getValues().ZEROES : result;
        }  */
        

        /*
        protected BigInteger getUnscaled(Object data) {
            if (data instanceof BigInteger) {
                return (BigInteger) data;
            } else {
                int places = decimalPlaces();
                BigDecimal bigD = (BigDecimal) data;
                if (bigD.scale() != places) {
                    bigD = bigD.setScale(decimalPlaces());
                }
                
                return bigD.unscaledValue();
            }
        } */
    }
}