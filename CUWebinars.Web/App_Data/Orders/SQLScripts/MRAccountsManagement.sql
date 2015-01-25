--DELETE FROM dbo.WebUser WHERE idUser IN (SELECT idUser FROM dbo.WebUser WHERE UserType = 1 AND email LIKE '%ttstrain.com')

--UPDATE dbo.WebUser SET email = LOWER(LastName)+'@ttspresenters.com' WHERE UserType = 3
--(SELECT * FROM dbo.WebUser WHERE --UserType = 3 AND 
--email LIKE '%ttstrain.com')
SELECT o.Total,  * FROM dbo.[Order] o INNER JOIN dbo.OrderRow r ON r.idOrder = o.idOrder --INNER JOIN dbo.AdditionalLocation a ON a.idOrderRow = r.idOrderRow
WHERE o.idOrder = 37