namespace Mntone.MiiverseClient
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

		public string UserName { get; set; }
		public string Password { get; set; }
	}
}
