
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace HttpTask;

public class MainViewModel:INotifyPropertyChanged
{



	//readonly HttpClient _httpClient;
	private ObservableCollection<User> users;
    public ObservableCollection<User> Users
	{
		get => users;
		set
		{
			users=value;
			OnpropertyChanged();
		}
	}
	private User selectedUser;
	public User SelectedUser
	{
		get => selectedUser;
		set
		{
			selectedUser=value;
			OnpropertyChanged();
			FirstName=SelectedUser.FirstName;
			LastName=SelectedUser.LastName;
			Age=SelectedUser.Age;
		}
	}
	private string firstName;
	public string FirstName
	{

		get => firstName;
		set
		{
			firstName=value;
			OnpropertyChanged();
		}
	}
	private string lastName;
	public string LastName 
	{ 
		get => lastName;
		set
		{
			lastName=value;
			OnpropertyChanged();
		}
    }
	private int age;
    public int Age
	{
		get => age;
		set
		{
			age=value;
			OnpropertyChanged();
		}
	}

	public RelayCommand AddCommand { get; set; }
    public RelayCommand DeleteCommand { get; set; }
    public RelayCommand UpdateCommand { get; set; }
    public RelayCommand RefreshCommand { get; set; }

    public MainViewModel()
    {
		
        RefreshCommand=new(executeRefresh);
        DeleteCommand=new(executeDeleteCommand,canDeleteCommand);
        UpdateCommand=new(executeUpdateCommand);
        AddCommand=new(executeAdd);

    }

	private bool canDeleteCommand(object arg)
	{
		if(SelectedUser is null) return false;
		else return true;
	}

	private async void executeAdd(object obj)
	{
		try
		{
			var url = "http://localhost:27001/Users";
			using var httpClient=new HttpClient();
			httpClient.BaseAddress = new Uri(url);
			Random random = new Random();
			var json = JsonSerializer.Serialize(new User() { LastName=LastName, FirstName=FirstName, Age=Age, Id=random.Next(1,1000) });
			LastName=string.Empty;
			FirstName=string.Empty;
			Age=0;
			var content = new StringContent(json);
			var response = await httpClient.PostAsync(url, content);	
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async void executeUpdateCommand(object obj)
	{
		try
		{
			var url = "http://localhost:27001/Users";
			using var httpClient=new HttpClient();
			httpClient.BaseAddress = new Uri(url);
			int userIdToDelete = SelectedUser.Id;
			var json = JsonSerializer.Serialize(new User() { LastName=LastName, FirstName=FirstName, Age=Age, Id=userIdToDelete });
			var content= new StringContent(json);
			LastName=string.Empty;
			FirstName=string.Empty;
			Age=0;
			var response = await httpClient.PutAsync(url, content);
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async void executeDeleteCommand(object obj)
	{
		int userIdToDelete = SelectedUser.Id;

		using HttpClient client = new HttpClient();

		try
		{

			StringContent content = new StringContent(userIdToDelete.ToString(), Encoding.UTF8, "text/plain");

			string url = $"http://localhost:27001/users/{userIdToDelete}";
			LastName=string.Empty;
			FirstName=string.Empty;
			Age=0;
			HttpResponseMessage response = await client.DeleteAsync(url);




		}
		catch (HttpRequestException e)
		{
			Console.WriteLine("Error in client request:");
			Console.WriteLine(e.Message);
		}
	}

	private async void executeRefresh(object obj)
	{
		try
		{
			using (HttpClient client = new HttpClient())
			{
				string url = "http://localhost:27001/Users";

				var response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();
			

				var respondBody= await response.Content.ReadAsStringAsync();
				var users=JsonSerializer.Deserialize<ObservableCollection<User>>(respondBody);
				if (users is not null)
					Users=users;

			}
		}
		catch (Exception ex)
		{

			Console.WriteLine($"Request Error:{ex.Message}");
		}


	}
	protected virtual void OnpropertyChanged([CallerMemberName] string name = null) => PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));



	public event PropertyChangedEventHandler? PropertyChanged;
}
