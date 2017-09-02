using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mntone.MiiverseClient.Entities.Response
{
    public  class UserProfileResponse
    {
        public UserProfileResponse(User.User user)
        {
            User = user;
        }

        public User.User User { get; }
    }
}
