using System;
using System.Linq;
using System.Collections.Generic;

namespace cb2dotnet {

    /**
    * base class for Group.
    */
    public class Group : ElementOf<byte[]>{
    
        public override int LengthOfBytes {get {
            if (this.Children.Contains(null))
            {
                Console.WriteLine(this.name + "Contains null");
            }

            return this.Children
                        .Select(child => child.Offset + child.LengthOfBytes)
                        .Max()
                -  this.Offset;
        }}

        public string Redefines;

        //*
        public override int Offset {get {
            return ! String.IsNullOrEmpty(Redefines) 
                ? this.Parent.Children.First(child => child.name == this.Redefines).Offset
                : base.Offset;
        } } //*/

        public override Type valueType {get{return Type.GetType("null");}}

        //protected Group(string name, int level, int occurs): base(name, level, occurs){}
        public Group(string name, int level, int occurs, string redefines): base(name, level, occurs){
            this.Redefines = redefines;
        }

        public override object getValue(byte[] bytes){

            throw new NotSupportedException("Cannot get value from a Group instant.");
        }

        public override void setValue(byte[] bytes, object value){
            
            if  (value is byte[])
                Buffer.BlockCopy((byte[])value, this.Offset, bytes, this.Offset, this.LengthOfBytes);
            else
                throw new NotSupportedException("Cannot set value to a Group instant, unless it's byte to byte copy");
        }

        protected override byte[] TryParse(string value)
        {
            throw new NotImplementedException();
        }

        protected override byte[] getTypedValue(byte[] bytes)
        {
            return bytes;
        }

        protected override void setTypedValue(byte[] value, byte[] bytes)
        {
            Buffer.BlockCopy(value,0, bytes, 0, bytes.Length);
        }
    }
}