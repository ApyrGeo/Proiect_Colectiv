import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import "../import-users.css";
import { Autocomplete, Box, Button, Card, CardContent, TextField, Typography } from "@mui/material";
import { toast } from "react-hot-toast";
import Circular from "../../../components/loading/Circular.tsx";
import useAddTeacherApi, { type FacultyResult, type TeacherResult } from "../useAddTeacherApi.ts";
import type { AxiosError } from "axios";

const AddTeacherPage = () => {
  const { getAllFaculties, getUsersByEmail, createTeacher } = useAddTeacherApi();
  const { t } = useTranslation();

  const [email, setEmail] = useState("");
  const [faculties, setFaculties] = useState<FacultyResult[]>([]);
  const [faculty, setFaculty] = useState<FacultyResult | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [createdTeacher, setCreatedTeacher] = useState<TeacherResult | null>(null);

  useEffect(() => {
    (async () => {
      try {
        const allFaculties = await getAllFaculties();
        setFaculties(allFaculties);
      } catch (error) {
        console.error("Failed to load faculties", error);
        toast.error(t("createUsers.addTeacher.messages.couldNotLoadFaculties"));
      }
    })();
  }, [getAllFaculties]);

  const handleAddTeacher = async () => {
    const trimmedEmail = email.trim();

    if (!trimmedEmail) {
      toast.error(t("createUsers.addTeacher.messages.pleaseEnterEmail"));
      return;
    }

    if (!faculty) {
      toast.error(t("createUsers.addTeacher.messages.pleaseSelectFaculty"));
      return;
    }

    setIsLoading(true);
    setCreatedTeacher(null);

    try {
      const users = await getUsersByEmail(trimmedEmail);
      if (users.length === 0) {
        toast.error(t("createUsers.addTeacher.messages.noUserFound"));
        return;
      }

      const matchedUser = users[0];

      const teacher = await createTeacher({ userId: matchedUser.id, facultyId: faculty.id });

      setCreatedTeacher(teacher);
      toast.success(t("createUsers.addTeacher.messages.teacherAdded"));
    } catch (error) {
      const axiosError = error as AxiosError<unknown>;

      if (axiosError.response && axiosError.response.status === 422 && axiosError.response.data) {
        const data = axiosError.response.data as { Description?: string };
        if (typeof data.Description === "string") {
          toast.error(data.Description);
        } else {
          toast.error(t("createUsers.addTeacher.messages.dataNotValid"));
        }
      } else {
        console.error("Failed to create teacher", error);
        toast.error(t("createUsers.addTeacher.messages.createTeacherFailed"));
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="create-users-page">
      <h1 className="create-users-title">{t("createUsers.addTeacher.title")}</h1>
      <p className="create-users-subtext">{t("createUsers.addTeacher.subtext")}</p>

      <Card className="create-users-card">
        <CardContent>
          <Box className="promotion-fields-row">
            <TextField
              label={t("createUsers.addTeacher.userEmail")}
              type="email"
              size="small"
              value={email}
              className="promotion-text-field"
              onChange={(e) => setEmail(e.target.value)}
            />
            <Autocomplete
              size="small"
              options={faculties}
              getOptionLabel={(option) => option.name}
              value={faculty}
              className="promotion-text-field"
              sx={{ minWidth: 260 }}
              onChange={(_e, value) => setFaculty(value)}
              renderInput={(params) => <TextField {...params} label={t("createUsers.addTeacher.faculty")} />}
              slotProps={{
                paper: {
                  sx: { minWidth: 320 },
                },
                listbox: {
                  sx: { maxHeight: 320, overflowY: "auto" },
                },
              }}
              noOptionsText={t("createUsers.addTeacher.noFaculties")}
            />
          </Box>

          <Box className="create-users-upload-section">
            <div className="create-users-input-group" />

            <Button
              variant="contained"
              color="primary"
              onClick={handleAddTeacher}
              disabled={isLoading}
              className="create-users-upload-button"
            >
              {isLoading ? t("createUsers.addTeacher.saving") : t("createUsers.addTeacher.addButton")}
            </Button>
          </Box>

          {isLoading && <Circular />}

          {createdTeacher && (
            <Box mt={3} className="promotion-summary">
              <Typography variant="h6" className="create-users-results-title">
                {t("createUsers.addTeacher.createdTitle")}
              </Typography>
              <Typography variant="body2">
                {t("createUsers.addTeacher.userLabel")} {createdTeacher.user.firstName} {createdTeacher.user.lastName} (
                {createdTeacher.user.email})
              </Typography>
              {faculty && (
                <Typography variant="body2">
                  {t("createUsers.addTeacher.facultyLabel")} {faculty.name}
                </Typography>
              )}
              {createdTeacher.subGroup && (
                <Typography variant="body2">
                  {t("createUsers.addTeacher.subgroupLabel")} {createdTeacher.subGroup.name}
                </Typography>
              )}
            </Box>
          )}
        </CardContent>
      </Card>
    </div>
  );
};

export default AddTeacherPage;
