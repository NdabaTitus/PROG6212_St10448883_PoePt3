# BuildTrack Claim System

## Project Overview

BuildTrack is a web-based claim management system designed to streamline workflows for construction projects. It allows Workers, Project Managers, Construction Managers, and HR to manage claims efficiently in a single portal.

## Features

* **Role-based Access**: Different functionalities for Workers, Project Managers, Construction Managers, and HR.
* **Workflow Automation**: Workers submit claims → Project Manager review → Construction Manager approval → HR finalization.
* **Payment Simulation**: Workers can simulate payment amounts.
* **Notifications**: Automated notifications for pending approvals and updates.
* **Export Options**: HR can export claim data to CSV for reporting purposes.
* **Responsive Design**: Mobile-friendly layout using Bootstrap.

## Technologies Used

* ASP.NET Core MVC
* C#
* Bootstrap 5
* SQL Server (Database)
* jQuery & JavaScript
* Git & GitHub for version control

## Database Integration

SQL Server is used to store claims, users, and workflow data. Queries and stored procedures ensure efficient data retrieval, validation, and automation of claim approvals.

## Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/NdabaTitus/PROG6212_St10448883_PoePt3.git
   ```
2. Open the solution in Visual Studio.
3. Restore NuGet packages.
4. Update the database connection string in `appsettings.json`.
5. Run the project.

## Usage

1. Navigate to the homepage.
2. Log in using your role credentials:

   * Worker: [worker@site.com] / Worker@123!
   * Project Manager: [pm@site.com]/ Pm123!
   * Construction Manager: [cm@site.com] / Cm@123!
   * HR: [hr@site.com] / Hr@123!
3. Explore role-specific features and manage claims accordingly.

## Workflow

```
Worker > Project Manager > Construction Manager > HR
```

## Youtube & Repository Links

* GitHub Repository: [https://github.com/NdabaTitus/PROG6212_St10448883_PoePt3.git)
* YouTube: [https://www.youtube.com/your-video-link](https://www.youtube.com/your-video-link)

## Author

Ndaba Titus
Student Number: ST10448883

## License

This project is for academic purposes and does not have a commercial license.
