using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mntone.MiiverseClient.Entities.Response
{
    public  class GameResponse
    {
        public GameResponse(bool canPost, bool isFavorite, string nextPageUrl, bool before, IReadOnlyList<Post.Post> posts)
        {
            NextPageUrl = nextPageUrl;
            CanPost = canPost;
            Posts = posts;
            IsFavorite = isFavorite;
            BeforeRenewal = before;
        }

        public string NextPageUrl { get; }

        public bool CanPost { get; }

        public bool IsFavorite { get; }

        public bool BeforeRenewal { get; }

        public IReadOnlyList<Post.Post> Posts { get; }
    }

    public  class OldGameResponse
    {
        public OldGameResponse(double nextPageUrl, IReadOnlyList<Post.Post> posts)
        {
            NextPageTimestamp = nextPageUrl;
            Posts = posts;
        }

        public double NextPageTimestamp { get; }

        public IReadOnlyList<Post.Post> Posts { get; }
    }
}
