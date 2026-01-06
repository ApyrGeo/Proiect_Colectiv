import { Grid } from "@mui/material";
import "./academics.css";
import AddFacultyComponent from "./components/AddFacultyComponent.tsx";
import AddSpecialisationComponent from "./components/AddSpecializationComponent.tsx";
import AddPromotionComponent from "./components/AddPromotionComponent.tsx";
import AddGroupsComponent from "./components/AddGroupsComponent.tsx";

const AdminAcademicsPage: React.FC = () => {
  return (
    <Grid container spacing={5} padding={10}>
      <AddFacultyComponent />
      <AddGroupsComponent />
      <AddSpecialisationComponent />
      <AddPromotionComponent />
    </Grid>
  );
};

export default AdminAcademicsPage;
