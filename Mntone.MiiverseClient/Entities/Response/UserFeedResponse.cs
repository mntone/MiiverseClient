using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mntone.MiiverseClient.Entities.Response
{
    public sealed class UserFeedResponse
    {
        internal UserFeedResponse(IReadOnlyList<Post.Post> posts)
        {
            Posts = posts;
        }

        public IReadOnlyList<Post.Post> Posts { get; }
    }
}
