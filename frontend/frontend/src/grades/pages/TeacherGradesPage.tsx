// import React from "react";
// import "./TeacherGradesPage.css";
// import { useTeacherGrades } from "./hooks/useTeacherGrades";
// import type { TeacherGradesPageProps } from "./types/teacherGrades.types";
// import TableGlimmer from "../../components/loading/TableGlimmer";
//
// const TeacherGradesPage: React.FC<TeacherGradesPageProps> = ({ teacherId, user }) => {
//   const { locations, subjects, groups, loading, loadSubjectGroups, saveAll } = useTeacherGrades(teacherId);
//
//   return (
//     <div className="teacher-grades-container">
//       <h2>
//         Profesor: {user.firstName} {user.lastName}
//       </h2>
//
//       <select onChange={(e) => loadSubjectGroups(Number(e.target.value))}>
//         <option value="" disabled selected>
//           Selectează materia
//         </option>
//         {subjects.map((s) => (
//           <option key={s.id} value={s.id}>
//             {s.name}
//           </option>
//         ))}
//       </select>
//
//       <table className="teacher-grades-table">
//         <thead>
//           <tr>
//             <th>Grupa</th>
//             <th>Locația</th>
//             <th>Clasa</th>
//             <th>Data</th>
//             <th>Durata</th>
//           </tr>
//         </thead>
//         <tbody>
//           {loading && <TableGlimmer no_lines={8} no_cols={5} />}
//           {!loading &&
//             groups.map((g) => (
//               <tr key={g.id}>
//                 <td>{g.name}</td>
//                 <td>{/* select locație */}</td>
//                 <td>{/* select clasă */}</td>
//                 <td>{/* datetime */}</td>
//                 <td>{/* durata */}</td>
//               </tr>
//             ))}
//         </tbody>
//       </table>
//
//       {groups.length > 0 && (
//         <div className="teacher-grades-actions">
//           <button onClick={saveAll}>Update</button>
//         </div>
//       )}
//     </div>
//   );
// };
//
// export default TeacherGradesPage;
