using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public ReportingStructure CountDirectReports(string id)
        {
            //Newing up a ReportingStructure instance, you could also use DI service for this
            ReportingStructure employeeToCount = new ReportingStructure();
            //code to count the direct reports
            int totalReports = 0;

            if (!String.IsNullOrEmpty(id))
            {
                //Adding the employee object to the ReportingStructure object
                employeeToCount.Employee = _employeeRepository.GetById(id);


                //Counting the direct reports of our direct reports
                foreach (Employee report in employeeToCount.Employee.DirectReports ?? Enumerable.Empty<object>())
                {
                    //Counting each direct report
                    totalReports++;

                    foreach(var reportOfReport in report.DirectReports ?? Enumerable.Empty<object>())
                    {
                        //Counting each report of the direct report
                        totalReports++;
                    }
                    
                }
                
                //Adding the total direct report amount to our ReportingStructure object
                employeeToCount.NumberOfReports = totalReports;
            }

            //Return the ReportingStructure (employee record and number of reports)
            return employeeToCount;
                
        }
    }
}
