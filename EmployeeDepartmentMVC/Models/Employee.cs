using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeDepartmentMVC.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name ="Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Department Id")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
