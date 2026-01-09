import { Slider, Card, Button, ThemeProvider, FormControl, Select, InputLabel, MenuItem } from "@mui/material";
import { theme } from "../theme.ts";
import { useEffect, useState } from "react";
import useAdminAcademicsApi from "../useAdminAcademicsApi.ts";
import type { Faculty } from "../props.ts";
import { useTranslation } from "react-i18next";
import { toast } from "react-hot-toast";

type AddPromotionProps = {
  faculties: Faculty[];
  refreshFaculties: () => void;
};

const AddPromotionComponent: React.FC<AddPromotionProps> = (props) => {
  const [promotionLenght, setPromotionLenght] = useState(3);
  const [specId, setSpecId] = useState<number | "">("");

  const { postPromotion } = useAdminAcademicsApi();
  const { t } = useTranslation();
  const [error, setError] = useState("");
  useEffect(() => {
    if (error) {
      toast.error(t("Error"));
    }
  }, [error, t]);

  const handleAddPromotion = () => {
    if (!specId) return;

    const startYear = new Date().getFullYear();
    const endYear = startYear + promotionLenght;
    postPromotion({ specialisationId: specId, startYear, endYear })
      .then(() => {
        toast.success(t("Success"));
        props.refreshFaculties();
      })
      .catch(() => {
        setError("Error");
      });
  };

  return (
    <ThemeProvider theme={theme}>
      <div className={"admin-academic-component"}>
        <Card>
          <div className={"academic-title"}>{t("NewPromotion")}</div>
          <FormControl fullWidth>
            <InputLabel id="demo-simple-select-label">{t("Specialization")}</InputLabel>
            <Select
              labelId="demo-simple-select-label"
              id="demo-simple-select"
              label={t("Specialisation")}
              value={specId}
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
          <div className="academic-row">
            <div className="academic-column">
              <div className={"academic-label"}>{t("Length")}</div>
              <div className="academic-slider">
                <Slider
                  value={promotionLenght}
                  min={1}
                  max={6}
                  valueLabelDisplay="auto"
                  onChange={(_event, value) => {
                    setPromotionLenght(value);
                  }}
                />
              </div>
            </div>
            <Button onClick={handleAddPromotion}>{t("AddPromotion")}</Button>
          </div>
        </Card>
      </div>
    </ThemeProvider>
  );
};

export default AddPromotionComponent;
