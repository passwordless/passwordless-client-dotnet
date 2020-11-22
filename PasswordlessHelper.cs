using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class PasswordlessHelper
    {
        /// <summary>
        /// The base URL. Can be changed for testing/self-hosting purposes.
        /// </summary>
        public static string ApiUrl = "https://api.passwordless.dev/";

        /// <summary>
        /// Verifies the token during sign in and returns data related to the sign such as username.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<PasswordlessSignInToken> VerifyPasswordlessSignInToken(this HttpClient client, PasswordlessVerifyTokenParameters parameters)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, parameters.ApiUrl);
            req.Headers.Add("ApiSecret", parameters.ApiSecret);
            var json = JsonConvert.SerializeObject(parameters);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(req);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<PasswordlessSignInToken>(await response.Content.ReadAsStringAsync());
        }


        /// <summary>
        /// Creates a token that allows regsitering a credential against a username
        /// </summary>
        /// <param name="client"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<string> GetPasswordlessRegisterToken(this HttpClient client, PasswordlessTokenParameters parameters)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, parameters.ApiUrl);
            req.Headers.Add("ApiSecret", parameters.ApiSecret);
            var json = JsonConvert.SerializeObject(parameters);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(req);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Lists all credentials associated with the username.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<PasswordlessStoredCredential[]> ListPasswordlessCredentialsAsync(this HttpClient client, PasswordlessGetCredentialParameters parameters)
        {
            var uriBuilder = new UriBuilder(parameters.ApiUrl);
            uriBuilder.Query = "username=" + parameters.Username;
            var req = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri.ToString());
            req.Headers.Add("ApiSecret", parameters.ApiSecret);

            var response = await client.SendAsync(req);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<PasswordlessStoredCredential[]>(json);
        }

        /// <summary>
        /// Deletes the credential with the given id
        /// </summary>
        /// <param name="client"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task DeletePasswordlessCredentialAsync(this HttpClient client, PasswordlessDeleteCredentialParameters parameters)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, parameters.ApiUrl);
            req.Headers.Add("ApiSecret", parameters.ApiSecret);

            var json = JsonConvert.SerializeObject(parameters);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(req);
            response.EnsureSuccessStatusCode();
        }
    }

    public class PasswordlessStoredCredential
    {
        public byte[] UserId { get; set; }
        public dynamic Descriptor { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] UserHandle { get; set; }
        public uint SignatureCounter { get; set; }
        public string CredType { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid AaGuid { get; set; }
        public DateTime LastUsedAt { get; set; }
        public string RPID { get; set; }
        public string Origin { get; set; }
        public string Country { get; set; }
        public string Device { get; set; }
        public string Nickname { get; set; }
    }

    public class PasswordlessSignInToken
    {

        public DateTime ExpiresAt { get; set; }
        public string Username { get; set; }
        public DateTime Timestamp { get; set; }
        public string RPID { get; set; }
        public string Origin { get; set; }
        public bool Success { get; set; }
        public string Device { get; set; }
        public string Country { get; set; }
        public string Nickname { get; set; }
    }


    public class PasswordlessVerifyTokenParameters
    {
        [JsonIgnore]
        public string ApiUrl { get; set; } = PasswordlessHelper.ApiUrl + "signin/verify";
        [JsonIgnore]
        public string ApiSecret { get; set; }
        public string Token { get; set; }

        public PasswordlessVerifyTokenParameters(string apiSecret, string token)
        {
            ApiSecret = apiSecret;
            Token = token;
        }

    }

    public class PasswordlessGetCredentialParameters
    {
        [JsonIgnore]
        public string ApiUrl { get; set; } = PasswordlessHelper.ApiUrl + "credentials/list";
        [JsonIgnore]
        public string ApiSecret { get; set; }
        public string Username { get; set; }


        public PasswordlessGetCredentialParameters(string apiSecret, string username)
        {
            ApiSecret = apiSecret;
            Username = username;
        }
    }

    public class PasswordlessDeleteCredentialParameters
    {
        [JsonIgnore]
        public string ApiUrl { get; set; } = PasswordlessHelper.ApiUrl + "credentials/delete";
        [JsonIgnore]
        public string ApiSecret { get; set; }
        public string CredentialId { get; set; }


        public PasswordlessDeleteCredentialParameters(string apiSecret, string credentialId)
        {
            ApiSecret = apiSecret;
            CredentialId = credentialId;
        }
    }

    public class PasswordlessTokenParameters
    {
        public PasswordlessTokenParameters(string apiSecret, string username)
        {
            ApiSecret = apiSecret;
            Username = username;
        }

        [JsonIgnore]
        public string ApiSecret { get; set; }
        [JsonIgnore]
        public string ApiUrl { get; set; } = PasswordlessHelper.ApiUrl + "register/token";

        public string Username { get; set; }
        public string Displayname { get; set; } = "Test";
        public string AttType { get; set; } = "None";
        public string AuthType { get; set; }
        public bool RequireResidentKey { get; set; } = false;
        public string UserVerification { get; set; } = "Preferred";
    }
}
