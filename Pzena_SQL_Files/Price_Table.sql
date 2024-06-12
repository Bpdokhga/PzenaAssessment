USE [PzenaAssessment]
GO

/****** Object:  Table [dbo].[Price]    Script Date: 6/12/2024 5:32:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Price](
	[Ticker] [varchar](50) NOT NULL,
	[Date] [date] NOT NULL,
	[Open] [decimal](18, 2) NULL,
	[High] [decimal](18, 2) NULL,
	[Low] [decimal](18, 2) NULL,
	[Close] [decimal](18, 2) NULL,
	[Volume] [decimal](18, 2) NULL,
	[CloseAdj] [decimal](18, 2) NULL,
	[CloseUnadj] [decimal](18, 2) NULL,
	[LastUpdated] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Ticker] ASC,
	[Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


