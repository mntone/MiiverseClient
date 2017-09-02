using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Mntone.MiiverseClient.Entities.Community;
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

		public MiiverseContext(string userName, string clientID, string sessionValue, string language = "en-US")
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
            handler.CookieContainer.Add(MiiverseConstantValues.MIIVERSE_DOMAIN_URI, new Cookie("lang", language, "/", MiiverseConstantValues.MIIVERSE_DOMAIN)
            {
                Secure = true,
                HttpOnly = true,
            });
            var client = new HttpClient(handler, true);
            client.DefaultRequestHeaders.Add("Accept-Language", $"{language},en;q=0.5");
            Client = client;

        }

	    public Task<GameResponse> GetGameAsync(Game game, string nextPageUrl = "")
	    {
            AccessCheck();

            var baseUrl = "https://miiverse.nintendo.net/";
	        if (!string.IsNullOrEmpty(nextPageUrl))
	        {
	            baseUrl += nextPageUrl;
	        }
	        else
	        {
	            baseUrl += game.TitleUrl;
	        }

            var req = new HttpRequestMessage(HttpMethod.Get, baseUrl);
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
	        return Client.SendAsync(req).ToTaskOfStream().ContinueWith(stream =>
	        {
                var doc = new HtmlDocument();
                doc.Load(stream.Result);

	            var favoriteButton =
	                doc.DocumentNode.Descendants("button")
	                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("favorite-button"));

	            var isFavorite = favoriteButton != null && favoriteButton.GetAttributeValue("class", string.Empty).Contains("checked");

	            var textArea =
	                doc.DocumentNode.Descendants("textarea")
	                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("textarea-text"));

	            var canPost = textArea != null;

                //   var postListNode = doc.DocumentNode.Descendants("div")
                //           .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("list post-list"));

                //var nextPage = postListNode?.GetAttributeValue("data-next-page-url", string.Empty);
                //   var postNodes = postListNode?.ChildNodes.Where(n => n.HasClassName("post") && !n.HasClassName("none"));
                var posts = new List<Post>();
                return new GameResponse(canPost, isFavorite, "", posts);
	        });
	    }

	    public Task<CommunityListResponse> GetCommunityGameListAsync(GameSearchList searchOption, GamePlatformSearch platformSearch, int offset)
	    {
            AccessCheck();

            var baseUrl = "https://miiverse.nintendo.net/communities/categories/";

	        switch (platformSearch)
	        {
                case GamePlatformSearch.Nintendo3ds:
                    baseUrl += "3ds";
                    break;
                case GamePlatformSearch.Wiiu:
                    baseUrl += "wiiu";
                    break;
            }
	        switch (searchOption)
	        {
	            case GameSearchList.All:
	                baseUrl += "_all?offset=" + offset;
	                break;
                case GameSearchList.Game:
                    baseUrl += "_game?offset=" + offset;
                    break;
                case GameSearchList.Other:
                    baseUrl += "_other?offset=" + offset;
                    break;
                case GameSearchList.VirtualConsole:
                    baseUrl += "_virtualconsole?offset=" + offset;
                    break;
            }

            var req = new HttpRequestMessage(HttpMethod.Get, baseUrl);
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
	        return Client.SendAsync(req).ToTaskOfStream().ContinueWith(stream =>
	        {
                var doc = new HtmlDocument();
                doc.Load(stream.Result);
	            var gameListNode =
	                doc.DocumentNode.Descendants("ul")
	                    .FirstOrDefault(
	                        node =>
	                            node.GetAttributeValue("class", string.Empty) == "list community-list");
	            if (gameListNode == null)
	            {
                    return new CommunityListResponse(null);
                }

                var gamesList = gameListNode.Descendants("li");
	            var output = new List<Game>();
	            foreach (var game in gamesList)
	            {
	                var id = game.GetAttributeValue("id", string.Empty);
                    var titleUrl = game.GetAttributeValue("data-href", string.Empty);

	                var iconImg = game.Descendants("img").FirstOrDefault();
	                var icon = iconImg?.GetAttributeValue("src", string.Empty);

	                var body =
	                    game.Descendants("div")
	                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty) == "body");

	                var titleNode =
                        body.Descendants("a")
	                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty) == "title");
	                var title = titleNode?.InnerText;

	                var imageNode = body.Descendants("img").FirstOrDefault();
	                var image = imageNode?.GetAttributeValue("src", string.Empty);
	                var imageFilename = string.Empty;
	                if (!string.IsNullOrEmpty(image))
	                {
                        var uri = new Uri(image);
                        imageFilename = System.IO.Path.GetFileName(uri.LocalPath).Split('?').FirstOrDefault();
                    }

	                var gameTextSpan = body.Descendants("span").LastOrDefault();
	                var gameText = gameTextSpan.InnerText;
                    output.Add(new Game(id, title, titleUrl, new Uri(icon), imageFilename, gameText));
	            }
                return new CommunityListResponse(output);
	        });

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
                var nickNameNode = mainNode.Descendants("a").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty) == "nick-name");
                var nickName = nickNameNode?.InnerText;

                var userNameNode = mainNode.Descendants("p").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty) == "id-name");
                var userName = userNameNode?.InnerText;

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
                var gameSkill = gameSkillSpan?.GetAttributeValue("class", string.Empty).Replace("test-game-skill", "").Trim();
                
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

                return new UserProfileResponse(new User(nickName, userName, avatarUri,
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

                var mainBody = doc.GetElementbyId("main-body");
                var postList = mainBody.Descendants("div").Where(node => node.GetAttributeValue("id", string.Empty).Contains("post-"));
                var posts = postList.Select(ParsePost).ToList();
                return new UserFeedResponse(posts);
            });
        } 

		public Task<ActivityResponse> GetActivityAsync(bool friendsOnly = false)
		{
			AccessCheck();
            var url = "https://miiverse.nintendo.net/activity?fragment=activityfeed";
		    if (friendsOnly)
		    {
		        url += "&filter=friend";
		    }
			var req = new HttpRequestMessage(HttpMethod.Get, url);
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
            //var timestampAnchorNode = postNode.GetElementByClassName("timestamp-container").FirstChild;
            HtmlNode postContentNode;
            try
            {
                // Post List Page
                var bodyNode = postNode.Descendants("div").Where(n => n.GetAttributeValue("class", string.Empty).Contains("body")).FirstOrDefault();
                postContentNode = bodyNode.GetElementByClassName("post-content");
            }
            catch (InvalidOperationException)
            {
                // Individual Post Page
                postContentNode = postNode.Descendants("div").Where(n => n.GetAttributeValue("class", string.Empty).Contains("body")).FirstOrDefault();
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

            var userNameAnchorNode = postNode.Descendants("p").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty) == "user-name").FirstChild;
            var userName = userNameAnchorNode.GetAttributeValue("href", string.Empty).Substring(7);
            var screenName = userNameAnchorNode.InnerText;
            HtmlNode userIconContainer;
            try
            {
                userIconContainer = postNode.GetElementByClassName("icon-container");
            }
            catch (Exception)
            {
                userIconContainer = postNode.Descendants("a").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty) == "icon-container");
            }
            var userIconUri = userIconContainer.Descendants("img").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty) == "icon").GetAttributeValue("src", "");
            var feeling = FeelingTypeHelpers.DetectFeelingTypeFromIconUri(new Uri(userIconUri));
            var normalUserIconUri = FeelingTypeHelpers.GetNormalFaceIconUri(new Uri(userIconUri), feeling);

            HtmlNode communityAnchorNode = communityAnchorNode = postNode.Descendants("a").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty) == "test-community-link");
            if (communityAnchorNode == null)
            {
                var testNode = postNode.Descendants("h1").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty) == "community-container-heading");
                communityAnchorNode = testNode.Descendants("a").FirstOrDefault();
            }
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
