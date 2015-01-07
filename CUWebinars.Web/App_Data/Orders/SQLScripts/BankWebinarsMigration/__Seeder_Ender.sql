UPDATE  dbo.Presenter
SET     BiographyLong = REPLACE(BiographyLong,
                                'https://ttseast.blob.core.windows.net',
                                'http://images.ttstrain.com') ,
        PhotoFull = REPLACE(PhotoFull, 'https://ttseast.blob.core.windows.net',
                            'http://images.ttstrain.com') ,
        PhotoThumb = REPLACE(PhotoThumb,
                             'https://ttseast.blob.core.windows.net',
                             'http://images.ttstrain.com')
							 WHERE idUser > 1




PRINT '-----------------Seeder '
PRINT 'finished '
SELECT TOP 1 date, LTRIM(RTRIM(Title)), idWebinar FROM BankWebinars.dbo.Webinar WHERE Status = 2 ORDER BY Date





