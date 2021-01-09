using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using mshtml;
using System.ComponentModel;
using System.Data;
using System.Drawing;

namespace CRAWLING
{
    class Program
    {
        static int counter;
        static SqlConnection con;
        static string insert;
        static SqlCommand cmd;
        static Queue q;
        static List<string> list;
        static int NumOfThread;
        static public string eng(string input)
        {
            bool xy = false;
            int uppLimit = xy ? 255 : 127;
            char[] engChars = input.Where(c => (int)c <= uppLimit).ToArray();
            return new string(engChars);
        }
        static public string getOnlyText(string Code)
        {
            Code = Regex.Replace(Code, @"<script[^>]*>[\s\S]*?</script>", " ");
            Code = Regex.Replace(Code, @"<!--[\s\S]*?-->", " ");
            var doc_ = new HtmlAgilityPack.HtmlDocument();
            doc_.LoadHtml(Code);
            Code = doc_.DocumentNode.InnerText;
            Code = Regex.Replace(Code, @"<[^>]+>|&nbsp;", " ");
            Code = Regex.Replace(Code, @"&#[\s\S]*?;", " ");
            Code = Regex.Replace(Code, @"\s+", " ", RegexOptions.Multiline);
            return Code;
        }
        static public void PutPageInfoInDB(string link, string Content)
        {
            insert = @"insert into Pages_Content(URL,Page_Content) Values(@URL,@TEXT)";
            cmd = new SqlCommand(insert, con);
            cmd.Parameters.AddWithValue("@URL", link);
            cmd.Parameters.AddWithValue("@TEXT", Content);
            cmd.ExecuteNonQuery();


        }
        static public void func(object startLink)
        {
            String URL;
            string rString;
            string EnglishText;
            string link;
            WebRequest myWebRequest;
            WebResponse myWebResponse;
            Stream streamResponse;
            StreamReader sReader;
            HTMLDocument y = new HTMLDocument();
            IHTMLDocument2 doc = (IHTMLDocument2)y;
            IHTMLElementCollection elements;

            try
            {
                URL = (String)startLink;
                // Create a new 'WebRequest' object to the mentioned URL.
                myWebRequest = WebRequest.Create(URL);
                // The response object of 'WebRequest' is assigned to a WebResponse' variable.
                myWebResponse = myWebRequest.GetResponse();
                streamResponse = myWebResponse.GetResponseStream();
                sReader = new StreamReader(streamResponse);
                rString = sReader.ReadToEnd();
                EnglishText = getOnlyText(rString);
                EnglishText = eng(EnglishText);
                list.Add(URL);


                doc.write(rString);
                elements = doc.links;
                foreach (IHTMLElement el in elements)
                {
                    link = (string)el.getAttribute("href", 0);
                    if (!link.Contains("about:"))
                    {
                        if (list.Count + q.Count <= 3000)
                        {
                            if (!list.Contains(link))
                            {
                                if (!q.Contains(link))
                                {
                                    q.Enqueue(link);
                                }
                            }
                        }



                    }

                }
                PutPageInfoInDB(URL, EnglishText);
                counter++;
                Console.Write(counter);
                Console.WriteLine(" :  " + URL);
                streamResponse.Close();
                sReader.Close();
                myWebResponse.Close();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ex message : " + ex.Message);
                if (ex is OverflowException)
                {
                    list.Clear();
                    q.Clear();
                    q.Enqueue("https://zookeys.pensoft.net/");
                    
                }
            }
            NumOfThread--;

        }
        static public void Clean()
        {
            SqlConnection con1 = new SqlConnection("Data Source=AHMED;Initial Catalog=Information_Retrieval_Project;Integrated Security=True");
            con.Open();
            SqlConnection con2 = new SqlConnection("Data Source=AHMED;Initial Catalog=SearchEngineSystem;Integrated Security=True");
            con2.Open();
            SqlCommand cmd1 = new SqlCommand(" select * from Pages_Content where URL like ('http'+'%'+'://en.'+'%') And Page_Content not like (' User'+'%') ", con1);
            cmd1.CommandType = CommandType.Text;
            SqlDataReader reader = cmd1.ExecuteReader();
            //------------------------------------------------------------------------
            string insert = @"insert into EnglishPages(Id,Link,PageContent)
            values(@id,@link,@c)";


            //---------------------------------------------------------------------------
            int counter = 0;
            while (reader.Read())
            {
                if (counter == 1500) break;
                try
                {
                    SqlCommand cmd2 = new SqlCommand(insert, con2);
                    SqlParameter id = new SqlParameter("@id", ++counter);
                    cmd2.Parameters.Add(id);
                    Console.WriteLine((String)reader[1]);

                    SqlParameter link = new SqlParameter("@link", (String)reader[1]);
                    cmd2.Parameters.Add(link);
                    SqlParameter pc = new SqlParameter("@c", (string)reader[2]);
                    cmd2.Parameters.Add(pc);

                    cmd2.ExecuteNonQuery();
                    Thread.Sleep(100);
                }
                catch (Exception a) { Console.WriteLine(counter); counter--; }


            }
            reader.Close();
            con1.Close();
            con2.Close();
            Console.WriteLine("Done");
            Console.WriteLine("counter");
        }
        static void Main(string[] args)
        {
            con= new SqlConnection("Data Source=AHMED;Initial Catalog=Information_Retrieval_Project;Integrated Security=True");
            con.Open();
            q = new Queue();
            counter = 0;
            NumOfThread=0;
            list = new List<string>();
            q.Enqueue(" https://en.wikipedia.org/wiki/Special:Statistics");
            q.Enqueue("https://en.wikipedia.org/wiki/Malay_language");
            q.Enqueue("http://www.radioaustralia.net.au/tokpisin/2012-03-19/king-blong-tonga-i-dai-pinis/427272");
            q.Enqueue("https://www.bbc.com");
            while (true)
            {
                try
                {
                    if (NumOfThread<10)
                    {
                        NumOfThread++;
                       Thread th = new Thread(new ParameterizedThreadStart(func));
                       th.Start(q.Dequeue());
                    }
                    
                }
                catch (Exception ex)
                {
                    Thread.Sleep(500);
                }
            }
            con.Close();
            Clean();

            
        }



    }
}
