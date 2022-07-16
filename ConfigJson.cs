using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeeoBotDSharpPlus
{
    public struct ConfigJson
    {
        [JsonProperty("Token")]
        public string Token { get; private set; }
        [JsonProperty("Prefix")]
        public string Prefix { get; private set; }
        [JsonProperty("RiotDevToken")]
        public string RiotDevToken { get; private set; }
    }
}
