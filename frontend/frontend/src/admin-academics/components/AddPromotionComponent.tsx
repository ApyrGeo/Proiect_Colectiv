import { Grid, Slider, Card, Button, ThemeProvider, FormControl, Select, InputLabel, MenuItem } from "@mui/material";
import { theme } from "../theme.ts";
import { useState } from "react";
import useAdminAcademicsApi from "../useAdminAcademicsApi.ts";
import type { Faculty } from "../props.ts";

type AddPromotionProps = {
  faculties: Faculty[];
  refreshFaculties: () => void;
};

const AddPromotionComponent: React.FC<AddPromotionProps> = (props) => {
  const [promotionLenght, setPromotionLenght] = useState(1);
  const [specId, setSpecId] = useState<number | null>(null);

  const { postPromotion } = useAdminAcademicsApi();

  const handleAddPromotion = () => {
    if (!specId) return;

    const startYear = new Date().getFullYear();
    const endYear = startYear + promotionLenght;
    postPromotion({ specialisationId: specId, startYear, endYear })
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
          <div className={"academic-title"}>New Promotion</div>
          <FormControl fullWidth>
            <InputLabel id="demo-simple-select-label">Specialization</InputLabel>
            <Select
              labelId="demo-simple-select-label"
              id="demo-simple-select"
              label="Specialization"
              onChange={(event) => {
                if (!event.target.value) return;
                setSpecId(Number(event.target.value));
              }}
            >
              {props.faculties.map((f) =>
                f.specialisations.map((s) => (
                  <MenuItem value={s.id} key={s.id}>
                    {s.name}
                  </MenuItem>
                ))
              )}
            </Select>
          </FormControl>
          <div className={"academic-label"}>Length</div>
          <Slider
            value={promotionLenght}
            min={1}
            max={6}
            valueLabelDisplay="auto"
            onChange={(_event, value) => {
              setPromotionLenght(value);
            }}
          />
          <Button onClick={handleAddPromotion}>Add Promotion</Button>
        </Card>
      </Grid>
    </ThemeProvider>
  );
};

export default AddPromotionComponent;
