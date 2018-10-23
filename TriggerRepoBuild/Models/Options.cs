using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace TriggerRepoBuild.Models
{
    class Options
    {
        [Option('g',"gogsurl", HelpText ="URL of the gogs server to use", Required = true)]
        public string GogsUrl { get; set; }

        [Option('w',"webhookurl", HelpText ="URL of the webhook", Required = true)]
        public string WebhookEndpoint { get; set; }

        [Option('f',"file", HelpText ="CSV to load the repos from", Required = true)]
        public string InputFile { get; set; }

        [Option('t',"token", HelpText ="GOGS token to use to talk to GOGS", Required = true)]
        public string GOGSToken { get; set; }

        [Option('u',"user", HelpText ="Username to use in render", Required = true)]
        public string User { get; set; }

        [Option('r',"renderurl", HelpText = "Render output url", Required = true)]
        public string RenderUrl { get; internal set; }
    }
}
