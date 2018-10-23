using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerRepoBuild.Models
{
    class RequestCommit
    {
        public string id;
        public string message;
        public string url;
        public CommitUser author;
        public CommitUser committer;
    }
}
