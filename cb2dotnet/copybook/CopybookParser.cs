using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using net.sf.cb2xml.sablecc.lexer;
using net.sf.cb2xml.sablecc.parser;
using net.sf.cb2xml.sablecc.node;

namespace cb2dotnet {
    /**
    * 
    * This class is the starting point for parsing copybooks
    * 
    * <p>To parse or create data, you need to first parse the
    * copybook.  The returned CopyBook instance will allow for 
    * working with data.
    */
    public class CopybookParser
    {
        
        private static bool debug = false;
        
        /**
        * Parses a copybook definition and returns a Copybook instance
        * 
        * @param name the name of the copybook.  For future use.
        * @param stream the copybook definition's source stream
        * 
        * @return a copybook instance containing the parse tree for the definition
        */
        
        //public static Copybook parse(String name, Stream stream)
        public static Group parse(String name, Stream stream)
        {        
            return parse(name, new StreamReader(stream));
        } 

        public static Group parse(String name, StreamReader reader) {

            return parse(Settings.Default(), name, reader);
        }

        public static Group parse(Settings settings, String name, Stream stream)
        {
            return parse(settings, name, new StreamReader(stream));
        }

        /**
        * Parses a copybook definition and returns a Copybook instance
        * 
        * @param name the name of the copybook.  For future use.
        * @param reader the copybook definition's source reader
        * 
        * @return a copybook instance containing the parse tree for the definition
        */
        public static Group parse(Settings settings, String name, StreamReader reader)
        {        
            String preProcessed = CobolPreprocessor.preProcess(reader, settings);
            StringReader sr = new StringReader(preProcessed);

            var lexer = new Lexer(sr);

            Parser parser = new Parser(lexer);

            CopybookAnalyzer copyBookAnalyzer = new CopybookAnalyzer(name, parser, settings);
            Start ast;
            try {
                ast = parser.Parse();
            } catch (Exception e) when (e is ParserException || e is LexerException || e is IOException) {
                throw new Exception("fatal parse error\n", e);
            }
            ast.Apply(copyBookAnalyzer);
            copyBookAnalyzer.document.walkTree(copyBookAnalyzer.document);

            var doc = copyBookAnalyzer.getDocument();
            doc.walkOccurs(doc);
            
            return copyBookAnalyzer.getDocument();
        }
    }

    public static class CopybookParserExtend
    {
        public static void walkTree(this Item item, Item document)
        {            
            Element previousElement = null;
            foreach (var childItem in item.children)
            {
                //childItem.Element.Parent = (Group) item.Element;
                item.Element.AddChild(childItem.Element);
                childItem.Element.Previous = childItem.previous.Element;
                childItem.walkTree(document);

                previousElement = childItem.Element;
            }
        }

        public static Element walkOccurs(this Element current, Element end){

            Console.WriteLine($"walkOccurs {current.name}");

            var children = current.Children.ToArray();
            foreach (var child in children)
            {
                end = child.walkOccurs(end);
            }

            Console.WriteLine($"walkOccurs {current.name} completed foreach.");
            if (current.occurs < 2){
                return current;
            }

            Console.WriteLine($"walkOccurs {current.name} start copying occurs.");
            Element clone = current;
            for (int i = 1; i < current.occurs; i++){
                Console.WriteLine($"{i}");
                clone = clone.CopyToNext(current.Parent);
                clone.occursIndex = 1 + i;
                Console.WriteLine($"{i} completed");
            }
            end = clone.GetLastChildren();

            if (clone.Parent == null)
            {
                Console.WriteLine($"{clone.name}");
            }

            int index = clone.Parent.Children.IndexOf(clone);
            var next = clone.Parent == null ? null : clone.Parent.Children.ElementAtOrDefault(index + 1);
            if (next != null){
                next.Previous = end;
            }
            return end;
        }

        public static Element GetLastChildren(this Element current){
            return current == null              ? current
                 : current.Children.Count == 0  ? current
                 : current.Children.Last().GetLastChildren();
        }
    }
}

