using System;
using System.Runtime.Serialization;
using Mntone.MiiverseClient.Entities.Feeling;

namespace Mntone.MiiverseClient.Entities.Post
{
	[DataContract]
	public  class Post
	{
        public Post()
        {

        }

		public Post(string id, bool accept, string discussionType, DateTime time, string text, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, PostUser user, FeelingType feeling, PostCommunity community)
			: this(id, accept, discussionType, time, text, replyCount, empathyCount, isPlayed, isSpoiler, null, user, feeling, community)
		{ }

		public Post(string id, bool accept, string discussionType, DateTime time, string text, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, Uri screenShotUri, PostUser user, FeelingType feeling, PostCommunity community)
			: this(id, accept, discussionType, time, null, text, replyCount, empathyCount, isPlayed, isSpoiler, screenShotUri, user, feeling, community)
		{
			this.Text = text;
			this.ImageUri = null;
		}

		public Post(string id, bool accept, string discussionType, DateTime time, PostTag tag, string text, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, PostUser user, FeelingType feeling, PostCommunity community)
			: this(id, accept, discussionType, time, tag, replyCount, empathyCount, isPlayed, isSpoiler, null, user, feeling, community)
		{
			this.Text = text;
			this.ImageUri = null;
		}

		public Post(string id, bool accept, string discussionType, DateTime time, PostTag tag, string text, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, Uri screenShotUri, PostUser user, FeelingType feeling, PostCommunity community)
			: this(id, accept, discussionType, time, tag, replyCount, empathyCount, isPlayed, isSpoiler, screenShotUri, user, feeling, community)
		{
			this.Text = text;
			this.ImageUri = null;
		}

		public Post(string id, bool accept, string discussionType, DateTime time, Uri imageUri, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, PostUser user, FeelingType feeling, PostCommunity community)
			: this(id, accept, discussionType, time, imageUri, replyCount, empathyCount, isPlayed, isSpoiler, null, user, feeling, community)
		{ }

		public Post(string id, bool accept, string discussionType, DateTime time, Uri imageUri, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, Uri screenShotUri, PostUser user, FeelingType feeling, PostCommunity community)
			: this(id, accept, discussionType, time, null, imageUri, replyCount, empathyCount, isPlayed, isSpoiler, screenShotUri, user, feeling, community)
		{
			this.Text = string.Empty;
			this.ImageUri = imageUri;
		}

		public Post(string id, bool accept, string discussionType, DateTime time, PostTag tag, Uri imageUri, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, PostUser user, FeelingType feeling, PostCommunity community)
			: this(id, accept, discussionType, time, tag, replyCount, empathyCount, isPlayed, isSpoiler, null, user, feeling, community)
		{
			this.Text = null;
			this.ImageUri = imageUri;
		}

		public Post(string id, bool accept, string discussionType, DateTime time, PostTag tag, Uri imageUri, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, Uri screenShotUri, PostUser user, FeelingType feeling, PostCommunity community)
			: this(id, accept, discussionType, time, tag, replyCount, empathyCount, isPlayed, isSpoiler, screenShotUri, user, feeling, community)
		{
			this.Text = null;
			this.ImageUri = imageUri;
		}

		private Post(string id, bool accept, string discussionType, DateTime time, PostTag tag, uint replyCount, uint empathyCount, bool isPlayed, bool isSpoiler, Uri screenShotUri, PostUser user, FeelingType feeling, PostCommunity community)
		{
			this.ID = id;
            this.IsAcceptingResponse = accept;
            this.DiscussionType = discussionType;
            this.PostedDate = time;
			this.Tag = tag;
			this.ReplyCount = replyCount;
			this.EmpathyCount = empathyCount;
			this.IsPlayed = isPlayed;
			this.IsSpoiler = isSpoiler;
			this.ScreenShotUri = screenShotUri;
			this.User = user;
			this.Feeling = feeling;
			this.Community = community;
		}

		/// <summary>
		/// Post ID
		/// </summary>
		[DataMember(Name = "id")]
		public string ID { get; set; }

		/// <summary>
		/// Tag
		/// </summary>
		[DataMember(Name = "tag")]
		public PostTag Tag { get; set; }

		/// <summary>
		/// Text content
		/// </summary>
		[DataMember(Name = "text")]
		public string Text { get; set; }

        /// <summary>
		/// The date of the post
		/// </summary>
		[DataMember(Name = "posted_date")]
        public DateTime PostedDate { get; set; }

		/// <summary>
		/// Image content
		/// </summary>
		[DataMember(Name = "image_uri")]
		public Uri ImageUri { get; set; }

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
		public bool IsPlayed { get; set; }

		/// <summary>
		/// Spoiler or not
		/// </summary>
		[DataMember(Name = "spoiler")]
		public bool IsSpoiler { get; set; }

        /// <summary>
		/// Is Accepting Responses
		/// </summary>
		[DataMember(Name = "accepting")]
        public bool IsAcceptingResponse { get; set; }

        /// <summary>
		/// Topic Type
		/// </summary>
        [DataMember(Name = "discussion_type")]
        public string DiscussionType { get; set; }

		/// <summary>
		/// Screen Shot
		/// </summary>
		[DataMember(Name = "screen_shot_uri")]
		public Uri ScreenShotUri { get; set; }

		/// <summary>
		/// User
		/// </summary>
		[DataMember(Name = "user")]
		public PostUser User { get; set; }

		/// <summary>
		/// Feeling
		/// </summary>
		[DataMember(Name = "feeling")]
		public FeelingType Feeling { get; set; }

		/// <summary>
		/// Community
		/// </summary>
		[DataMember(Name = "community")]
		public PostCommunity Community { get; set; }
	}
}
