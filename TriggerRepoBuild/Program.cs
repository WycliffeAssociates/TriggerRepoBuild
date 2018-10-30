using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggerRepoBuild.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using LibGit2Sharp;
using CsvHelper;
using CsvHelper.Configuration;
using CommandLine;

namespace TriggerRepoBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed((arguments) => { Process(arguments); });
        }

        private static void Process(Options options)
        {
            string secret = options.GOGSToken;
            string senderUserName = options.User;

            string user;
            string repo;
            CsvReader parser = new CsvReader(new StreamReader(new FileStream(options.InputFile, FileMode.Open)), new CsvHelper.Configuration.Configuration() { HasHeaderRecord = true });
            parser.Read();
            parser.ReadHeader();
            while (parser.Read())
            {
                repo = parser.GetField(options.RepoField);
                user = parser.GetField(options.UserField);
                HttpClient client = new HttpClient();
                var repoResponse = client.GetAsync($"{options.GogsUrl}/api/v1/repos/{Uri.EscapeDataString(user)}/{Uri.EscapeDataString(repo)}").Result;
                if ((int)repoResponse.StatusCode != 404)
                {
                    RequestRepository repoObject = JsonConvert.DeserializeObject<RequestRepository>(repoResponse.Content.ReadAsStringAsync().Result);

                    Console.WriteLine($"Processing: {repoObject.owner.username}/{repoObject.name}");
                    if (IsStuck(repoObject.owner.username, repoObject.name, options.RenderUrl))
                    {
                        Console.WriteLine("Repo is stuck");
                        TriggerRerender(secret, senderUserName, repoObject.owner.username, repoObject.name, options.WebhookEndpoint, options.GogsUrl);
                    }
                    else
                    {
                        Console.WriteLine("Repo isn't stuck");
                    }
                }
                else
                {
                    Console.WriteLine($"Repo {user}/{repo} doesn't exist in GOGS");
                }
            }
        }

        private static void TriggerRerender(string secret, string senderUserName, string user, string repo, string webhookUrl, string gitUrl)
        {
            Console.WriteLine("Cloning repo to get commit data");
            var commits = GetRepoCommits(user, repo, gitUrl);
            if (commits.Count == 0)
            {
                Console.WriteLine("Missing commits");
                return;
            }

            User repoOwner = new User()
            {
                username = user
            };
            User sender = new User()
            {
                username = senderUserName,
            };
            Request request = new Request()
            {
                secret = secret,
                refs = "refs/heads/master",
                after = commits[0].id,
                before = commits[0].id,
                compare_url = "",
                commits = commits,
                repository = new RequestRepository()
                {
                    default_branch = "master",
                    owner = repoOwner,
                    full_name = $"{user}/{repo}",
                    clone_url = $"{gitUrl}/{user}/{repo}.git",
                    html_url = $"{gitUrl}/{user}/{repo}",
                    name = repo
                },
                pusher = sender,
                sender = sender,
            };

            Console.WriteLine("Sending request");
            // Send the request
            bool sent = false;
            int sendCount = 0;
            while (!(sent || sendCount>=10))
            {
                sendCount++;
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Gogs-Event", "push");
                StringContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var result = client.PostAsync(webhookUrl, content).Result;
                // Output status code
                Console.WriteLine(result.StatusCode);
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                sent = true;
            }
            if (sendCount > 10)
            {
                Console.WriteLine("Couldn't reach webhook after 10 tries");
            }

        }
        static bool IsStuck(string user, string repo, string renderedBaseUrl)
        {
            HttpClient client = new HttpClient();
            var door43Result = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, $"{renderedBaseUrl}/{user}/{repo}/")).Result;

            var gogsResult = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, $"{renderedBaseUrl}/{user}/{repo}/")).Result;
            return (int)door43Result.StatusCode == 404 && (int)gogsResult.StatusCode == 200;
        }

        static List<RequestCommit> GetRepoCommits(string user, string repoName, string gogsBaseUrl, int numberOfCommits = 5)
        {
            List<RequestCommit> output = new List<RequestCommit>();
            CloneOptions cloneOptions = new CloneOptions()
            {
                Checkout = false
            };
            try
            {

            Repository.Clone($"{gogsBaseUrl}/{Uri.EscapeDataString(user)}/{Uri.EscapeDataString(repoName)}.git", "tmp", cloneOptions);
            Repository repo = new Repository("tmp");
            foreach (var commit in repo.Commits.Take(numberOfCommits))
            {
                CommitUser author = new CommitUser()
                {
                    name = commit.Author.Name,
                    email = commit.Author.Email
                };
                CommitUser committer = new CommitUser()
                {
                    name = commit.Committer.Name,
                    email = commit.Committer.Email,
                };
                output.Add(new RequestCommit()
                {
                    author = author,
                    committer = committer,
                    id = commit.Id.Sha,
                    message = commit.Message,
                    url = $"{gogsBaseUrl}/{user}/{repoName}/commit/{commit.Id.Sha}"
                });
            }
            repo.Dispose();
            }
            catch
            {
                return output;
            }
            RecursiveDelete("tmp");
            return output;
        }

        /// <summary>
        /// Delete a directory and all of its children. Essentially rm -rf
        /// </summary>
        /// <param name="dir">Directory to delete</param>
        /// <remarks>Directory.Delete is stupid so I need this workaround</remarks>
        static void RecursiveDelete(string dir)
        {
            foreach (var file in Directory.GetFiles(dir))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            bool deleted = false;
            foreach (var d in Directory.GetDirectories(dir))
            {
                while (!deleted)
                {
                    try
                    {
                        if (Directory.Exists(d))
                        {
                            RecursiveDelete(d);
                            Directory.Delete(d);
                        }
                        deleted = true;
                    }
                    catch
                    {

                    }
                }
            }
            Directory.Delete(dir);
        }
    }
}
