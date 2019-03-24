using System;
using System.IO;


using System.Threading;
using System.Threading.Tasks;

namespace cb2dotnet
{
class Program
{
    static void Main(string[] args)
    {

        /* 
        var source = new CancellationTokenSource();
        var token = source.Token;
        token.Register(() => {
            Console.WriteLine("Token has been canceled.");
            //token.ThrowIfCancellationRequested();
            });
        
    
        source.CancelAfter(10 *1000);
        Console.WriteLine("CancelAfter set to 10 second.");

        var t = Task.Run(() => {
            Console.WriteLine("Task.Run() is running the action");
            Thread.Sleep(3 *1000);
            Console.WriteLine("Task.Run() is going to complete");
        });

        t.Wait();
        Console.WriteLine("The line after Tt.Wait();");

        Task.
         Task.WhenAny(Task.Delay(12*1000, token));
        Console.WriteLine("The line after Task.Delay()");

        //source.Cancel();
        return; */

        Console.WriteLine("Hello World!");
        using (var stream = File.Open(args[0], FileMode.Open))
        {
            var group = CopybookParser.parse("test", stream);
            PrintElement(group, 1);

            dynamic record = new Data(group);

            
            Console.WriteLine(record.Name);
            Console.WriteLine(record.CLIRTVO_REC.Name);
            Console.WriteLine(record.CLIRTVO_REC.MESSAGE_HEADER.Name);

            record.CLIRTVO_REC.MESSAGE_HEADER.MSGIDA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Console.WriteLine(ElementExtend.BytesToHex(record.CLIRTVO_REC.MESSAGE_HEADER.MSGIDA.GetBytes()));

            string result = record.CLIRTVO_REC.MESSAGE_HEADER.MSGIDA;
            Console.WriteLine(result);

            record.CLIRTVO_REC.MESSAGE_HEADER.MSGLNG = 12345;
            Console.WriteLine(ElementExtend.BytesToHex(record.CLIRTVO_REC.MESSAGE_HEADER.MSGLNG.GetBytes()));
            decimal msglng = record.CLIRTVO_REC.MESSAGE_HEADER.MSGLNG;
            Console.WriteLine(msglng);

            record.CLIRTVO_REC.MESSAGE_HEADER.MSGCNT = -123.456;
            Console.WriteLine(ElementExtend.BytesToHex(record.CLIRTVO_REC.MESSAGE_HEADER.MSGCNT.GetBytes()));
            decimal msgcnt = record.CLIRTVO_REC.MESSAGE_HEADER.MSGCNT;
            Console.WriteLine(msgcnt);

            record.CLIRTVO_REC.MESSAGE_DATA.BGEN_XXXXX[0].BGEN_XXXXX_TRANS_NO1 = 1234;
            Console.WriteLine(ElementExtend.BytesToHex(record.CLIRTVO_REC.MESSAGE_DATA.BGEN_XXXXX[0].BGEN_XXXXX_TRANS_NO1.GetBytes()));
            decimal tran_no1 = record.CLIRTVO_REC.MESSAGE_DATA.BGEN_XXXXX[0].BGEN_XXXXX_TRANS_NO1;
            Console.WriteLine(tran_no1);


            record.CLIRTVO_REC.MESSAGE_DATA.BGEN_XXXXX[1].BGEN_XXXXX_TRANS_NO3 = -123.12;
            Console.WriteLine(ElementExtend.BytesToHex(record.CLIRTVO_REC.MESSAGE_DATA.BGEN_XXXXX[1].BGEN_XXXXX_TRANS_NO3.GetBytes()));
            decimal tran_no3 = record.CLIRTVO_REC.MESSAGE_DATA.BGEN_XXXXX[1].BGEN_XXXXX_TRANS_NO3;
            Console.WriteLine(tran_no3);
        }
    }

    static void PrintElement(Element e, int intent){

        Console.Write(e.Id.Substring(24) + ":");
        Console.Write(e.Previous != null ? e.Previous.Id.Substring(24) : "            ");
        for (int i = 0; i < intent; i++)
        {
            Console.Write("    ");    
        }
        
        //Console.WriteLine($"{e.level} {e.name} {e.GetType()} ");
        Console.WriteLine($"{e.level} {e.name} {e.GetType()} Offset[{e.Offset}] LengthOfBytes[{e.LengthOfBytes}] " + (e is Group ? $"redefines[{((Group)e).Redefines}]" : ""));

        foreach (var child in e.Children)
        {
            PrintElement(child, intent + 1);
        }    
    }
}
}
