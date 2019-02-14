using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft;

namespace QuerySteam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public enum pState { Offline, Online, Busy, Away, Snooze, Looking_to_trade, Looking_to_play };

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string gFriendsList = "http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key=[webapikey]&steamid=[userid]&relationship=friend";
            string gProfileData = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=[webapikey]&steamids=[userid]";
            string gUserStats = "http://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v0001/?appid=[gameid]&key=[webapikey]&steamid=[userid]";
            string gGameList = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=[webapikey]&steamid=[userid]&format=json&include_appinfo=1";
            ResourceManager rm = new ResourceManager("QuerySteam.auth",
                Assembly.GetExecutingAssembly());

            string webApiKey = rm.GetString("webapikey");//this is held in a seperate resources file for security
            string userID = "76561198294755353";

            string url = gProfileData;
            url = url.Replace("[webapikey]", webApiKey);//replace the webapikey
            url = url.Replace("[userid]", userID);

            Console.WriteLine(url);
            InitializeComponent();
            
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            Rootobject tmp = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(responseFromServer);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(tmp.response.players[0].avatarfull, UriKind.Absolute);
            bitmap.EndInit();
            img1.Source = bitmap;
            pName.Text = tmp.response.players[0].personaname;
            //Now get GameList ??
            url = gGameList;
            url = url.Replace("[webapikey]", webApiKey);//replace the webapikey
            url = url.Replace("[userid]", userID);

            request = WebRequest.Create(url);

            response = request.GetResponse();
            dataStream = response.GetResponseStream();
            reader = new StreamReader(dataStream);
            responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            GameListRootobject GLRO = Newtonsoft.Json.JsonConvert.DeserializeObject<GameListRootobject>(responseFromServer);
            gameList.DataContext = GLRO.response.games;


        }
    }



    //Steam Object Classes
    public class Rootobject
    {
        public Response response { get; set; }
    }

    public class Response
    {
        public Player[] players { get; set; }
    }

    public class Player
    {
        public string steamid { get; set; }
        public int communityvisibilitystate { get; set; }
        public int profilestate { get; set; }
        public string personaname { get; set; }
        public int lastlogoff { get; set; }
        public string profileurl { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public pState personastate { get; set; }
    }
    //Sloppy as anything....


    public class GameListRootobject
    {
        public GameListResponse response { get; set; }
    }

    public class GameListResponse
    {
        public int game_count { get; set; }
        public Game[] games { get; set; }
    }

    public class Game
    {
        public int appid { get; set; }
        public string name { get; set; }
        public int playtime_forever { get; set; }
        public string img_icon_url { get; set; }
        public string img_logo_url { get; set; }
        public bool has_community_visible_stats { get; set; }
    }

}
