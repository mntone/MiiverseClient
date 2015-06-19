using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Mntone.MiiverseClient.Entities.Feeling;
using Mntone.MiiverseClient.Entities.Post;
using Mntone.MiiverseClient.Entities.Response;
using Mntone.MiiverseClient.Entities.User;
using Mntone.MiiverseClient.Tools.Constants;
using Mntone.MiiverseClient.Tools.Extensions;

namespace Mntone.MiiverseClient.Context
{
	public sealed class MiiverseContext : IDisposable
	{
		private bool _isEnabled = true;

		public MiiverseContext(string userName, string clientID, string sessionValue)
		{
			UserName = userName;
			ClientID = clientID;
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
			Client = new HttpClient(handler, true);
		}

	    public Task<UserProfileResponse> GetUserProfileAsync(string username)
	    {
            AccessCheck();

            var req = new HttpRequestMessage(HttpMethod.Get, "https://miiverse.nintendo.net/users/" + username);
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            return Client.SendAsync(req).ToTaskOfStream().ContinueWith(stream =>
            {
                var doc = new HtmlDocument();
                doc.Load(stream.Result);
                var mainNode = doc.GetElementbyId("main-body");
                var avatarUrlNode = mainNode.Descendants("img").FirstOrDefault();
                var avatarUri = new Uri(avatarUrlNode?.GetAttributeValue("src", string.Empty));
                var nickNameNode = mainNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty) == "nick-name");
                var nickName = nickNameNode?.InnerText;

                var userNameNode = nickNameNode?.GetElementByClassName("id-name");
                var userName = userNameNode?.InnerText;
                var trueNickname = string.Empty;
                if (nickName != null && userName != null)
                {
                    trueNickname = nickName?.Remove(nickName.IndexOf(userName, StringComparison.Ordinal), userName.Length);
                }

                var followButton =
                    mainNode.Descendants("button")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("follow-button button symbol"));

                // If "none" is contained in the button, then it's hidden, meaning the user is being followed. 
                var isFollowing = followButton == null;

                var userNode = mainNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty) == "user-data");
                var countryNode = userNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("note"));

                var birthdayNode = userNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("birthday"));

                var gameSkillNode = userNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("game-skill"));
                var gameSkillSpan = gameSkillNode?.Descendants("span").LastOrDefault();
                var gameSkill = gameSkillSpan?.GetAttributeValue("class", string.Empty);

                var gameSystemsNode = userNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("game data-content"));
                var gameSystems = gameSystemsNode?.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("note"));

                var systems = new List<string>();
                var htmlNodes2 = gameSystems?.Descendants("div");
                if (htmlNodes2 != null)
                {
                    systems.AddRange(htmlNodes2.Select(div => div.GetAttributeValue("class", string.Empty)));
                }

                var favoriteGameGenreNode = userNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("favorite-game-genre"));
                var favoriteGameGenre = favoriteGameGenreNode?.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("note"));
                var genres = new List<string>();
                var htmlNodes = favoriteGameGenre?.Descendants("span");
                if (htmlNodes != null)
                {
                    genres.AddRange(htmlNodes.Select(node => node.InnerText));
                }

                return new UserProfileResponse(new User(trueNickname, userName, avatarUri,
                    countryNode?.InnerText, birthdayNode?.InnerText, gameSkill, systems, genres, isFollowing, UserName.ToLower().Equals(userName.ToLower())));
            });
        }

	    public Task<PostResponse> GetPostAsync(string id)
	    {
            AccessCheck();

            var req = new HttpRequestMessage(HttpMethod.Get, "https://miiverse.nintendo.net/posts/" + id);
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
	        return Client.SendAsync(req).ToTaskOfStream().ContinueWith(stream =>
	        {
                var doc = new HtmlDocument();
                doc.Load(stream.Result);
	            var postNode = doc.GetElementbyId("post-content");
	            var post = ParsePost(postNode);
                return new PostResponse(post);
            });
        }

	    public Task<UserFeedResponse> GetUserFeedAsync(string username)
	    {
            AccessCheck();

            var req = new HttpRequestMessage(HttpMethod.Get, $"https://miiverse.nintendo.net/users/{username}/posts");
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            return Client.SendAsync(req).ToTaskOfStream().ContinueWith(stream =>
            {
                var doc = new HtmlDocument();
                doc.Load(stream.Result);

                var postsNode = doc.GetElementbyId("main-body").GetElementByClassName("post-list").ChildNodes.Where(n => n.HasClassName("post") && !n.HasClassName("none"));
                var posts = postsNode.Select(ParsePost).ToList();
                return new UserFeedResponse(posts);
            });
        } 

		public Task<ActivityResponse> GetActivityAsync()
		{
			AccessCheck();

			var req = new HttpRequestMessage(HttpMethod.Get, "https://miiverse.nintendo.net/activity?fragment=activityfeed");
			req.Headers.Add("X-Requested-With", "XMLHttpRequest");
			return Client.SendAsync(req).ToTaskOfStream().ContinueWith(stream =>
			{
				var doc = new HtmlDocument();
				doc.Load(stream.Result);

				var postsNode = doc.GetElementbyId("main-body").GetElementByClassName("post-list").ChildNodes.Where(n => n.HasClassName("post") && !n.HasClassName("none"));
				var posts = postsNode.Select(ParsePost).ToList();
			    return new ActivityResponse(posts);
			});
		}

		public Task SignOutAsync()
		{
			AccessCheck();
			return Client.HeadAsync(string.Format(MiiverseConstantValues.MIIVERSE_SIGN_OUT_URI_STRING, ClientID));
		}

		private void AccessCheck()
		{
			if (!_isEnabled)
			{
				throw new Exception();
			}
		}

		public void Dispose()
		{
			Client.Dispose();
			_isEnabled = false;
		}

		public string UserName { get; }
		public string ClientID { get; }
		public string SessionValue { get; }

		private HttpClient Client { get; }

        private Post ParsePost(HtmlNode postNode)
        {
            var timestampAnchorNode = postNode.GetElementByClassName("timestamp-container").FirstChild;
            HtmlNode postContentNode;
            try
            {
                // Post List Page
                postContentNode = postNode.GetElementByClassName("body").GetElementByClassName("post-content");
            }
            catch (InvalidOperationException)
            {
                // Individual Post Page
                postContentNode = postNode.GetElementByClassName("body");
            }
            var postMetaNode = postContentNode.GetElementByClassName("post-meta");

            var id = postNode.Id.Substring(5);
            var replyCount = postMetaNode.GetElementByClassName("reply").GetElementByClassName("reply-count").GetInnerTextAsUInt32();
            var empathyCount = postMetaNode.GetElementByClassName("empathy").GetElementByClassName("empathy-count").GetInnerTextAsUInt32();
            var isPlayed = postMetaNode.GetElementsByClassName("played").Count() != 0;
            var isSpoiler = postNode.HasClassName("hidden");

            string text = null;
            Uri imageUri = null;
            var textNodes = postContentNode.GetElementsByClassName("post-content-text");
            var isImagePost = !textNodes.Any();
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
                return new Post(
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
                    new PostCommunity(titleID, communityID, communityName, communityIconUri));
            }

            return new Post(
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
                new PostCommunity(titleID, communityID, communityName, communityIconUri));
        }

    }
}
