using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mntone.MiiverseClient.Entities.Community
{
    [DataContract]
    public  class Game
    {
        public Game(string id, string title, string titleUrl, Uri iconUri, string platform, string type)
        {
            Id = id;
            Title = title;
            TitleUrl = titleUrl;
            IconUri = iconUri;
            Platform = GamePlatformHelper.DetectPlatformFromImageName(platform);
            Type = type;
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "title_url")]
        public string TitleUrl { get; set; }

        [DataMember(Name = "icon_uri")]
        public Uri IconUri { get; set; }

        [DataMember(Name = "platform")]
        public Platform Platform { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
