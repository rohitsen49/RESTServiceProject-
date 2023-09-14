var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:#####/api/");
// client.DefaultRequestHeaders.Add("Accept", "application/json");

var result = await client.GetAsync("users");

var json = await result.Content.ReadAsStringAsync();

Console.WriteLine(json);
Console.ReadLine();