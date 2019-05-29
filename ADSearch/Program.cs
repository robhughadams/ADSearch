using System;
using System.Linq;
using System.DirectoryServices.AccountManagement;

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

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domainName))
            {
                using (UserPrincipal user = new UserPrincipal(context))
                {
                    user.DisplayName = $"*{searchTerm}*";
                    var pS = new PrincipalSearcher
                    {
                        QueryFilter = user
                    };

                    var results = pS.FindAll();

                    foreach (var pc in results)
                    {
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
    }
}
