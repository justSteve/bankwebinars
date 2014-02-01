USE BankWebinars
go
--SELECT TOP 1000  'EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = ''idWebUser=' + CAST( [idUser] AS VARCHAR)
--+'&FirstName=' + [FirstName]
--+'&lastname=' + [LastName]
--+'&username=' + firstName + lastName
--+'&institution=' + (SELECT DisplayTitle FROM TTSWebinars2.dbo.Affiliate WHERE idUser = [BankWebinarsSeeder].[dbo].[WebUser].idUser)
--+'&password=....' + LOWER(lastname)
--+'&confirmPassword=....' + LOWER(lastname)
--+'&email=' + [email]
--+'&usertype=1&title=na'
--+'&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'''
--  FROM [BankWebinarsSeeder].[dbo].[WebUser]
--  WHERE idUser IN (SELECT idUser FROM TTSWebinars2.dbo.Affiliate)

SET NOCOUNT ON
PRINT '--==========--'
PRINT '_________________________________________________________________________BEGINS Affiliate RESTORATION'
PRINT '--==========--'

DECLARE @ErrorLogID INT
DECLARE @UserID INT
SET NOCOUNT ON;
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=19&FirstName=Mark&lastname=Bennett&username=MarkBennett&institution=Total Training Solutions&password=....bennett&confirmPassword=....bennett&email=Mark_Bennett@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=29&FirstName=Stephanie Fisher&lastname=MBA&username=Stephanie FisherMBA&institution=Michigan Bankers Association&password=....mba&confirmPassword=....mba&email=StephanieFisher_MBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=62&FirstName=Deb Rondeau&lastname=CFTNOW&username=Deb RondeauCFTNOW&institution=CFT - Atlantic & Central States&password=....cftnow&confirmPassword=....cftnow&email=DebRondeau_CFTNOW@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=375&FirstName=Dawn Hoover&lastname=CBAO&username=Dawn HooverCBAO&institution=Community Bankers Association of Ohio&password=....cbao&confirmPassword=....cbao&email=DawnHoover_CBAO@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=376&FirstName=Sandy Kuehn&lastname=CFT-NCS&username=Sandy KuehnCFT-NCS&institution=CFT - North Central States&password=....cft-ncs&confirmPassword=....cft-ncs&email=SandyKuehn_CFT-NCS@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=379&FirstName=Shannon Duffy&lastname=CFT-UMW&username=Shannon DuffyCFT-UMW&institution=CFT - Upper Midwest&password=....cft-umw&confirmPassword=....cft-umw&email=ShannonDuffy_CFT-UMW@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=380&FirstName=Rhonda Potter&lastname=CFT-WS&username=Rhonda PotterCFT-WS&institution=CFT - Western States&password=....cft-ws&confirmPassword=....cft-ws&email=RhondaPotter_CFT-WS@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=383&FirstName=Connie Laguna&lastname=CFT-SF&username=Connie LagunaCFT-SF&institution=South Florida CFT&password=....cft-sf&confirmPassword=....cft-sf&email=ConnieLaguna_CFT-SF@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=384&FirstName=Pam Owens&lastname=CFT-VA&username=Pam OwensCFT-VA&institution=CFT - Virginia&password=....cft-va&confirmPassword=....cft-va&email=PamOwens_CFT-VA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=385&FirstName=Debbie Baker&lastname=CFT-GPR&username=Debbie BakerCFT-GPR&institution=CFT - Great Plains Region&password=....cft-gpr&confirmPassword=....cft-gpr&email=DebbieBaker_CFT-GPR@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=386&FirstName=CFT-RNY Admin&lastname=CFT-RNY&username=CFT-RNY AdminCFT-RNY&institution= &password=....cft-rny&confirmPassword=....cft-rny&email=CFT-RNYAdmin_CFT-RNY@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=387&FirstName=CFT -Upstate NY&lastname=CFT-UNY&username=CFT -Upstate NYCFT-UNY&institution=CFT - Upstate New York&password=....cft-uny&confirmPassword=....cft-uny&email=CFT-UpstateNY_CFT-UNY@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=394&FirstName=CFT-NJ Admin&lastname=CFT-NJ&username=CFT-NJ AdminCFT-NJ&institution=nj&password=....cft-nj&confirmPassword=....cft-nj&email=CFT-NJAdmin_CFT-NJ@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=395&FirstName=Lisa Phillips&lastname=CFT-SER&username=Lisa PhillipsCFT-SER&institution=Southeast Regional CFT&password=....cft-ser&confirmPassword=....cft-ser&email=LisaPhillips_CFT-SER@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=396&FirstName=Cathy Richardson&lastname=CFT-TS&username=Cathy RichardsonCFT-TS&institution=Tristate CFT&password=....cft-ts&confirmPassword=....cft-ts&email=CathyRichardson_CFT-TS@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=963&FirstName=Steve Sanpaolo&lastname=CFT-OZ&username=Steve SanpaoloCFT-OZ&institution=Ozark Empire CFT&password=....cft-oz&confirmPassword=....cft-oz&email=SteveSanpaolo_CFT-OZ@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=1775&FirstName=Courtenay Pope&lastname=GABA&username=Courtenay PopeGABA&institution=Georgia Bankers Association&password=....gaba&confirmPassword=....gaba&email=CourtenayPope_GABA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=1827&FirstName=Elizabeth C. Dobbins-Smith&lastname=NCBA&username=Elizabeth C. Dobbins-SmithNCBA&institution=North Carolina Bankers Association&password=....ncba&confirmPassword=....ncba&email=ElizabethC.Dobbins-Smith_NCBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=2550&FirstName=Jim Hopkins&lastname=CPD&username=Jim HopkinsCPD&institution=California Professional Development&password=....cpd&confirmPassword=....cpd&email=JimHopkins_CPD@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=2610&FirstName=Marlene Wells&lastname=INBA&username=Marlene WellsINBA&institution=Indiana Bankers Association&password=....inba&confirmPassword=....inba&email=MarleneWells_INBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=2636&FirstName=Allison Herr&lastname=ORBA&username=Allison HerrORBA&institution=Oregon Bankers Association&password=....orba&confirmPassword=....orba&email=AllisonHerr_ORBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=2984&FirstName=Bill Uffelman&lastname=NVBA&username=Bill UffelmanNVBA&institution=Nevada Bankers Association&password=....nvba&confirmPassword=....nvba&email=BillUffelman_NVBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=2986&FirstName=Charlie Bross&lastname=ABTC&username=Charlie BrossABTC&institution=American Bank Training Center&password=....abtc&confirmPassword=....abtc&email=CharlieBross_ABTC@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=2988&FirstName=Kyle Bennett&lastname=TTS-KB&username=Kyle BennettTTS-KB&institution=BankWebinars&password=....tts-kb&confirmPassword=....tts-kb&email=KyleBennett_TTS-KB@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=2989&FirstName=Wisti Rosenthal&lastname=IDBA&username=Wisti RosenthalIDBA&institution=Idaho Bankers Association&password=....idba&confirmPassword=....idba&email=WistiRosenthal_IDBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=2990&FirstName=Debbie Pharr&lastname=ALBA&username=Debbie PharrALBA&institution=Alabama Bankers Association&password=....alba&confirmPassword=....alba&email=DebbiePharr_ALBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=2992&FirstName=Marcy Bourgeois&lastname=LABA&username=Marcy BourgeoisLABA&institution=Louisiana Bankers Association&password=....laba&confirmPassword=....laba&email=MarcyBourgeois_LABA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=2993&FirstName=Joan Deal&lastname=SDBA&username=Joan DealSDBA&institution=South Dakota Bankers Association&password=....sdba&confirmPassword=....sdba&email=JoanDeal_SDBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=2994&FirstName=Donna Atkinson&lastname=WVBA&username=Donna AtkinsonWVBA&institution=West Virginia Bankers Association&password=....wvba&confirmPassword=....wvba&email=DonnaAtkinson_WVBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=2997&FirstName=Becky Wilkes&lastname=UTBA&username=Becky WilkesUTBA&institution=Utah Bankers Association&password=....utba&confirmPassword=....utba&email=BeckyWilkes_UTBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10749&FirstName=Cheryl Johnston&lastname=WYBA&username=Cheryl JohnstonWYBA&institution=Wyoming Bankers Association&password=....wyba&confirmPassword=....wyba&email=CherylJohnston_WYBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=11240&FirstName=Dorothy Lick&lastname=NDBA&username=Dorothy LickNDBA&institution=North Dakota Bankers Association&password=....ndba&confirmPassword=....ndba&email=DorothyLick_NDBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=11464&FirstName=Tanya Kolonoski&lastname=CFT-EA&username=Tanya KolonoskiCFT-EA&institution=Center for Financial Training & Education Alliance&password=....cft-ea&confirmPassword=....cft-ea&email=TanyaKolonoski_CFT-EA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=12014&FirstName=Greg&lastname=GSCS&username=GregGSCS&institution=Greg Souther Consulting&password=....gscs&confirmPassword=....gscs&email=Greg_GSCS@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=14577&FirstName=Courtney Fleming&lastname=VABA&username=Courtney FlemingVABA&institution=Virginia  Bankers Association&password=....vaba&confirmPassword=....vaba&email=CourtneyFleming_VABA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=16132&FirstName=Jamie Zussman&lastname=Rate-Watch&username=Jamie ZussmanRate-Watch&institution=Rate-Watch&password=....rate-watch&confirmPassword=....rate-watch&email=JamieZussman_Rate-Watch@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=17146&FirstName=Brian Hoffman&lastname=ILBA&username=Brian HoffmanILBA&institution=Illinois  Bankers Association&password=....ilba&confirmPassword=....ilba&email=BrianHoffman_ILBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=17147&FirstName=Kim Philipson&lastname=MNBA&username=Kim PhilipsonMNBA&institution=Minnesota  Bankers Association&password=....mnba&confirmPassword=....mnba&email=KimPhilipson_MNBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=17148&FirstName=Jane Stone&lastname=ILLG&username=Jane StoneILLG&institution=Illinois League of Financial Institutions&password=....illg&confirmPassword=....illg&email=JaneStone_ILLG@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=17149&FirstName=Elisa Legg&lastname=NYBA&username=Elisa LeggNYBA&institution=New York Bankers Association&password=....nyba&confirmPassword=....nyba&email=ElisaLegg_NYBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=17150&FirstName=Mary Beth Nelsen&lastname=CTBA&username=Mary Beth NelsenCTBA&institution=Connecticut  Bankers Association&password=....ctba&confirmPassword=....ctba&email=MaryBethNelsen_CTBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=17151&FirstName=Tanya Duncan&lastname=MABA&username=Tanya DuncanMABA&institution=Massachusetts Bankers Association&password=....maba&confirmPassword=....maba&email=TanyaDuncan_MABA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=17152&FirstName=Sandy Tracy&lastname=NHBA&username=Sandy TracyNHBA&institution=New Hampshire Bankers Association&password=....nhba&confirmPassword=....nhba&email=SandyTracy_NHBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=17430&FirstName=Nick Loppnow&lastname=WISBANK&username=Nick LoppnowWISBANK&institution=Wisconsin Bankers Association&password=....wisbank&confirmPassword=....wisbank&email=NickLoppnow_WISBANK@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=17431&FirstName=Cris Goncalves&lastname=NJBANKERS&username=Cris GoncalvesNJBANKERS&institution=New Jersey Bankers Association&password=....njbankers&confirmPassword=....njbankers&email=CrisGoncalves_NJBANKERS@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=19741&FirstName=Kami Taylor&lastname=ArkBankers&username=Kami TaylorArkBankers&institution=Arkansas Bankers Association&password=....arkbankers&confirmPassword=....arkbankers&email=KamiTaylor_ArkBankers@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=20385&FirstName=Pam OReilly&lastname=MTBA&username=Pam OReillyMTBA&institution=Montana Bankers Association&password=....mtba&confirmPassword=....mtba&email=PamOReilly_MTBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=21066&FirstName=Liz Wilson&lastname=WABANKERS&username=Liz WilsonWABANKERS&institution=Washington Bankers Association&password=....wabankers&confirmPassword=....wabankers&email=LizWilson_WABANKERS@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=22805&FirstName=Scott Spiewak&lastname=ConferencesandSeminars.org&username=Scott SpiewakConferencesandSeminars.org&institution=NewsWatchMedia&password=....conferencesandseminars.org&confirmPassword=....conferencesandseminars.org&email=ScottSpiewak_ConferencesandSeminars.org@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=23801&FirstName=Debbie Schaefer&lastname=NMBA&username=Debbie SchaeferNMBA&institution=New Mexico Bankers Association&password=....nmba&confirmPassword=....nmba&email=DebbieSchaefer_NMBA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=24033&FirstName=Laurie Eaton&lastname=CABA&username=Laurie EatonCABA&institution=California Bankers Association&password=....caba&confirmPassword=....caba&email=LaurieEaton_CABA@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=26276&FirstName=Darcy Burnett&lastname=IowaBankers&username=Darcy BurnettIowaBankers&institution=Iowa Bankers Association&password=....iowabankers&confirmPassword=....iowabankers&email=DarcyBurnett_IowaBankers@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'


INSERT [dbo].[Affiliate]
           

SELECT [idUserAff]
      ,[CommissionModel]
      ,[URL]
      ,[WebBanner]
      ,[WebFooter]
      ,[EmailBanner]
      ,[EmailFooter]
      ,[ttsDomain]
      ,[GAPass]
      ,[supportEmail]
      ,[DisplayTitle]
      ,[BillingModel]
      ,[Logo]
      ,[ContactPerson]
      ,[ContactPhone]
      ,[ContactEmail]
      ,[ContactFax]
      ,[ContactAddress]
      ,[TechEmail]
      ,[TechPhone]
      ,[TechName]
      ,[EmailPromo]
      ,[WebUser_Id]
  FROM BankWebinarsSeeder.[dbo].[Affiliate]
GO

--SELECT 'INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES (''' + CAST([ID] AS VARCHAR(40)) + ''', ''http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE'', ''Affiliate'')'
--  FROM [dbo].[UserAccounts] WHERE Email IN (
--'Mark_Bennett@ttstrain.com'
--,'StephanieFisher_MBA@ttstrain.com'
--,'DebRondeau_CFTNOW@ttstrain.com'
--,'DawnHoover_CBAO@ttstrain.com'
--,'SandyKuehn_CFT-NCS@ttstrain.com'
--,'ShannonDuffy_CFT-UMW@ttstrain.com'
--,'RhondaPotter_CFT-WS@ttstrain.com'
--,'ConnieLaguna_CFT-SF@ttstrain.com'
--,'PamOwens_CFT-VA@ttstrain.com'
--,'DebbieBaker_CFT-GPR@ttstrain.com'
--,'CFT-UpstateNY_CFT-UNY@ttstrain.com'
--,'CFT-NJAdmin_CFT-NJ@ttstrain.com'
--,'LisaPhillips_CFT-SER@ttstrain.com'
--,'CathyRichardson_CFT-TS@ttstrain.com'
--,'SteveSanpaolo_CFT-OZ@ttstrain.com'
--,'CourtenayPope_GABA@ttstrain.com'
--,'ElizabethC.Dobbins-Smith_NCBA@ttstrain.com'
--,'JimHopkins_CPD@ttstrain.com'
--,'MarleneWells_INBA@ttstrain.com'
--,'AllisonHerr_ORBA@ttstrain.com'
--,'BillUffelman_NVBA@ttstrain.com'
--,'CharlieBross_ABTC@ttstrain.com'
--,'KyleBennett_TTS-KB@ttstrain.com'
--,'WistiRosenthal_IDBA@ttstrain.com'
--,'DebbiePharr_ALBA@ttstrain.com'
--,'MarcyBourgeois_LABA@ttstrain.com'
--,'JoanDeal_SDBA@ttstrain.com'
--,'DonnaAtkinson_WVBA@ttstrain.com'
--,'BeckyWilkes_UTBA@ttstrain.com'
--,'CherylJohnston_WYBA@ttstrain.com'
--,'DorothyLick_NDBA@ttstrain.com'
--,'TanyaKolonoski_CFT-EA@ttstrain.com'
--,'Greg_GSCS@ttstrain.com'
--,'CourtneyFleming_VABA@ttstrain.com'
--,'JamieZussman_Rate-Watch@ttstrain.com'
--,'BrianHoffman_ILBA@ttstrain.com'
--,'KimPhilipson_MNBA@ttstrain.com'
--,'JaneStone_ILLG@ttstrain.com'
--,'ElisaLegg_NYBA@ttstrain.com'
--,'MaryBethNelsen_CTBA@ttstrain.com'
--,'TanyaDuncan_MABA@ttstrain.com'
--,'SandyTracy_NHBA@ttstrain.com'
--,'NickLoppnow_WISBANK@ttstrain.com'
--,'CrisGoncalves_NJBANKERS@ttstrain.com'
--,'KamiTaylor_ArkBankers@ttstrain.com'
--,'PamOReilly_MTBA@ttstrain.com'
--,'LizWilson_WABANKERS@ttstrain.com'
--,'ScottSpiewak_ConferencesandSeminars.org@ttstrain.com'
--,'DebbieSchaefer_NMBA@ttstrain.com'
--,'LaurieEaton_CABA@ttstrain.com'
--,'DarcyBurnett_IowaBankers@ttstrain.com'
--)
--GO


USE MembershipReboot
go

INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('C6633BE3-B6C6-44E3-94F0-1F695224E174', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('D8288650-8D74-4CE2-BB11-209FFC07C831', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('7B943D23-1221-4BB6-BA5F-25052BA390DD', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('E8E1EB1D-90A1-4DC4-AE41-291F699CFA7E', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('9662749D-1590-402F-B304-2E37FFA6F7AC', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('F8884833-95E4-4C25-A436-2FF90B7B3788', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('A69CC592-A2A9-4D1E-ABD5-3886DBC35020', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('D7016450-47A5-4B85-8960-3B1883EFA75B', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('BE7BC92E-EBAF-4929-9A19-3CA3F0383792', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('F1664E47-95EB-4A8C-A4FA-3EC6387CD8B1', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('D043E5F6-51F7-49F3-856E-42220DAD246B', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('2FBEEEC2-29AC-4CC4-80DC-42DD1D8FF643', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('A833CB7E-EC7F-4364-8C63-4570EE448EEC', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('3EEC9C1F-2E98-4999-BFF1-4C14BF18C981', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('66C14D05-7092-4CB3-8B3C-4CF5580D6C1B', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('6B66DDC1-EA81-4357-B9FB-607B8AFB35BE', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('96FB4F87-BB29-4D73-B3F9-6221B8328B1C', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('037764B5-325E-4D09-BBEC-67EFA8EEC6E9', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('63DF46CC-87DC-45E2-B3A0-6EBCBE9FC4A9', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('2CD0B20E-9DE1-4D01-AF64-7350855EE35A', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('8D674385-D988-4628-B2E3-79C083CE6342', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('E79A425F-1C11-47B3-B8A6-82D493530B1A', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('35148679-CDCA-4F72-9B6F-85CAEE0885BB', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('21363370-C048-4151-80B4-889838AF50A1', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('B297C105-AD0A-4579-9884-88EB14F717DA', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('6AF72BB7-6CAA-4C78-86B1-8EE0B0E8C399', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('13DC650A-C990-4B98-9109-92B2D0552F11', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('ECF08B98-C24F-485C-A10E-931D0391625F', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('DEC05F3A-FE63-4539-8CF2-9B48D805A894', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('0A65B2F5-E4B0-41BB-83CB-9CC463F4A1EF', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('5B208E57-BF99-49BC-85EC-9CC46DD82169', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('45C95BDC-F988-434C-BBCA-A7313A83CBFE', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('E19D1BE8-873E-42FA-9BD3-A7EDB448E06A', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('BF85D9EC-EE1D-4573-817B-B164F767FEE2', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('F8B088CE-925B-4398-B1CA-B8A9A1604BDC', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('BEA95C69-44A0-4960-AC7D-B916A1DA06F4', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('9FB99A88-F72A-490B-AE7B-BC052B64FC0A', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('23139037-0327-4079-A9B5-C080090B5F04', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('2A336154-55EE-404F-B483-D135DE3B43AA', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('E623E418-1FB9-4F53-B36A-D9EA7F38DC45', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('A0E43F8D-07D7-41A2-B66C-DA6F005C4238', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('B0382ED8-FC23-4CF3-95A5-DAB919FCA7A5', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('205B381E-4B8A-4026-9742-DB0DC1664A57', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('C35BEA54-E623-4611-BC3B-DD0D2797C587', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('92C58826-031E-4836-A4FA-E45C62AB7A64', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('D20CE7CD-816C-4E0C-B625-E84679E6D675', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('B48B1F85-49D7-4567-9966-F1DE6BF916CF', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('5577D910-9890-4CED-8D08-F61EAD8A930E', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('AE8A0538-4106-402F-85FC-F9174515A925', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('D48483DC-5340-474A-9295-FC8F1444DE6D', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('0EEC2F68-0EC6-4267-A11E-FEB0E54CEF5A', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Affiliate')