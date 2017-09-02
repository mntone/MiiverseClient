using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mntone.MiiverseClient.Entities.Response
{
    public sealed class DrawingResponse
    {
        internal DrawingResponse(double nextPageUrl, IReadOnlyList<Post.Post> posts)
        {
            NextPageTimestamp = nextPageUrl;
            Posts = posts;
        }

        public double NextPageTimestamp { get; }

        public IReadOnlyList<Post.Post> Posts { get; }
    }
}
