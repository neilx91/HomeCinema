using HomeCinema.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HomeCinema.Services.Utilities
{
    public class MembershipContext
    {
        public IPrincipal Principle { get; set; }
        public User MyProperty { get; set; }
        public bool IsValid()
        {
            return Principle != null;
        }
    }
}
