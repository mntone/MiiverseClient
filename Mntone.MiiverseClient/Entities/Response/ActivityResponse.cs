using System.Collections.Generic;

namespace Mntone.MiiverseClient.Entities.Response
{
    public  class ActivityResponse
    {
        public ActivityResponse(List<Post.Post> posts)
        {
            Posts = posts;
        }

        public IReadOnlyList<Post.Post> Posts { get; set; }
    }
}