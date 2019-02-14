
namespace cb2dotnet {

    public class Usage
    {
        private Usage() {}
        
        public static   Usage BINARY = new Usage();
        
        public static   Usage COMPUTATIONAL = new Usage(); // binary 
        
        public static   Usage COMPUTATIONAL_1 = new Usage(); // single precision float
        
        public static   Usage COMPUTATIONAL_2 = new Usage(); // double precision float
        
        public static   Usage COMPUTATIONAL_3 = new Usage(); // packed
        
        public static   Usage COMPUTATIONAL_4 = new Usage(); // binary
        
        public static   Usage COMPUTATIONAL_5 = new Usage(); // binary or comp-5
        
    //    public static   Usage DISPLAY = new Usage(); // uh, who cares?
    //    
        public static   Usage DISPLAY_1 = new Usage();
        
        public static   Usage INDEX = new Usage(); // 4 bytes
        
        public static   Usage NATIONAL = new Usage();  // not supported
        
        public static   Usage OBJECT_REFERENCE = new Usage(); // ???
        
        public static   Usage PACKED_DECIMAL = new Usage(); 
        
        public static   Usage POINTER = new Usage(); // ??? binary?  how many bytes?
        
        public static   Usage PROCEDURE_POINTER = new Usage(); // ???
        
        public static   Usage FUNCTION_POINTER = new Usage(); // ???
    }
}