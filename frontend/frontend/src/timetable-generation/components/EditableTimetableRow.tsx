import { ComboBox } from "./ComboBox";
import type { EditableHourRow } from "../props";

interface Props {
  row: EditableHourRow;
  formations: { id: number; name: string }[];
  onUpdate: (id: string, patch: Partial<EditableHourRow>) => void;
}

const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
const intervals = ["08-10", "10-12", "12-14", "14-16", "16-18"];
const types = ["Lecture", "Seminar", "Laboratory"];

const subjects = [
  { id: 1, name: "Programare" },
  { id: 2, name: "Structuri de date" },
];

const classrooms = [
  { id: 1, name: "A101" },
  { id: 2, name: "B202" },
];

const teachers = [
  { id: 1, name: "Popa Edgar" },
  { id: 2, name: "Ionescu Maria" },
];

const EditableTimetableRow: React.FC<Props> = ({ row, onUpdate }) => {
  return (
    <tr className="timetable-row">
      <td>
        <ComboBox
          options={days.map((d) => ({ value: d, label: d }))}
          value={row.day ? { value: row.day, label: row.day } : undefined}
          onChange={(v) => onUpdate(row.id, { day: v.value })}
        />
      </td>

      <td>
        <ComboBox
          options={intervals.map((i) => ({ value: i, label: i }))}
          value={row.interval ? { value: row.interval, label: row.interval } : undefined}
          onChange={(v) => onUpdate(row.id, { interval: v.value })}
        />
      </td>

      <td>{row.type}</td>

      <td>{row.formationGroupId}</td>

      <td>
        <ComboBox
          placeholder="Classroom"
          options={classrooms.map((c) => ({ value: c.id, label: c.name }))}
          value={
            row.classroomId
              ? {
                  value: row.classroomId,
                  label: classrooms.find((c) => c.id === row.classroomId)?.name ?? "",
                }
              : undefined
          }
          onChange={(v) => onUpdate(row.id, { classroomId: v.value })}
        />
      </td>

      <td>
        <ComboBox
          placeholder="Subject"
          options={subjects.map((s) => ({ value: s.id, label: s.name }))}
          value={
            row.subjectId
              ? {
                  value: row.subjectId,
                  label: subjects.find((s) => s.id === row.subjectId)?.name ?? "",
                }
              : undefined
          }
          onChange={(v) =>
            onUpdate(row.id, {
              subjectId: v.value,
              teacherId: undefined,
            })
          }
        />
      </td>

      <td>
        <ComboBox
          placeholder="Teacher"
          options={teachers.map((t) => ({ value: t.id, label: t.name }))}
          value={
            row.teacherId
              ? {
                  value: row.teacherId,
                  label: teachers.find((t) => t.id === row.teacherId)?.name ?? "",
                }
              : undefined
          }
          onChange={(v) => onUpdate(row.id, { teacherId: v.value })}
          disabled={!row.subjectId}
        />
      </td>
    </tr>
  );
};

export default EditableTimetableRow;
