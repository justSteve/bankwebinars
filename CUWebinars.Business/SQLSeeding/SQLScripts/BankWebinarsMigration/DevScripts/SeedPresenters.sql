DELETE  BWClean.dbo.Affiliate
WHERE   idUserAff = 27121
DELETE  BWClean.dbo.WebUser
WHERE   idUser = 27121
INSERT  BWClean.dbo.Presenter
        ( idUser ,
          Biography ,
          BiographyLong ,
          PhotoFull ,
          PhotoThumb
        )
        SELECT  idUser ,
                Biography ,
                BiographyLong ,
                PhotoFull ,
                PhotoThumb
        FROM    TTSWebinars2.dbo.Presenter
        WHERE   idUser IN ( SELECT  idPresenter
                            FROM    TTSWebinars2.dbo.Webinar w
                            WHERE   w.status < 4 )
                AND idUser NOT IN ( 20621, 23630, 26016, 28875 )
