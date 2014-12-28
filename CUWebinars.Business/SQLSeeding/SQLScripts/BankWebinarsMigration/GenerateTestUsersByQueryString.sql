USE BankWebinars
GO
DELETE  FROM dbo.WebUser
WHERE   email = 'BW_15_PreEvent_1hr@LiveSessionOnly'

SET NOCOUNT On
DECLARE @idRegType INT
DECLARE @idWebinar INT
SET @idWebinar = 1

DECLARE @Name VARCHAR(MAX)
DECLARE @Phone VARCHAR(MAX)
DECLARE @StreetAddress VARCHAR(MAX)
DECLARE @StreetAddress2 VARCHAR(MAX)
DECLARE @City VARCHAR(MAX)
DECLARE @Zip VARCHAR(MAX)
DECLARE @State VARCHAR(MAX)
DECLARE @iterator INT
SET @iterator = 1
WHILE @idWebinar < 11
    BEGIN
        DECLARE myCURSOR CURSOR
        FOR
            SELECT  r.idRegType
            FROM    dbo.RegType r
                    INNER JOIN dbo.RegTypesXref rx ON rx.idRegType = r.idRegType
                    INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idRegTypeGroup = rx.idRegTypeGroup
                    INNER JOIN dbo.Webinar w ON w.idWebinar = rgx.idWebinar
                    INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
            WHERE   @idWebinar = w.idWebinar AND r.RegTypeLabel NOT LIKE '%upgrade%'
        OPEN myCURSOR
        FETCH NEXT FROM myCURSOR INTO @idRegType
        WHILE @@FETCH_STATUS = 0
            BEGIN
                IF EXISTS ( SELECT  idRegType
                            FROM    dbo.RegType
                            WHERE   idRegType = @idRegType )
                    SET @Name = CAST(@iterator AS VARCHAR) + '. '+ SUBSTRING(( SELECT  r.RegTypeLabel
                                            FROM    dbo.RegType r
                                                    INNER JOIN dbo.RegTypesXref rx ON rx.idRegType = r.idRegType
                                                    INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idRegTypeGroup = rx.idRegTypeGroup
                                                    INNER JOIN dbo.Webinar w ON w.idWebinar = rgx.idWebinar
                                                    INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
                                            WHERE   w.idWebinar = @idWebinar
                                                    AND @idRegType = r.idRegType
                                          ), 0, 39)

                SET @City = SUBSTRING(( SELECT  SKU
                                        FROM    dbo.RegType r
                                                INNER JOIN dbo.RegTypesXref rx ON rx.idRegType = r.idRegType
                                                INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idRegTypeGroup = rx.idRegTypeGroup
                                                INNER JOIN dbo.Webinar w ON w.idWebinar = rgx.idWebinar
                                                INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
                                        WHERE   w.idWebinar = @idWebinar
                                                AND @idRegType = r.idRegType
                                      ), 0, 39)

                              
							  
                SET @Phone = SUBSTRING(( SELECT regGroup.RegTypeGroupDesc
                                         FROM   dbo.RegType r
                                                INNER JOIN dbo.RegTypesXref rx ON rx.idRegType = r.idRegType
                                                INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idRegTypeGroup = rx.idRegTypeGroup
                                                INNER JOIN dbo.Webinar w ON w.idWebinar = rgx.idWebinar
                                                INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
                                         WHERE  w.idWebinar = @idWebinar
                                                AND @idRegType = r.idRegType
                                       ), 0, 39) 
										
                SET @State = 'state'	
                SET @StreetAddress = SUBSTRING(REPLACE(( SELECT
                                                              r.RegTypeLabel
                                                         FROM dbo.RegType r
                                                              INNER JOIN dbo.RegTypesXref rx ON rx.idRegType = r.idRegType
                                                              INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idRegTypeGroup = rx.idRegTypeGroup
                                                              INNER JOIN dbo.Webinar w ON w.idWebinar = rgx.idWebinar
                                                              INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
                                                         WHERE
                                                              w.idWebinar = @idWebinar
                                                              AND @idRegType = r.idRegType
                                                       ), ' ', ''), 0, 39)
                SET @StreetAddress2 = '' 
                SET @Zip = '54636'
													   
                SELECT  ( 'BillingAddress.AddressType=Billing&BillingAddress.Name=' + @Name + '&BillingAddress.Phone=' + @Phone + '&BillingAddress.StreetAddress=' + @StreetAddress + '&BillingAddress.StreetAddress2= ' + @StreetAddress2 + '&BillingAddress.City=' + @City + '&BillingAddress.Zip=' + @Zip + '&BillingAddress.State=' + @State + '&BillingAddress.Country=United States&ShippingAddress.AddressType=Shipping&ShippingAddress.Name=' + @Name + '&ShippingAddress.Phone=' + @Phone + '&ShippingAddress.StreetAddress=' + @StreetAddress + '&ShippingAddress.StreetAddress2= ' + @StreetAddress2 + '&ShippingAddress.City=' + @City + '&ShippingAddress.Zip=' + @Zip + '&ShippingAddress.State=' + @State + '&ShippingAddress.Country=United States&Email=test1_1@ttstrain.com&Title=Loan Officer&Institution=TTS&FirstName=test' + CAST(@idWebinar AS VARCHAR)+ '&LastName=' + CAST(@idRegType AS VARCHAR)+ '&idRegType=200&idWebinar='+CAST(@idWebinar AS VARCHAR) )					
                FETCH NEXT FROM myCURSOR INTO @idRegType
            END
        SET @idWebinar = @idWebinar + 1
		SET @iterator = @iterator + 1

        CLOSE myCURSOR
        DEALLOCATE myCURSOR
    END
	GO
    