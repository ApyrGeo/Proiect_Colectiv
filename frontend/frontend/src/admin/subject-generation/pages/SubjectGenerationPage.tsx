import React, { useEffect, useState } from "react";
import { ComboBox, type ComboOption } from "../../../timetable-generation/components/ComboBox.tsx";
import { useTranslation } from "react-i18next";
import type { FacultyProps, GroupYearProps, SemesterProps, SpecialisationProps } from "../props.ts";
import "../subject-generation.css";
import useSubjectGenerationApi from "../useSubjectGenerationApi.ts";
import type { TeacherProps } from "../../../exam/props.ts";
import { toast } from "react-hot-toast";

type SubjectType = "Optional" | "Required" | "Facultative";
type FormationType = "Course_Seminar" | "Course_Laboratory" | "Course_Seminar_Laboratory";

const SubjectGenerationPage: React.FC = () => {
  const api = useSubjectGenerationApi();

  const { t } = useTranslation();

  const [faculties, setFaculties] = useState<FacultyProps[]>([]);
  const [specialisations, setSpecialisations] = useState<SpecialisationProps[]>([]);
  const [promotions, setPromotions] = useState<GroupYearProps[]>([]);

  const [selectedFaculty, setSelectedFaculty] = useState<ComboOption<FacultyProps> | null>(null);
  const [selectedSpecialisation, setSelectedSpecialisation] = useState<ComboOption<SpecialisationProps> | null>(null);
  const [selectedPromotion, setSelectedPromotion] = useState<ComboOption<GroupYearProps> | null>(null);

  const [name, setName] = useState("");
  const [credits, setCredits] = useState<number>(0);
  const [teacher, setTeacher] = useState<ComboOption<TeacherProps> | null>(null);
  const [code, setCode] = useState("");
  const [subjectType, setSubjectType] = useState<ComboOption<SubjectType> | null>(null);
  const [optionalPackage, setOptionalPackage] = useState<number>(0);
  const [semester, setSemester] = useState<ComboOption<SemesterProps> | null>(null);
  const [formation, setFormation] = useState<FormationType | null>(null);

  const [teachers, setTeachers] = useState<TeacherProps[]>([]);
  React.useEffect(() => {
    if (!selectedFaculty) {
      setTeachers([]);
      return;
    }
    api.getTeachers(selectedFaculty?.value.id).then((data) => {
      setTeachers(data);
    });
  }, [selectedFaculty]);
  const [semesters, setSemesters] = useState<SemesterProps[]>([]);

  const subjectTypes: ComboOption<SubjectType>[] = [
    { value: "Optional", label: "Optional" },
    { value: "Required", label: "Required" },
    { value: "Facultative", label: "Facultative" },
  ];

  const formationTypes: ComboOption<FormationType>[] = [
    { value: "Course_Seminar", label: "Course + Seminar" },
    { value: "Course_Laboratory", label: "Course + Laboratory" },
    { value: "Course_Seminar_Laboratory", label: "Course + Seminar + Laboratory" },
  ];

  useEffect(() => {
    api.getFaculties().then(setFaculties);
  }, []);

  const fetchSemesters = (promotion: ComboOption<GroupYearProps>) => {
    if (!promotion) {
      setSemesters([]);
      return;
    }
    setSemesters(promotion.value.semesters);
  };

  const handleSubmit = () => {
    if (!code || !name || !subjectType || !semester || !formation || !teacher) {
      toast("Please fill in all required fields.");
      return;
    }
    if (credits <= 0 || credits > 10) {
      toast("Credits must be between 1 and 10.");
      return;
    }
    if (subjectType.value === "Optional" && (optionalPackage < 1 || optionalPackage > 6)) {
      toast("Optional package must be between 1 and 6 for Optional subjects.");
      return;
    }
    api
      .addSubject({
        name,
        code,
        numberOfCredits: credits,
        holderTeacherId: teacher.value.id,
        semesterId: semester.value.id,
        type: subjectType.value,
        formationType: formation,
        optionalPackage,
      })
      .then(() => {
        toast.success("Subject created successfully.");
        setName("");
        setCredits(0);
        setTeacher(null);
        setCode("");
        setSubjectType(null);
        setOptionalPackage(0);
        setSemester(null);
        setFormation(null);
      })
      .catch((error) => {
        console.error(error);
        toast.error("Failed to create subject. " + (error.response?.data?.Description || ""));
      });
  };

  return (
    <div className="container">
      <div className="subject-page">
        <div className="subject-title">Subject Generation</div>

        <div className="subject-filter">
          <ComboBox
            placeholder={t("Faculty")}
            options={faculties.map((f) => ({ value: f, label: f.name }))}
            value={selectedFaculty ?? undefined}
            onChange={(f) => {
              setSelectedFaculty(f);
              console.log(faculties);
              setSpecialisations(f.value.specialisations);
              setSelectedSpecialisation(null);
              setSelectedSpecialisation(null);
            }}
          />
          <ComboBox
            placeholder={t("Specialization")}
            options={specialisations.map((s) => ({ value: s, label: s.name }))}
            value={selectedSpecialisation ?? undefined}
            onChange={(v) => {
              setSelectedSpecialisation(v);
              setPromotions(v.value.promotions);
              setSelectedPromotion(null);
            }}
            disabled={!selectedFaculty}
          />
          <ComboBox
            placeholder={t("Promotion")}
            options={promotions.map((p) => ({ value: p, label: `${p.startYear}-${p.endYear}` }))}
            value={selectedPromotion ?? undefined}
            onChange={(v) => {
              setSelectedPromotion(v);
              fetchSemesters(v);
            }}
            disabled={!selectedSpecialisation}
          />
        </div>

        <div className="container">
          <div className="subject-generation-container">
            <div className="subject-form-card">
              <div className="subject-form-group">
                <label className="subject-form-label">Name</label>
                <input
                  type="text"
                  className="subject-form-input"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="Enter subject name"
                />
              </div>

              <div className="subject-form-group">
                <label className="subject-form-label">Code</label>
                <input
                  type="text"
                  className="subject-form-input"
                  value={code}
                  onChange={(e) => setCode(e.target.value)}
                  placeholder="e.g. CS101"
                />
              </div>

              <div className="subject-form-group">
                <label className="subject-form-label">Number of Credits</label>
                <input
                  type="number"
                  className="subject-form-number"
                  value={credits}
                  onChange={(e) => setCredits(Number(e.target.value))}
                  min={1}
                  max={10}
                />
              </div>

              <div className="subject-form-group">
                <label className="subject-form-label">Optional Package</label>
                <input
                  type="number"
                  max={6}
                  min={1}
                  className="subject-form-number"
                  value={optionalPackage}
                  onChange={(e) => setOptionalPackage(Number(e.target.value))}
                />
              </div>

              <div className="subject-form-group">
                <label className="subject-form-label">Teacher</label>
                <ComboBox
                  placeholder="Select teacher"
                  options={teachers.map((t) => ({ value: t, label: t.user.lastName + " " + t.user.firstName }))}
                  value={teacher ?? undefined}
                  onChange={setTeacher}
                />
              </div>

              <div className="subject-form-group">
                <label className="subject-form-label">Subject Type</label>
                <ComboBox
                  placeholder="Select type"
                  options={subjectTypes}
                  value={subjectType ?? undefined}
                  onChange={setSubjectType}
                />
              </div>

              <div className="subject-form-group">
                <label className="subject-form-label">Semesters</label>
                <ComboBox
                  placeholder="Select semester"
                  options={semesters.map((s) => ({ value: s, label: s.semesterNumber.toString() }))}
                  value={semester ?? undefined}
                  onChange={setSemester}
                />
              </div>

              <div className="subject-form-group">
                <label className="subject-form-label">Formation Type</label>
                <ComboBox
                  placeholder="Formation type"
                  options={formationTypes}
                  value={formation ? { value: formation, label: formation } : undefined}
                  onChange={(v) => setFormation(v.value)}
                />
              </div>

              <button className="subject-submit-button" onClick={handleSubmit}>
                Create Subject
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SubjectGenerationPage;
