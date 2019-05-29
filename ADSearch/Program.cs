using System;
using System.Linq;
using System.DirectoryServices.AccountManagement;

namespace ADSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            var searchTerm = args[0];

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                using (UserPrincipal user = new UserPrincipal(context))
                {
                    user.DisplayName = $"*{searchTerm}*";
                    var pS = new PrincipalSearcher
                    {
                        QueryFilter = user
                    };

                    //Perform the search
                    var results = pS.FindAll();

                    //If necessary, request more details
                    var pc = results.First();

                    Console.WriteLine("Sam Account Name: {0}", pc.SamAccountName);

                    Console.WriteLine("AD Groups:");
                    var groups = pc.GetGroups();
                    foreach (var g in groups)
                    {
                        Console.WriteLine("{0}\t{1}", g.SamAccountName, g.DisplayName);
                    }
                }
            }
        }
    }
}
