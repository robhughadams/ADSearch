using System;
using System.Linq;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;

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
            var domainName = args.Length > 1 ? args[1] : null;

            if (args.Any(a => a == "-ds"))
            {
                FindUserWithDirectorySearcher(searchTerm, domainName);
                return;
            }

            FindUserWithPrincipalSearcher(searchTerm, domainName);
        }

        private static void FindUserWithPrincipalSearcher(string searchTerm, string domainName)
        {
            using (var context = new PrincipalContext(ContextType.Domain, domainName))
            {
                using (var user = new UserPrincipal(context))
                {
                    user.DisplayName = $"*{searchTerm}*";
                    var pS = new PrincipalSearcher
                    {
                        QueryFilter = user
                    };

                    var results = pS.FindAll();

                    foreach (var pc in results)
                    {
                        Console.WriteLine("Display Name: {0}", pc.DisplayName);
                        Console.WriteLine();

                        Console.WriteLine("Sam Account Name: {0}", pc.SamAccountName);
                        Console.WriteLine();

                        Console.WriteLine("AD Groups:");
                        var groups = pc.GetGroups();
                        foreach (var g in groups)
                        {
                            Console.WriteLine("{0}\t{1}", g.SamAccountName, g.DisplayName);
                        }

                        Console.WriteLine();
                        Console.WriteLine();
                    }
                }
            }
        }

        private static void FindUserWithDirectorySearcher(string searchTerm, string domainName)
        {
            var context = new DirectoryContext(DirectoryContextType.Forest, domainName);

            using (var currentForest = Forest.GetForest(context)) //Forest.GetCurrentForest())
            using (var gc = currentForest.FindGlobalCatalog())
            using (var directorySearcher = gc.GetDirectorySearcher())
            {
                //directorySearcher.Filter = "(objectCategory=msExchExchangeServer)";
                directorySearcher.Filter = $"(&((&(objectCategory=Person)(objectClass=User)))(DisplayName=*{searchTerm}*))";

                foreach (SearchResult searchResult in directorySearcher.FindAll())
                {
                    //const string propertyName = "name";
                    //Console.WriteLine(propertyName + ":");
                    //foreach (var value in searchResult.Properties[propertyName])
                    //{
                    //    Console.WriteLine("\t" + value);
                    //}


                    foreach (System.Collections.DictionaryEntry property in searchResult.Properties)
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
