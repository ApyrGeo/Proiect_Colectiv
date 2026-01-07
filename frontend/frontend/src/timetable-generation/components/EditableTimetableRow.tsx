import { ComboBox } from "./ComboBox";
import type { ClassroomProps, EditableHourRow, PutTimeTableGenerationDto } from "../props";
import type { TeacherProps } from "../../exam/props.ts";

interface Props {
  row: EditableHourRow;
  formations: { id: number; name: string }[];
  onUpdate: (id: number | undefined, patch: Partial<EditableHourRow>) => void;
}

const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
const intervals = ["08:00-10:00", "10:00-12:00", "12:00-14:00", "14:00-16:00", "16:00-18:00"];

const classrooms: ClassroomProps[] = [
  { id: 1, name: "A101", locationId: 1 },
  { id: 2, name: "B202", locationId: 2 },
];

const frequency = ["Weekly", "First week", "Second week"];

const teachers: TeacherProps[] = [
  {
    id: 1,
    user: {
      id: 1,
      firstName: "John",
      lastName: "Doe",
      role: 1,
      phoneNumber: "",
      email: "",
    },
    userId: 0,
    facultyId: 0,
  },
  {
    id: 2,
    user: {
      id: 2,
      firstName: "Jane",
      lastName: "Smith",
      role: 1,
      phoneNumber: "",
      email: "",
    },
    userId: 0,
    facultyId: 0,
  },
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
          value={row.hourInterval ? { value: row.hourInterval, label: row.hourInterval } : undefined}
          onChange={(v) => onUpdate(row.id, { hourInterval: v.value })}
        />
      </td>

      <td>{row.category}</td>

      <td>{row.format}</td>

      <td>
        <ComboBox
          placeholder="Classroom"
          options={classrooms.map((c) => ({ value: c, label: c.name }))}
          value={
            row.classroom
              ? {
                  value: row.classroom,
                  label: classrooms.find((c) => c.name === row.classroom?.name)?.name ?? "",
                }
              : undefined
          }
          onChange={(v) => onUpdate(row.id, { classroom: v.value })}
        />
      </td>

      <td>{row.subject?.name}</td>

      <td>
        <ComboBox
          placeholder="Teacher"
          options={teachers.map((t) => ({ value: t, label: t.user.lastName + " " + t.user.firstName }))}
          value={
            row.teacher
              ? {
                  value: row.teacher,
                  label: teachers.find((t) => t.id === row.teacher?.id)?.user.lastName ?? "",
                }
              : undefined
          }
          onChange={(v) => onUpdate(row.id, { teacher: v.value })}
        />
      </td>
    </tr>
  );
};

export default EditableTimetableRow;
