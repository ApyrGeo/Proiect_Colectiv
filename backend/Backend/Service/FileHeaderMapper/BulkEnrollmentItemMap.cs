using CsvHelper.Configuration;
using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Service.FileHeaderMapper;

internal class BulkEnrollmentItemMap : ClassMap<BulkEnrollmentItem>
{
    public BulkEnrollmentItemMap() => Map(x => x.UserEmail).Name("Email", "email", "useremail", "UserEmail", "User Email", "user email", "User_Email");
}
