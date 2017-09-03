using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mntone.MiiverseClient.Entities.User
{
    [DataContract]
    public  class User
    {
        public User(string name, 
            string screenName, 
            Uri iconUri,
            string country,
            string birthday,
            string gameSkill,
            IEnumerable<string> gameSystems,
            IEnumerable<string> favoriteGameGenre,
            bool isFollowing,
            bool isCurrentUser)
        {
            Name = name;
            ScreenName = screenName;
            IconUri = iconUri;
            Country = country;
            Birthday = birthday;
            GameSkill = GameSkillHelper.DetectGameSkillFromClassName(gameSkill);
            GameSystem = new List<GameSystem>();
            foreach (var gameSystem in gameSystems)
            {
                GameSystem.Add(GameSystemHelper.DetectGameSystemFromClassName(gameSystem));
            }
            FavoriteGameGenre = favoriteGameGenre.ToList();
            IsFollowing = isFollowing;
            IsCurrentUser = isCurrentUser;
        }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "icon_uri")]
        public Uri IconUri { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "birthday")]
        public string Birthday { get; set; }

        [DataMember(Name = "game_skill")]
        public GameSkill GameSkill { get; set; }

        [DataMember(Name = "game")]
        public List<GameSystem> GameSystem { get; set; }

        [DataMember(Name = "favorite_game_genre")]
        public List<string> FavoriteGameGenre { get; set; }

        [DataMember(Name = "is_following")]
        public bool IsFollowing { get; set; }

        [DataMember(Name = "is_current_user")]
        public bool IsCurrentUser { get; set; }
    }
}
