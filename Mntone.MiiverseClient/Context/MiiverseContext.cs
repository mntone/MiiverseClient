using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Mntone.MiiverseClient.Entities.Feeling;
using Mntone.MiiverseClient.Entities.Post;
using Mntone.MiiverseClient.Managers;
using Mntone.MiiverseClient.Tools.Constants;
using Mntone.MiiverseClient.Tools.Extensions;

namespace Mntone.MiiverseClient.Context
{
	public sealed class MiiverseContext : IDisposable
	{
		private bool _isEnabled = true;

		public MiiverseContext(string userName, string clientID, string sessionValue)
		{
			this.UserName = userName;
			this.ClientID = clientID;
			var handler = new HttpClientHandler()
			{
				AllowAutoRedirect = false,
				AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
			};
			handler.CookieContainer.Add(MiiverseConstantValues.MIIVERSE_DOMAIN_URI, new Cookie("ms", sessionValue, "/", MiiverseConstantValues.MIIVERSE_DOMAIN)
			{
				Secure = true,
				HttpOnly = true,
			});
			this.Client = new HttpClient(handler, true);
		}

		public Task<ActivityResponse> GetActivityAsync()
		{
			this.AccessCheck();

			var req = new HttpRequestMessage(HttpMethod.Get, "https://miiverse.nintendo.net/activity?fragment=activityfeed");
			req.Headers.Add("X-Requested-With", "XMLHttpRequest");
			return this.Client.SendAsync(req).ToTaskOfStream().ContinueWith(stream =>
			{
				var doc = new HtmlDocument();
				doc.Load(stream.Result);

				var postsNode = doc.GetElementbyId("main-body").GetElementByClassName("post-list").ChildNodes.Where(n => n.HasClassName("post") && !n.HasClassName("none"));
				var posts = new List<Post>();
				foreach (var postNode in postsNode)
				{
					var timestampAnchorNode = postNode.GetElementByClassName("timestamp-container").FirstChild;
					var postContentNode = postNode.GetElementByClassName("body").GetElementByClassName("post-content");
					var postMetaNode = postContentNode.GetElementByClassName("post-meta");

					var id = postNode.Id.Substring(5);
					var replyCount = postMetaNode.GetElementByClassName("reply").GetElementByClassName("reply-count").GetInnerTextAsUInt32();
					var empathyCount = postMetaNode.GetElementByClassName("empathy").GetElementByClassName("empathy-count").GetInnerTextAsUInt32();
					var isPlayed = postMetaNode.GetElementsByClassName("played").Count() != 0;
					var isSpoiler = postNode.HasClassName("hidden");

					string text = null;
					Uri imageUri = null;
					var textNodes = postContentNode.GetElementsByClassName("post-content-text");
					var isImagePost = textNodes.Count() == 0;
					if (isImagePost)
					{
						imageUri = postContentNode.GetElementByClassName("post-content-memo").GetImageSource();
					}
					else
					{
						text = textNodes.Single().InnerText;
					}

					var tagType = TagType.None;
					var tagID = string.Empty;
					var tag = string.Empty;
					postContentNode.ChildNodes.MatchClassName("post-tag",
						some: n =>
						{
							var hrefText = n.GetAttributeValue("href", string.Empty);
							var questionMarkIndex = hrefText.IndexOf('?');
							var equalMarkIndex = hrefText.IndexOf('=');
							var tagTypeText = hrefText.Substring(questionMarkIndex + 1, equalMarkIndex - questionMarkIndex - 1);
							if (tagTypeText == "official_tag_id")
							{
								tagType = TagType.Official;
								tagID = hrefText.Substring(equalMarkIndex + 1);
								tag = n.InnerText;
							}
							else if (tagTypeText == "topic_tag_id")
							{
								tagType = TagType.Topic;
								tagID = hrefText.Substring(equalMarkIndex + 1);
								tag = n.InnerText;
							}
						});

					Uri screenShotUri = null;
					postContentNode.ChildNodes.MatchClassName("screenshot-container",
						some: n => screenShotUri = n.GetImageSource());

					var userNameAnchorNode = postNode.GetElementByClassName("user-name").FirstChild;
					var userName = userNameAnchorNode.GetAttributeValue("href", string.Empty).Substring(7);
					var screenName = userNameAnchorNode.InnerText;
					var userIconUri = postNode.GetElementByClassName("icon-container").GetImageSource();
					var feeling = FeelingTypeHelpers.DetectFeelingTypeFromIconUri(userIconUri);
					var normalUserIconUri = FeelingTypeHelpers.GetNormalFaceIconUri(userIconUri, feeling);

					var communityAnchorNode = postNode.GetElementByClassName("community-container").FirstChild;
					var communityIconImageNode = communityAnchorNode.GetElementByTagName("img");
					var comInfo = communityAnchorNode.GetAttributeValue("href", string.Empty).Substring(1).Split('/');
					var titleID = Convert.ToUInt64(comInfo[1]);
					var communityID = Convert.ToUInt64(comInfo[2]);
					var communityIconUri = communityAnchorNode.GetImageSource();
					var communityName = communityAnchorNode.InnerText;

					if (isImagePost)
					{
						posts.Add(new Post(
							id,
							new PostTag(tagType, tagID, tag),
							imageUri,
							replyCount,
							empathyCount,
							isPlayed,
							isSpoiler,
							screenShotUri,
							new PostUser(userName, screenName, normalUserIconUri),
							feeling,
							new PostCommunity(titleID, communityID, communityName, communityIconUri)));
					}
					else
					{
						posts.Add(new Post(
							id,
							new PostTag(tagType, tagID, tag),
							text,
							replyCount,
							empathyCount,
							isPlayed,
							isSpoiler,
							screenShotUri,
							new PostUser(userName, screenName, normalUserIconUri),
							feeling,
							new PostCommunity(titleID, communityID, communityName, communityIconUri)));
					}
				}
				return new ActivityResponse(posts);
			});
		}

		public Task SignOutAsync()
		{
			this.AccessCheck();
			return this.Client.HeadAsync(string.Format(MiiverseConstantValues.MIIVERSE_SIGN_OUT_URI_STRING, this.ClientID));
		}

		private void AccessCheck()
		{
			if (!this._isEnabled)
			{
				throw new Exception();
			}
		}

		public void Dispose()
		{
			this.Client.Dispose();
			this._isEnabled = false;
		}

		public string UserName { get; }
		public string ClientID { get; }
		public string SessionValue { get; }

		private HttpClient Client { get; }
	}
}
