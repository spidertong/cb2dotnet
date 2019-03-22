using System;
using System.Linq;

using net.sf.cb2xml.sablecc.analysis;
using net.sf.cb2xml.sablecc.node;
using net.sf.cb2xml.sablecc.parser;

namespace cb2dotnet {
    class CopybookAnalyzer : DepthFirstAdapter
    {
        public Item document;
        private Item current;
    
        private Parser parser;
        private Settings settings;

        /**
        * Creates a new instance with the given parser and
        * name
        * 
        * @param copyBookName the name to give this copybook
        * @param parser sablecc parser instance 
        */
        public CopybookAnalyzer(String copyBookName
                                , Parser parser
                                , Settings settings
                                )
        {
            document = new Item(null, settings);
            document.name = copyBookName;
            current = document;
            this.parser = parser;
            this.settings = settings;
        }

        public Group getDocument()
        {
            return (Group) document.Element;
	    }	

        /** 
         * enter copybook, set up XML DOM and root element
	     */
        public override void InARecordDescription(ARecordDescription node)
        {
            // TODO begin
        }
        
        /**
        * exit root element, save XML as file 
        */
        public override void OutARecordDescription(ARecordDescription node)
        {
            // TODO end
            base.OutARecordDescription(node);
            //walkTree(document);
        }

        /**
        * main elementary item
        * enter item, set up Item object
        */	
        public override void InAItem(AItem node)
        {
            Item prevItem = current;
            current = new Item(prevItem, settings);
            current.level = Int32.Parse(node.GetNumberNot88().ToString().Trim());
            current.name = node.GetDataNameOrFiller().ToString().Trim();
            
            if (current.level <= 77) {
                current.setParent(prevItem);
            }		
        }
        /**
        * end of parsing the element?
        */
        public override void OutAItem(AItem node)
        {
            //current.createElement();
            //Console.WriteLine($"{current.level} {current.name} occurs[{current.occurs}] picture[{current.picture}] isAlpha[{current.isAlpha}] redefines[{current.redefines}]");
        }

        public override void InARedefinesClause(ARedefinesClause node)
        {
            //String dataName = node.GetDataName().getText();
            //current.redefines = dataName;
            current.redefines = node.GetDataName().Text;
        }

        public override void InAFixedOccursFixedOrVariable(AFixedOccursFixedOrVariable node)
        {
            current.occurs = Int32.Parse(node.GetNumber().ToString().Trim());
        }

        public override void InAVariableOccursFixedOrVariable(AVariableOccursFixedOrVariable node) 
        {
            current.occurs = Int32.Parse(node.GetNumber().ToString().Trim());
            current.dependsOn = node.GetDataName().Text;
        }

        public override void InAOccursTo(AOccursTo node) 
        {
            current.minOccurs = Int32.Parse(node.GetNumber().ToString().Trim());
        }

        public override void InAPictureClause(APictureClause node)
        {
            current.picture = removeChars(node.GetCharacterString()//node.getCharacterString()
                .ToString().ToUpper()//.toUpperCase()
                , " ");
            
            //for (int i = 0; i < current.picture.length(); i++) {
            for (int i = 0; i < current.picture.Count(); i++) {
                //char c = current.picture.charAt(i);
                char c = current.picture.ElementAt(i);
                switch (c) {
                case 'A': /* alpha */
                case 'B': /* space */
                case 'X': /* number or alpha */
                case '/': /* '/' */
                case ',': /* ',' */
                    current.isAlpha = true;
                    break;
                case 'N': /* national */
                    throw new ArgumentException("national data not yet supported");
                case 'E': /* exponent */
                    throw new ArgumentException("E in picture String not yet supported");
                case 'G': /* ??? */
                    throw new ArgumentException("G in picture String not yet supported");
                case 'P': //decimal position?
                    throw new ArgumentException("P in picture String not yet supported");
                case 'C': /* CR - credit */
                case 'D': /* DR - debit */
                    /* skip R */
                    i++; 
                    break; //cb2dotnet added
                case '.': /* decimal position */
                case 'V': /* decimal position */
                case '$': /* '$' */
                case 'Z':
                case '9':
                case '0':
                case '+':
                case '-':
                case '*':
                    break;
                }
            }
        }
        
        public override void InASignClause(ASignClause node)
        {
            //if (node.getSeparateCharacter() != null) {
            if (node.GetSeparateCharacter() != null) {
                current.signSeparate = true;
            }
        }

        public override void InALeadingLeadingOrTrailing(ALeadingLeadingOrTrailing node)
        {
            //current.getSettings().setSignPosition(SignPosition.LEADING);
            //current.SignPosition = SignPosition.LEADING;
        }

        public override void InATrailingLeadingOrTrailing(ATrailingLeadingOrTrailing node)
        {
            //current.getSettings().setSignPosition(SignPosition.TRAILING);
            //current.SignPosition = SignPosition.TRAILING;
        }

        //======================= USAGE CLAUSE ==========================
        
        public override void InABinaryUsagePhrase(ABinaryUsagePhrase node) {
            current.usage = Usage.BINARY;
        }

        public override void InACompUsagePhrase(ACompUsagePhrase node) {
            current.usage = Usage.COMPUTATIONAL;
        }

        public override void InAComp1UsagePhrase(AComp1UsagePhrase node) {
            current.usage = Usage.COMPUTATIONAL_1;
        }

        public override void InAComp2UsagePhrase(AComp2UsagePhrase node) {
            current.usage = Usage.COMPUTATIONAL_2;
        }

        public override void InAComp3UsagePhrase(AComp3UsagePhrase node) {
            current.usage = Usage.COMPUTATIONAL_3;
        }

        public override void InAComp4UsagePhrase(AComp4UsagePhrase node) {
            current.usage = Usage.COMPUTATIONAL_4;
        }
        
        public override void InAComp5UsagePhrase(AComp5UsagePhrase node) {
            current.usage = Usage.COMPUTATIONAL_5;
        }

        public override void InADisplay1UsagePhrase(ADisplay1UsagePhrase node) {
            throw new ArgumentException("display-1");
        }

        public override void InAIndexUsagePhrase(AIndexUsagePhrase node) {
            throw new ArgumentException("index");
        }

        public override void InANationalUsagePhrase(ANationalUsagePhrase node) {
            throw new ArgumentException("national");
        }

        public override void InAObjectReferencePhrase(AObjectReferencePhrase node) {
            throw new ArgumentException("object-reference");//, node.GetDataName().getText());
        }
        
        public override void InAPackedDecimalUsagePhrase(APackedDecimalUsagePhrase node) {
            current.usage = Usage.PACKED_DECIMAL;
        }

        public override void InAPointerUsagePhrase(APointerUsagePhrase node) {
            throw new ArgumentException("pointer");
        }

        public override void InAProcedurePointerUsagePhrase(AProcedurePointerUsagePhrase node) {
            throw new ArgumentException("procedure-pointer");
        }
        
        public override void InAFunctionPointerUsagePhrase(AFunctionPointerUsagePhrase node) {
            throw new ArgumentException("function-pointer");
        }
        
        //	======================= 88 / VALUE CLAUSE ==========================
        
        public void caseTZeros(TZeros node) {
            //???? UNDONE
            //current.value = values.ZEROES;
        }
        
        public void caseTSpaces(TSpaces node) {
            //???? UNDONE
            //current.value = values.SPACES;
        }
        
        public void caseTHighValues(THighValues node) {
            //???? UNDONE
            //current.value = values.HIGH_VALUES;
        }
        
        public void caseTLowValues(TLowValues node) {
            //???? UNDONE
            //current.value = values.LOW_VALUES;
        }
        
        public void caseTQuotes(TQuotes node) {
            //???? UNDONE
            //current.value = values.QUOTES;
        }
        
        public void caseTNulls(TNulls node) {
            //???? UNDONE
            //current.value = values.NULLS;
        }
        
        public void caseTAlphanumericLiteral(TAlphanumericLiteral node)
        {
            //???? UNDONE
            //current.value = values.new Literal(node.getText());
        }
        
        public override void OutAValueClause(AValueClause node) {
            //???? UNDONE
            //current.value = values.new Literal(node.getLiteral().ToString().Trim());
        }
        
        // 88 LEVEL CONDITION NODE
        public override void InAValueItem(AValueItem node) {
    //		String name = node.GetDataName().getText();
    //		curItem = new Item();
    //		curItem.element = document.createElement("condition");
    //		// curItem.element.setAttribute("level", "88");
    //		curItem.element.setAttribute("name", name);
    //		prevItem.element.appendChild(curItem.element);
            throw new ArgumentException("'a value item' not yet supported");
        }
        
        public override void OutASingleLiteralSequence(ASingleLiteralSequence node) {
    //		if (node.getAll() != null) {
    //			curItem.element.setAttribute("all", "true");
    //		}
    //		Element element = document.createElement("condition");
    //		element.setAttribute("value", node.getLiteral().ToString().Trim());
    //		curItem.element.appendChild(element);
            throw new ArgumentException("'a single literal sequence' not yet supported");
        }
        
        public override void OutASequenceLiteralSequence(ASequenceLiteralSequence node) {
    //		Element element = document.createElement("condition");
    //		element.setAttribute("value", node.getLiteral().ToString().Trim());
    //		curItem.element.appendChild(element);
            throw new ArgumentException("'a sequence literal sequence' not yet supported");
        }

        public override void OutAThroughSingleLiteralSequence(AThroughSingleLiteralSequence node) {
    //		Element element = document.createElement("condition");
    //		element.setAttribute("value", node.getFrom().ToString().Trim());
    //		element.setAttribute("through", node.getTo().ToString().Trim());
    //		curItem.element.appendChild(element);
            throw new ArgumentException("'a through single literal sequence' not yet supported");
        }

        public override void OutAThroughSequenceLiteralSequence(AThroughSequenceLiteralSequence node) {
    //		Element element = document.createElement("condition");
    //		element.setAttribute("value", node.getFrom().ToString().Trim());
    //		element.setAttribute("through", node.getTo().ToString().Trim());
    //		curItem.element.appendChild(element);	
            throw new ArgumentException("'a through sequence literal sequence' not yet supported");
        }	
        
        //===============================================================================

        /*
        private String removeChars(String s, String charToRemove) {
            StringTokenizer st = new StringTokenizer(s, charToRemove, false);
            StringBuffer b = new StringBuffer();
            while (st.hasMoreElements()) {
                b.append(st.nextElement());
            }
            return b.ToString();
        }  */        private String removeChars(String s, String charToRemove) {
            return s.Replace(charToRemove, "");
        }
    }
} 