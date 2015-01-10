USE BankWebinars
GO

--
UPDATE dbo.Affiliate SET ttsDomain = 'BankWebinars' WHERE idUserAff = 19

UPDATE dbo.Affiliate SET WebFooter = '<p>PO Box 310 . Waunakee, Wisconsin . 53597<br><strong>phone:</strong> <a href="tel:8008310678" class="tele">800.831.0678</a><br><strong>fax:</strong> 800.831.3776<br><strong>email:</strong> <a href="mailto:support@bankwebinars.com">support@bankwebinars.com</a></p>~<ul class="social"><li><a class="socicon small facebook" href="http://www.facebook.com/home.php#!/pages/Total-Training-Solutions/300546163412" target="_blank" data-placement="bottom" title="Follow us on Facebook"></a></li><li><a class="socicon small twitterbird" href="http://twitter.com/#!/ttstrain" target="_blank" data-placement="bottom" title="Follow us on Twitter"></a></li><li><a class="socicon small linkedin" href="https://www.linkedin.com/company/bankwebinars.com" target="_blank" data-placement="bottom" title="Follow us on LinkedIn"></a></li></ul>'

 ,Logo = 'https://http://images.ttstrain.com/images/logoBW.png'

UPDATE dbo.Affiliate SET WebFooter = '<p><strong>HEADQUARTERS:</strong> 33 Broadway P.O. Box 969 Norwich, CT 06360<br><strong>TOLL FREE PHONE:</strong> 800-795-5242<br><strong>email:</strong> <a href="mailto:support@bankwebinars.com">support@bankwebinars.com</a></p>~<ul class="social"><li><a class="socicon small facebook" href="http://www.facebook.com/home.php#!/pages/Total-Training-Solutions/300546163412" target="_blank" data-placement="bottom" title="Follow us on Facebook"></a></li><li><a class="socicon small twitterbird" href="http://twitter.com/#!/ttstrain" target="_blank" data-placement="bottom" title="Follow us on Twitter"></a></li><li><a class="socicon small linkedin" href="https://www.linkedin.com/company/bankwebinars.com" target="_blank" data-placement="bottom" title="Follow us on LinkedIn"></a></li></ul>'

 ,Logo = 'https://ttseast.blob.core.windows.net/images/logoACS.png'
WHERE idUserAff = 62
