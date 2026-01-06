import { Grid, TextField, Card, Button, ThemeProvider, FormControl, Select, InputLabel, MenuItem } from "@mui/material";
import { theme } from "../theme.ts";

const AddSpecialisationComponent: React.FC = () => {
  return (
    <ThemeProvider theme={theme}>
      <Grid>
        <Card>
          <div className={"academic-title"}>New Specialization</div>
          <FormControl fullWidth>
            <InputLabel id="demo-simple-select-label">Faculty</InputLabel>
            <Select labelId="demo-simple-select-label" id="demo-simple-select" label="Faculty" onChange={() => 1}>
              <MenuItem value={10}>Ten</MenuItem>
              <MenuItem value={20}>Twenty</MenuItem>
              <MenuItem value={30}>Thirty</MenuItem>
            </Select>
          </FormControl>
          <TextField id="outlined-basic" label="Specialization name" variant="outlined" />
          <Button>Add Specialization</Button>
        </Card>
      </Grid>
    </ThemeProvider>
  );
};

export default AddSpecialisationComponent;
