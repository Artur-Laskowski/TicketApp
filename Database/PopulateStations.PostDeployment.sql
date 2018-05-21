/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF OBJECT_ID('tempdb.dbo.#CitiesTemp', 'U') IS NOT NULL
  DROP TABLE #CitiesTemp; 

CREATE TABLE #CitiesTemp
(
	city NVARCHAR(64),
	city_ascii NVARCHAR(64),
	lat float,
	lng float,
	pop float,
	country VARCHAR(64),
	iso2 VARCHAR(32),
	iso3 VARCHAR(32),
	province NVARCHAR(64)
 
)

BULK INSERT #CitiesTemp
    FROM 'C:\dev\TicketApp\Database\simplemaps-worldcities-basic.csv'
    WITH
    (
	CODEPAGE = 'RAW',
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',  --CSV field delimiter
    ROWTERMINATOR = '\n',   --Use to shift the control to next row
    TABLOCK
    )

SELECT * FROM #CitiesTemp

DELETE FROM dbo.Station;
GO

INSERT INTO dbo.Station ([Name], Lat, Lng)
SELECT city_ascii, lat, lng
FROM #CitiesTemp
WHERE country='Poland';
