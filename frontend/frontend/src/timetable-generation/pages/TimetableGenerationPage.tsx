import React, { useEffect, useState } from "react";
import "../timetable-generation.css";

import useTimetableGenerationApi from "../useTimetableGenerationApi";
import EditableTimetable from "../components/EditableTimetable";
import Circular from "../../components/loading/Circular";

import { ComboBox, type ComboOption } from "../components/ComboBox";
import type { EditableHourRow, Semester, SpecialisationProps, GroupProps } from "../props";
import type { HourProps, SubjectProps } from "../../timetable/props";

const hourToEditableRow = (hour: HourProps): EditableHourRow => ({
  id: String(hour.id ?? crypto.randomUUID()),
  day: hour.day,
  interval: hour.hourInterval,
  frequency: hour.frequency,
  formationGroupId: undefined, // backend later
  locationId: hour.location?.id,
  classroomId: hour.classroom?.id,
  type: hour.category,
  subjectId: hour.subject?.id,
  teacherId: hour.teacher?.id,
});


const TimetableGenerationPage: React.FC = () => {
  const api = useTimetableGenerationApi();

  const [specialisations, setSpecialisations] = useState<SpecialisationProps[]>([]);
  const [groups, setGroups] = useState<GroupProps[]>([]);
  const [subjects, setSubjects] = useState<SubjectProps[]>([]);

  const [selectedSpecialisation, setSelectedSpecialisation] = useState<ComboOption<number> | null>(null);
  const [year, setYear] = useState<ComboOption<number> | null>(null);
  const [semester, setSemester] = useState<ComboOption<Semester> | null>(null);

  const [hours, setHours] = useState<HourProps[]>([]);
  const [rows, setRows] = useState<EditableHourRow[]>([]);

  const [loadingFetch, setLoadingFetch] = useState(false);
  const [loadingGenerate, setLoadingGenerate] = useState(false);

  useEffect(() => {
    api.getSpecialisations().then(setSpecialisations);
  }, []);

  useEffect(() => {
    if (!selectedSpecialisation || !year || !semester) {
      setGroups([]);
      setSubjects([]);
      setHours([]);
      setRows([]);
      return;
    }

    api.getGroups(selectedSpecialisation.value, year.value).then(setGroups);
    api.getSubjects(selectedSpecialisation.value, year.value, semester.value).then(setSubjects);
  }, [selectedSpecialisation, year, semester]);

  useEffect(() => {
    if (!selectedSpecialisation || !year || !semester) return;

    setLoadingFetch(true);
    api.getGeneratedTimetable(selectedSpecialisation, year, semester).then((res) => {
      setHours(res);
      setRows(res.map((h) => hourToEditableRow(h)));
      setLoadingFetch(false);
    });
  }, [selectedSpecialisation, year, semester]);

  const handleGenerate = async () => {
    if (!subjects.length) return;

    setLoadingGenerate(true);
    await api.generateTimetable(subjects);
    setLoadingGenerate(false);

    setLoadingFetch(true);
    const res = await api.getGeneratedTimetable(selectedSpecialisation, year, semester);
    setHours(res);
    setRows(res.map((h) => hourToEditableRow(h)));
    setLoadingFetch(false);
  };

  const canGenerate = subjects.length > 0 && hours.length === 0;

  return (
    <div className="container">
      <div className="timetable-page">
        <div className="timetable-title">Timetable Generation</div>

        <div className="timetable-filter">
          <ComboBox
            placeholder="Specialisation"
            options={specialisations.map((s) => ({
              value: s.id,
              label: s.name,
            }))}
            value={selectedSpecialisation ?? undefined}
            onChange={(v) => {
              setSelectedSpecialisation(v);
              setYear(null);
              setSemester(null);
            }}
          />

          <ComboBox
            placeholder="Year"
            options={[
              { value: 1, label: "Year 1" },
              { value: 2, label: "Year 2" },
              { value: 3, label: "Year 3" },
            ]}
            value={year ?? undefined}
            onChange={(v) => {
              setYear(v);
              setSemester(null);
            }}
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

        {(loadingFetch || loadingGenerate) && <Circular />}

        {!loadingFetch && !loadingGenerate && rows.length === 0 && canGenerate && (
          <button className="timetable-back-button" onClick={handleGenerate}>
            Generate timetable
          </button>
        )}

        {!loadingFetch && rows.length > 0 && (
          <EditableTimetable rows={rows} setRows={setRows} canEdit={true} formations={groups} onAddRow={() => {}} />
        )}
      </div>
    </div>
  );
};

export default TimetableGenerationPage;
