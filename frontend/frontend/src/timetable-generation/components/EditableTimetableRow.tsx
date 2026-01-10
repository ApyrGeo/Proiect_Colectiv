import { ComboBox } from "./ComboBox";
import type { ClassroomProps, EditableHourRow, LocationProps } from "../props";
import type { TeacherProps } from "../../exam/props.ts";
import { useEffect, useState } from "react";

interface Props {
  row: EditableHourRow;
  onUpdate: (id: number | undefined, patch: Partial<EditableHourRow>) => void;
  teachers: TeacherProps[];
  locations: LocationProps[];
}

const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
const intervals = ["08:00-10:00", "10:00-12:00", "12:00-14:00", "14:00-16:00", "16:00-18:00"];
const frequencies = ["Weekly", "FirstWeek", "SecondWeek"];

const EditableTimetableRow: React.FC<Props> = ({ row, onUpdate, teachers, locations }) => {
  const [selectedLocation, setSelectedLocation] = useState<LocationProps | null>(null);
  useEffect(() => {
    if (row.location) {
      setSelectedLocation(row.location);
    }
  }, [row.location]);

  const classrooms: ClassroomProps[] = selectedLocation?.classrooms ?? [];

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

      <td>
        <ComboBox
          options={frequencies.map((f) => ({ value: f, label: f }))}
          value={row.frequency ? { value: row.frequency, label: row.frequency } : undefined}
          onChange={(v) => onUpdate(row.id, { frequency: v.value })}
        />
      </td>

      <td>{row.category}</td>

      <td>{row.format}</td>

      <td>
        <ComboBox
          placeholder="Location"
          options={locations.map((l) => ({
            value: l,
            label: l.name,
          }))}
          value={selectedLocation ? { value: selectedLocation, label: selectedLocation.name } : undefined}
          onChange={(v) => {
            setSelectedLocation(v.value);
            onUpdate(row.id, { classroom: undefined });
          }}
        />
      </td>

      <td>
        <ComboBox
          placeholder="Classroom"
          options={classrooms.map((c) => ({
            value: c,
            label: c.name,
          }))}
          value={row.classroom ? { value: row.classroom, label: row.classroom.name } : undefined}
          onChange={(v) => onUpdate(row.id, { classroom: v.value })}
          disabled={classrooms.length === 0}
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
                  label: row.teacher.user.lastName + " " + row.teacher.user.firstName,
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
