import EditableTimetableRow from "./EditableTimetableRow";
import type { EditableHourRow, PutTimeTableGenerationDto } from "../props";
import useTimetableGenerationApi from "../useTimetableGenerationApi";
import { toast } from "react-hot-toast";
import React from "react";

interface Props {
  rows: EditableHourRow[];
  setRows: React.Dispatch<React.SetStateAction<EditableHourRow[]>>;
  refreshHours: () => Promise<void>;
}

const EditableTimetable: React.FC<Props> = ({ rows, setRows, refreshHours }) => {
  console.log(rows);
  const api = useTimetableGenerationApi();

  const updateRow = (id: number, patch: Partial<EditableHourRow>) => {
    const rowIndex = rows.findIndex((r) => r.id === id);
    if (rowIndex === -1) return;

    const updatedRow = { ...rows[rowIndex], ...patch };
    const newRows = [...rows];
    newRows[rowIndex] = updatedRow;
    setRows(newRows);

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
        toast.error(error.response.data.Description.split(":")[1]);
        refreshHours();
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
              <th>Type</th>
              <th>Group</th>
              <th>Classroom</th>
              <th>Subject</th>
              <th>Professor</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((row) => (
              <EditableTimetableRow key={row.id} row={row} onUpdate={updateRow} />
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default EditableTimetable;
