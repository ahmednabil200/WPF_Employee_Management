using Domain;
using Infrastructure;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Application.Repositories;
using System.Text.Json.Serialization;

namespace Client;

public class EmployeeView
{
    private IEmployeeRepository _employeeRepository;

    public EmployeeView(IEmployeeRepository employeeService)
    {
        _employeeRepository = employeeService;
    }

    public EmployeeView()
    {
      
    }

    public ObservableCollection<Employee> Employees(int pageNumber)
    {
        var task = Task.Run(async () => await _employeeRepository.GetEmployees(pageNumber));
        if (task.Result != null) return new ObservableCollection<Employee>(task.Result);
        return new ObservableCollection<Employee>();
    }

    public ObservableCollection<Employee> Employees(int pageNumber,string searchQuery)
    {
        var task = Task.Run(async () => await _employeeRepository.GetEmployees(pageNumber, searchQuery));
        if (task.Result != null) return new ObservableCollection<Employee>(task.Result);
        return new ObservableCollection<Employee>();
    }

    public int AddEmployee(Employee emp)
    {
        var task = Task.Run(async () => await _employeeRepository.AddEmployee(emp));
        return task.Result;
    }

    public bool UpdateEmployee(Employee emp)
    {
        var task = Task.Run(async () => await _employeeRepository.UpdateEmployee(emp));
        return task.Result;
    }

    public Employee? GetEmployee(int id)
    {
        var task = Task.Run(async () => await _employeeRepository.GetEmployee(id.ToString()));
        return task.Result;

    }

    public bool DeleteEmployee(int id)
    {
        var task = Task.Run(async () => await _employeeRepository.DeleteEmployee(id.ToString()));
        return task.Result;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public Gender Gender { get; set; }
    public Status Status { get; set; }

}