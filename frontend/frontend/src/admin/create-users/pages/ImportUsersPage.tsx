import React, { useState } from "react";
import { useTranslation } from "react-i18next";
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
  const { t } = useTranslation();

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
      toast.error(t("createUsers.importUsers.selectFileFirst"));
      return;
    }

    setIsLoading(true);
    setResults(null);

    try {
      const result = await uploadUsersFile(selectedFile);

      if (result.isValid) {
        toast.success(t("createUsers.importUsers.allCreated"));
      } else {
        toast.error(t("createUsers.importUsers.someFailed"));
      }

      setResults(result.users);
    } catch (error) {
      const axiosError = error as AxiosError<unknown>;

      if (axiosError.response && axiosError.response.status === 422 && axiosError.response.data) {
        const data = axiosError.response.data as BulkUserResult & { StatusCode?: number; Description?: string };

        if (Array.isArray((data as BulkUserResult).users)) {
          toast.error(t("createUsers.importUsers.someFailed"));
          setResults((data as BulkUserResult).users);
        } else if (typeof (data as { Description?: string }).Description === "string") {
          toast.error((data as { Description: string }).Description);
        } else {
          toast.error(t("createUsers.importUsers.invalidUploadedFile"));
        }
      } else {
        console.error("Failed to upload users file", error);
        toast.error(t("createUsers.importUsers.failedProcessFile"));
      }
    } finally {
      setIsLoading(false);
    }
  };

  const hasErrors = (user: BulkUserItemResult) => !user.isValid || (user.errors && user.errors.length > 0);

  const errorRows = results?.filter((u) => hasErrors(u)) ?? [];

  return (
    <div className="create-users-page">
      <h1 className="create-users-title">{t("createUsers.importUsers.title")}</h1>
      <p className="create-users-subtext">{t("createUsers.importUsers.subtext")}</p>

      <Box className="create-users-template-row">
        <Typography variant="body2" className="create-users-template-text">
          {t("createUsers.importUsers.templateText")}
        </Typography>
        <Button
          variant="outlined"
          color="primary"
          size="small"
          component="a"
          href="/import-users-template.csv"
          download
          className="create-users-template-button"
        >
          {t("createUsers.importUsers.downloadTemplate")}
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
                  {t("createUsers.importUsers.browse")}
                  <input
                    id="users-file"
                    type="file"
                    hidden
                    accept=".csv, application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    onChange={handleFileChange}
                  />
                </Button>
                <span className="create-users-file-name">
                  {selectedFile ? selectedFile.name : t("createUsers.importUsers.noFileSelected")}
                </span>
              </div>
            </div>

            <Button
              variant="contained"
              color="primary"
              onClick={handleUpload}
              disabled={isLoading || !selectedFile}
              className="create-users-upload-button"
            >
              {isLoading ? t("createUsers.importUsers.uploading") : t("createUsers.importUsers.upload")}
            </Button>
          </Box>

          {isLoading && <Circular />}

          {results && results.length > 0 && (
            <Box className="create-users-results">
              <Typography variant="h6" className="create-users-results-title">
                {t("createUsers.importUsers.validationResults")}{" "}
                {errorRows.length > 0 ? `(errors: ${errorRows.length})` : "(all valid)"}
              </Typography>
              <div className="create-users-table-wrapper">
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>{t("createUsers.importUsers.table.row")}</TableCell>
                      <TableCell>{t("createUsers.importUsers.table.email")}</TableCell>
                      <TableCell>{t("createUsers.importUsers.table.status")}</TableCell>
                      <TableCell>{t("createUsers.importUsers.table.errors")}</TableCell>
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
                        <TableCell>
                          {hasErrors(user)
                            ? t("createUsers.importUsers.statuses.invalid")
                            : t("createUsers.importUsers.statuses.created")}
                        </TableCell>
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
