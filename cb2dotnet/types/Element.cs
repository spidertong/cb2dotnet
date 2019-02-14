using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace cb2dotnet {

    /**
    * base class for types.
    */
    public abstract class Element {
        
        /** the name of the element from the copybook */
        public      string name {get; protected set;}
        /** the level of the element from the copybook */
        public      int level {get; protected set;}
        /** how many times the element occurs in the application data */
        public      int occurs {get; protected set;}
        /** the absolute position of the where this item starts in data */
        public      int position {get; set;}
        /** the instance that represents the data that defines this element */
        private Settings setting_;
        public      Settings settings {
            get { return setting_ != null ?  settings
                       : Parent != null ? Parent.settings
                       : Settings.Default()
                       ;}
            set { setting_ = value;}
        }

        /** the parent of this element */
        //public  Group parent {get;set;}

        private Group parent = null;
        public  Group Parent {
            get {
                return this.parent;
            } 
            set{
                this.parent = (Group) value;
                value.Children.Add(this);    
            }}
        public Element Previous;


        /** the distance to the beginning of parent. */
        //public abstract      int Offset {get;}
        public virtual int Offset {get {
            return this.Previous == null   ? 0 
                 : this.Previous is Group  ? this.Previous.Offset
                 : this.Previous.Offset + this.Previous.LengthOfBytes;
        } }


        /**  the number of bytes of this element will occupy */
        public abstract      int LengthOfBytes {get;}

        public List<Element> Children {get;set;} = new List<Element>();

        public virtual bool IsGroup {get{return Children.Count > 0;}}

        public abstract      Type valueType{get;}

        /*
        * Expected the whole, original byte array of the Data will in passed in as parameter. 
        */
        public abstract    object getValue(byte[] array);

        /*
        * Expected the whole, original byte array of the Data will in passed in as parameter. 
        */
        public abstract    void   setValue(byte[] bytes, object value);

        
        /**
        * constructor
        * 
        * @param name the element name
        * @param level the level of the element
        * @param occurs how many times the element occurs
        */
        protected Element(   string name,    int level,    int occurs) {
            this.name = name;
            this.level = level;
            this.occurs = occurs;
        }
        
        
        /**
        * creates a new empty Data instance for this element
        * 
        * @return a new empty Data instance for this element
        */
        public virtual Data create(){ 
            return new Data(this, null); 
        }
        
        /**
        * helper method for converting the given bytes to a string with
        * the parent copybook's encoding
        * 
        * @param data the data to convert to a string
        * @return the string value
        */
        //public    string getstring(Data data) {
        //    return new string(data.getBytes(offset, length), settings.getEncoding()); 
        //}

        public override string ToString() {
            //return new string(getSettings().getValues().SPACES.fill(level)) + name + ": '" 
            //    + this.getClass() + " " + getLength() + "'\n";
            return new string(this.name);
        }
    }

    public static class ElementExtend{
        private static readonly string PATTEN_QUOTE_LENGTH = @"\([0-9]+\)"; // match  "(ddd)", while d is digital
        private static readonly string PATTEN_VIRTUAL_DECIMAL = $"V{PATTEN_QUOTE_LENGTH}"; // match V(dd), while 
        private static readonly string PATTEN_INTEGAL = $"S?9{PATTEN_QUOTE_LENGTH}";
        private static readonly string PATTEN_NUMERIC = $"{PATTEN_INTEGAL}({PATTEN_VIRTUAL_DECIMAL})?";
        private static readonly string PATTEN_ALPHABETIC = $"A{PATTEN_QUOTE_LENGTH}";
        private static readonly string PATTEN_ALPHANUMERIC = $"X{PATTEN_QUOTE_LENGTH}";

        /**
        *  PIC S9(03)V(02)   return length of 5 = 03 + 02
        */
        public static int[] GetPictureLengths(this string picture){
                                                       
            var list = new List<int>();
            foreach (Match match in Regex.Matches(picture, PATTEN_QUOTE_LENGTH))
            {
                list.Add(int.Parse(match.Value
                                    .Replace("(", "")
                                    .Replace(")", "")
                                    )) ;
            }
            return list.ToArray();                                                                                          
        }

        public static bool IsPictureNumeric(this string picture){
            return Regex.Match(picture, PATTEN_NUMERIC).Success;
        }
        public static int GetPicIntegalLength(this string picture){
            if (! IsPictureNumeric(picture))
            {
                throw new ArgumentException("paramter  is not a cobol PICTURE format S9(04)V(05)");
            }
            return  GetPictureLengths(picture)[0];

        }

        public static int GetPicFractionalLength(this string picture){
            if (! IsPictureNumeric(picture))
            {
                throw new ArgumentException("paramter  is not a cobol PICTURE format like: S9(04)V(05)");
            }
            var list = picture.GetPictureLengths();
            return list.Length == 2 ? list[1] : 0;
        }

        //public static Type GetPictureType(this string picture)
        //{}

        public static string BytesToHex(byte[] bytes){
            var input = "";
            for (int i = 0; i < bytes.Length; i++)            {
                input = input + Convert.ToString(bytes[i], 16);
            }
            return input.ToUpper();
        }

        public static byte[] HexToBytes(string hex){

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++){
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
    }

}