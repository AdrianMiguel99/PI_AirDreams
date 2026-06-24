USE AirDreams;
GO

IF COL_LENGTH('Itinerary', 'itineraryStatus') IS NULL
BEGIN
    ALTER TABLE Itinerary
    ADD itineraryStatus VARCHAR(20) NOT NULL
        CONSTRAINT DF_Itinerary_Status DEFAULT 'Active';
END;
GO

IF OBJECT_ID('CK_Itinerary_Status', 'C') IS NULL
BEGIN
    ALTER TABLE Itinerary
    ADD CONSTRAINT CK_Itinerary_Status
    CHECK (itineraryStatus IN ('Active', 'Cancelled'));
END;
GO