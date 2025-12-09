import React, { useEffect, useState } from "react";
import useExamApi from "../ExamApi.ts";
import type { LocationProps, StudentExamRowProps, StudentIdProps } from "../props.ts";
import "../ExamPage.css";

const ExamPageByStudent: React.FC<StudentIdProps> = ({ id }) => {
  const { getExamsByStudent, getLocations } = useExamApi();

  const [examRows, setExamRows] = useState<StudentExamRowProps[]>([]);
  const [locations, setLocations] = useState<LocationProps[]>([]);
  const [loadingInitial, setLoadingInitial] = useState(true);

  const fetchExams = async () => {
    try {
      const locs = await getLocations();
      setLocations(locs);

      const data = await getExamsByStudent(id);

      const mapped: StudentExamRowProps[] = [];
      data.forEach((t: any) => {
        t.examEntries.forEach((e: any) => {
          const location = locs.find((l: any) => l.id === e.classroom?.locationId);

          mapped.push({
            examId: e.id,
            subjectName: e.subject?.name ?? "",
            examDate: e.date ? new Date(e.date).toLocaleString() : "N/A",
            examDuration: e.duration ?? null,
            locationName: location?.name ?? "-",
            classroomName: e.classroom?.name ?? "-",
          });
        });
      });

      setExamRows(mapped);
    } catch (err) {
      console.error("Eroare la încărcarea examenelor:", err);
      setExamRows([]);
    } finally {
      setLoadingInitial(false);
    }
  };

  useEffect(() => {
    fetchExams();
  }, [id]);

  // Loader pentru încărcarea inițială
  function BigSpinner() {
    return <h2 className="loading-center">🌀 Se încarcă datele...</h2>;
  }

  function Glimmer() {
    return (
      <div className="glimmer-panel">
        <div className="glimmer-line" />
        <div className="glimmer-line" />
        <div className="glimmer-line" />
      </div>
    );
  }

  if (loadingInitial) return <BigSpinner />;

  const getMaxLocationWidth = () => {
    const maxLength = Math.max(...locations.map((l) => l.name.length), 0);
    return maxLength * 8 + 20;
  };

  const getMaxClassroomWidth = () => {
    let maxLength = 0;
    examRows.forEach((exam) => {
      if (exam.classroomName && exam.classroomName.length > maxLength) maxLength = exam.classroomName.length;
    });
    return maxLength * 8 + 20;
  };

  const locationWidth = getMaxLocationWidth();
  const classroomWidth = getMaxClassroomWidth();

  return (
    <div className="exam-page-container">
      <h2>Examene Student</h2>

      {!examRows.length && <Glimmer />}

      {examRows.length > 0 && (
        <table className="exam-table">
          <thead>
            <tr>
              <th>Materie</th>
              <th>Data examen</th>
              <th>Durată (min)</th>
              <th>Locație</th>
              <th>Clasă</th>
            </tr>
          </thead>
          <tbody>
            {examRows.map((exam) => (
              <tr key={exam.examId}>
                <td>{exam.subjectName}</td>
                <td>{exam.examDate}</td>
                <td>{exam.examDuration ?? "-"}</td>
                <td style={{ minWidth: `${locationWidth}px` }}>{exam.locationName ?? "-"}</td>
                <td style={{ minWidth: `${classroomWidth}px` }}>{exam.classroomName ?? "-"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default ExamPageByStudent;
