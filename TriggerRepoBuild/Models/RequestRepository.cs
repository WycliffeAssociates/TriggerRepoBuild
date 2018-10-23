using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggerRepoBuild.Models
{
    class RequestRepository
    {
        public User owner;
        public string name;
        public string full_name;
        public string default_branch;
        public string clone_url;
        public string html_url;
    }
}
