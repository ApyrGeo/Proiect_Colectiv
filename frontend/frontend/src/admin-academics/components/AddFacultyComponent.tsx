import { Grid, TextField, Button, Card, ThemeProvider } from "@mui/material";
import { theme } from "../theme.ts";
import useAdminAcademicsApi from "../useAdminAcademicsApi.ts";
import { useState } from "react";

type AddFacultyProps = {
  refreshFaculties: () => void;
};

const AddFacultyComponent: React.FC<AddFacultyProps> = (props) => {
  const { postFaculty } = useAdminAcademicsApi();

  const [facultyName, setFacultyName] = useState("");

  const handleAddFaculty = () => {
    if (facultyName == "") return;
    postFaculty({ name: facultyName })
      .then(() => {
        props.refreshFaculties();
      })
      .catch(() => {
        //TODO handle error
      });
  };

  return (
    <ThemeProvider theme={theme}>
      <Grid>
        <Card>
          <div className={"academic-title"}>New Faculty</div>
          <TextField
            onChange={(event) => {
              setFacultyName(event.target.value);
            }}
            id="outlined-basic"
            label="Faculty name"
            variant="outlined"
          />
          <Button onClick={handleAddFaculty}>Add Faculty</Button>
        </Card>
      </Grid>
    </ThemeProvider>
  );
};

export default AddFacultyComponent;
