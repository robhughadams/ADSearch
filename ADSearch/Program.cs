using System;
using System.Linq;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices;

namespace ADSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage ADSearch <Display Name>");
                return;
            }

            var searchTerm = args[0];

            using (var currentForest = Forest.GetCurrentForest())
            using (var gc = currentForest.FindGlobalCatalog())
            using (var userSearcher = gc.GetDirectorySearcher())
            {
                userSearcher.Filter = $"(&((&(objectCategory=Person)(objectClass=User)))(DisplayName=*{searchTerm}*))";

                foreach (SearchResult searchResult in userSearcher.FindAll())
                {
                    foreach(System.Collections.DictionaryEntry property in searchResult.Properties)
                    {
                        Console.WriteLine(property.Key + ":");

                        foreach (var value in (ResultPropertyValueCollection)property.Value)
                        {
                            Console.WriteLine("\t" + value);
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
        }
    }
}
