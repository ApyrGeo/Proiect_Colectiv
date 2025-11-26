using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackForUBB.Repository.EFEntities;

public class Contract
{
	public int Id { get; set; }
	public int SemesterId { get; set; }
	public required PromotionSemester Semester { get; set; }

	public int EnrollmentId { get; set; }
	public required Enrollment Enrollment { get; set; }

	public List<Subject> Subjects { get; set; } = [];
}
