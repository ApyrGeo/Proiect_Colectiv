import EditableTimetableRow from "./EditableTimetableRow";
import type { EditableHourRow } from "../props";
import useTimetableGenerationApi from "../useTimetableGenerationApi";

interface Props {
  rows: EditableHourRow[];
  setRows: React.Dispatch<React.SetStateAction<EditableHourRow[]>>;
}

const EditableTimetable: React.FC<Props> = ({ rows, setRows }) => {
  const api = useTimetableGenerationApi();

  const updateRow = (id: string, patch: Partial<EditableHourRow>) => {
    setRows((prev) => prev.map((r) => (r.id === id ? { ...r, ...patch } : r)));

    api.updateHour(Number(id), {
      day: patch.day,
      hourInterval: patch.interval,
      category: patch.type,
      classroomId: patch.classroomId,
      subjectId: patch.subjectId,
      teacherId: patch.teacherId,
      studentGroupId: patch.formationGroupId,
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
