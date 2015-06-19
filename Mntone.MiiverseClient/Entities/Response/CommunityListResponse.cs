using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mntone.MiiverseClient.Entities.Community;

namespace Mntone.MiiverseClient.Entities.Response
{
    public sealed class CommunityListResponse
    {
        internal CommunityListResponse(List<Game> game)
        {
            this.Games = game;
        }
        public List<Game> Games { get; } 
    }
}
