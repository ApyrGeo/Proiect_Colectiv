# TrackForUBB

An alternative academic platform developed as a university project, aimed at improving the access to academic resources.

![GitHub watchers](https://img.shields.io/github/watchers/ApyrGeo/Proiect_Colectiv?style=for-the-badge)
[![Website](https://img.shields.io/website?url=https%3A%2F%2Fwww.trackforubb.store%2F&style=for-the-badge)](https://trackforubb.store/)
![GitHub contributors](https://img.shields.io/github/contributors/ApyrGeo/Proiect_Colectiv?style=for-the-badge)
![University Project](https://img.shields.io/badge/project-university-blueviolet?style=for-the-badge)
![Status](https://img.shields.io/badge/status-finished-blue?style=for-the-badge&logo=checkmarx)

# Overview

This project was developed by a team of students seeking a capable and flexible academic management application. This platform is designed not only for administrators to manage universities and their faculties, but also for students and teachers. It enables the management of student groups and enrollments, allows timetables to be customized for each type of subject, and introduces new ways to interact with academic data. Students and teachers can interact with grades and easily modify user information which is helpful in generating contracts.

# Features

## Front Page

<img width="1919" height="943" alt="image" src="https://github.com/user-attachments/assets/997c79f4-31b7-4e4c-9cf2-ea1383eb6a7e" />

- Carousel for displaying Faculty news
- Language selection (Romanian, English, German, Hungarian)
- User authentication via Microsoft Login, with given credentials

## Student Page

- Costumizable timetable viewer
  - Displays only the courses in which the user is enrolled
  - Show courses locations:
    - By default, display all the location the user has courses in
    - By clicking a location, see that location on map and grant the posibility to see navigation routes from user location to that place
  - Download timetable as .ics
<img width="500" height="300" alt="image" src="https://github.com/user-attachments/assets/4c34a380-6fd8-488b-9c0c-d6a434014d0c" />
<img width="500" height="300" alt="image" src="https://github.com/user-attachments/assets/5e4c2921-75ff-4345-9b7c-ee269666d700" />

- Grades visualizer
  - See scholarship status, by selecting a year and a semester
- Exam dates visualizer
<img width="600" height="300" alt="image" src="https://github.com/user-attachments/assets/cc713518-766b-4218-b6b9-c2bc88e0c9da" />
<img width="400" height="300" alt="image" src="https://github.com/user-attachments/assets/a6d5dc85-079c-4c64-8502-fa18dca28715" />

- Modify user data
- Generate contracts with saved user data
  - All data, including signature will be autocompleted 
<img width="300" height="300" alt="image" src="https://github.com/user-attachments/assets/863f4bfa-3aed-4c5f-9b71-dfcfd8ea6284" />

> Example of a generated contract: [contract.pdf](https://github.com/user-attachments/files/25497055/contract-3.pdf)

## Teacher Page

- Timetable viewer
- Modify user data
- Add grades
- Schedule exam dates
<img width="500" height="350" alt="image" src="https://github.com/user-attachments/assets/c424f874-fe25-4ce0-96b4-c6f76f4ebd9c" />
<img width="500" height="350" alt="image" src="https://github.com/user-attachments/assets/d17f8721-f061-4e1f-b463-a9d34b6359ea" />


## Admin Page 

- User management
  - Import users from a .cvs or .xlsx file
    - Auto-generate Microsoft credentials
    - Send credentials on email
  - Import promotions from .csv or .xlsx
    - Send joined group info on email   
  - Add teachers 

- Timetable costumization
  - Detection for overlapping locations and hours 
<img width="800" height="400" alt="image" src="https://github.com/user-attachments/assets/95d675c5-187e-4777-831c-b8a2cdd2df12" />

- Academic information manager
  - Add Faculties, Specialisations, Groups
  - Add new teaching locations using Google Maps component
  - Add subjects

<img width="300" height="300" alt="image" src="https://github.com/user-attachments/assets/5b614be8-41ae-49ca-9a29-f3adc7cb8cd6" />
<img width="300" height="300" alt="image" src="https://github.com/user-attachments/assets/1e5adc64-c1c8-4c3c-92fd-12236b879b33" />
<img width="300" height="300" alt="image" src="https://github.com/user-attachments/assets/5029e3d7-aafb-475f-936c-3cf682cb34dd" />

# Used Technologies

**Frontend**

![Static Badge](https://img.shields.io/badge/React-%2361DAFB?style=for-the-badge&logo=React&labelColor=black) ![Static Badge](https://img.shields.io/badge/TypeScript-%233178C6?style=for-the-badge&logo=TypeScript&labelColor=black) ![Static Badge](https://img.shields.io/badge/CSS-%23663399?style=for-the-badge&logo=CSS)

**Backend**

![Static Badge](https://img.shields.io/badge/.Net_Core-%23512BD4?style=for-the-badge&logo=.NET) ![Static Badge](https://img.shields.io/badge/Entity%20Framework-%230066FF?style=for-the-badge&labelColor=black) ![Static Badge](https://img.shields.io/badge/PostGreSQL-%234169E1?style=for-the-badge&logo=PostGreSQL&labelColor=black)

**Tools**

![Static Badge](https://img.shields.io/badge/GitHub-%23181717?style=for-the-badge&logo=GitHub) ![Static Badge](https://img.shields.io/badge/Swagger-%2385EA2D?style=for-the-badge&logo=Swagger&labelColor=black) 

# Team 

<img width="300" height="300" alt="image" src="https://github.com/user-attachments/assets/17684e67-0830-4fb0-9e35-4e0f099a4a7a" />

**Team Members:**

- ![Horodniceanu Andrei](https://github.com/the-horo) - Backend developer & DevOps
- ![Negrea Răzvan](https://github.com/NegreaRazvan) - Frontend developer
- ![Răduță Cristian](https://github.com/raduta-cristian) - Frontend developer
- ![Rotaru Andrei Ionuț](https://github.com/Andrei0921) - Backend developer
- ![Secure George-Sebastian](https://github.com/ApyrGeo) - Backend developer
- ![Stanca Vlad Tudor](https://github.com/TudorStanca) - Backend developer & DevOps
- ![Scutariu Edward](https://github.com/scutedi) - Frontend developer

While each member had primary responsibilities, everyone collaborated across branches, assisting both backend and frontend development.

We applied Scrum methodology during this project, experimenting with sprints and roles to organize our workflow:

- **Product Owner:** Stanca Vlad Tudor – managed backlog and coordinated feature priorities
- **Scrum Master:** Secure George-Sebastian – ensured smooth sprint workflow and removed blockers
- **Developers:** We applied Scrum methodology in a learning context; all team members contributed as developers.
  
# Usage

The live version is available at:

> www.trackforubb.store

For testing purposes, we offer credentials to already created accounts:

**Student**

> Email: vlad.tudor24@trackforubb.onmicrosoft.com  
> Password: ubqrwDAASfweeon123

**Teacher**

> Email: george.voicu62@trackforubb.onmicrosoft.com  
> Password: ubqrwDAASfweeon123

# Status

This project is completed and was submitted as part of a course. No further development is planned at this time, but we welcome any feedback or suggestions 😊.
