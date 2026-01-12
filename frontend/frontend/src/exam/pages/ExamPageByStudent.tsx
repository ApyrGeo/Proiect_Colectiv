import React, { useEffect, useState } from "react";
import useExamApi from "../ExamApi.ts";
import type { ExamProps, LocationProps, StudentExamRowProps } from "../props.ts";
import "../ExamPage.css";
import { toast } from "react-hot-toast";
import TableGlimmer from "../../components/loading/TableGlimmer.tsx";
import { useTranslation } from "react-i18next";

interface ExamPageByStudentProps {
  id: number;
  firstName: string;
  lastName: string;
}

const ExamPageByStudent: React.FC<ExamPageByStudentProps> = ({ id, firstName, lastName }) => {
  const { getExamsByStudent, getLocations } = useExamApi();

  const { t } = useTranslation();

  const [examRows, setExamRows] = useState<StudentExamRowProps[]>([]);
  const [locations, setLocations] = useState<LocationProps[]>([]);
  const [loadingInitial, setLoadingInitial] = useState(true);

  const fetchExams = async () => {
    try {
      const locs = await getLocations();
      setLocations(locs);

      const data = await getExamsByStudent(id);

      const mapped: StudentExamRowProps[] = [];
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      data.forEach((t: any) => {
        t.examEntries.forEach((e: ExamProps) => {
          const location = locs.find((l: LocationProps) => l.id === e.classroom?.locationId);

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
    } catch {
      toast.error(t("Error_loading_exams"));
      setExamRows([]);
    } finally {
      setLoadingInitial(false);
    }
  };

  useEffect(() => {
    fetchExams();
  }, [id]);

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
      <h2>
        {t("Student")}: {firstName} {lastName}
      </h2>

      <table className="exam-table">
        <thead>
          <tr>
            <th>{t("Subject")}</th>
            <th>{t("Date")}</th>
            <th>{t("Duration")}</th>
            <th>{t("Location")}</th>
            <th>{t("Classroom")}</th>
          </tr>
        </thead>
        <tbody>
          {loadingInitial && <TableGlimmer no_lines={3} no_cols={5} />}
          {!loadingInitial && examRows.length > 0 && (
            <>
              {examRows.map((exam) => (
                <tr key={exam.examId}>
                  <td>{exam.subjectName}</td>
                  <td>{exam.examDate}</td>
                  <td>{exam.examDuration ?? "-"}</td>
                  <td style={{ minWidth: `${locationWidth}px` }}>{exam.locationName ?? "-"}</td>
                  <td style={{ minWidth: `${classroomWidth}px` }}>{exam.classroomName ?? "-"}</td>
                </tr>
              ))}
            </>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default ExamPageByStudent;
