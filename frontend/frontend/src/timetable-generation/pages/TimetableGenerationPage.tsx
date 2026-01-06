import React, { useEffect, useState } from "react";
import "../timetable-generation.css";

import useTimetableGenerationApi from "../useTimetableGenerationApi";
import EditableTimetable from "../components/EditableTimetable";
import Circular from "../../components/loading/Circular";

import { ComboBox, type ComboOption } from "../components/ComboBox";
import type { EditableHourRow, Semester, SpecialisationProps, FacultyProps } from "../props";
import type { HourProps } from "../../timetable/props";

const hourToEditableRow = (hour: HourProps): EditableHourRow => ({
  id: String(hour.id),
  day: hour.day,
  interval: hour.hourInterval,
  frequency: hour.frequency,
  locationId: hour.location?.id,
  classroomId: hour.classroom?.id,
  type: hour.category,
  subjectId: hour.subject?.id,
  teacherId: hour.teacher?.id,
});

const TimetableGenerationPage: React.FC = () => {
  const api = useTimetableGenerationApi();

  const [faculties, setFaculties] = useState<FacultyProps[]>([]);
  const [specialisations, setSpecialisations] = useState<SpecialisationProps[]>([]);
  const [years, setYears] = useState<ComboOption<number>[]>([]);
  useEffect(() => {
    const currentDate = new Date();
    const currentYear = currentDate.getFullYear();
    const currentMonth = currentDate.getMonth();

    const startingYear = 2025;
    const tempYears: ComboOption<number>[] = [];

    for (let year = startingYear; year < currentYear; year++) {
      tempYears.push({ value: year, label: `${year}-${year + 1}` });
    }

    if (currentMonth >= 8) {
      tempYears.push({ value: currentYear + 1, label: `${currentYear + 1}-${currentYear + 2}` });
    }

    // eslint-disable-next-line react-hooks/set-state-in-effect
    setYears(tempYears);
    console.log(tempYears);
  }, []);

  const [selectedFaculty, setSelectedFaculty] = useState<ComboOption<number> | null>(null);
  const [selectedSpecialisation, setSelectedSpecialisation] = useState<ComboOption<number> | null>(null);
  const [year, setYear] = useState<ComboOption<number> | null>(null);
  const [semester, setSemester] = useState<ComboOption<Semester> | null>(null);

  const [rows, setRows] = useState<EditableHourRow[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    api.getFaculties().then(setFaculties);
    api.getFaculties().then((facs) => {
      const specs: SpecialisationProps[] = [];
      facs.forEach((f) => {
        f.specialisations.forEach((s) => {
          specs.push(s);
        });
      });
      setSpecialisations(specs);
    });
  }, []);

  useEffect(() => {
    if (!selectedSpecialisation || !year || !semester) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setRows([]);
      return;
    }

    setLoading(true);

    api.getGeneratedTimetable(selectedSpecialisation, year, semester).then((res) => {
      setRows(res.map(hourToEditableRow));
      setLoading(false);
    });
  }, [selectedSpecialisation, year, semester]);

  const handleGenerate = async () => {
    if (!selectedSpecialisation || !year || !semester) return;

    setLoading(true);
    await api.generateTimetable(selectedSpecialisation.value, year.value, semester.value);

    const res = await api.getGeneratedTimetable(selectedSpecialisation, year, semester);
    setRows(res.map(hourToEditableRow));
    setLoading(false);
  };

  return (
    <div className="container">
      <div className="timetable-page">
        <div className="timetable-title">Timetable Generation</div>

        <div className="timetable-filter">
          <ComboBox
            placeholder="Faculty"
            options={faculties.map((f) => ({ value: f.id, label: f.name }))}
            value={selectedFaculty ?? undefined}
            onChange={(f) => {
              setSelectedFaculty(f);
              setSelectedSpecialisation(null);
              setYear(null);
              setSemester(null);
            }}
          />
          <ComboBox
            placeholder="Specialisation"
            options={specialisations.map((s) => ({ value: s.id, label: s.name }))}
            value={selectedSpecialisation ?? undefined}
            onChange={(v) => {
              setSelectedSpecialisation(v);
              setYear(null);
              setSemester(null);
            }}
            disabled={!selectedFaculty}
          />

          <ComboBox
            placeholder="Promotion"
            options={years}
            value={year ?? undefined}
            onChange={setYear}
            disabled={!selectedSpecialisation}
          />

          <ComboBox
            placeholder="Semester"
            options={[
              { value: 1, label: "Semester 1" },
              { value: 2, label: "Semester 2" },
            ]}
            value={semester ?? undefined}
            onChange={setSemester}
            disabled={!year}
          />
        </div>

        {loading && <Circular />}

        {!loading && rows.length === 0 && selectedSpecialisation && year && semester && (
          <button className="timetable-back-button" onClick={handleGenerate}>
            Generate timetable
          </button>
        )}

        {!loading && rows.length > 0 && <EditableTimetable rows={rows} setRows={setRows} />}
      </div>
    </div>
  );
};

export default TimetableGenerationPage;
