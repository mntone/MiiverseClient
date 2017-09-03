using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mntone.MiiverseClient.Entities.Response
{
    public  class DrawingResponse
    {
        public DrawingResponse(double currentTimestamp, double nextPageUrl, IReadOnlyList<Post.Post> posts)
        {
            CurrentTimestamp = currentTimestamp;
            NextPageTimestamp = nextPageUrl;
            Posts = posts;
        }

        public double CurrentTimestamp { get; set; }

        public double NextPageTimestamp { get; set; }

        public IReadOnlyList<Post.Post> Posts { get; set; }
    }
}
