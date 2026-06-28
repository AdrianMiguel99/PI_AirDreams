USE AirDreams;
GO

IF OBJECT_ID('ReservationCancellationToken', 'U') IS NULL
BEGIN
    CREATE TABLE ReservationCancellationToken (
        tokenId INT IDENTITY(1,1) PRIMARY KEY,
        transactionId VARCHAR(20) NOT NULL,
        token NVARCHAR(255) NOT NULL,
        expirationDate DATETIME NOT NULL,
        isUsed BIT NOT NULL DEFAULT 0,
        createdAt DATETIME NOT NULL DEFAULT GETDATE(),

        CONSTRAINT FK_ReservationCancellationToken_Itinerary
            FOREIGN KEY (transactionId)
            REFERENCES Itinerary(transactionId)
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_ReservationCancellationToken_Token'
      AND object_id = OBJECT_ID('ReservationCancellationToken')
)
BEGIN
    CREATE UNIQUE INDEX IX_ReservationCancellationToken_Token
    ON ReservationCancellationToken(token);
END;
GO