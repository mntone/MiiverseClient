using System;

namespace Mntone.MiiverseClient.Entities.Token
{
	public sealed class NintendoNetworkSessionToken
	{
		public NintendoNetworkSessionToken(string clientID, string responseType, string redirectUri, string state)
		{
			this.ClientID = clientID;
			this.ResponseType = responseType;
			this.RedirectUri = new Uri(Uri.UnescapeDataString(redirectUri));
			this.State = state;
		}

		public string ClientID { get; }
		public string ResponseType { get; }
		public Uri RedirectUri { get; }
		public string State { get; }
	}
}
