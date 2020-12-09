using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

/*
 Aktif olarak thread'ler ile çalışmadığım ve yoğun takvim nedeniyle thread kısmı ve düzen tam istediğim gibi olmasa da kodum aşağıdaki gibidir.
     
     Cem Özkan
     */

namespace KelimeCumleSayac
{
    public static class Program
    {
        public static List<String> threadIdCount = new List<string>();
        public static Dictionary<string, int> liste = new Dictionary<string, int>();
        public static string[] sentences;
        public static int counter = 0;
        public static int threadCount = 5;

        public static string StripPunctuation(this string s)
        {
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public static void countIt()
        {
            if (counter < sentences.Length)
            {
                string[] words = sentences[counter].Split(' ');
                counter++;
                for (int i = 0; i < words.Length; i++)
                {
                    words[i] = words[i].StripPunctuation(); 
                    if (liste.ContainsKey(words[i]))
                    {
                        int sayi = liste[words[i]];
                        liste[words[i]] = sayi + 1;
                    }
                    else
                    {
                        liste.Add(words[i], 1);
                    }
                }
            }                  
        }

        public static void printList()
        {
            threadIdCounter();
            foreach (var item in ThreadWriter())
            {
                Console.WriteLine(item);
            }

            foreach (var item in liste.OrderByDescending(key => key.Value))
            {
                Console.WriteLine("{0} {1}", item.Key,item.Value);
            }
        }

        public static void splitToSentences(string text)
        {
            sentences = Regex.Split(text, @"(?<=[\.!\?])\s+");
        }

        public static void threadStarter(int threadCount)
        {
            ThreadStart mainThread = new ThreadStart(printList);
            ThreadStart childThread = new ThreadStart(countIt);

            int counter = 0;
            while(sentences.Length > counter)
            {
                for (int i = 0; i < threadCount; i++)
                {
                    Thread child = new Thread(childThread);
                    child.Name = "" + i;
                    child.Start();
                    child.Join();
                    counter++;
                }
            }            
            Thread main = new Thread(mainThread);
            main.Start();
        }

        public static int avgWordCount()
        {
            int retVal = 0;
            foreach (var item in sentences)
            {
                string[] words = item.Split(' ');
                retVal += words.Length;
            }
            retVal /= sentences.Length;
            return retVal;
        }

        public static void threadIdCounter()
        {
            int count = 1;
            int tcount = 1;
            while(tcount <= sentences.Length)
            {
                if(count == 1 || count == 0)
                {
                    for (int i = 1; i <= threadCount ; i++)
                    {
                        if (i > sentences.Length) count = 0;
                        threadIdCount.Add("ThreadId=" + i + ", Count=" + count);
                        tcount++;
                    }
                    count++;
                }
                else
                {
                    for(int i=0;i< threadCount; i++)
                    {
                        threadIdCount[i] = threadIdCount[i].Remove(threadIdCount[i].Length - 1);
                        threadIdCount[i] += tcount;
                    }
                    tcount++;
                }                        
            }            
        }

        public static ArrayList ThreadWriter()
        {
            ArrayList printList = new ArrayList();
            printList.Add("Sentence Count : " + sentences.Length);
            printList.Add("Avg. Word Count : "+ avgWordCount());           
            printList.Add("Thread Counts : ");            
            foreach (var item in threadIdCount)
            {
                printList.Add("\t "+item);
            }
            return printList;
        }

        public static void Main(string[] args)
        {
            
            Console.WriteLine("Enter thread count: ");
            threadCount = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Please enter the URL: ");
            string url = Console.ReadLine();

            string text = System.IO.File.ReadAllText(url); // read txt file

            splitToSentences(text);

            threadStarter(threadCount);

            Console.ReadLine(); 
        }
    }
}
