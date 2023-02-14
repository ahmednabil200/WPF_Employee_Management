using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Application.Repositories;
using AutoMapper;
using Newtonsoft.Json;
using Infrastructure;
using Domain;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using Syncfusion.Windows.Forms.Tools;
using System.Text.RegularExpressions;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly EmployeeView? _employeeView;
        private ushort _pageNo = 1;

        public MainWindow(IEmployeeRepository employeeRepository)
        {
            InitializeComponent();
            _employeeRepository = employeeRepository;
            _employeeView = new EmployeeView(_employeeRepository);
            DisplayEmployees();
        }

        #region Helper


        private void DisplayEmployees()
        {
            if (_employeeView is not null)
            {
                var toView = _employeeView.Employees(_pageNo);
                EmployeesLst.ItemsSource = toView;
            }
        }

        private void SuccessRefresh(string message)
        {
            MessageBox.Show(message);
            DisplayEmployees();
        }

        #endregion

        #region Mapping

        private void MapEmployeeToUi(EmployeeView? emp)
        {
            if (emp is not null)
            {
                NameTxt.Text = emp.Name;
                StatusTxt.Text = emp.Status.ToString();
                GenderTxt.Text = emp.Gender.ToString();
                EmailTxt.Text = emp.Email;
                IdText.Text = emp.Id.ToString();
            }
        }

        private EmployeeView? MapEmployeeDetailsFromUi()
        {
            if (!ValidateFields())
            {
                return null;
            }
            var id = string.IsNullOrEmpty(IdText.Text) ? 0 : int.Parse(IdText.Text);
            var emp = new EmployeeView
            {
                Id = id,
                Email = EmailTxt.Text,
                Gender = Enum.Parse<Gender>(GenderTxt.Text,true),
                Status = Enum.Parse<Status>(StatusTxt.Text, true),
                Name = NameTxt.Text
            };

            return emp;
        }

        private bool ValidateFields()
        {
            StringBuilder errorBuilder = new StringBuilder("Invalid input please check below errors");
            errorBuilder.AppendLine(" :");
            errorBuilder.AppendLine("-----------------------------------");
            var isValid = true;
            if (int.TryParse(IdText.Text, out var id))
            {
                if (id < 0)
                {
                    errorBuilder.AppendLine($"Id can not be a negative value");
                    isValid = false;
                }
            }
            else
            {
                errorBuilder.AppendLine($"{id} is not a valid ID");
                isValid = false;
            }
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(emailPattern);

            if (!regex.IsMatch(EmailTxt.Text))
            {
                errorBuilder.AppendLine($"{EmailTxt.Text} is not a valid email");
                isValid = false;
            }

            if (!Enum.TryParse<Gender>(GenderTxt.Text, true, out _))
            {
                errorBuilder.AppendLine($"{GenderTxt.Text} is not a valid gender");
                isValid = false;
            }

            if (!Enum.TryParse<Status>(StatusTxt.Text, true, out _))
            {
                errorBuilder.AppendLine($"{StatusTxt.Text} is not a valid status");
                isValid = false;
            }

            if (!isValid)
            {
                MessageBox.Show(errorBuilder.ToString());
            }
            return isValid;
        }

        private static Employee MapEmployeeFromView(EmployeeView emp)
        {
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<EmployeeView, Employee>()
            );
            var mapper = new Mapper(config);
            return mapper.Map<Employee>(emp);
        }

        private static EmployeeView MapEmployeeFromModel(Employee emp)
        {
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<Employee, EmployeeView>()
            );
            var mapper = new Mapper(config);
            return mapper.Map<EmployeeView>(emp);
        }


        #endregion

        #region Events

        private void EmployeesLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = EmployeesLst.SelectedItem;

            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<Employee, EmployeeView>()
            );

            var mapper = new Mapper(config);
            var emp = mapper.Map<EmployeeView>(selected);
            MapEmployeeToUi(emp);
        }

        private void PageNumberTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ushort.TryParse(PageNumberTxt.Text, out var pageNumber))
            {
                if (pageNumber != 0)
                {
                    _pageNo = pageNumber;
                    DisplayEmployees();
                    return;
                }
            }

            PageNumberTxt.Text = "1";
        }

        private void NextBtn_Click_1(object sender, RoutedEventArgs e)
        {
            PageNumberTxt.Text = (++_pageNo).ToString();
        }

        private void PrevBtn_Click(object sender, RoutedEventArgs e)
        {
            PageNumberTxt.Text = (--_pageNo).ToString();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var employeeDetails = MapEmployeeDetailsFromUi();
            if(employeeDetails == null)
                return;
            var emp = MapEmployeeFromView(employeeDetails);
            if (_employeeView != null)
            {
                var id = _employeeView.AddEmployee(emp);
                if (id > 0)
                {
                    SuccessRefresh($"Employee has been added successfully with id {id}");
                }
                else
                {
                    MessageBox.Show("Internal error, please check logs");
                }
            }

        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            PageNumberTxt.Text = "1";
            _employeeView?.Employees(_pageNo, SearchTxt.Text);
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            var employeeDetails = MapEmployeeDetailsFromUi();
            if (employeeDetails == null)
                return;
            var emp = MapEmployeeFromView(employeeDetails);
            var result = _employeeView?.UpdateEmployee(emp);
            if (result.HasValue && result.Value)
            {
                SuccessRefresh("Employee has been updated successfully");
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RetrieveTxt.Text, out var id))
            {
                var result = _employeeView?.GetEmployee(id);
                if (result is not null)
                {
                    MapEmployeeToUi(MapEmployeeFromModel(result));
                }
                else
                {
                    MessageBox.Show("No Employee with the given Id");
                }
            }
            else
            {
                MessageBox.Show("Invalid user Id input");
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(IdText.Text))
            {
                MessageBox.Show("No employee has been selected");
                return;
            }
            var confirmation = MessageBox.Show("Are you sure you want to delete this employee", "The Title", MessageBoxButton.YesNo, icon: MessageBoxImage.Question);
            if (confirmation == MessageBoxResult.Yes)
            {

                var result = _employeeView?.DeleteEmployee(int.Parse(IdText.Text));
                if (result.HasValue && result.Value)
                {
                    SuccessRefresh("Employee has been deleted successfully");
                }
            }

        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {

            var filePath = $"employees_page{_pageNo}.csv";

            using var streamWriter = new StreamWriter(filePath)
            {
                NewLine = null,
                AutoFlush = false
            };
            streamWriter.WriteLine("Id,Name,Email,Gender,Status");

            if (_employeeView is not null)
            {
                foreach (var employee in _employeeView.Employees(_pageNo))
                {
                    streamWriter.WriteLine(
                        $"{employee.Id},{employee.Name},{employee.Email},{employee.Gender},{employee.Status}");
                }

            }
            SuccessRefresh($"Employees records have been exported to {filePath}");

        }

        #endregion

    }
}

