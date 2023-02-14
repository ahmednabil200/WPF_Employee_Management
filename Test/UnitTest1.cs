using Newtonsoft.Json;
using System.Net;
using System.Text;
using Application.Repositories;
using Domain;
using Infrastructure;
using Moq;
using System.Net.Http.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.Protected;

namespace Test
{
    [TestClass]
    public class EmployeeRepositoryTests
    {
        private EmployeeRepository _employeeRepository;
        private Mock<ITokenProvider> _tokenProviderMock;
        private Mock<HttpClient> _httpClientMock;

        [TestInitialize]
        public void Setup()
        {
            _tokenProviderMock = new Mock<ITokenProvider>();
            _tokenProviderMock.Setup(x => x.GetToken()).Returns("some-token");
            _httpClientMock = new Mock<HttpClient>();

            _employeeRepository = new EmployeeRepository(_httpClientMock.Object, _tokenProviderMock.Object);
        }

        [TestMethod]
        public async Task AddEmployee_Should_Return_Employee_Id()
        {

            // Instantiate the mock object
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            // Set up the SendAsync method behavior.
            httpMessageHandlerMock
                .Protected() // <= here is the trick to set up protected!!!
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage());
            // create the HttpClient
            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var Client = httpClient.PostAsync("employees", null, CancellationToken.None);

            // Arrange
            var employee = new Employee(0, "abc", "abc@abc.abc", "male", "active");
            var expectedId = 123;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new Employee(id:expectedId,"","","","")))
            };
            _httpClientMock.Setup(x => x.PostAsJsonAsync(It.IsAny<Uri>(), It.IsAny<Employee>(),CancellationToken.None)).ReturnsAsync(response);

            // Act
            var id = await _employeeRepository.AddEmployee(employee);

            // Assert
            Assert.AreEqual(expectedId, id);
        }

        [TestMethod]
        public async Task UpdateEmployee_Should_Return_True_On_Success()
        {
            // Arrange
            var employee = new Employee(123, "abc", "abc@abc.abc", "male", "active");
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            };
            _httpClientMock.Setup(x => x.PutAsJsonAsync(It.IsAny<string>(), It.IsAny<Employee>(),CancellationToken.None)).ReturnsAsync(response);

            // Act
            var result = await _employeeRepository.UpdateEmployee(employee);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteEmployee_Should_Return_True_On_Success()
        {
            // Arrange
            var employeeId = "123";
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            };
            _httpClientMock.Setup(x => x.DeleteAsync(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await _employeeRepository.DeleteEmployee(employeeId);

            // Assert
            Assert.IsTrue(result);
        }
    }
}