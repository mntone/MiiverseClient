using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mntone.MiiverseClient.Entities.Community;

namespace Mntone.MiiverseClient.Entities.Response
{
    public  class CommunityListResponse
    {
        public CommunityListResponse(List<Game> game)
        {
            this.Games = game;
        }
        public List<Game> Games { get; } 
    }
}
