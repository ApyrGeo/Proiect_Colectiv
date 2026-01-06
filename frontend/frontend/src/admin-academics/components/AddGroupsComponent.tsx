import {
  Grid,
  Slider,
  Card,
  Button,
  ThemeProvider,
  TextField,
  FormControl,
  Select,
  InputLabel,
  MenuItem,
} from "@mui/material";
import { theme } from "../theme.ts";
import { useState } from "react";

const AddSpecialisationComponent: React.FC = () => {
  const [groupCount, setGroupCount] = useState(1);

  const [groupNames, setGroupNames] = useState<string[]>(["1"]);

  return (
    <ThemeProvider theme={theme}>
      <Grid>
        <Card>
          <div className={"academic-title"}>New Groups</div>
          <div className={"academic-row"}>
            <div className={"academic-column"}>
              <FormControl fullWidth>
                <InputLabel id="demo-simple-select-label">Promotion</InputLabel>
                <Select labelId="demo-simple-select-label" id="demo-simple-select" label="Promotion" onChange={() => 1}>
                  <MenuItem value={10}>Ten</MenuItem>
                  <MenuItem value={20}>Twenty</MenuItem>
                  <MenuItem value={30}>Thirty</MenuItem>
                </Select>
              </FormControl>
              <div className={"academic-label"}>Count</div>
              <Slider
                defaultValue={groupCount}
                min={1}
                max={20}
                value={groupCount}
                onChange={(_event, value) => {
                  setGroupCount(value);
                  const names = [];
                  for (let i = 0; i < value; i++) names.push(String(value));
                  setGroupNames(names);
                }}
                valueLabelDisplay="auto"
              />
              <Button>Add Promotion</Button>
            </div>
            <div className={"academic-column"}>
              <div className={"academic-label"}>Group Names</div>
              {groupNames.map((value) => (
                <TextField defaultValue={value}></TextField>
              ))}
            </div>
          </div>
        </Card>
      </Grid>
    </ThemeProvider>
  );
};

export default AddSpecialisationComponent;
