import EditableTimetableRow from "./EditableTimetableRow";
import type { EditableHourRow, LocationProps } from "../props";
import useTimetableGenerationApi from "../useTimetableGenerationApi.ts";
import { toast } from "react-hot-toast";
import React, { useState } from "react";
import type { TeacherProps } from "../../exam/props.ts";

interface Props {
  facultyId: number;
  rows: EditableHourRow[];
  setRows: React.Dispatch<React.SetStateAction<EditableHourRow[]>>;
  refreshHours: () => Promise<void>;
}

const EditableTimetable: React.FC<Props> = ({ facultyId, rows, setRows, refreshHours }) => {
  const api = useTimetableGenerationApi();
  const [teachers, setTeachers] = useState<TeacherProps[]>([]);
  const [locations, setLocations] = useState<LocationProps[]>([]);
  React.useEffect(() => {
    api.getTeachers(facultyId).then((data) => setTeachers(data));
  }, [facultyId]);
  React.useEffect(() => {
    api.getLocations().then((data) => setLocations(data));
  }, []);

  const updateRow = (id: number, patch: Partial<EditableHourRow>) => {
    const rowIndex = rows.findIndex((r) => r.id === id);
    if (rowIndex === -1) return;

    const updatedRow = { ...rows[rowIndex], ...patch };
    const newRows = [...rows];
    newRows[rowIndex] = updatedRow;
    setRows(newRows);

    console.log(updatedRow);

    api
      .updateHour(Number(id), {
        id: Number(id),
        day: updatedRow.day,
        hourInterval: updatedRow.hourInterval,
        category: updatedRow.category ?? "",
        classroomId: updatedRow.classroom?.id ?? null,
        subjectId: updatedRow.subject?.id,
        teacherId: updatedRow.teacher?.id ?? null,
        studentGroupId: updatedRow.studentGroup?.id ?? null,
        studentSubGroupId: updatedRow.studentSubGroup?.id ?? null,
        frequency: updatedRow.frequency,
        groupYearId: updatedRow.promotion?.id ?? null,
      })
      .then(() => {})
      .catch((error) => {
        console.log(error);
        toast.error(error.response.data.Description.split(":")[1]);
        refreshHours();
      });
  };

  return (
    <div className="timetable">
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
              onUpdate={updateRow}
              teachers={teachers ?? []}
              locations={locations ?? []}
            />
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default EditableTimetable;
