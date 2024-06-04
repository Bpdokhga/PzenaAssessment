use [PzenaInterviewAssessment]
go

CREATE TABLE Prices (
  ticker VARCHAR(255) NOT NULL,  -- Ticker symbol (foreign key)
  [date] DATE NOT NULL,  -- Date of the price record
  [open] DECIMAL(10,2),  -- Opening price
  [high] DECIMAL(10,2),  -- High price
  [low] DECIMAL(10,2),  -- Low price
  [close] DECIMAL(10,2),  -- Closing price
  volume DECIMAL(10,2),  -- Volume traded
  closeadj DECIMAL(10,2),  -- Adjusted closing price
  closeunadj DECIMAL(10,2),  -- Unadjusted closing price
  lastupdated DATE,  -- Last date data was updated
  CONSTRAINT fk_stock_data FOREIGN KEY (ticker) REFERENCES Stock_Data(ticker)  -- Foreign key to stock_data.ticker
);