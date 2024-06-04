use [PzenaInterviewAssessment]
go

CREATE TABLE Stock_Data (
  [table] VARCHAR(255) NOT NULL,  -- Name of the data source (SEP in your example)
  permaticker VARCHAR(255) PRIMARY KEY,  -- Unique identifier for the security
  ticker VARCHAR(255) NOT NULL,  -- Stock ticker symbol
  [name] VARCHAR(255) NOT NULL,  -- Full company name
  exchange VARCHAR(255),  -- Stock exchange (e.g., NASDAQ, NYSE)
  isdelisted CHAR(1),  -- Flag indicating if delisted (Y or N)
  category VARCHAR(255),  -- Security type (e.g., ADR Common Stock)
  cusips VARCHAR(255),  -- CUSIP identifiers (comma-separated)
  siccode VARCHAR(255),  -- Standard Industrial Classification code
  sicsector VARCHAR(255),  -- SIC industry sector
  sicindustry VARCHAR(255),  -- SIC industry
  famasector VARCHAR(255),  -- Fama-French sector
  famaindustry VARCHAR(255),  -- Fama-French industry
  sector VARCHAR(255),  -- Additional sector classification
  industry VARCHAR(255),  -- Additional industry classification
  scalemarketcap VARCHAR(255),  -- Market capitalization (scaled value)
  scalerevenue VARCHAR(255),  -- Revenue (scaled value)
  relatedtickers VARCHAR(255),  -- Comma-separated list of related tickers
  currency VARCHAR(255),  -- Currency code (e.g., USD)
  [location] VARCHAR(255),  -- Company location
  lastupdated DATETime,  -- Last date data was updated
  firstadded DATETime,  -- Date the security was first added
  firstpricedate DATETime,  -- Date of first available price data
  lastpricedate DATETime,  -- Date of last available price data
  firstquarter DATETime,  -- Date of the first quarter in financial statements
  lastquarter DATETime,  -- Date of the last quarter in financial statements
  secfilings nVARCHAR(255),  -- URL for SEC filings
  companysite nvarchar(255),
  CONSTRAINT unique_ticker UNIQUE (ticker)  -- Enforce unique ticker symbol (optional)
);