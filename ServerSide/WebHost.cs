using Azure;
using Azure.Core;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ServerSide;

public class WebHost
{
	private int _port;
    public List<User> Users { get; set; }
    private HttpListener _listener;

    public WebHost(int port,List<User> users)
    {
           _port = port;
           Users=users;
    }

    public void Run()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{_port}/");
        _listener.Start();
        Console.WriteLine($"Listiner start listen on{_port}");
        while (true)
        {
            var context =_listener.GetContext();
            Task.Run(() => { HandleRequest(context); });

        }
    }

	private void HandleRequest(HttpListenerContext context)
	{
        try
        {
            if (context.Request.HttpMethod=="GET")
                HandleGet(context);
            else if (context.Request.HttpMethod=="DELETE")
                HandleDelete(context);
            else if (context.Request.HttpMethod=="POST")
                HandlePost(context);
            else if (context.Request.HttpMethod=="PUT")
                HandlePut(context);

        }
        catch (Exception ex )
        {

            Console.WriteLine($"Handle Request Error: {ex.Message}");
        }
	}

	private void HandlePut(HttpListenerContext context)
	{
		using var redaer = new StreamReader(context.Request.InputStream);
		var body = redaer.ReadToEnd();
		var newUser = JsonSerializer.Deserialize<User>(body);
        foreach (var user in Users)
        {
            if (user.Id==newUser!.Id)
            {
                user.FirstName = newUser.FirstName;
                user.LastName = newUser.LastName;
                user.Age = newUser.Age;
            }
        }
            
	}

	private void HandlePost(HttpListenerContext context)
	{
        try
        {
            using var redaer = new StreamReader(context.Request.InputStream);
            var body = redaer.ReadToEnd();
            var newUser=JsonSerializer.Deserialize<User>(body); 
            Users.Add(newUser);

        }
        catch (Exception ex)
        {

            throw;
        }
	}

	private void HandleDelete(HttpListenerContext context)
	{
        var request = context.Request;
        var response = context.Response;
		var rawUrl = request.RawUrl;
		var segments = rawUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

		if (segments.Length > 0)
		{
			string lastSegment = segments[^1];
			Console.WriteLine($"{lastSegment}");
			if (int.TryParse(lastSegment, out int userId))
			{
				Users.Remove(Users.FirstOrDefault(U => U.Id==userId));

			}
		}


		response.OutputStream.Close();
	}

	private void HandleGet(HttpListenerContext context)
	{
        try
        {
            var json = JsonSerializer.Serialize(Users);
            var buffer=Encoding.UTF8.GetBytes(json);
            context.Response.ContentType= "application/json";   
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error at Get method:{ex.Message}");
        }
	}
}
