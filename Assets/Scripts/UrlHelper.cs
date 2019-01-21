using UnityEngine;

public class UrlHelper
{
    public static string GetUrl(string id)
    {
        switch (id)
        {
            case "github":
                return GithubUrl;

            case "twitter":
                return TwitterUrl;

            case "email":
                return EmailUrl;
        }
        return "";
    }

    public static string TwitterUrl
    {
        get { return "https://twitter.com/JamesADurie"; }
    }

    public static string EmailUrl
    {
        get { return "mailto:james@maple.scot";  }
    }

    public static string AppUrl
    {
        get
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "https://play.google.com/store/apps/details?id=com.maple.scot.pigpens";
                case RuntimePlatform.IPhonePlayer:
                    return "https://itunes.apple.com/us/app/pig-pens/id1448367077?ls=1&mt=8";
                default:
                    return "https://maplescot.itch.io/pig-pens";
            }
        }
    }

    public static string MapleScotUrl
    {
        get
        {
            return "https://maple.scot/our-games/24-pig-pens-digital";
        }
    }

    public static string GithubUrl
    {
        get
        {
            return "https://github.com/MapleScotDevelopment/PigPens";
        }
    }
}