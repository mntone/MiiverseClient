using System.Runtime.Serialization;

namespace Mntone.MiiverseClient.Entities.Post
{
	[DataContract]
	public sealed class PostTag
	{
		public PostTag(TagType tagType, string tagID, string tag)
		{
			this.TagType = tagType;
			this.TagID = tagID;
			this.Tag = tag;
		}

		/// <summary>
		/// Tag Type
		/// </summary>
		[DataMember(Name = "tag_type")]
		public TagType TagType { get; }

		/// <summary>
		/// Tag ID
		/// </summary>
		[DataMember(Name = "tag_id")]
		public string TagID { get; }

		/// <summary>
		/// Tag
		/// </summary>
		[DataMember(Name = "tag")]
		public string Tag { get; }
	}
}