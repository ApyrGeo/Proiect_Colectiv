import type { ScholarshipStatus } from "../props";

const ScholarshipStatusComponent = ({ status }: { status: ScholarshipStatus }) => {
  return (
    <div className="scholarship-status">
      <h3>Scholarship Status</h3>
      <p>
        <strong>Average Score:</strong> {status.averageScore.toFixed(2)}
      </p>
      <p>
        <strong>Rank:</strong> {status.rank} out of {status.totalStudents} students
      </p>
      <p>
        <strong>Eligibility:</strong> {status.isEligible ? "Eligible" : "Not Eligible"}
      </p>
      {status.isEligible && status.scholarshipType && (
        <p>
          <strong>Scholarship Type:</strong> {status.scholarshipType}
        </p>
      )}
    </div>
  );
};
export default ScholarshipStatusComponent;
