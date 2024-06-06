use [PzenaAssessment3]

go

CREATE PROCEDURE dbo.CalculateTickerStatistics
    @Ticker VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MostRecentDate DATE;
    DECLARE @StartDate52Day DATE;
    DECLARE @StartDate52Week DATE;

    -- Get the most recent date available for the given ticker
    SELECT @MostRecentDate = MAX([Date])
    FROM dbo.Price
    WHERE Ticker = @Ticker;

    -- Calculate start dates
    SET @StartDate52Day = DATEADD(DAY, -51, @MostRecentDate); -- 52-day range
    SET @StartDate52Week = DATEADD(WEEK, -52, @MostRecentDate); -- 52-week range

    -- 52-day moving average price
    DECLARE @MovingAvg52Day DECIMAL(18, 2);
    SELECT @MovingAvg52Day = AVG([Close])
    FROM dbo.Price
    WHERE Ticker = @Ticker
      AND Date BETWEEN @StartDate52Day AND @MostRecentDate;

    -- 52-week high price
    DECLARE @High52Week DECIMAL(18, 2);
    SELECT @High52Week = MAX([High])
    FROM dbo.Price
    WHERE Ticker = @Ticker
      AND Date BETWEEN @StartDate52Week AND @MostRecentDate;

    -- 52-week low price
    DECLARE @Low52Week DECIMAL(18, 2);
    SELECT @Low52Week = MIN([Low])
    FROM dbo.Price
    WHERE Ticker = @Ticker
      AND Date BETWEEN @StartDate52Week AND @MostRecentDate;

    -- Return the results
    SELECT 
        @Ticker AS Ticker,
        @MostRecentDate AS AsOfDate,
        @MovingAvg52Day AS MovingAvg52Day,
        @High52Week AS High52Week,
        @Low52Week AS Low52Week;
END
GO