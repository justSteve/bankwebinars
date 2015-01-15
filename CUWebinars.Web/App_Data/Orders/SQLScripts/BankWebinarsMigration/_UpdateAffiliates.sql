USE BankWebinars
GO
alter table dbo.Address 
  DROP CONSTRAINT [FK_dbo.Address_dbo.WebUser_idUser]

alter table dbo.Address 
  add constraint 
[FK_dbo.Address_dbo.WebUser_idUser]
      foreign key (idUser) 
      references dbo.WebUser(idUser) 
      on delete cascade 
      on update cascade
--
UPDATE  dbo.Affiliate
SET     ttsDomain = 'BankWebinars'
WHERE   idUserAff = 19
UPDATE  dbo.Affiliate
SET     ttsDomain = LOWER(ttsDomain) ,
        WebBanner = '' ,
        Logo = 'http://images.ttstrain.com/images/logo_' + LOWER(ttsDomain) + '.png' ,
        WebFooter = '<p>PO Box 310 . Waunakee, Wisconsin . 53597<br><strong>phone:</strong> <a href="tel:8008310678" class="tele">800.831.0678</a><br><strong>fax:</strong> 800.831.3776<br><strong>email:</strong> <a href="mailto:support@bankwebinars.com">support@bankwebinars.com</a></p>~<ul class="social"><li><a class="socicon small facebook" href="http://www.facebook.com/home.php#!/pages/Total-Training-Solutions/300546163412" target="_blank" data-placement="bottom" title="Follow us on Facebook"></a></li><li><a class="socicon small twitterbird" href="http://twitter.com/#!/ttstrain" target="_blank" data-placement="bottom" title="Follow us on Twitter"></a></li><li><a class="socicon small linkedin" href="https://www.linkedin.com/company/bankwebinars.com" target="_blank" data-placement="bottom" title="Follow us on LinkedIn"></a></li></ul>'
WHERE   idUserAff > 0

UPDATE  dbo.Affiliate
SET     WebFooter = '<p><strong>HEADQUARTERS:</strong> 33 Broadway P.O. Box 969 Norwich, CT 06360<br><strong>TOLL FREE PHONE:</strong> 800-795-5242<br><strong>email:</strong> <a href="mailto:support@bankwebinars.com">support@bankwebinars.com</a></p>~<ul class="social"><li><a class="socicon small facebook" href="http://www.facebook.com/home.php#!/pages/Total-Training-Solutions/300546163412" target="_blank" data-placement="bottom" title="Follow us on Facebook"></a></li><li><a class="socicon small twitterbird" href="http://twitter.com/#!/ttstrain" target="_blank" data-placement="bottom" title="Follow us on Twitter"></a></li><li><a class="socicon small linkedin" href="https://www.linkedin.com/company/bankwebinars.com" target="_blank" data-placement="bottom" title="Follow us on LinkedIn"></a></li></ul>' 
        
WHERE   idUserAff = 62
SELECT * FROM dbo.Affiliate
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
update WebUser set idUser = 19 where email = 'affiliate@ttstrain.com'
update WebUser set idUser = 29 where email = 'sfisher@mibankers.com'
update WebUser set idUser = 62 where email = 'deb@cftnow.org'
update WebUser set idUser = 375 where email = 'mataylor@cbao.com'
update WebUser set idUser = 376 where email = 'Sandy@cftncs.org'
update WebUser set idUser = 379 where email = 'shannon@cftum.org'
update WebUser set idUser = 380 where email = 'rhondap@cftws.org'
update WebUser set idUser = 384 where email = 'info@cftva.org'
update WebUser set idUser = 385 where email = 'dbaker@cftgpr.org'
update WebUser set idUser = 386 where email = 'cftrny@aol.com'
update WebUser set idUser = 387 where email = 'dphelps@cftuny.com'
update WebUser set idUser = 395 where email = 'claguna@mdc.edu'
update WebUser set idUser = 396 where email = 'cathy@tristatecft.org'
update WebUser set idUser = 963 where email = 'help@ozarkempirecft.org'
update WebUser set idUser = 1775 where email = 'ddriggers@gabankers.com'
update WebUser set idUser = 1827 where email = 'liz@ncbankers.org'
update WebUser set idUser = 2550 where email = 'jk.hopkins@verizon.net'
update WebUser set idUser = 2610 where email = 'ekilty@indianabankers.org'
update WebUser set idUser = 2636 where email = 'anewsom@oregonbankers.com'
update WebUser set idUser = 2984 where email = 'nvbankers@att.net'
update WebUser set idUser = 2986 where email = 'cbross@retrainingcenter.com'
update WebUser set idUser = 2988 where email = 'kbennett@ttstrain.com'
update WebUser set idUser = 2989 where email = 'wrosenthal@idahobankers.org'
update WebUser set idUser = 2990 where email = 'dpharr@mycbaa.com'
update WebUser set idUser = 2992 where email = 'bourgeois@lba.org'
update WebUser set idUser = 2993 where email = 'jdeal@sdba.com'
update WebUser set idUser = 2994 where email = 'datkinson@wvbankers.org'
update WebUser set idUser = 2997 where email = 'bwilkes@uba.org'
update WebUser set idUser = 10749 where email = 'cheryl@wyomingbankers.com'
update WebUser set idUser = 11240 where email = 'dorothy@ndba.com'
update WebUser set idUser = 11464 where email = 'tanya@cftea.org'
update WebUser set idUser = 12014 where email = 'greg@gregsouther.com'
update WebUser set idUser = 14577 where email = 'cfleming@vabankers.org'
update WebUser set idUser = 16132 where email = 'jamie.zussman@rate-watch.com'
update WebUser set idUser = 16392 where email = 'honey@interaction-training.com'
update WebUser set idUser = 17146 where email = 'bhoffman@ilbanker.com'
update WebUser set idUser = 17147 where email = 'kimp@minnbankers.com'
update WebUser set idUser = 17148 where email = 'jstone@ilfi.org'
update WebUser set idUser = 17149 where email = 'elegg@nyba.com'
update WebUser set idUser = 17150 where email = 'nelsenm@ctbank.com'
update WebUser set idUser = 17151 where email = 'tduncan@massbankers.org'
update WebUser set idUser = 17152 where email = 'stracy@nhbankers.com'
update WebUser set idUser = 17430 where email = 'nloppnow@wisbank.com'
update WebUser set idUser = 17431 where email = 'cgoncalves@njbankers.com'
update WebUser set idUser = 19741 where email = 'kami.taylor@arkbankers.org'
update WebUser set idUser = 20385 where email = 'pam@montanabankers.com'
update WebUser set idUser = 21066 where email = 'Liz@wabankers.com'
update WebUser set idUser = 22805 where email = 'scottspiewak@newswatchmedia.com'
update WebUser set idUser = 23801 where email = 'dschaefer@nmbankers.com'
update WebUser set idUser = 24033 where email = 'leaton@calbankers.com'
update WebUser set idUser = 24870 where email = 'auroratrainingadvantage@gmail.com'
update WebUser set idUser = 25993 where email = 'sanal@trainingpitstop.com'
update WebUser set idUser = 26276 where email = 'dburnett@iowabankers.com'
update WebUser set idUser = 26966 where email = 'sdspec@aol.com'
update WebUser set idUser = 27391 where email = 'pcross@kybanks.com'
update WebUser set idUser = 27842 where email = 'mtaylor@ksbankers.com'
update WebUser set idUser = 31267 where email = 'cunews@cunews.com'
----,
--SELECT  'DELETE FROM dbo.[Order] WHERE BillingEmail =''' 
--        + ( SELECT  email
--            FROM    dbo.WebUser u
--            WHERE   idUser = ( SELECT   idUserAff
--                               FROM     dbo.Affiliate a
--                               WHERE    a.idUserAff = u.idUser
--                             )
--                    AND a.idUserAff = u.idUser
--          ) + ''''
--FROM    dbo.Affiliate a
