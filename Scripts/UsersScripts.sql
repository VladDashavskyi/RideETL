-- Find out which `PULocationId` (Pick-up location ID) has the highest tip_amount on average.
SELECT PULocationId, AVG(TipAmount) AS AvgTipAmount 
FROM [dbo].[RideData] 
GROUP BY PULocationId
ORDER BY AvgTipAmount DESC

-- Find the top 100 longest fares in terms of `trip_distance`.
SELECT TOP(100) *
FROM [dbo].[RideData] 
ORDER BY TripDistance DESC 

-- Find the top 100 longest fares in terms of time spent traveling.
SELECT TOP(100)*,
DATEDIFF(MINUTE,PickUpDateTime, DropOffDateTime) AS TimeSpentTraveling
FROM [dbo].[RideData] 
ORDER BY TimeSpentTraveling DESC 

-- Search, where part of the conditions is `PULocationId`.
DECLARE @PULocationId INT = 1

SELECT
*FROM [dbo].[RideData]
WHERE PULocationId = @PULocationId