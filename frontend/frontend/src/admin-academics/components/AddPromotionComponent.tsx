import { Grid, Slider, Card, Button, ThemeProvider, FormControl, Select, InputLabel, MenuItem } from "@mui/material";
import { theme } from "../theme.ts";

const AddSpecialisationComponent: React.FC = () => {
  return (
    <ThemeProvider theme={theme}>
      <Grid>
        <Card>
          <div className={"academic-title"}>New Promotion</div>
          <FormControl fullWidth>
            <InputLabel id="demo-simple-select-label">Specialization</InputLabel>
            <Select
              labelId="demo-simple-select-label"
              id="demo-simple-select"
              label="Specialization"
              onChange={() => {}}
            >
              <MenuItem value={10}>Ten</MenuItem>
              <MenuItem value={20}>Twenty</MenuItem>
              <MenuItem value={30}>Thirty</MenuItem>
            </Select>
          </FormControl>
          <div className={"academic-label"}>Length</div>
          <Slider defaultValue={3} min={1} max={6} valueLabelDisplay="auto" />
          <Button>Add Promotion</Button>
        </Card>
      </Grid>
    </ThemeProvider>
  );
};

export default AddSpecialisationComponent;
