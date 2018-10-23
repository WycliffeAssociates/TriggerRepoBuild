using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerRepoBuild.Models
{
    class Request
    {
        public string secret;
        [JsonProperty(PropertyName = "ref")]
        public string refs;
        public string after;
        public string compare_url;
        public List<RequestCommit> commits;
        public User pusher;
        public User sender;
        public RequestRepository repository;
        public string before;
    }
}
