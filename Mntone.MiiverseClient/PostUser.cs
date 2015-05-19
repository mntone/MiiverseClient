using System;
using System.Runtime.Serialization;

namespace Mntone.MiiverseClient
{
	[DataContract]
	public sealed class PostUser
	{
		internal PostUser(string name, string screenName, Uri iconUri, FeelingType feeling)
		{
			this.Name = name;
			this.ScreenName = screenName;
			this.IconUri = iconUri;
			this.Feeling = feeling;
		}

		[DataMember(Name = "name")]
		public string Name { get; }

		[DataMember(Name = "screen_name")]
		public string ScreenName { get; }

		[DataMember(Name = "icon_uri")]
		public Uri IconUri { get; }

		[DataMember(Name = "feeling")]
		public FeelingType Feeling { get; }
	}
}
