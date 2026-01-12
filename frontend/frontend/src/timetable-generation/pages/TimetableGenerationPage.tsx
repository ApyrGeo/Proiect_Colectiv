import React, { useEffect, useState } from "react";
import "../timetable-generation.css";

import useTimetableGenerationApi from "../useTimetableGenerationApi.ts";
import EditableTimetable from "../components/EditableTimetable";
import Circular from "../../components/loading/Circular";

import { ComboBox, type ComboOption } from "../components/ComboBox";
import type { EditableHourRow, SpecialisationProps, FacultyProps, SemesterProps, GroupYearProps } from "../props";
import { useTranslation } from "react-i18next";

const TimetableGenerationPage: React.FC = () => {
  const api = useTimetableGenerationApi();

  const [faculties, setFaculties] = useState<FacultyProps[]>([]);
  const [specialisations, setSpecialisations] = useState<SpecialisationProps[]>([]);
  const [years, setYears] = useState<GroupYearProps[]>([]);
  const [semesters, setSemesters] = useState<SemesterProps[]>([]);

  const { t } = useTranslation();

  const fetchSemesters = (promotion: ComboOption<GroupYearProps>) => {
    if (!promotion) {
      setSemesters([]);
      return;
    }
    setSemesters(promotion.value.semesters);
  };

  const [selectedFaculty, setSelectedFaculty] = useState<ComboOption<FacultyProps> | null>(null);
  const [selectedSpecialisation, setSelectedSpecialisation] = useState<ComboOption<SpecialisationProps> | null>(null);
  const [year, setYear] = useState<ComboOption<GroupYearProps> | null>(null);
  const [semester, setSemester] = useState<ComboOption<SemesterProps> | null>(null);

  const [rows, setRows] = useState<EditableHourRow[]>([]);
  const [loading, setLoading] = useState(false);

  const handleRefreshHours = async () => {
    if (!selectedSpecialisation || !year || !semester) {
      setRows([]);
      return;
    }

    setLoading(true);

    const res = await api.getGeneratedTimetable(year.value.id, semester.value.semesterNumber);
    setRows(res.hours);
    setLoading(false);
  };

  useEffect(() => {
    api.getFaculties().then(setFaculties);
  }, []);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    handleRefreshHours();
  }, [selectedSpecialisation, year, semester]);

  const handleGenerate = async () => {
    if (!selectedSpecialisation || !year || !semester) return;

    setLoading(true);
    await api.generateTimetable(selectedSpecialisation.value.id, semester.value.id);

    const res = await api.getGeneratedTimetable(year.value.id, semester.value.semesterNumber);
    setRows(res.hours);
    setLoading(false);
  };

  return (
    <div className="container">
      <div className="timetable-page">
        <div className="timetable-title">Timetable Generation</div>

        <div className="timetable-filter">
          <ComboBox
            placeholder={t("Faculty")}
            options={faculties.map((f) => ({ value: f, label: f.name }))}
            value={selectedFaculty ?? undefined}
            onChange={(f) => {
              setSelectedFaculty(f);
              setSpecialisations(f.value.specialisations);
              setSelectedSpecialisation(null);
              setYear(null);
              setSemester(null);
            }}
          />
          <ComboBox
            placeholder={t("Specialization")}
            options={specialisations.map((s) => ({ value: s, label: s.name }))}
            value={selectedSpecialisation ?? undefined}
            onChange={(v) => {
              setSelectedSpecialisation(v);
              setYears(v.value.promotions);
              setYear(null);
              setSemester(null);
            }}
            disabled={!selectedFaculty}
          />

          <ComboBox
            placeholder={t("Promotion")}
            options={years.map((y) => ({ value: y, label: `${y.startYear}-${y.endYear}` }))}
            value={year ?? undefined}
            onChange={(v) => {
              setYear(v);
              fetchSemesters(v);
              setSemester(null);
            }}
            disabled={!selectedSpecialisation}
          />

          <ComboBox
            placeholder={t("Semester")}
            options={semesters.map((s) => ({ value: s, label: s.semesterNumber.toString() }))}
            value={semester ?? undefined}
            onChange={(v) => {
              setSemester(v);
            }}
            disabled={!year}
          />
        </div>

        {loading && <Circular />}

        {!loading && rows.length === 0 && selectedSpecialisation && year && semester && (
          <button className="timetable-back-button" onClick={handleGenerate}>
            Generate timetable
          </button>
        )}

        {!loading && rows.length > 0 && (
          <EditableTimetable
            rows={rows}
            setRows={setRows}
            refreshHours={handleRefreshHours}
            facultyId={selectedFaculty?.value.id ?? 0}
          />
        )}
      </div>
    </div>
  );
};

export default TimetableGenerationPage;
