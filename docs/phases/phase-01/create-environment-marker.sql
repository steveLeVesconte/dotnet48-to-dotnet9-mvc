/* create-environment-marker.sql
   Run this script in EACH target database (Identity DB and MusicStore DB).
   Purpose: Explicit NONPROD safety gate for the Seed CLI.
*/

IF OBJECT_ID('dbo.__EnvironmentMarker', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.__EnvironmentMarker
    (
        MarkerId         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK___EnvironmentMarker PRIMARY KEY,
        EnvironmentName  NVARCHAR(50) NOT NULL,
        AllowSeedCli     BIT NOT NULL,
        ProjectSeedToken NVARCHAR(200) NOT NULL,
        CreatedUtc       DATETIME2(0) NOT NULL CONSTRAINT DF___EnvironmentMarker_CreatedUtc DEFAULT (SYSUTCDATETIME()),
        CreatedBy        NVARCHAR(200) NOT NULL,
        Notes            NVARCHAR(400) NULL
    );

    /* Optional hardening: prevent duplicates for this tool */
    CREATE UNIQUE INDEX UX___EnvironmentMarker_ProjectSeedToken
        ON dbo.__EnvironmentMarker(ProjectSeedToken);
END
GO

DECLARE @ExpectedToken NVARCHAR(200) = N'dotnet48-to-dotnet9-mvc/validation-dataset';
DECLARE @CreatedBy     NVARCHAR(200) = SUSER_SNAME();
DECLARE @Notes         NVARCHAR(400) = N'Explicit NONPROD gate for Seed CLI (manual operator action).';

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.__EnvironmentMarker
    WHERE EnvironmentName  = N'NONPROD'
      AND AllowSeedCli     = 1
      AND ProjectSeedToken = @ExpectedToken
)
BEGIN
    INSERT INTO dbo.__EnvironmentMarker(EnvironmentName, AllowSeedCli, ProjectSeedToken, CreatedBy, Notes)
    VALUES (N'NONPROD', 1, @ExpectedToken, @CreatedBy, @Notes);
END
GO
