using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Shared_Library.DTO
{
    public class UserDTO
    {
        int Id {  get; set; }
        string Name { get; set; } = "";
        UserRole Role { get; set; }

    }

    public enum UserRole {
        Student = 0,
        Teacher = 1,
    }
}
