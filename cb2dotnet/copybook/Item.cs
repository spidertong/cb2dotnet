using System;
using System.Collections.Generic;

namespace cb2dotnet {
/**
* our internal representation of a copybook "item" node
*/
    public class Item
    {
        public bool document {get; protected set;}
        
        //public Values values {get; protected set;}


        public Settings settings {get;protected set;}
        
        /**
        * @param analyzer
        */
        public Item( //Values values,   
            Item previous, 
            //bool document, 
            Settings settings
            )
        {
            //this.values = values;
            this.previous = previous;
            //this.document = document;
            this.document = this.previous == null;
            this.settings = settings;
            this.SignPosition = settings.signPosition;
        }

        public string name;
        public int level;
        Item parent;
        public Item previous;
        int length;
        
        public readonly List<Item> children = new List<Item>();
        
        public string redefines;
        public int occurs = 1;
        public int minOccurs;  // not supported
        public string dependsOn; // not supported
        
        public bool isAlpha;
        public bool signSeparate;
        public SignPosition SignPosition;
        
        public string picture;
        //Value value;
        public Usage usage;
        
        private Element element;

        public void setParent(Item candidate)
        {
            if (level > candidate.level) {
                parent = candidate;
                parent.children.Add(this);
            } else {
                setParent(candidate.parent);
            }
        }
        
        public void createElement()
        {
            if (document) {
                createDocument();
            } 
            else 
            if (picture == null) {
                if (usage == Usage.COMPUTATIONAL_1) {
                    createSingleFloat();
                } else if (usage == Usage.COMPUTATIONAL_2) {
                    createDoubleFloat();
                } else {
                    createGroup();
                }
            } 
            else 
            if (isAlpha) {
                createAlphaNumeric();
            } 
            else {
                if (usage == Usage.BINARY) {
                    createBinary();
                } else if (usage == Usage.COMPUTATIONAL) {
                    createBinary();
                } else if (usage == Usage.PACKED_DECIMAL) {
                    createPacked();
                } else if (usage == Usage.COMPUTATIONAL_3) {
                    createPacked();
                } else if (usage == Usage.COMPUTATIONAL_4) {
                    createBinary();
                } else if (usage == Usage.COMPUTATIONAL_5) {
                    createNativeBinary();
                } else if (signSeparate) {
                    createSignedSeparate();
                } else {
                    createDecimal();
                }
            }
            
            //if (value != null) {
            //    element.setValue(value);
            //}
        }
        
        private void createDocument()
        {
            //element = new Copybook(name, values, settings);
            element = new Group(name, level, occurs, null);
        }
        
        private void createGroup()
        {
            element = new Group(name, level, occurs, redefines);
        }
        
        private void createBinary()
        {
            //element = new Binary(name, level, occurs, picture);
        }
        
        private void createNativeBinary()
        {
            //element = new Binary.Native(name, level, occurs, picture);
        }
        
        private void createPacked()
        {
            element = new Packed(name, level, occurs, picture, SignPosition);
        }
        
        private void createSignedSeparate()
        {
            //element = new SignedSeparate(name, level, occurs, picture, signPosition);
        }
        
        private void createDecimal()
        {
            //element = new Decimal(name, level, occurs, picture, signPosition);
            element = new Zoned(name, level, occurs, picture, SignPosition);
        }
        
        private void createAlphaNumeric()
        {
            element = new AlphaNumeric(name, level, occurs, picture);
        }
        
        private void createSingleFloat()
        {
            //element = new Floating(name, level, occurs, Conversion.SINGLE);
        }
        
        private void createDoubleFloat()
        {
            //element = new Floating(name, level, occurs, Conversion.DOUBLE);
        }
        
        public Element getElement()
        {
            if (element == null) 
                createElement();
            
            return element;
        }
    }

}