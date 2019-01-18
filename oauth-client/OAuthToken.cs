using System;
namespace oauth_client
{
    public class OAuthToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string bearer { get; set; }
    }
}
