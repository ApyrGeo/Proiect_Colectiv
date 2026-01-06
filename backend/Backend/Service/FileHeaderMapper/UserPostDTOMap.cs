using CsvHelper.Configuration;
using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Service.FileHeaderMapper;
internal sealed class UserPostDTOMap : ClassMap<UserPostDTO>
{
    public UserPostDTOMap()
    {
        Map(x => x.FirstName).Name("FirstName", "First Name", "firstname", "first name", "first_name");
        Map(x => x.LastName).Name("LastName", "Last Name", "lastname", "last name", "last_name");
        Map(x => x.PhoneNumber).Name("PhoneNumber", "Phone Number", "phonenumber", "phone number", "phone_number");
        Map(x => x.Email).Name("Email", "email");
        Map(x => x.Role).Name("UserRole", "User Role", "userrole", "user role", "user_role", "Role", "role");
    }
}
