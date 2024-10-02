using Azure;
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
        }
        catch (Exception ex )
        {

            Console.WriteLine($"Handle Request Error: {ex.Message}");
        }
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
