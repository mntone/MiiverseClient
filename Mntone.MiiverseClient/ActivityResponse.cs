using System.Collections.Generic;

namespace Mntone.MiiverseClient
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
