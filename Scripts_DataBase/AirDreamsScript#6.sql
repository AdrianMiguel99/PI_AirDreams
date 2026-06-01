CREATE FUNCTION dbo.fn_TotalLuggageCost
(
    @basePrice DECIMAL(10,2),
    @multiplier DECIMAL(5,2),
    @quantity INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @total DECIMAL(10,2) = 0;
    IF @quantity <= 0 RETURN 0;
    IF @multiplier = 0
        SET @total = @basePrice * @quantity;
    ELSE
        SET @total = @basePrice * (POWER(1 + @multiplier, @quantity) - 1) / @multiplier;
    RETURN @total;
END;

