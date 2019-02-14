using System;
using System.Text;
using System.IO;

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

        //public static Copybook parse(String name, StreamReader reader) {
        public static Group parse(String name, StreamReader reader) {

            return parse(Settings.Default(), name, reader);
        }

        //public static Copybook parse(Settings settings, String name, Stream stream)
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
        //public static Copybook parse(Settings settings, String name, StreamReader reader)
        public static Group parse(Settings settings, String name, StreamReader reader)
        {        
            String preProcessed = CobolPreprocessor.preProcess(reader, settings);
            StringReader sr = new StringReader(preProcessed);

            //PushbackReader pbr = new PushbackReader(sr, 1000);
            //Lexer lexer = debug ? new DebugLexer(pbr) : new Lexer(pbr);
            
            //var lexer = new Lexer(pbr);
            var lexer = new Lexer(sr);

            Parser parser = new Parser(lexer);

            CopybookAnalyzer copyBookAnalyzer = new CopybookAnalyzer(name, parser, settings);
            Start ast;
            try {
                ast = parser.Parse();
            } catch (Exception e) when (e is ParserException || e is LexerException || e is IOException)
            {
                throw new Exception("fatal parse error\n"
                    //+ (lexer is DebugLexer 
                    //? "=== buffer dump start ===\n" + ((DebugLexer) lexer).getBuffer() + "\n=== buffer dump end ===" 
                    //: "") 
                    , e);
            }
            ast.Apply(copyBookAnalyzer);
            copyBookAnalyzer.document.walkTree(copyBookAnalyzer.document);
            
            return copyBookAnalyzer.getDocument();
        }
    }

    public static class CopybookParserExtend
    {
        
        public static void walkTree(this Item item, Item document)
        {
            //item.getElement().settings = document.getElement().settings;
            
            //for (Iterator<?> i = item.children.iterator(); i.hasNext();) {
            
            Console.WriteLine("walkTree " + item.name);
            
            Element previous = null;
            foreach (var child in item.children)
            {
                Console.WriteLine(item.name + " ==> " + child.name);
                //item.getElement().addChild(child.getElement());

                //child.getElement().Parent = item.getElement();
                //item.getElement().Children.Add(child.getElement());

                child.getElement().Parent = (Group) item.getElement();
                child.getElement().Previous = child.previous.getElement();
                child.walkTree(document);

                previous = child.getElement();
            }
        }
    }
}

