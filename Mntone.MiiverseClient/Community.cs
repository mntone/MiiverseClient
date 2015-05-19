using System;
using System.Runtime.Serialization;

namespace Mntone.MiiverseClient
{
	public sealed class Community
	{
		internal Community(ulong titleID, ulong id, string name, Uri iconUri)
		{
			this.TitleID = titleID;
			this.ID = id;
			this.Name = name;
			this.IconUri = iconUri;
		}

		/// <summary>
		/// Title ID
		/// </summary>
		[DataMember(Name = "title_id")]
		public ulong TitleID { get; }

		/// <summary>
		/// Community ID
		/// </summary>
		[DataMember(Name = "id")]
		public ulong ID { get; }

		/// <summary>
		/// Community Name
		/// </summary>
		[DataMember(Name = "name")]
		public string Name { get; }

		/// <summary>
		/// Community Icon Uri
		/// </summary>
		[DataMember(Name = "icon_uri")]
		public Uri IconUri { get; }
	}
}
