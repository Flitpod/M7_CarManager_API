using System.Security.Policy;

namespace M7_CarClient.Model
{
    internal class TokenModel
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}