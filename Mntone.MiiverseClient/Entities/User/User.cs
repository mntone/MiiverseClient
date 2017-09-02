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
        public string Name { get; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; }

        [DataMember(Name = "icon_uri")]
        public Uri IconUri { get; }

        [DataMember(Name = "country")]
        public string Country { get; }

        [DataMember(Name = "birthday")]
        public string Birthday { get; }

        [DataMember(Name = "game_skill")]
        public GameSkill GameSkill { get; }

        [DataMember(Name = "game")]
        public List<GameSystem> GameSystem { get; }

        [DataMember(Name = "favorite_game_genre")]
        public List<string> FavoriteGameGenre { get; }

        [DataMember(Name = "is_following")]
        public bool IsFollowing { get; }

        [DataMember(Name = "is_current_user")]
        public bool IsCurrentUser { get; }
    }
}
