using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Gracenote
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var sr = new StreamReader (@"C:\Users\rod\Documents\projects\Becoming Gracenote\Becoming Gracenote\test.txt"))
            {
                //var tokenized = new Tokenized (sr.ReadToEnd ());

                //foreach (var token in tokenized)
                //    Console.WriteLine (token);


                var parsed = new Parsed (sr.ReadToEnd ());



                var sw = new StreamWriter (@"c:\users\rod\desktop\gracenote_test.html", false, Encoding.UTF8);

                foreach (var expression in parsed)
                {
                    //Console.WriteLine (expression.GenerateCode ());

                    sw.WriteLine (Encoding.UTF8.GetString (Encoding.UTF8.GetBytes (expression.GenerateCode ())));
                }

                sw.Close ();

                System.Diagnostics.Process.Start (new System.Diagnostics.ProcessStartInfo ("iexplore.exe", @"c:\users\rod\desktop\gracenote_test.html"));
            }

            //var xs = new[] { 2, 4, 6, 8, 10 };

            //var index = new Indexical<int> (xs);

            //while (index)
            //{
            //    Console.WriteLine (index.Value);
            //    index++;
            //}

            //index = index - 2;

            ////Console.WriteLine (index.Value);

            //foreach (var x in index)
            //    Console.WriteLine (x);


            //var xs = Enumerable.Range (1, 100);

            //var index = new Indexical<int> (xs.ToArray());

            //var old = Console.ForegroundColor;

            //while (index)
            //{
            //    Console.WriteLine (index.Value);

            //    Console.WriteLine ((index + 30).IsValid);                

            //    index++;
            //}

            //Console.ReadLine ();


            //var text = "\"Today, August \\26th, is the *best* day of the rest of our lives\" \r\n --me";

            //var tokenized = new TokenizedSpans (text);

            ////foreach (var token in tokenized)
            ////    Console.WriteLine (token);


            ////text = "(__hello world__) __&__ \"world\" **sdfsdf**";

            //var parsed = new ParsedTextRun (text);

            //var code = parsed.GenerateCode ();

            //var sw = new StreamWriter (@"c:\users\rod\desktop\gracenote.html");

            //sw.Write (code);

            //sw.Close ();
            
            
            //Console.WriteLine (code);
           
           // Console.ReadLine ();
        }
    }
}
