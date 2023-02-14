using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Application.Repositories;
using Domain;
using Infrastructure;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace Tests;

public class EmployeeRepositoryTests
{
    [Fact]
    public async Task AddEmployee_ReturnsPositiveInt()
    {
        // Arrange
        var expectedUri = new Uri("https://gorest.co.in/public/v2/users");
        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock_token");
        var mockHttp = new Mock<HttpMessageHandler>();
        var employee = new Employee(123, "Name", "email@domain.com", "male", "active");
        var httpClient = new HttpClient(mockHttp.Object);
        
        mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock_token");

        var employeeRepo = new EmployeeRepository(httpClient, mockTokenProvider.Object);
        mockHttp.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent(JsonConvert.SerializeObject(employee).ToString()),
            });

        // Act
        var result = await employeeRepo.AddEmployee(employee);

        // Assert
        Assert.True(result > 0);

        mockHttp.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Content != null
                && req.Method == HttpMethod.Post 
                && req.RequestUri == expectedUri 
                && req.Content.ReadAsStringAsync().Result.Contains("123") 
                && req.Content.ReadAsStringAsync().Result.Contains("Name") 
                && req.Content.ReadAsStringAsync().Result.Contains("email@domain.com") 
                && req.Content.ReadAsStringAsync().Result.Contains("male")
                && req.Content.ReadAsStringAsync().Result.Contains("active")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task AddEmployee_ReturnsError_WhenAPIReturnsBadRequest()
    {
        // Arrange
        var employee = new Employee(123, "Name", "email@domain.com", "male", "active");
        var mockHttp = new Mock<HttpMessageHandler>();
        mockHttp.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });
        var httpClient = new HttpClient(mockHttp.Object);

        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock_token");

        var employeeRepo = new EmployeeRepository(httpClient, mockTokenProvider.Object);


        // Act
        var result = await employeeRepo.AddEmployee(employee);

        //Asset
        Assert.True(result==-1);

    }

    [Fact]
    public async Task AddEmployee_ReturnsError_WhenAddingWrongGender()
    {
        // Arrange
        var employee = new Employee(123, "Name", "email@domain.com", "xx", "active");
        var mockHttp = new Mock<HttpMessageHandler>();
        mockHttp.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.UnprocessableEntity
            });
        var httpClient = new HttpClient(mockHttp.Object);

        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock_token");

        var employeeRepo = new EmployeeRepository(httpClient, mockTokenProvider.Object);


        // Act
        var result = await employeeRepo.AddEmployee(employee);

        //Asset
        Assert.True(result == -1);

    }

    [Fact]
    public async Task AddEmployee_ReturnsError_WhenAddingWrongStatus()
    {
        // Arrange
        var employee = new Employee(123, "Name", "email@domain.com", "male", "xx");
        var mockHttp = new Mock<HttpMessageHandler>();
        mockHttp.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.UnprocessableEntity
            });
        var httpClient = new HttpClient(mockHttp.Object);

        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock_token");

        var employeeRepo = new EmployeeRepository(httpClient, mockTokenProvider.Object);


        // Act
        var result = await employeeRepo.AddEmployee(employee);

        //Asset
        Assert.True(result == -1);

    }
    
    [Fact]
    public async Task AddEmployee_ReturnsError_WhenAddingWrongEmail()
    {
        // Arrange
        var employee = new Employee(123, "Name", "email", "male", "xx");
        var mockHttp = new Mock<HttpMessageHandler>();
        mockHttp.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.UnprocessableEntity
            });
        var httpClient = new HttpClient(mockHttp.Object);

        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock_token");

        var employeeRepo = new EmployeeRepository(httpClient, mockTokenProvider.Object);


        // Act
        var result = await employeeRepo.AddEmployee(employee);

        //Asset
        Assert.True(result == -1);

    }

    [Fact]
    public async Task UpdateEmployee_ReturnsTrue()
    {
        // Arrange
        var mockHttp = new Mock<HttpMessageHandler>();
        var employee = new Employee(123, "Name", "email@domain.com", "male", "active");
        var expectedUri = new Uri("https://gorest.co.in/public/v2/users/123");
        mockHttp.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(employee).ToString()),
            });
        var httpClient = new HttpClient(mockHttp.Object);

        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock_token");

        var employeeRepo = new EmployeeRepository(httpClient, mockTokenProvider.Object);


        // Act
        var result = await employeeRepo.UpdateEmployee(employee);

        // Assert
        Assert.True(result);

        mockHttp.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Content != null
                && req.Method == HttpMethod.Put 
                && req.RequestUri == expectedUri 
                && req.Content.ReadAsStringAsync().Result.Contains("123") 
                && req.Content.ReadAsStringAsync().Result.Contains("Name") 
                && req.Content.ReadAsStringAsync().Result.Contains("email@domain.com") 
                && req.Content.ReadAsStringAsync().Result.Contains("male") 
                && req.Content.ReadAsStringAsync().Result.Contains("active")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }
    
    [Fact]
    public async Task DeleteEmployee_ReturnsTrue_WhenAPIReturnsSuccess()
    {
        // Arrange
        var mockHttp = new Mock<HttpMessageHandler>();
        mockHttp.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });
        var httpClient = new HttpClient(mockHttp.Object);

        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock_token");

        var employeeRepo = new EmployeeRepository(httpClient, mockTokenProvider.Object);

        // Act
        var result = await employeeRepo.DeleteEmployee("123");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteEmployee_ReturnsFalse_WhenAPIReturnsNotFound()
    {
        // Arrange
        var mockHttp = new Mock<HttpMessageHandler>();
        mockHttp.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });
        var httpClient = new HttpClient(mockHttp.Object);

        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock_token");

        var employeeRepo = new EmployeeRepository(httpClient, mockTokenProvider.Object);

        // Act
        var result = await employeeRepo.DeleteEmployee("123");

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task GetEmployee_ReturnsEmployee_WhenAPIReturnsSuccess()
    {
        // Arrange
        var mockHttp = new Mock<HttpMessageHandler>();
        var employee = new Employee(123, "Name", "email@domain.com", "male", "active");
        mockHttp.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(employee).ToString()),
            });
        var httpClient = new HttpClient(mockHttp.Object);

        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock_token");

        var employeeRepo = new EmployeeRepository(httpClient, mockTokenProvider.Object);

        // Act
        var result = await employeeRepo.GetEmployee("123");

        // Assert
        Assert.NotNull(result);
        if (result != null)
        {
            Assert.Equal(123, result.Id);
            Assert.Equal("Name", result.Name);
            Assert.Equal("email@domain.com", result.Email);
            Assert.Equal("male", result.Gender);
            Assert.Equal("active", result.Status);
        }
    }

    [Fact]
    public async Task GetEmployee_ReturnsNull_WhenAPIReturnsNotFound()
    {
        // Arrange
        var mockHttp = new Mock<HttpMessageHandler>();
        mockHttp.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });
        var httpClient = new HttpClient(mockHttp.Object);

        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock_token");

        var employeeRepo = new EmployeeRepository(httpClient, mockTokenProvider.Object);

        // Act
        var result = await employeeRepo.GetEmployee("123");

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetEmployees_ReturnsEmployees_WhenAPIReturnsSuccess()
    {
        // Arrange
        var expectedEmployees = new[]
        {
            new Employee(123, "Name", "email@domain.com", "male", "active"),
            new Employee(1234, "Name2", "email2@domain.com", "female", "active"),
            new Employee(12345, "Name3", "email3@domain.com", "female", "inactive")
        };
        var expectedJson = JsonConvert.SerializeObject(expectedEmployees);
        var mockHttp = new Mock<HttpMessageHandler>();
        mockHttp.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedJson, Encoding.UTF8, "application/json")
            });
        var httpClient = new HttpClient(mockHttp.Object);

        var mockTokenProvider = new Mock<ITokenProvider>();
        mockTokenProvider.Setup(tp => tp.GetToken()).Returns("mock_token");

        var employeeRepo = new EmployeeRepository(httpClient, mockTokenProvider.Object);

        // Act
        var result = await employeeRepo.GetEmployees(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEmployees, result);
    }
}