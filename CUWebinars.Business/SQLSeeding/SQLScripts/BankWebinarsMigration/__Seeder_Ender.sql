
PRINT '-----------------Seeder '
PRINT 'finished '
SELECT TOP 1 date, LTRIM(RTRIM(Title)), idWebinar FROM BankWebinars.dbo.Webinar WHERE Status = 2 ORDER BY Date
