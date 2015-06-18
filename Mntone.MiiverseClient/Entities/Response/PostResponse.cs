
namespace Mntone.MiiverseClient.Entities.Response
{
    public sealed class PostResponse
    {
        internal PostResponse(Post.Post post)
        {
            this.Post = post;
        }

        public Post.Post Post { get; }
    }
}
