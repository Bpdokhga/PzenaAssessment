use [PzenaAssessment2]

go

CREATE TABLE Price (
    Ticker VARCHAR(50) NOT NULL,
    [Date] DATE NOT NULL,
    [Open] DECIMAL(18, 2) NULL,
    [High] DECIMAL(18, 2) NULL,
    [Low] DECIMAL(18, 2) NULL,
    [Close] DECIMAL(18, 2) NULL,
    Volume DECIMAL(18, 2) NULL,
    CloseAdj DECIMAL(18, 2) NULL,
    CloseUnadj DECIMAL(18, 2) NULL,
    LastUpdated DATETIME NULL,
    PRIMARY KEY (Ticker, Date)
);

CREATE TABLE Ticker (
    [Table] VARCHAR(50) NOT NULL,
    Permaticker VARCHAR(50) NOT NULL,
    [Ticker] VARCHAR(50) NOT NULL,
    [Name] VARCHAR(255) NOT NULL,
    Exchange VARCHAR(50),
    IsDelisted CHAR(1) NOT NULL,
    Category VARCHAR(255),
    Cusips VARCHAR(255),
    SicCode VARCHAR(10),
    SicSector VARCHAR(255),
    SicIndustry VARCHAR(255),
    FamaSector VARCHAR(255),
    FamaIndustry VARCHAR(255),
    Sector VARCHAR(255),
    Industry VARCHAR(255),
    ScaleMarketCap VARCHAR(50),
    ScaleRevenue VARCHAR(50),
    RelatedTickers VARCHAR(255),
    Currency VARCHAR(10),
    [Location] VARCHAR(255),
    LastUpdated DATETIME,
    FirstAdded DATETIME,
    FirstPriceDate DATETIME,
    LastPriceDate DATETIME,
    FirstQuarter DATETIME,
    LastQuarter DATETIME,
    SecFilings VARCHAR(255),
    CompanySite VARCHAR(255),
    PRIMARY KEY ([Table], Permaticker, [Ticker])
);