using System;
using System.Collections.Generic;
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
			Console.WriteLine("-----------");

			var ctx = oauthClient.Authorize(token, new NintendoNetworkAuthenticationToken(userName, password)).GetAwaiter().GetResult();
			var activityResponse = ctx.GetActivityAsync().GetAwaiter().GetResult();
			foreach (var post in activityResponse.Posts)
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
