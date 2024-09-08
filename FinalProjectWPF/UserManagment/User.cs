using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectWPF.UserManagment
{
    internal class User
    {
        private int _id;
        private String _fullName;
        private DateTime _lastLogin;
        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                _fullName = value;
            }
        }
        public int ID
        {
            get
            {
                return _id;
            }
            private set
            {
                _id = value;
            }
        }
        public DateTime LastLogin
        {
            get
            {
                return _lastLogin;
            }
            set
            {
                _lastLogin = value;
            }
        }

        public User(string fullName, int id)
        {
            FullName = fullName;
            ID = id;
            LastLogin = DateTime.Now;
        }


    }
}
