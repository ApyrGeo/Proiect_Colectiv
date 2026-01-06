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
const freqs = ["Weekly", "FirstWeek", "SecondWeek"];

const subjects = [
  { id: 1, name: "Mathematics" },
  { id: 2, name: "Programming" },
];

const classrooms = [
  { id: 1, name: "301" },
  { id: 2, name: "302" },
];

const locations = [
  { id: 1, name: "Main Building" },
  { id: 2, name: "Lab Wing" },
];

const teachers = [
  { id: 1, name: "Popescu Ion" },
  { id: 2, name: "Ionescu Maria" },
];

const EditableTimetableRow: React.FC<Props> = ({ row, formations, onUpdate }) => {
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

      <td>
        <ComboBox
          options={freqs.map((f) => ({ value: f, label: f }))}
          value={row.frequency ? { value: row.frequency, label: row.frequency } : undefined}
          onChange={(v) => onUpdate(row.id, { frequency: v.value })}
        />
      </td>

      <td>
        <ComboBox
          placeholder="Group"
          options={formations.map((f) => ({
            value: f.id,
            label: f.name,
          }))}
          value={
            row.formationGroupId
              ? {
                  value: row.formationGroupId,
                  label: formations.find((f) => f.id === row.formationGroupId)?.name ?? "",
                }
              : undefined
          }
          onChange={(v) => onUpdate(row.id, { formationGroupId: v.value })}
          disabled={true}
        />
      </td>

      <td>
        <ComboBox
          placeholder="Location"
          options={locations.map((l) => ({
            value: l.id,
            label: l.name,
          }))}
          value={
            row.locationId
              ? {
                  value: row.locationId,
                  label: locations.find((l) => l.id === row.locationId)?.name ?? "",
                }
              : undefined
          }
          onChange={(v) => onUpdate(row.id, { locationId: v.value })}
        />
      </td>

      <td>
        <ComboBox
          placeholder="Classroom"
          options={classrooms.map((c) => ({
            value: c.id,
            label: c.name,
          }))}
          value={
            row.classroomId
              ? {
                  value: row.classroomId,
                  label: classrooms.find((c) => c.id === row.classroomId)?.name ?? "",
                }
              : undefined
          }
          onChange={(v) => onUpdate(row.id, { classroomId: v.value })}
          disabled={!row.locationId}
        />
      </td>

      <td>
        <ComboBox
          options={types.map((t) => ({ value: t, label: t }))}
          value={row.type ? { value: row.type, label: row.type } : undefined}
          onChange={(v) => onUpdate(row.id, { type: v.value })}
          disabled={true}
        />
      </td>

      <td>
        <ComboBox
          placeholder="Subject"
          options={subjects.map((s) => ({
            value: s.id,
            label: s.name,
          }))}
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
              type: undefined,
              teacherId: undefined,
            })
          }
          disabled={true}
        />
      </td>

      <td>
        <ComboBox
          placeholder="Teacher"
          options={teachers.map((t) => ({
            value: t.id,
            label: t.name,
          }))}
          value={
            row.teacherId
              ? {
                  value: row.teacherId,
                  label: teachers.find((t) => t.id === row.teacherId)?.name ?? "",
                }
              : undefined
          }
          onChange={(v) => onUpdate(row.id, { teacherId: v.value })}
          disabled={!row.subjectId || !row.type}
        />
      </td>
    </tr>
  );
};

export default EditableTimetableRow;
