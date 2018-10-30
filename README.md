# Building

The standard .net core build either download the dotnet sdk and run dotnet build or use Visual Studio

# Running
This is a console tool so it needs to be run from there. There are a couple of parameters that are needed

```
  -g, --gogsurl       Required. URL of the gogs server to use

  -w, --webhookurl    Required. URL of the webhook

  -f, --file          Required. CSV to load the repos from

  -t, --token         Required. GOGS token to use to talk to GOGS

  -u, --user          Required. Username to use in render

  -r, --renderurl     Required. Render output url

  --userfield         Required. (Default: user_id) Name of user field in input csv

  --repofield         Required. (Default: repo_name) Name of repo field in input csv
```

If you are compiling and running from source you would run the following

`dotnet run --gogsurl="https://<gogs-server>" --webhookurl="https://<api-server>/client/webhook" --file="input.csv" --token="<gogs-token>" --user="<username>" --renderurl="https://<door43-url>/u/" --userfield="user_name" --repofield="repo_id"`

If you are running a release build run the following (For windows)

`TriggerRepoBuild.exe --gogsurl="https://<gogs-server>" --webhookurl="https://<api-server>/client/webhook" --file="input.csv" --token="<gogs-token>" --user="<username>" --renderurl="https://<door43-url>/u/" --userfield="user_name" --repofield="repo_id"`

For other platforms replace TriggerRepoBuild.exe with the name of your platform executable