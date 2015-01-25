USE BankWebinars
GO

--alter table dbo.Address 
--  DROP CONSTRAINT [FK_dbo.Address_dbo.WebUser_idUser]

--alter table dbo.Address 
--  add constraint 
--[FK_dbo.Address_dbo.WebUser_idUser]
--      foreign key (idUser) 
--      references dbo.WebUser(idUser) 
--      on delete cascade 
--      on update cascade
--
UPDATE  dbo.Affiliate
SET     ttsDomain = 'BankWebinars'
WHERE   idUserAff = 19
UPDATE  dbo.Affiliate a
SET     ttsDomain = LOWER(ttsDomain) ,
        WebBanner = '' ,
        Logo = 'http://images.ttstrain.com/images/logo_' + LOWER(ttsDomain) + '.png' ,
        WebFooter = '<p>' + DisplayTitle
        + ' .  ' + (SELECT City FROM dbo.WebUser WHERE idUser = a.idUserAff) + 'Waunakee, Wisconsin . 53597<br><strong>phone:</strong> <a href="tel:8008310678" class="tele">800.831.0678</a><br><strong>fax:</strong> 800.831.3776<br><strong>email:</strong> <a href="mailto:support@bankwebinars.com">support@bankwebinars.com</a></p>~<ul class="social"><li><a class="socicon small facebook" href="http://www.facebook.com/home.php#!/pages/Total-Training-Solutions/300546163412" target="_blank" data-placement="bottom" title="Follow us on Facebook"></a></li><li><a class="socicon small twitterbird" href="http://twitter.com/#!/ttstrain" target="_blank" data-placement="bottom" title="Follow us on Twitter"></a></li><li><a class="socicon small linkedin" href="https://www.linkedin.com/company/bankwebinars.com" target="_blank" data-placement="bottom" title="Follow us on LinkedIn"></a></li></ul>'
WHERE   idUserAff > 0
SET NOCOUNT OFF
UPDATE  dbo.Affiliate
SET     WebFooter = '<p><strong>HEADQUARTERS:</strong> 33 Broadway P.O. Box 969 Norwich, CT 06360<br><strong>TOLL FREE PHONE:</strong> 800-795-5242<br><strong>email:</strong> <a href="mailto:support@bankwebinars.com">support@bankwebinars.com</a></p>~<ul class="social"><li><a class="socicon small facebook" href="http://www.facebook.com/home.php#!/pages/Total-Training-Solutions/300546163412" target="_blank" data-placement="bottom" title="Follow us on Facebook"></a></li><li><a class="socicon small twitterbird" href="http://twitter.com/#!/ttstrain" target="_blank" data-placement="bottom" title="Follow us on Twitter"></a></li><li><a class="socicon small linkedin" href="https://www.linkedin.com/company/bankwebinars.com" target="_blank" data-placement="bottom" title="Follow us on LinkedIn"></a></li></ul>'
WHERE   idUserAff = 62
SET NOCOUNT ON
SELECT  *
FROM    dbo.Affiliate
--SELECT  'update WebUser set idUser = ' + CAST(( SELECT  u.idUser
--                                                FROM    dbo.WebUser u
--                                                WHERE   idUser = ( SELECT   idUserAff
--                                                                   FROM     dbo.Affiliate a
--                                                                   WHERE    a.idUserAff = u.idUser
--                                                                 )
--                                                        AND a.idUserAff = u.idUser
--                                              ) AS VARCHAR) + ' where email = '''
--        + ( SELECT  email
--            FROM    dbo.WebUser u
--            WHERE   idUser = ( SELECT   idUserAff
--                               FROM     dbo.Affiliate a
--                               WHERE    a.idUserAff = u.idUser
--                             )
--                    AND a.idUserAff = u.idUser
--          ) + ''''
--FROM    dbo.Affiliate a
