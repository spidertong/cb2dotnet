using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace cb2dotnet {

    /**
    * base class for types.
    */
    public abstract class ElementOf<T> : Element{

        public override Type valueType {get{return typeof(T);}}
        protected ElementOf(string name, int level, int occurs): base(name, level, occurs){}

        public override object getValue(byte[] bytes, Dictionary<string, string> settings){
            var array = new byte[this.LengthOfBytes];
            Buffer.BlockCopy(bytes, this.Offset, array, 0, LengthOfBytes );
            T value = getTypedValue(array, settings);
            return value;
        }

        protected abstract T TryParse(string value);

        public override void setValue(byte[] bytes, object value, Dictionary<string, string> settings){

            var array = new byte[this.LengthOfBytes];

            if (value is byte[]){   //TODO  Look for a elegent way
                setTypedValue((T)value, array, settings);   
            }else {
                setTypedValue(TryParse(value.ToString()), array, settings);
            }
            Buffer.BlockCopy(array, 0, bytes, this.Offset, this.LengthOfBytes);
        }

        protected abstract T getTypedValue(byte[] bytes, Dictionary<string, string> settings);
        protected abstract void setTypedValue(T value, byte[] bytes, Dictionary<string, string> settings);
    }
}