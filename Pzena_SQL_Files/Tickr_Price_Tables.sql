USE [PzenaAssessment3]
GO

/****** Object:  Table [dbo].[Ticker]    Script Date: 6/6/2024 7:07:07 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Ticker](
	[Table] [varchar](50) NOT NULL,
	[Permaticker] [varchar](50) NOT NULL,
	[Ticker] [varchar](50) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Exchange] [varchar](50) NULL,
	[IsDelisted] [char](1) NOT NULL,
	[Category] [varchar](255) NULL,
	[Cusips] [varchar](255) NULL,
	[SicCode] [varchar](10) NULL,
	[SicSector] [varchar](255) NULL,
	[SicIndustry] [varchar](255) NULL,
	[FamaSector] [varchar](255) NULL,
	[FamaIndustry] [varchar](255) NULL,
	[Sector] [varchar](255) NULL,
	[Industry] [varchar](255) NULL,
	[ScaleMarketCap] [varchar](50) NULL,
	[ScaleRevenue] [varchar](50) NULL,
	[RelatedTickers] [varchar](255) NULL,
	[Currency] [varchar](10) NULL,
	[Location] [varchar](255) NULL,
	[LastUpdated] [datetime] NULL,
	[FirstAdded] [datetime] NULL,
	[FirstPriceDate] [datetime] NULL,
	[LastPriceDate] [datetime] NULL,
	[FirstQuarter] [datetime] NULL,
	[LastQuarter] [datetime] NULL,
	[SecFilings] [varchar](255) NULL,
	[CompanySite] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Table] ASC,
	[Permaticker] ASC,
	[Ticker] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Ticker] UNIQUE NONCLUSTERED 
(
	[Ticker] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


