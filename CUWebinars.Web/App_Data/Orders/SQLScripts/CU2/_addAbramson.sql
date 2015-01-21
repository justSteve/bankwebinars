UPDATE dbo.Presenter SET photoFull = '/content/images/presenters/abramson.jpg', photoThumb = '/content/images/presenters/abramson.jpg' WHERE idUser = 33033
UPDATE users SET userType = 3 WHERE idUser = 33033
SELECT * FROM dbo.Users WHERE userType = 3

INSERT dbo.Presenter
        ( idUser ,
          biography ,
          biographyLong ,
          altEmail ,
          altPhone ,
          instantMessage ,
          photoFull ,
          photoThumb
        )
VALUES  ( 33033 , -- idUser - int
          '<p>Joel Abramson has been involved in all aspects of the financial services industry for more than twenty years.</p>' , -- biography - varchar(140)
          '<p>Joel Abramson has been involved in all aspects of the financial services industry for more than twenty years. He is currently the Director of Business Development and Strategy for Complete Data Products (CDP). Complete Data Products Inc. is the leader in paperless document management solutions, electronic receipts, and secure, encrypted digital signature technology. CDP also offers custom professional services such as email encryption, backup and disaster recovery, and custom laser print services. </p><p>Prior to joining CDP, Mr. Abramson was a Senior Vice President of a publicly traded bank and then moved on to a role as Chief Operating Officer of an international technology company that provided outsourced security solutions to organizations within the financial services vertical throughout the United States. </p>' , -- biography - varchar(140)
          '' , -- altEmail - varchar(50)
          '' , -- altPhone - varchar(50)
          '' , -- instantMessage - varchar(50)
          'abramson.jpg' , -- photoFull - varchar(200)
          'abramson.jpg'  -- photoThumb - varchar(200)
        )