import React, { useState } from "react";
import "../import-users.css";
import {
  Box,
  Button,
  Card,
  CardContent,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { toast } from "react-hot-toast";
import Circular from "../../../components/loading/Circular.tsx";
import useImportUsersApi, { type BulkUserItemResult, type BulkUserResult } from "../useImportUsersApi.ts";
import type { AxiosError } from "axios";

const ImportUsersPage = () => {
  const { uploadUsersFile } = useImportUsersApi();

  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [results, setResults] = useState<BulkUserItemResult[] | null>(null);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0] ?? null;
    setSelectedFile(file);
    setResults(null);
  };

  const handleUpload = async () => {
    if (!selectedFile) {
      toast.error("Please select a file first.");
      return;
    }

    setIsLoading(true);
    setResults(null);

    try {
      const result = await uploadUsersFile(selectedFile);

      if (result.isValid) {
        toast.success("All users were created successfully.");
      } else {
        toast.error("Some users could not be created. See details below.");
      }

      setResults(result.users);
    } catch (error) {
      const axiosError = error as AxiosError<unknown>;

      if (axiosError.response && axiosError.response.status === 422 && axiosError.response.data) {
        const data = axiosError.response.data as BulkUserResult & { StatusCode?: number; Description?: string };

        if (Array.isArray((data as BulkUserResult).users)) {
          toast.error("Some users could not be created. See details below.");
          setResults((data as BulkUserResult).users);
        } else if (typeof (data as { Description?: string }).Description === "string") {
          toast.error((data as { Description: string }).Description);
        } else {
          toast.error("The uploaded file is not valid.");
        }
      } else {
        console.error("Failed to upload users file", error);
        toast.error("Failed to process the file. Please try again.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  const hasErrors = (user: BulkUserItemResult) => !user.isValid || (user.errors && user.errors.length > 0);

  const errorRows = results?.filter((u) => hasErrors(u)) ?? [];

  return (
    <div className="create-users-page">
      <h1 className="create-users-title">Create Users</h1>
      <p className="create-users-subtext">Upload an Excel or CSV file with user information.</p>

      <Box className="create-users-template-row">
        <Typography variant="body2" className="create-users-template-text">
          You can download a CSV template and fill it in before uploading.
        </Typography>
        <Button
          variant="outlined"
          color="primary"
          size="small"
          component="a"
          href="/add-bulk-users-template.csv"
          download
          className="create-users-template-button"
        >
          Download template
        </Button>
      </Box>

      <Card className="create-users-card">
        <CardContent>
          <Box className="create-users-upload-section">
            <div className="create-users-input-group">
              <div className="create-users-buttons-row">
                <Button
                  variant="outlined"
                  component="label"
                  disabled={isLoading}
                  className="create-users-browse-button"
                >
                  Browse
                  <input
                    id="users-file"
                    type="file"
                    hidden
                    accept=".csv, application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    onChange={handleFileChange}
                  />
                </Button>
                <span className="create-users-file-name">{selectedFile ? selectedFile.name : "No file selected"}</span>
              </div>
            </div>

            <Button
              variant="contained"
              color="primary"
              onClick={handleUpload}
              disabled={isLoading || !selectedFile}
              className="create-users-upload-button"
            >
              {isLoading ? "Uploading..." : "Upload"}
            </Button>
          </Box>

          {isLoading && <Circular />}

          {results && results.length > 0 && (
            <Box className="create-users-results">
              <Typography variant="h6" className="create-users-results-title">
                Validation results {errorRows.length > 0 ? `(errors: ${errorRows.length})` : "(all valid)"}
              </Typography>
              <div className="create-users-table-wrapper">
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>Row</TableCell>
                      <TableCell>Email</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Errors</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {(errorRows.length > 0 ? errorRows : results).map((user) => (
                      <TableRow
                        key={user.row}
                        className={hasErrors(user) ? "create-users-row-error" : "create-users-row-success"}
                      >
                        <TableCell>{user.row}</TableCell>
                        <TableCell>{user.email === "" || user.email === null ? "-" : user.email}</TableCell>
                        <TableCell>{hasErrors(user) ? "Invalid" : "Created"}</TableCell>
                        <TableCell>
                          {user.errors && user.errors.length > 0
                            ? user.errors.map((err, idx) => <div key={idx}>{err}</div>)
                            : "-"}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            </Box>
          )}
        </CardContent>
      </Card>
    </div>
  );
};

export default ImportUsersPage;
