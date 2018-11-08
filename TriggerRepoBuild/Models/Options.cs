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
        public string RenderUrl { get; set; }

        [Option("userfield", HelpText = "Name of user field in input csv", Default ="user_id", Required = true)]
        public string UserField { get; set; }

        [Option("repofield", HelpText = "Name of repo field in input csv", Default ="repo_name", Required = true)]
        public string RepoField { get; set; }

        [Option("forcerender", HelpText = "Force a rerender even if the repo isn't stuck", Default = false, Required = true)]
        public bool ForceRender { get; set; }
    }
}
