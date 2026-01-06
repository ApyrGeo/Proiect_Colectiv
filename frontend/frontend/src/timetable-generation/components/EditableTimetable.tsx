import EditableTimetableRow from "./EditableTimetableRow";
import type { EditableHourRow } from "../props";

interface Props {
  rows: EditableHourRow[];
  setRows: React.Dispatch<React.SetStateAction<EditableHourRow[]>>;
  onAddRow: () => void;
  canEdit: boolean;
  formations: { id: number; name: string }[];
}

const EditableTimetable: React.FC<Props> = ({ rows, setRows, onAddRow, canEdit, formations }) => {
  const updateRow = (id: string, patch: Partial<EditableHourRow>) => {
    setRows((prev) => prev.map((r) => (r.id === id ? { ...r, ...patch } : r)));
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
              <th>Formation</th>
              <th>Location</th>
              <th>Classroom</th>
              <th>Type</th>
              <th>Subject</th>
              <th>Professor</th>
            </tr>
          </thead>

          <tbody>
            {rows.map((row) => (
              <EditableTimetableRow key={row.id} row={row} formations={formations} onUpdate={updateRow} />
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default EditableTimetable;
