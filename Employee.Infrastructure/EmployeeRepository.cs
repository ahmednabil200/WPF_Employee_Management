using System.Net.Http.Json;
using Application.Repositories;
using Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure;

public class EmployeeRepository : IEmployeeRepository
{
    private  readonly  ITokenProvider _tokenProvider;
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl = "https://gorest.co.in/public/v2/users";

    public EmployeeRepository(HttpClient httpClient, ITokenProvider tokenProvider)
    {
        _httpClient  = httpClient;
        _tokenProvider = tokenProvider;
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.GetToken());
    }

    public async Task<int> AddEmployee(Employee employee)
    {
        var response = await _httpClient.PostAsJsonAsync(_apiUrl, employee);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var emp =  JsonConvert.DeserializeObject<Employee>(json);
            return emp?.Id ?? -1;
        }
        return -1;
    }

    public async Task<bool> UpdateEmployee(Employee employee)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_apiUrl}/{employee.Id.ToString()}", employee);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteEmployee(string employeeId)
    {
        var response = await _httpClient.DeleteAsync($"{_apiUrl}/{employeeId}");
        return response.IsSuccessStatusCode;
    }
    

    public async Task<Employee?> GetEmployee(string employeeId)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/{employeeId}");
        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Employee>(json);
        }

        return null;
    }

    public async Task<IEnumerable<Employee>?> GetEmployees(int pageNumber,string? searchFirstName = null)
    {

        var requestUrl = $"{_apiUrl}?page={pageNumber}";
        if (searchFirstName is not null)
        {
            requestUrl = $"{requestUrl}&first_name={searchFirstName}";
        }
        var response = await _httpClient.GetAsync(requestUrl);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Employee[]>(json);

        }
        return null;
    }






}
