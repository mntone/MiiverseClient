using System.Collections.Generic;
using Mntone.MiiverseClient.Entities.Post;

namespace Mntone.MiiverseClient.Managers
{
	public sealed class ActivityResponse
	{
		internal ActivityResponse( List<Post> posts)
		{
			this.Posts = posts;
		}

		public IReadOnlyList<Post> Posts { get; }
	}
}
