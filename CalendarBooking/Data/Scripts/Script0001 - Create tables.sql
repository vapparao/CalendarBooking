CREATE TABLE [dbo].[Booking] (
    [Id]          BIGINT           IDENTITY (1, 1) NOT NULL,
    [PeriodStart] DATETIME2 (7) NOT NULL,
    [PeriodEnd]   DATETIME2 (7) NOT NULL,
    [Status]      VARCHAR (20)  NOT NULL,
    [CreatedBy]   VARCHAR (20),
    [CreatedDate]   DATETIME2 (7),
    [ModifiedBy]   VARCHAR (20),
    [ModifiedDate]   DATETIME2 (7),

    CONSTRAINT PK_Booking PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_Booking_Search]
    ON [dbo].[Booking]([PeriodStart] ASC, [PeriodEnd] ASC, [Status] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IDX_Booking_Unique]
    ON [dbo].[Booking]([PeriodStart] ASC, [PeriodEnd] ASC, [Status] ASC);