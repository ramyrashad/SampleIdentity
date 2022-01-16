using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Entities.ApplicationUserAggregate
{
    public class ApplicationUser : IdentityUser<string>
    {
        #region Constants

        #endregion

        #region Constructors

        private ApplicationUser()
        {

        }

        public ApplicationUser(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;    
        }


        #endregion

        #region Members

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public DateTime? Birthdate { get; private set; }

        #endregion

        #region Public Methods



        #endregion



    }
}
