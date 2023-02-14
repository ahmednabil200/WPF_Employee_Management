using Domain;

namespace Application.Repositories;

public interface IEmployeeRepository
{
    public Task<int> AddEmployee(Employee employee);
    public Task<bool> UpdateEmployee(Employee employee);
    public Task<bool> DeleteEmployee(string employeeId);
    public Task<Employee?> GetEmployee(string employeeId);
    public Task<IEnumerable<Employee>?> GetEmployees(int pageNumber,string? searchFirstName = null);
}