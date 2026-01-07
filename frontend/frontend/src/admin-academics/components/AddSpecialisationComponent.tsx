import { Grid, TextField, Card, Button, ThemeProvider, FormControl, Select, InputLabel, MenuItem } from "@mui/material";
import { theme } from "../theme.ts";
import type { Faculty } from "../props.ts";
import useAdminAcademicsApi from "../useAdminAcademicsApi.ts";
import { useState } from "react";

type AddSpecialisationProps = {
  faculties: Faculty[];
  refreshFaculties: () => void;
};
const AddSpecialisationComponent: React.FC<AddSpecialisationProps> = (props) => {
  const { postSpecialisation } = useAdminAcademicsApi();

  const [specName, setSpecName] = useState("");
  const [selectedFacultyId, setSelectedFacultyId] = useState<number | null>(null);

  const handleAddSpecialisation = () => {
    if (specName == "" || selectedFacultyId == null) return;

    postSpecialisation({ name: specName, facultyId: selectedFacultyId })
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
          <div className={"academic-title"}>New Specialisation</div>
          <FormControl fullWidth={true}>
            <InputLabel>Faculty</InputLabel>
            <Select
              label="Faculty"
              onChange={(event) => {
                if (!event.target.value) return;
                setSelectedFacultyId(Number(event.target.value));
              }}
            >
              {props.faculties.map((f) => (
                <MenuItem value={f.id} key={f.id}>
                  {f.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <TextField
            onChange={(event) => {
              setSpecName(event.target.value);
            }}
            id="outlined-basic"
            label="Specialization name"
            variant="outlined"
          />
          <Button onClick={handleAddSpecialisation}>Add Specialisation</Button>
        </Card>
      </Grid>
    </ThemeProvider>
  );
};

export default AddSpecialisationComponent;
