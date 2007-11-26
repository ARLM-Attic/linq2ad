/*
 * LINQ to Active Directory
 * http://www.codeplex.com/LINQtoAD
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoAD/Project/License.aspx for more information.
 */

#region Namespace imports

using System;
using ActiveDs;
using BdsSoft.DirectoryServices.Linq;

#endregion

namespace Demo
{
    [DirectorySchema("user", typeof(IADsUser))]
    class User
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private int logonCount;

        public int LogonCount
        {
            get { return logonCount; }
            set { logonCount = value; }
        }

        private DateTime pwdLastSet;

        [DirectoryAttribute("PasswordLastChanged", DirectoryAttributeType.ActiveDs)]
        public DateTime PasswordLastSet
        {
            get { return pwdLastSet; }
            set { pwdLastSet = value; }
        }

        private string distinguishedName;

        [DirectoryAttribute("distinguishedName")]
        public string Dn
        {
            get { return distinguishedName; }
            set { distinguishedName = value; }
        }

        private string[] memberOf;

        [DirectoryAttribute("memberOf")]
        public string[] Groups
        {
            get { return memberOf; }
            set { memberOf = value; }
        }
    }

    [DirectorySchema("group")]
    class Group
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string[] members;

        [DirectoryAttribute("member")]
        public string[] Members
        {
            get { return members; }
            set { members = value; }
        }
    }

    [DirectorySchema("user", typeof(IADsUser))]
    class MyUser : DirectoryEntity
    {
        private DateTime expiration;

        [DirectoryAttribute("AccountExpirationDate", DirectoryAttributeType.ActiveDs)]
        public DateTime AccountExpirationDate
        {
            get { return expiration; }
            set
            {
                if (expiration != value)
                {
                    expiration = value;
                    OnPropertyChanged("AccountExpirationDate");
                }
            }
        }
	
        private string first;

        [DirectoryAttribute("givenName")]
        public string FirstName
        {
            get { return first; }
            set
            {
                if (first != value)
                {
                    first = value;
                    OnPropertyChanged("FirstName");
                }
            }
        }

        private string last;

        [DirectoryAttribute("sn")]
        public string LastName
        {
            get { return last; }
            set
            {
                if (last != value)
                {
                    last = value;
                    OnPropertyChanged("LastName");
                }
            }
        }

        private string office;

        [DirectoryAttribute("physicalDeliveryOfficeName")]
        public string Office
        {
            get { return office; }
            set
            {
                if (office != value)
                {
                    office = value;
                    OnPropertyChanged("Office");
                }
            }
        }

        private string accoutName;

        [DirectoryAttribute("sAMAccountName")]
        public string AccountName
        {
            get { return accoutName; }
            set
            {
                if (accoutName != value)
                {
                    accoutName = value;
                    OnPropertyChanged("AccountName");
                }
            }
        }

        public bool SetPassword(string password)
        {
            return this.DirectoryEntry.Invoke("SetPassword", new object[] { password }) == null;
        }
    }
}
