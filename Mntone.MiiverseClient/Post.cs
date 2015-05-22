using System;
using System.Runtime.Serialization;

namespace Mntone.MiiverseClient
{
	[DataContract]
	public sealed class Post
	{
		internal Post(string id, string text, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, PostUser user, PostCommunity community)
			: this(id, text, replyCount, empathyCount, isPlayed, isSpoiler, null, user, community)
		{ }

		internal Post(string id, string text, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, Uri screenShotUri, PostUser user, PostCommunity community)
			: this(id, null, text, replyCount, empathyCount, isPlayed, isSpoiler, screenShotUri, user, community)
		{
			this.Text = text;
			this.ImageUri = null;
		}

		internal Post(string id, PostTag tag, string text, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, PostUser user, PostCommunity community)
			: this(id, tag, replyCount, empathyCount, isPlayed, isSpoiler, null, user, community)
		{
			this.Text = text;
			this.ImageUri = null;
		}

		internal Post(string id, PostTag tag, string text, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, Uri screenShotUri, PostUser user, PostCommunity community)
			: this(id, tag, replyCount, empathyCount, isPlayed, isSpoiler, screenShotUri, user, community)
		{
			this.Text = text;
			this.ImageUri = null;
		}

		internal Post(string id, Uri imageUri, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, PostUser user, PostCommunity community)
			: this(id, imageUri, replyCount, empathyCount, isPlayed, isSpoiler, null, user, community)
		{ }

		internal Post(string id, Uri imageUri, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, Uri screenShotUri, PostUser user, PostCommunity community)
			: this(id, null, imageUri, replyCount, empathyCount, isPlayed, isSpoiler, screenShotUri, user, community)
		{
			this.Text = string.Empty;
			this.ImageUri = imageUri;
		}

		internal Post(string id, PostTag tag, Uri imageUri, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, PostUser user, PostCommunity community)
			: this(id, tag, replyCount, empathyCount, isPlayed, isSpoiler, null, user, community)
		{
			this.Text = null;
			this.ImageUri = imageUri;
		}

		internal Post(string id, PostTag tag, Uri imageUri, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, Uri screenShotUri, PostUser user, PostCommunity community)
			: this(id, tag, replyCount, empathyCount, isPlayed, isSpoiler, screenShotUri, user, community)
		{
			this.Text = null;
			this.ImageUri = imageUri;
		}

		private Post(string id, PostTag tag, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, Uri screenShotUri, PostUser user, PostCommunity community)
		{
			this.ID = id;
			this.Tag = tag;
			this.ReplyCount = replyCount;
			this.EmpathyCount = empathyCount;
			this.IsPlayed = isPlayed;
			this.IsSpoiler = isSpoiler;
			this.ScreenShotUri = screenShotUri;
			this.User = user;
			this.Community = community;
		}

		/// <summary>
		/// Post ID
		/// </summary>
		[DataMember(Name = "id")]
		public string ID { get; }

		/// <summary>
		/// Tag
		/// </summary>
		[DataMember(Name = "tag")]
		public PostTag Tag { get; }

		/// <summary>
		/// Text content
		/// </summary>
		[DataMember(Name = "text")]
		public string Text { get; }

		/// <summary>
		/// Image content
		/// </summary>
		[DataMember(Name = "image_uri")]
		public Uri ImageUri { get; }

		/// <summary>
		/// Reply count
		/// </summary>
		[DataMember(Name = "reply_count")]
		public uint ReplyCount { get; set; }

		/// <summary>
		/// Empathy count
		/// </summary>
		[DataMember(Name = "empathy_count")]
		public uint EmpathyCount { get; set; }

		/// <summary>
		/// Played or not
		/// </summary>
		public bool IsPlayed { get; }

		/// <summary>
		/// Spoiler or not.
		/// </summary>
		[DataMember(Name = "spoiler")]
		public bool IsSpoiler { get; }

		/// <summary>
		/// Screen Shot
		/// </summary>
		[DataMember(Name = "screen_shot_uri")]
		public Uri ScreenShotUri { get; }

		/// <summary>
		/// User
		/// </summary>
		[DataMember(Name = "user")]
		public PostUser User { get; }

		/// <summary>
		/// Community
		/// </summary>
		[DataMember(Name = "community")]
		public PostCommunity Community { get; }
	}
}
