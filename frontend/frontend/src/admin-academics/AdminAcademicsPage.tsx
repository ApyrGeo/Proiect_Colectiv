import { Grid } from "@mui/material";
import "./academics.css";
import AddFacultyComponent from "./components/AddFacultyComponent.tsx";
import AddSpecialisationComponent from "./components/AddSpecializationComponent.tsx";
import AddPromotionComponent from "./components/AddPromotionComponent.tsx";
import AddGroupsComponent from "./components/AddGroupsComponent.tsx";
import useAdminAcademicsApi from "./useAdminAcademicsApi.ts";
import { useCallback, useEffect, useState } from "react";
import type { Faculty } from "./props.ts";

const AdminAcademicsPage: React.FC = () => {
  const { getFaculties } = useAdminAcademicsApi();
  const [faculties, setFaculties] = useState<Faculty[]>([]);
  const [fetchError, setFetchError] = useState("");

  const refreshFaculties = useCallback(() => {
    getFaculties()
      .then((res) => {
        setFaculties(res.faculties);
      })
      .catch(() => {
        setFetchError("Couldn't fetch faculties");
      });
  }, [getFaculties]);

  useEffect(() => {
    if (fetchError == "") return;

    refreshFaculties();
  }, [fetchError, refreshFaculties]);

  return (
    <Grid container spacing={5} padding={10}>
      <AddFacultyComponent refreshFaculties={refreshFaculties} />
      <AddSpecialisationComponent faculties={faculties} refreshFaculties={refreshFaculties} />
      <AddPromotionComponent faculties={faculties} refreshFaculties={refreshFaculties} />
      <AddGroupsComponent faculties={faculties} refreshFaculties={refreshFaculties} />
    </Grid>
  );
};

export default AdminAcademicsPage;
