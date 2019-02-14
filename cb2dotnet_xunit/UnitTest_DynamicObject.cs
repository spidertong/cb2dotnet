using System;
using System.Dynamic;
using System.Text;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace cb2dotnet_xunit
{
    public class UnitTest_DynamicObject
    {
        public static  StringBuilder output;

        [Fact]
        public void Test_TryGetMember_TryConvert()
        {
            output = new StringBuilder();    

            dynamic data = new Data();        
            string abc = data.abc;

            var result = output.ToString().Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries);
            Assert.True(result[0].Contains("TryGetMember was involved") );
            Assert.True(result[1].Contains("TryConvert was involved") );
        }

        [Fact]
        public void Test_TrySetMember()
        {
            output = new StringBuilder();    

            dynamic data = new Data(); 
            data.abc.def = "abc";
            var result = output.ToString().Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries);
            Assert.True(result[0].Contains("TryGetMember was involved") );
            Assert.True(result[1].Contains("TrySetMember was involved") );
        }
    }

    internal class Data : DynamicObject{
        public override bool TryGetMember(GetMemberBinder binder, out object result){
            UnitTest_DynamicObject
                .output
                .AppendLine("==========TryGetMember was involved.==========");
            result = this;
            return true;
        }

        public override bool TryConvert(ConvertBinder binder, out object result){
            UnitTest_DynamicObject
                .output
                .AppendLine("==========TryConvert was involved.==========");
            result = null;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value){
            UnitTest_DynamicObject
                .output
                .AppendLine("==========TrySetMember was involved.==========");
            return true;
        }
    }
}
