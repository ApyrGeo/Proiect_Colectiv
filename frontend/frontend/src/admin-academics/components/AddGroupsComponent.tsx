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
import type { Faculty } from "../props.ts";
import useAdminAcademicsApi from "../useAdminAcademicsApi.ts";

type AddGroupProps = {
  faculties: Faculty[];
  refreshFaculties: () => void;
};

const AddSpecialisationComponent: React.FC<AddGroupProps> = (props) => {
  const [groupCount, setGroupCount] = useState(1);
  const [groupNames, setGroupNames] = useState<string[]>(["1"]);
  const [promId, setPromId] = useState<number | null>(null);

  const { postGroup, postSubGroup } = useAdminAcademicsApi();

  const handleAddPromotion = () => {
    if (!promId) return;

    for (let i = 0; i < groupCount; i++) handlePostGroup(groupNames[i], promId);
  };

  const handlePostGroup = (name: string, promId: number) => {
    postGroup({ groupYearId: promId, name: name })
      .then((res) => {
        postSubGroup({ studentGroupId: res.id, name: name + "/1" });
        postSubGroup({ studentGroupId: res.id, name: name + "/2" });
      })
      .catch(() => {
        //TODO handle error
      });
  };

  return (
    <ThemeProvider theme={theme}>
      <Grid>
        <Card>
          <div className={"academic-title"}>New Groups</div>
          <div className={"academic-row"}>
            <div className={"academic-column"}>
              <FormControl fullWidth>
                <InputLabel id="demo-simple-select-label">Promotion</InputLabel>
                <Select
                  labelId="demo-simple-select-label"
                  id="demo-simple-select"
                  label="Promotion"
                  onChange={(event) => {
                    if (!event.target.value) return;
                    setPromId(Number(event.target.value));
                  }}
                >
                  {props.faculties.map((f) =>
                    f.specialisations.map((s) =>
                      s.groupYears.map((g) => (
                        <MenuItem value={g.id} key={g.id}>
                          {s.name + ": " + g.startYear + " - " + g.endYear}
                        </MenuItem>
                      ))
                    )
                  )}
                </Select>
              </FormControl>
              <div className={"academic-label"}>Count</div>
              <Slider
                // defaultValue={groupCount}
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
              <Button onClick={handleAddPromotion}>Add Promotion</Button>
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
