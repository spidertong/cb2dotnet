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
            /* 
            if (value.GetType() != this.valueType)
            {
                throw new InvalidCastException($"Can not set member {this.name} to type {value.GetType()}");
            }
            //*/

            //if (!typeof(T).IsAssignableFrom(value.GetType()))
            //{
            //    throw new InvalidCastException($"Can not set member {this.name} to type {value.GetType()}");
            //}

            //T val = (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
            

            var array = new byte[this.LengthOfBytes];
            //setTypedValue((T)value, array);
            setTypedValue(TryParse(value.ToString()), array);
            Buffer.BlockCopy(array, 0, bytes, this.Offset, this.LengthOfBytes);
        }

        protected abstract T getTypedValue(byte[] bytes);
        protected abstract void setTypedValue(T value, byte[] bytes);
    }
}