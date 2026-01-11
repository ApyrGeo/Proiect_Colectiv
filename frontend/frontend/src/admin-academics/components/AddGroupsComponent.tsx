import {
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
import { muiThemeAcademics } from "../muiThemeAcademics.ts";
import { useEffect, useState } from "react";
import type { Faculty } from "../props.ts";
import useAdminAcademicsApi from "../useAdminAcademicsApi.ts";
import { useTranslation } from "react-i18next";
import { toast } from "react-hot-toast";

type AddGroupProps = {
  faculties: Faculty[];
  refreshFaculties: () => void;
};

const AddSpecialisationComponent: React.FC<AddGroupProps> = (props) => {
  const [groupCount, setGroupCount] = useState(4);
  const [groupNames, setGroupNames] = useState<string[]>(["1", "2", "3", "4"]);
  const [promId, setPromId] = useState<number | "">("");
  const { postGroup, postSubGroup } = useAdminAcademicsApi();
  const { t } = useTranslation();
  const [error, setError] = useState("");
  useEffect(() => {
    if (error) {
      toast.error(t("Error"));
    }
  }, [error, t]);

  const handleAddPromotion = () => {
    if (!promId) return;

    postAllGroups(promId).then(() => {
      toast.success("Success");
    });
  };
  const postAllGroups = async (promId: number) => {
    for (let i = 0; i < groupCount; i++) handlePostGroup(groupNames[i], promId);
  };

  const handlePostGroup = (name: string, promId: number) => {
    postGroup({ groupYearId: promId, name: name })
      .then((res) => {
        postSubGroup({ studentGroupId: res.id, name: name + "/1" });
        postSubGroup({ studentGroupId: res.id, name: name + "/2" });
      })
      .catch(() => {
        setError("Error");
      });
  };
  return (
    <ThemeProvider theme={muiThemeAcademics}>
      <div className={"admin-academic-component"}>
        <Card>
          <div className={"academic-title"}>{t("NewGroups")}</div>
          <FormControl fullWidth>
            <InputLabel id="demo-simple-select-label">{t("Promotion")}</InputLabel>
            <Select
              labelId="demo-simple-select-label"
              id="demo-simple-select"
              value={promId}
              label={t("Promotion")}
              onChange={(event) => {
                if (!event.target.value) return;
                setPromId(Number(event.target.value));
              }}
            >
              {props.faculties.map((f) =>
                f.specialisations.map((s) =>
                  s.promotions.map((g) => (
                    <MenuItem value={g.id} key={g.id}>
                      {s.name + ": " + g.startYear + " - " + g.endYear}
                    </MenuItem>
                  ))
                )
              )}
            </Select>
          </FormControl>
          <div className={"academic-row"}>
            <div className={"academic-column"}>
              <div className={"academic-label"}>{t("Count")}</div>
              <div className={"academic-slider"}>
                <Slider
                  // defaultValue={groupCount}
                  min={1}
                  max={20}
                  value={groupCount}
                  onChange={(_event, value) => {
                    setGroupCount(value);
                    const names = [];
                    for (let i = 0; i < value; i++) names.push(String(i + 1));
                    setGroupNames(names);
                  }}
                  valueLabelDisplay="auto"
                />
              </div>
              <Button onClick={handleAddPromotion}>{t("AddGroups")}</Button>
            </div>
            <div className={"academic-column"}>
              <div className={"academic-label"}>{t("GroupNames")}</div>
              {groupNames.map((value, index) => (
                <TextField
                  value={value}
                  id={String(index)}
                  key={String(index)}
                  onChange={(event) => {
                    const names = groupNames;
                    groupNames[index] = event.target.value;
                    setGroupNames(names);
                  }}
                ></TextField>
              ))}
            </div>
          </div>
        </Card>
      </div>
    </ThemeProvider>
  );
};

export default AddSpecialisationComponent;
