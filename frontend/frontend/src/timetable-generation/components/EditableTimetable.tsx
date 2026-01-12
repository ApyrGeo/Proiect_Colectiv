import EditableTimetableRow from "./EditableTimetableRow";
import type { EditableHourRow, LocationProps } from "../props";
import useTimetableGenerationApi from "../useTimetableGenerationApi";
import { toast } from "react-hot-toast";
import React, { useState } from "react";
import type { TeacherProps } from "../../exam/props.ts";

interface Props {
  facultyId: number;
  rows: EditableHourRow[];
  setRows: React.Dispatch<React.SetStateAction<EditableHourRow[]>>;
  refreshHours: () => Promise<void>;
}

const EditableTimetable: React.FC<Props> = ({ facultyId, rows, setRows }) => {
  const api = useTimetableGenerationApi();
  const [teachers, setTeachers] = useState<TeacherProps[]>([]);
  const [locations, setLocations] = useState<LocationProps[]>([]);
  React.useEffect(() => {
    api.getTeachers(facultyId).then((data) => setTeachers(data));
  }, [facultyId]);
  React.useEffect(() => {
    api.getLocations().then((data) => setLocations(data));
  }, []);

  // Ensure hour interval is in API format (08:00-10:00)
  const ensureAPIFormat = (interval: string): string => {
    if (!interval) return "";
    if (interval.includes(":")) return interval; // Already correct

    // Convert "8-10" to "08:00-10:00"
    const [start, end] = interval.split("-");
    return `${start.padStart(2, "0")}:00-${end.padStart(2, "0")}:00`;
  };

  const updateRow = (id: number, patch: Partial<EditableHourRow>) => {
    const rowIndex = rows.findIndex((r) => r.id === id);
    if (rowIndex === -1) return;

    const originalRow = rows[rowIndex]; // Save original state
    const updatedRow = { ...rows[rowIndex], ...patch };
    const newRows = [...rows];
    newRows[rowIndex] = updatedRow;
    setRows(newRows);

    api
      .updateHour(Number(id), {
        id: Number(id),
        day: updatedRow.day,
        hourInterval: ensureAPIFormat(updatedRow.hourInterval),
        category: updatedRow.category ?? "",
        classroomId: updatedRow.classroom?.id ?? null,
        subjectId: updatedRow.subject?.id,
        teacherId: updatedRow.teacher?.id ?? null,
        studentGroupId: updatedRow.studentGroup?.id ?? null,
        studentSubGroupId: updatedRow.studentSubGroup?.id ?? null,
        frequency: updatedRow.frequency,
        groupYearId: updatedRow.promotion?.id ?? null,
      })
      .then(() => {
        toast.success("Row updated successfully");
      })
      .catch((error) => {
        console.error(error);
        const errorMessage = error.response?.data?.Description || "An error occurred";
        toast.error(errorMessage.split(":")[1]?.trim() || errorMessage);

        // Revert the row to original state instead of refreshing entire table
        const revertedRows = [...rows];
        revertedRows[rowIndex] = originalRow;
        setRows(revertedRows);
      });
  };

  return (
    <div className="timetable">
      <div className="timetable-scroll-container">
        <table>
          <thead>
            <tr className="timetable-header">
              <th>Day</th>
              <th>Interval</th>
              <th>Frequency</th>
              <th>Category</th>
              <th>Format</th>
              <th>Location</th>
              <th>Classroom</th>
              <th>Subject</th>
              <th>Teacher</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((row) => (
              <EditableTimetableRow
                key={row.id}
                row={row}
                onUpdate={(x, y) => x && updateRow(x, y)}
                teachers={teachers ?? []}
                locations={locations ?? []}
              />
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default EditableTimetable;
