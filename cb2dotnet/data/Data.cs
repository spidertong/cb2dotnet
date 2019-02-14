using System;
using System.Dynamic;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace cb2dotnet{

    public  class Data : System.Dynamic.DynamicObject{
        
        public   Element definition {get; protected set;}
        public   Data parent {get; protected set;} 

        private byte[] bytes_;
        public byte[] bytes  {
            get{
                return this.parent != null ? this.parent.bytes
                     : this.bytes_;
            }
        }
        
        public string Name {get {return definition.name;}}

        public byte[] GetBytes() {
            var result = new byte[definition.LengthOfBytes];
            Buffer.BlockCopy(bytes, definition.Offset, result, 0, definition.LengthOfBytes );
            return result;
        }
        /**
        * constructor
        * 
        * @Element definition, the underlying type definition
        * @Data parent, the parent of this instance
        *
        * Only a instance with parent == null, will actually contain the underlying bytes array.
        */
        public Data(Element definition, Data parent = null) {
            this.definition = definition;
            this.parent = parent;
            if (this.parent == null)
            {
                this.bytes_ = new byte[definition.LengthOfBytes];
            }
        }

        public override bool TryGetMember (System.Dynamic.GetMemberBinder binder, out object result){
            var child = definition.Children
                        .Where(def => def.name == binder.Name.Replace("_", "-"))
                        .DefaultIfEmpty(null)
                        .SingleOrDefault();

            if (child != null)
            {
                result = new Data(child, this);
                return true;
            }
            result = null;
            return false;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == definition.valueType)
            {
                result = definition.getValue(this.bytes);
                return true;
            }
            result = null;
            return false;
        }

        
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var child = definition.Children
                        .Where(def => def.name == binder.Name.Replace("_", "-"))
                        .DefaultIfEmpty(null)
                        .SingleOrDefault();
                        ;

            if (child != null)
            {
                child.setValue(this.bytes, value);
                return true;
            }
            return false;
        } 
        
    }

}