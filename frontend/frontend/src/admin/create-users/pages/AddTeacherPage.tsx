import { useEffect, useState } from "react";
import "../import-users.css";
import { Autocomplete, Box, Button, Card, CardContent, TextField, Typography } from "@mui/material";
import { toast } from "react-hot-toast";
import Circular from "../../../components/loading/Circular.tsx";
import useAddTeacherApi, { type FacultyResult, type TeacherResult } from "../useAddTeacherApi.ts";
import type { AxiosError } from "axios";

const AddTeacherPage = () => {
  const { getAllFaculties, getUsersByEmail, createTeacher } = useAddTeacherApi();

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
        toast.error("Could not load faculties.");
      }
    })();
  }, [getAllFaculties]);

  const handleAddTeacher = async () => {
    const trimmedEmail = email.trim();

    if (!trimmedEmail) {
      toast.error("Please enter a user email.");
      return;
    }

    if (!faculty) {
      toast.error("Please select a faculty.");
      return;
    }

    setIsLoading(true);
    setCreatedTeacher(null);

    try {
      const users = await getUsersByEmail(trimmedEmail);
      if (users.length === 0) {
        toast.error("No user found with this email.");
        return;
      }

      const matchedUser = users[0];

      const teacher = await createTeacher({ userId: matchedUser.id, facultyId: faculty.id });

      setCreatedTeacher(teacher);
      toast.success("Teacher added successfully.");
    } catch (error) {
      const axiosError = error as AxiosError<unknown>;

      if (axiosError.response && axiosError.response.status === 422 && axiosError.response.data) {
        const data = axiosError.response.data as { Description?: string };
        if (typeof data.Description === "string") {
          toast.error(data.Description);
        } else {
          toast.error("The data is not valid.");
        }
      } else {
        console.error("Failed to create teacher", error);
        toast.error("Failed to create teacher. Please try again.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="create-users-page">
      <h1 className="create-users-title">Add teacher</h1>
      <p className="create-users-subtext">Assign an existing user to a faculty as a teacher.</p>

      <Card className="create-users-card">
        <CardContent>
          <Box className="promotion-fields-row">
            <TextField
              label="User email"
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
              renderInput={(params) => <TextField {...params} label="Faculty" />}
              slotProps={{
                paper: {
                  sx: { minWidth: 320 },
                },
                listbox: {
                  sx: { maxHeight: 320, overflowY: "auto" },
                },
              }}
              noOptionsText="No faculties found"
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
              {isLoading ? "Saving..." : "Add teacher"}
            </Button>
          </Box>

          {isLoading && <Circular />}

          {createdTeacher && (
            <Box mt={3} className="promotion-summary">
              <Typography variant="h6" className="create-users-results-title">
                Created teacher
              </Typography>
              <Typography variant="body2">
                User: {createdTeacher.user.firstName} {createdTeacher.user.lastName} ({createdTeacher.user.email})
              </Typography>
              {faculty && <Typography variant="body2">Faculty: {faculty.name}</Typography>}
              {createdTeacher.subGroup && (
                <Typography variant="body2">Subgroup: {createdTeacher.subGroup.name}</Typography>
              )}
            </Box>
          )}
        </CardContent>
      </Card>
    </div>
  );
};

export default AddTeacherPage;
