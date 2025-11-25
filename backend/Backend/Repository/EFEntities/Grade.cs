namespace TrackForUBB.Repository.EFEntities;

public class Grade
{
	public int Id { get; set; }
	public int Value { get; set; }
	public int SubjectId { get; set; }
	public required Subject Subject { get; set; }
	public int SemesterId { get; set; }
	public required PromotionSemester Semester { get; set; }
	public int EnrollmentId { get; set; }
	public required Enrollment Enrollment { get; set; }
}