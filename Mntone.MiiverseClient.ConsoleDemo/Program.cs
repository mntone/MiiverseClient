using System;
using System.Collections.Generic;
using System.Linq;
using Mntone.MiiverseClient.Entities.Community;
using Mntone.MiiverseClient.Entities.Token;
using Mntone.MiiverseClient.Managers;

namespace Mntone.MiiverseClient.ConsoleDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			var oauthClient = new MiiverseOAuthClient();
			var token = oauthClient.GetTokenAsync().GetAwaiter().GetResult();
			Console.WriteLine("client_id:\t{0}", token.ClientID);
			Console.WriteLine("response_type:\t{0}", token.ResponseType);
			Console.WriteLine("redirect_uri:\t{0}", token.RedirectUri);
			Console.WriteLine("state:\t{0}", token.State);
			Console.WriteLine("-----------");

			Console.WriteLine("Please input your NNID.");
			Console.Write("Username: ");
			var userName = Console.ReadLine();
			Console.Write("Password: ");
			var password = GetPassword();
			Console.WriteLine("");
            Console.WriteLine("-----------");

            var ctx = oauthClient.Authorize(token, new NintendoNetworkAuthenticationToken(userName, password)).GetAwaiter().GetResult();

		    var gameList = ctx.GetCommunityGameListAsync(GameSearchList.All, GamePlatformSearch.Wiiu, 300).GetAwaiter().GetResult();
		    foreach (var game in gameList.Games)
		    {
                Console.WriteLine("Title: {0}", game.Title);
                Console.WriteLine("TitleUrl: {0}", game.TitleUrl);
                Console.WriteLine("Id: {0}", game.Id);
                Console.WriteLine("Platform: {0}", game.Platform);
                Console.WriteLine("IconUri: {0}", game.IconUri);
                Console.WriteLine("Type: {0}", game.Type);
                Console.WriteLine("");
                Console.WriteLine("-----------");
            }

            var gameTest = gameList.Games.First(node => node.Id == "community-14866558073673172583");

            var indieGameDiscuss = ctx.GetDiscussAsync(gameTest).GetAwaiter().GetResult();
            var indieGameInGame = ctx.GetInGameAsync(gameTest).GetAwaiter().GetResult();
            var indieGameOld = ctx.GetOldGameAsync(gameTest).GetAwaiter().GetResult();
            var indieGameDrawing = ctx.GetDrawingAsync(gameTest).GetAwaiter().GetResult();
            var indieGame = ctx.GetGameAsync(gameTest).GetAwaiter().GetResult();
            var indieGameDiary = ctx.GetDiaryAsync(gameTest).GetAwaiter().GetResult();

            var userEntity = ctx.GetUserProfileAsync(userName).GetAwaiter().GetResult();
            Console.WriteLine("Name: {0}", userEntity.User.Name);
            Console.WriteLine("ScreenName: {0}", userEntity.User.ScreenName);
            Console.WriteLine("IconUri: {0}", userEntity.User.IconUri);
            Console.WriteLine("Country: {0}", userEntity.User.Country);
            Console.WriteLine("Birthday: {0}", userEntity.User.Birthday);
		    foreach (var gameSystem in userEntity.User.GameSystem)
		    {
                Console.WriteLine("GameSystem: {0}", gameSystem);
            }
            foreach (var gameGenre in userEntity.User.FavoriteGameGenre)
            {
                Console.WriteLine("GameGenre: {0}", gameGenre);
            }
            Console.WriteLine("GameSkill: {0}", userEntity.User.GameSkill);
            Console.WriteLine("IsFollowing: {0}", userEntity.User.IsFollowing);
            Console.WriteLine("IsCurrentUser: {0}", userEntity.User.IsCurrentUser);
            Console.WriteLine("-----------");
            var activityResponse = ctx.GetActivityAsync().GetAwaiter().GetResult();

			foreach (var post in activityResponse.Posts)
			{
				Console.WriteLine("{0}: {1}{2}", post.User.ScreenName, post.Text, post.ImageUri);
				Console.WriteLine("-----------");
			}

		    if (activityResponse.Posts.Any())
		    {
		        var activity = activityResponse.Posts.First();
		        var postResponse = ctx.GetPostAsync(activity.ID).GetAwaiter().GetResult();

                Console.WriteLine("Get Post " + activity.ID);
                Console.WriteLine("{0}: {1}{2}", postResponse.Post.User.ScreenName, postResponse.Post.Text, postResponse.Post.ImageUri);
                Console.WriteLine("-----------");
            }

            var userFeedResponse = ctx.GetUserFeedAsync("drasticactions").GetAwaiter().GetResult();
            foreach (var post in userFeedResponse.Posts)
            {
                Console.WriteLine("{0}: {1}{2}", post.User.ScreenName, post.Text, post.ImageUri);
                Console.WriteLine("-----------");
            }

            ctx.SignOutAsync().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
		}

		private static string GetPassword()
		{
			var inputList = new List<char>();

			ConsoleKeyInfo info;
			while ((info = Console.ReadKey(true)).Key != ConsoleKey.Enter)
			{
				if (info.Key == ConsoleKey.Backspace)
				{
					if (inputList.Count != 0)
					{
						inputList.RemoveAt(inputList.Count - 1);

						var indexMinusOne = Console.CursorLeft - 1;
						Console.SetCursorPosition(indexMinusOne, Console.CursorTop);
						Console.Write(" ");
						Console.SetCursorPosition(indexMinusOne, Console.CursorTop);
					}
					continue;
				}

				Console.Write("*");
				inputList.Add(info.KeyChar);
			}

			return string.Concat(inputList);
		}
	}
}
