use [PzenaInterviewAssessment]
go

CREATE PROCEDURE CalculateTickerStatistics
    @TickerSymbol NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Declare variables to hold the results
    DECLARE @MovingAverage52Days DECIMAL(18, 2);
    DECLARE @HighPrice52Weeks DECIMAL(18, 2);
    DECLARE @LowPrice52Weeks DECIMAL(18, 2);

    -- Calculate 52-day moving average price
    SELECT @MovingAverage52Days = AVG([close])
    FROM (
        SELECT TOP 52 [close]
        FROM Prices
        WHERE ticker = @TickerSymbol
        ORDER BY date DESC
    ) AS Last52Days;

    -- Calculate 52-week high price
    SELECT @HighPrice52Weeks = MAX([high])
    FROM Prices
    WHERE ticker = @TickerSymbol
      AND date >= DATEADD(WEEK, -52, GETDATE());

    -- Calculate 52-week low price
    SELECT @LowPrice52Weeks = MIN([low])
    FROM Prices
    WHERE ticker = @TickerSymbol
      AND date >= DATEADD(WEEK, -52, GETDATE());

    -- Return the results
    SELECT
        @TickerSymbol AS TickerSymbol,
        @MovingAverage52Days AS MovingAverage52Days,
        @HighPrice52Weeks AS HighPrice52Weeks,
        @LowPrice52Weeks AS LowPrice52Weeks;
END;
GO