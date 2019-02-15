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
    }
}