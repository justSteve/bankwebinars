/************************************************************************************************************/
/* show UserAccount records which do not appear to be properly verified */
/************************************************************************************************************/

SELECT
	[Key], IsAccountVerified,VerificationKey, VerificationPurpose, VerificationKeySent, VerificationStorage
FROM
	UserAccounts
WHERE
	IsAccountVerified = 0
OR	
	VerificationKey IS NOT NULL
OR	
	VerificationPurpose IS NOT NULL
OR	
	VerificationKeySent IS NOT NULL
OR	
	VerificationStorage IS NOT NULL

/************************************************************************************************************/
/* show UserAccount records with a 'troubled' history */
/************************************************************************************************************/
SELECT
	*
FROM
	UserAccounts
WHERE
	LastFailedLogin IS NOT NULL
OR
	FailedLoginCount > 0
OR
	IsAccountClosed > 0
OR
	AccountClosed IS NOT NULL

/************************************************************************************************************/
/* show UserAccounts records which do not have a FullName claim or a WebUser claim */
/************************************************************************************************************/
SELECT
	*
FROM
	UserAccounts ua
WHERE
	NOT EXISTS (SELECT 
					* 
				FROM 
					UserClaims uc	
				WHERE 
					uc.ParentKey = ua.[Key] 
				AND [Type] = 'http://cuwebinars.com/ws/2014/01/identity/claims/FullName')
OR
	NOT EXISTS (SELECT 
					* 
				FROM 
					UserClaims uc	
				WHERE 
					uc.ParentKey = ua.[Key] 
				AND [Type] = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role')

/************************************************************************************************************/
/* show WebUser records which do not have corresponding UserAccount records */
/************************************************************************************************************/
SELECT 
	wu.email, wu.idUser -- add any more cols desired
FROM 
	WebUser wu
WHERE
	NOT EXISTS (SELECT * FROM UserAccounts ua WHERE ua.Email = wu.Email)

/************************************************************************************************************/
/* show UserAccount records which do not have corresponding WebUser records */
/************************************************************************************************************/
SELECT 
	ua.email, ua.[Key] -- add any more cols desired
FROM
	UserAccounts ua
WHERE
	NOT EXISTS (SELECT * FROM WebUser wu WHERE wu.email = ua.Email)


/************************************************************************************************************/
/*	This is really to identify temporary anonymous users. This should be cleaned regularly. But care should be  
 *	taken to retain the record/row for problem orders, until the issue with that order has been resolved. */
/************************************************************************************************************/

SELECT 
	wu.email, wu.idUser -- add any more cols desired
FROM 
	WebUser wu
WHERE
	Email like '%@notauthenticated.com'
