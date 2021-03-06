﻿namespace Mntone.MiiverseClient
{
	public sealed class NintendoNetworkAuthenticationToken
	{
		public NintendoNetworkAuthenticationToken()
		{ }

		public NintendoNetworkAuthenticationToken(string userName, string password)
		{
			this.UserName = userName;
			this.Password = password;
		}

		/// <summary>
		/// User name (Nintendo Network ID)
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Password
		/// </summary>
		public string Password { get; set; }
	}
}
