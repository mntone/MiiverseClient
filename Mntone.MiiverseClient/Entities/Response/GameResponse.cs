using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mntone.MiiverseClient.Entities.Response
{
    public sealed class GameResponse
    {
        internal GameResponse(bool canPost, bool isFavorite, string nextPageUrl, IReadOnlyList<Post.Post> posts)
        {
            NextPageUrl = nextPageUrl;
            CanPost = canPost;
            Posts = posts;
            IsFavorite = isFavorite;
        }

        public string NextPageUrl { get; }

        public bool CanPost { get; }

        public bool IsFavorite { get; }

        public IReadOnlyList<Post.Post> Posts { get; }
    }
}
