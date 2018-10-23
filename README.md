# Building

The standard .net core build either download the dotnet sdk and run dotnet build or use Visual Studio

# Running
This is a console tool so it needs to be run from there. There are a couple of parameters as follows

```
  -g, --gogsurl       Required. URL of the gogs server to use

  -w, --webhookurl    Required. URL of the webhook

  -f, --file          Required. CSV to load the repos from

  -t, --token         Required. GOGS token to use to talk to GOGS

  -u, --user          Required. Username to use in render

  -r, --renderurl     Required. Render output url
```