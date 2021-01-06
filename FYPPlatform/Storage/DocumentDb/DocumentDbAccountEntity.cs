using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYPPlatform.Web.Storage.DocumentDb
{
    public class DocumentDbAccountEntity
    {
        public string PartitionKey { get; set; }
        [JsonProperty("id")]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Interests { get; set; }
        public string State { get; set; }
    }
}
