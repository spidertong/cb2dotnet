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

        public override object getValue(byte[] bytes){
            var array = new byte[this.LengthOfBytes];
            Buffer.BlockCopy(bytes, this.Offset, array, 0, LengthOfBytes );
            T value = getTypedValue(array);
            return value;
        }

        protected abstract T TryParse(string value);

        public override void setValue(byte[] bytes, object value){

            var array = new byte[this.LengthOfBytes];

            if (value is byte[]){   //TODO  Look for a elegent way
                setTypedValue((T)value, array);   
            }else {
                setTypedValue(TryParse(value.ToString()), array);
            }
            Buffer.BlockCopy(array, 0, bytes, this.Offset, this.LengthOfBytes);
        }

        protected abstract T getTypedValue(byte[] bytes);
        protected abstract void setTypedValue(T value, byte[] bytes);
    }
}