import "./academics.css";
import AddFacultyComponent from "./components/AddFacultyComponent.tsx";
import AddSpecialisationComponent from "./components/AddSpecialisationComponent.tsx";
import AddPromotionComponent from "./components/AddPromotionComponent.tsx";
import AddGroupsComponent from "./components/AddGroupsComponent.tsx";
import useAdminAcademicsApi from "./useAdminAcademicsApi.ts";
import { useCallback, useEffect, useState } from "react";
import type { Faculty } from "./props.ts";
import { toast } from "react-hot-toast";
import { useTranslation } from "react-i18next";

const AdminAcademicsPage: React.FC = () => {
  const { getFaculties } = useAdminAcademicsApi();
  const [faculties, setFaculties] = useState<Faculty[]>([]);
  const { t } = useTranslation();
  const [error, setError] = useState("");
  useEffect(() => {
    if (error) {
      toast.error(t("Error"));
    }
  }, [error, t]);

  const refreshFaculties = useCallback(() => {
    getFaculties()
      .then((res) => {
        setFaculties(res);
      })
      .catch(() => {
        setError("Couldn't fetch faculties");
      });
  }, [getFaculties]);

  useEffect(() => {
    if (error != "") return;

    refreshFaculties();
  }, [error, refreshFaculties]);

  return (
    <div className={"admin-academic-page"}>
      <div className={"admin-academic-column"}>
        <AddFacultyComponent refreshFaculties={refreshFaculties} />
        <AddSpecialisationComponent faculties={faculties} refreshFaculties={refreshFaculties} />
        <AddPromotionComponent faculties={faculties} refreshFaculties={refreshFaculties} />
      </div>
      <div className={"admin-academic-column"}>
        <AddGroupsComponent faculties={faculties} refreshFaculties={refreshFaculties} />
      </div>
    </div>
  );
};

export default AdminAcademicsPage;
