using System;

namespace Entity.User
{
    public class ApplicationUserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public int Phone { get; set; }
        public string Address { get;set; }
        public DateTime BirthDate { get; set; }
        public string Role { get; set; }
        public int Salary { get; set; }
        public bool IsHead { get; set; }
        public int DepartmentId { get; set; }
        public string IdCard { get; set; }
        public bool DefaultAdminRole { get; set; }
    }
}