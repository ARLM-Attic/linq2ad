/*
 * LINQ to Active Directory
 * http://www.codeplex.com/LINQtoAD
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoAD/Project/License.aspx for more information.
 */

/*
 * Running the demo:
 * - Set up an Active Directory domain *test* environment.
 * - Change the Program.ROOT static variable with the proper LDAP path and authentication information.
 */

#region Namespace imports

using System;
using System.DirectoryServices;
using System.Linq;
using BdsSoft.DirectoryServices.Linq;

#endregion

namespace Demo
{
    class Program
    {
        static DirectoryEntry ROOT = new DirectoryEntry("LDAP://localhost");

        static void Main(string[] args)
        {
            var users = new DirectorySource<User>(ROOT, SearchScope.Subtree);
            users.Log = Console.Out;
            var groups = new DirectorySource<Group>(ROOT, SearchScope.Subtree);
            groups.Log = Console.Out;

            //
            // Simple query with all-property projection (usr => usr).
            // I.e. users.Select(usr => usr);
            //
            var res1 = from usr in users
                       select usr;

            Console.WriteLine("QUERY 1\n=======");
            foreach (var w in res1)
                Console.WriteLine("{0}: {1} {2}", w.Name, w.Description, w.PasswordLastSet);
            Console.WriteLine();

            //
            // Query with selection criterion.
            // I.e. users.Where(usr => usr.Name == "A*").Select(usr => usr);
            //
            var res2 = from usr in users
                       where usr.Name == "A*"
                       select usr;

            Console.WriteLine("QUERY 2\n=======");
            foreach (var w in res2)
                Console.WriteLine("{0}'s full name is {1}", w.Name, w.Dn);
            Console.WriteLine();

            //
            // Query with selection criterion using a local variable and calling a method.
            // Uses complicated string matching and projection using an anonymous type and a local variable.
            // I.e. users.Where(usr => usr.Name == GetQueryStartWith("A")
            //                  && usr.Description.Contains("Built-in"))
            //           .Select(usr => new { usr.Name, usr.Description, usr.Groups });
            //
            var res3 = from usr in users
                       where usr.Name == GetQueryStartWith("A") && usr.Description.Contains("Built-in")
                       select new { usr.Name, usr.Description, usr.Groups, usr.LogonCount };

            Console.WriteLine("QUERY 3\n=======");
            foreach (var w in res3)
            {
                Console.WriteLine("{0} has logged on {2} times and belongs to {1} groups:", w.Name, w.Groups.Length, w.LogonCount);
                foreach (string group in w.Groups)
                    Console.WriteLine("- {0}", group);
            }
            Console.WriteLine();

            //
            // Query with selection criterion using a local variable involved in a larger expression.
            // Uses nested objects with anonymous types and expressions in initialization logic.
            // Illustrates the LDAP query construction based on tree traversal.
            // I.e. users.Where(usr => usr.Name.StartsWith("A") && usr.LogonCount > 1 * n) || usr.Name == "Guest")
            //           .Select(usr => new { usr.Name, usr.Description, usr.Dn, usr.PasswordLastSet,
            //                                Stats = new { usr.PasswordLastSet, usr.LogonCount, TwiceLogonCount = usr.LogonCount * 2 } });
            //
            var res4 = from usr in users
                       where (usr.Name.StartsWith("A") && usr.Name.EndsWith("strator")) || usr.Name == "Guest"
                       select new { usr.Name, usr.Description, usr.Dn, Stats = new { usr.PasswordLastSet, usr.LogonCount, TwiceLogonCount = usr.LogonCount * 2 } };

            Console.WriteLine("QUERY 4\n=======");
            foreach (var w in res4)
                Console.WriteLine("{0} has been logged on {1} times; password last set on {2}", w.Name, w.Stats.TwiceLogonCount - w.Stats.LogonCount, w.Stats.PasswordLastSet);
            Console.WriteLine();

            //
            // Query with sorting (not supported currently).
            // I.e. users.OrderBy(usr => usr.Name).Select(usr => usr);
            //
            var res5 = (from usr in users
                       //orderby usr.Name ascending //not supported in LDAP; alternative in-memory sort
                       select usr).AsEnumerable().OrderBy(usr => usr.Name);

            Console.WriteLine("QUERY 5\n=======");
            foreach (var w in res5)
                Console.WriteLine("{0}: {1}", w.Name, w.Description);
            Console.WriteLine();

            //
            // Query against groups in AD.
            // I.e. groups.Where(grp => grp.Name.EndsWith("ators")).Select(grp => new { grp.Name, MemberCount = grp.Members.Length });
            //
            var res6 = from grp in groups
                       where grp.Name.EndsWith("ators")
                       select new { grp.Name, MemberCount = grp.Members.Length };

            Console.WriteLine("QUERY 6\n=======");
            foreach (var w in res6)
                Console.WriteLine("{0} has {1} members", w.Name, w.MemberCount);
            Console.WriteLine();

            var myusers = new DirectorySource<MyUser>(ROOT.Children.Find("OU=Demo"), SearchScope.Subtree);
            myusers.Log = Console.Out;

            //
            // Query with update functionality using an entity MyUser : DirectoryEntity.
            //
            string oldOffice = "Test";
            string newOffice = "Demo";

            var res7 = from usr in myusers
                       where usr.Office == oldOffice
                       select usr;

            Console.WriteLine("QUERY 7\n=======");
            foreach (var u in res7)
            {
                Console.WriteLine("{0} {1} works in {2}", u.FirstName, u.LastName, u.Office);
                u.Office = newOffice;
            }
            Console.WriteLine();

            Console.WriteLine("Moving people to new office {0}...\n", newOffice);
            myusers.Update();

            int k = 0;
            foreach (var u in res7) //should be empty now
                Console.WriteLine("{0} {1} still works in {2}", u.FirstName, u.LastName, u.Office, k++);
            if (k == 0)
                Console.WriteLine("No results returned."); //expected case
            Console.WriteLine();

            var res7bis = from usr in myusers
                          where usr.Office == newOffice
                          select usr;

            foreach (var u in res7bis)
            {
                Console.WriteLine("{0} {1} now works in {2}", u.FirstName, u.LastName, u.Office);
                u.Office = oldOffice; //revert for next test run
            }
            Console.WriteLine();

            myusers.Update();

            //
            // Query with method call functionality using an entity MyUser : DirectoryEntity and a method SetPassword.
            //
            var res8 = from usr in myusers
                       select usr;

            string newPassword = "Hello W0rld!";

            Console.WriteLine("QUERY 8\n=======");
            foreach (var u in res8)
            {
                Console.Write("Setting the password of {0} {1}... ", u.FirstName, u.LastName);
                if (u.SetPassword(newPassword))
                {
                    Console.WriteLine("Done.");
                    Console.Write("Validating password... ");
                    try
                    {
                        new DirectoryEntry(ROOT.Path, u.AccountName, newPassword).RefreshCache();
                        Console.WriteLine("Successful.");

                        u.AccountExpirationDate = DateTime.Now.AddDays(30);
                    }
                    catch (DirectoryServicesCOMException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }
                }
                else
                    Console.WriteLine("Failed.");
            }
            Console.WriteLine();

            myusers.Update();
        }

        static string GetQueryStartWith(string s)
        {
            return s + "*";
        }
    }
}