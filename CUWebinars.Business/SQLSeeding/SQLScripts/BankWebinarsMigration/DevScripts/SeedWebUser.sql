INSERT  BWClean.dbo.WebUser
        ( idUser ,
          UserType ,
          AcctStatus ,
          DateCreated ,
          FirstName ,
          LastName ,
          idUserInstitution ,
          email ,
          futureMail ,
          generalComments ,
          taxExempt ,
          idSubscriptionDiscount ,
          timeZone ,
          Title
        )
        SELECT  idUser ,
                UserType ,
                AcctStatus ,
                DateCreated ,
                FirstName ,
                LastName ,
                idUserInstitution ,
                email ,
                futureMail ,
                generalComments ,
                taxExempt ,
                idSubscriptionDiscount ,
                timeZone ,
                Title
        FROM    TTSWebinars2.dbo.Users
        WHERE   userType > 1
