using Newtonsoft.Json;

namespace CroustiPizz.Mobile.Dtos.Authentications.Credentials
{
    public class SetPasswordRequest
    {
        [JsonProperty("old_password")] public string OldPassword { get; set; }

        [JsonProperty("new_password")] public string NewPassword { get; set; }
    }
}