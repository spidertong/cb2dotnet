using System;
using System.Text;
using System.IO;
using System.Linq;

/**
 * Very simple COBOL pre-processor that chops the left and right margins. Column
 * start and end positions are configurable using a properties file. Linefeeds
 * are retained as these are required by the main parser. COBOL files typically
 * contain some junk characters and comment indicators in the "margins" and this
 * routine removes those.

 */


namespace cb2dotnet{
    public class CobolPreprocessor {

        private CobolPreprocessor() {
        }

        public static String preProcess(StreamReader reader, Settings settings) {
            
            // TODO: figure out a way to pass copybook specific settings for non-default margins treated as comment.
            int columnStart = settings.ColumnStart;
            int columnEnd = settings.ColumnEnd;

            var sb = new StringBuilder();

                String s = null;
                while ((s = reader.ReadLine()) != null) {
                    if (s.Length > columnStart) {
                        int thisColumnStart = columnStart;
                        if (s.ElementAt(columnStart) == '/') {
                            sb.Append('*');
                            thisColumnStart++;
                        }
                        if (s.Length < columnEnd) {
                            sb.Append(s.Substring(thisColumnStart));
                        } else {
                            sb.Append(s.Substring(thisColumnStart, columnEnd));
                        }
                    }
                    sb.Append("\n");
                }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return sb.ToString();
        }
    }
}