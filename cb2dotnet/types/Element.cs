using System;
using System.Collections.Generic;
using System.Numerics;
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
        public      int occursIndex;

        public abstract Element CloneInstance();

        public Element CopyToNext(Element parent){
            var clone = this.CloneInstance();
            

            if (parent == this.Parent) {
                clone.Previous = this.GetLastChildren();
                parent.AddChild(clone, parent.Children.IndexOf(this) + 1);
            }
            else {
                clone.Previous = parent.GetLastChildren();
                parent.AddChild(clone);
            }
                
            
            foreach (var this_child in this.Children.ToArray()){
                var end = clone.GetLastChildren();
                var clone_child = this_child.CopyToNext(clone);
                clone_child.Previous = end;
                //g.AddChild(clone_child);
            }
            return clone;
        }

        /** the absolute position of the where this item starts in data */
        public      int position {get; set;}
        /** the instance that represents the data that defines this element */
        private Settings settings;
        public      Settings Settings {
            get { return settings != null   ?  settings
                       : Settings.Default()
                       ;}
            set { settings = value;}
        }

        /*
        private Group parent = null;
        public  Group Parent {
            get {
                return this.parent;
            } 
            set{
                this.parent = (Group) value;
                value.Children.Add(this);    
            }} */
        public Group Parent = null;
        public void AddChild(Element child, int index = -1){
            index = index == -1 ? this.Children.Count : index;
            this.Children.Insert(index, child);
            child.Parent = (Group)this;
        }

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
        public abstract    object getValue(byte[] array, Dictionary<string, string> settings);

        /*
        * Expected the whole, original byte array of the Data will in passed in as parameter. 
        */
        public abstract    void   setValue(byte[] bytes, object value, Dictionary<string, string> settings);

        
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
            this.Id = System.Guid.NewGuid().ToString("D");
        }
        
        public string Id {get; private set;}
        
        /**
        * creates a new empty Data instance for this element
        * 
        * @return a new empty Data instance for this element
        */
        public virtual Data create(){ 
            return new Data(this, null); 
        }

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
                input = input + Convert.ToString(bytes[i], 16).PadLeft(2, '0');
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

        public static string ReplaceAt(this string source, int index, char new_char){
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var chars = source.ToCharArray();
            chars[index] = new_char;
            return new string (chars);
        }

        public static decimal ShiftBy10(this decimal source, int exponent){
            return ( source * (decimal) BigInteger.Pow(10, exponent) ) ;
        }

    }

}