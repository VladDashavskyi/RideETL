CREATE TABLE RideData (
    PickUpDateTime DATETIME NOT NULL,
    DropOffDateTime DATETIME NOT NULL,
    PassengerCount INT NOT NULL,
    TripDistance FLOAT NOT NULL,
    StoreAndFwdFlag CHAR(3),
    PULocationId INT NOT NULL,
    DOLocationId INT NOT NULL,
    FareAmount DECIMAL(10,2) NOT NULL,
    TipAmount DECIMAL(10,2) NOT NULL)

	CREATE INDEX Idx_RideData_PULocationId ON RideData(PULocationId);
	CREATE INDEX Idx_RideData_TripDistance ON RideData(TripDistance);
	CREATE INDEX Idx_RideData_PickUpDateTime ON RideData(TripDistance);

