USE BankWebinars
go

/****** Script for SelectTopNRows command from SSMS  ******/
--SELECT TOP 1000  'EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = ''idWebUser=' + CAST( [idUser] AS VARCHAR)
--+'&FirstName=' + [FirstName]
--+'&lastname=' + [LastName]
--+'&username=' + firstName + lastName
--+'&institution=TTS' 
--+'&password=....' + LOWER(lastname)
--+'&confirmPassword=....' + LOWER(lastname)
--+'&email=' + [email]
--+'&usertype=1&title=na'
--+'&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'''
--  FROM [BankWebinarsSeeder].[dbo].[WebUser]
--  WHERE idUser IN (SELECT idUser FROM TTSWebinars2.dbo.Presenter)

  
DELETE BankWebinars.dbo.WebUser
DELETE BankWebinars.dbo.Affiliate
DELETE BankWebinars.dbo.Institution
DELETE BankWebinars.dbo.Addresses
DELETE BankWebinars.dbo.Presenter

SET NOCOUNT ON
PRINT '--==========--'
PRINT '_________________________________________________________________________BEGINS PRESENTER RESTORATION'
PRINT '--==========--'

DECLARE @ErrorLogID INT
DECLARE @UserID INT
SET NOCOUNT ON;
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=24&FirstName=Carl&lastname=Pry&username=CarlPry&institution=TTS&password=....pry&confirmPassword=....pry&email=Carl_Pry@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=25&FirstName=Honey&lastname=Shelton&username=HoneyShelton&institution=TTS&password=....shelton&confirmPassword=....shelton&email=Honey_Shelton@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=27&FirstName=Vin&lastname=DiCara&username=VinDiCara&institution=TTS&password=....dicara&confirmPassword=....dicara&email=Vin_DiCara@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=28&FirstName=Patrice&lastname=Konarik&username=PatriceKonarik&institution=TTS&password=....konarik&confirmPassword=....konarik&email=Patrice_Konarik@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=30&FirstName=Tony&lastname=Brissette&username=TonyBrissette&institution=TTS&password=....brissette&confirmPassword=....brissette&email=Tony_Brissette@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=32&FirstName=Al&lastname=Herrman&username=AlHerrman&institution=TTS&password=....herrman&confirmPassword=....herrman&email=Al_Herrman@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10563&FirstName=Adam&lastname=LaBoda&username=AdamLaBoda&institution=TTS&password=....laboda&confirmPassword=....laboda&email=Adam_LaBoda@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10564&FirstName=Ann&lastname=Thomas&username=AnnThomas&institution=TTS&password=....thomas&confirmPassword=....thomas&email=Ann_Thomas@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10565&FirstName=Dana&lastname=Sumner&username=DanaSumner&institution=TTS&password=....sumner&confirmPassword=....sumner&email=Dana_Sumner@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10566&FirstName=Bob&lastname=Gregg&username=BobGregg&institution=TTS&password=....gregg&confirmPassword=....gregg&email=Bob_Gregg@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10567&FirstName=David&lastname=P. McGuinn&username=DavidP. McGuinn&institution=TTS&password=....p. mcguinn&confirmPassword=....p. mcguinn&email=David_McGuinn@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10568&FirstName=Deborah&lastname=Crawford&username=DeborahCrawford&institution=TTS&password=....crawford&confirmPassword=....crawford&email=Deborah_Crawford@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10569&FirstName=Douglas&lastname=Waldorf&username=DouglasWaldorf&institution=TTS&password=....waldorf&confirmPassword=....waldorf&email=Douglas_Waldorf@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10570&FirstName=Eileen&lastname=Iles&username=EileenIles&institution=TTS&password=....iles&confirmPassword=....iles&email=Eileen_Iles@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10571&FirstName=Elizabeth&lastname=Fast&username=ElizabethFast&institution=TTS&password=....fast&confirmPassword=....fast&email=Elizabeth_Fast@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10572&FirstName=Eric&lastname=North&username=EricNorth&institution=TTS&password=....north&confirmPassword=....north&email=Eric_North@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10573&FirstName=Gary&lastname=Foltz&username=GaryFoltz&institution=TTS&password=....foltz&confirmPassword=....foltz&email=Gary_Foltz@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10574&FirstName=Greg&lastname=Souther&username=GregSouther&institution=TTS&password=....souther&confirmPassword=....souther&email=Greg_Souther@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10575&FirstName=Heberto&lastname=Gutiérrez&username=HebertoGutiérrez&institution=TTS&password=....gutiérrez&confirmPassword=....gutiérrez&email=Heberto_Gutiérrez@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10577&FirstName=J.T.&lastname=Turner&username=J.T.Turner&institution=TTS&password=....turner&confirmPassword=....turner&email=J.T._Turner@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10578&FirstName=Janice&lastname=Branch&username=JaniceBranch&institution=TTS&password=....branch&confirmPassword=....branch&email=Janice_Branch@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10580&FirstName=Jeffery&lastname=Johnson&username=JefferyJohnson&institution=TTS&password=....johnson&confirmPassword=....johnson&email=Jeffery_Johnson@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10581&FirstName=Jeni&lastname=Wehrmeyer&username=JeniWehrmeyer&institution=TTS&password=....wehrmeyer&confirmPassword=....wehrmeyer&email=Jeni_Wehrmeyer@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10582&FirstName=Jennifer&lastname=Burke&username=JenniferBurke&institution=TTS&password=....burke&confirmPassword=....burke&email=Jennifer_Burke@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10583&FirstName=Jim&lastname=Rechel&username=JimRechel&institution=TTS&password=....rechel&confirmPassword=....rechel&email=Jim_Rechel@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10584&FirstName=John&lastname=Barrickman&username=JohnBarrickman&institution=TTS&password=....barrickman&confirmPassword=....barrickman&email=John_Barrickman@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10585&FirstName=John&lastname=Oliver&username=JohnOliver&institution=TTS&password=....oliver&confirmPassword=....oliver&email=John_Oliver@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10586&FirstName=Karl&lastname=Nelson&username=KarlNelson&institution=TTS&password=....nelson&confirmPassword=....nelson&email=Karl_Nelson@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10587&FirstName=Kathy&lastname=Reed&username=KathyReed&institution=TTS&password=....reed&confirmPassword=....reed&email=Kathy_Reed@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10589&FirstName=Linda&lastname=Berke&username=LindaBerke&institution=TTS&password=....berke&confirmPassword=....berke&email=Linda_Berke@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10590&FirstName=Margaret&lastname=DeMarino&username=MargaretDeMarino&institution=TTS&password=....demarino&confirmPassword=....demarino&email=Margaret_DeMarino@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10591&FirstName=Mark&lastname=Trinkle&username=MarkTrinkle&institution=TTS&password=....trinkle&confirmPassword=....trinkle&email=Mark_Trinkle@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10592&FirstName=Mark&lastname=Tyrpin&username=MarkTyrpin&institution=TTS&password=....tyrpin&confirmPassword=....tyrpin&email=Mark_Tyrpin@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10594&FirstName=Michael&lastname=Caldwell&username=MichaelCaldwell&institution=TTS&password=....caldwell&confirmPassword=....caldwell&email=Michael_Caldwell@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10595&FirstName=Michael&lastname=Percy&username=MichaelPercy&institution=TTS&password=....percy&confirmPassword=....percy&email=Michael_Percy@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10596&FirstName=Nicole&lastname=Durbin&username=NicoleDurbin&institution=TTS&password=....durbin&confirmPassword=....durbin&email=Nicole_Durbin@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10598&FirstName=Patricia&lastname=Cashman&username=PatriciaCashman&institution=TTS&password=....cashman&confirmPassword=....cashman&email=Patricia_Cashman@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10599&FirstName=Phil&lastname=Bruno&username=PhilBruno&institution=TTS&password=....bruno&confirmPassword=....bruno&email=Phil_Bruno@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10600&FirstName=Ray&lastname=Adler&username=RayAdler&institution=TTS&password=....adler&confirmPassword=....adler&email=Ray_Adler@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10601&FirstName=Richard&lastname=Peterson&username=RichardPeterson&institution=TTS&password=....peterson&confirmPassword=....peterson&email=Richard_Peterson@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10602&FirstName=Roger&lastname=Morin&username=RogerMorin&institution=TTS&password=....morin&confirmPassword=....morin&email=Roger_Morin@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10603&FirstName=Ron&lastname=Shapiro&username=RonShapiro&institution=TTS&password=....shapiro&confirmPassword=....shapiro&email=Ron_Shapiro@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10604&FirstName=Scott&lastname=Becker&username=ScottBecker&institution=TTS&password=....becker&confirmPassword=....becker&email=Scott_Becker@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10605&FirstName=Scott&lastname=Fink&username=ScottFink&institution=TTS&password=....fink&confirmPassword=....fink&email=Scott_Fink@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10606&FirstName=Stan&lastname=Lange&username=StanLange&institution=TTS&password=....lange&confirmPassword=....lange&email=Stan_Lange@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10607&FirstName=Susan&lastname=Orr&username=SusanOrr&institution=TTS&password=....orr&confirmPassword=....orr&email=Susan_Orr@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10608&FirstName=Teresa&lastname=Allen&username=TeresaAllen&institution=TTS&password=....allen&confirmPassword=....allen&email=Teresa_Allen@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=10609&FirstName=Terri D.&lastname=Thomas&username=Terri D.Thomas&institution=TTS&password=....thomas&confirmPassword=....thomas&email=TerriD._Thomas@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=11114&FirstName=Nancy&lastname=Flynn&username=NancyFlynn&institution=TTS&password=....flynn&confirmPassword=....flynn&email=Nancy_Flynn@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=11158&FirstName=Anne&lastname=Lolley&username=AnneLolley&institution=TTS&password=....lolley&confirmPassword=....lolley&email=Anne_Lolley@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=12130&FirstName=Philip &lastname=Vassallo&username=Philip Vassallo&institution=TTS&password=....vassallo&confirmPassword=....vassallo&email=Philip_Vassallo@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=12981&FirstName=Richard&lastname=Symansky&username=RichardSymansky&institution=TTS&password=....symansky&confirmPassword=....symansky&email=Richard_Symansky@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=14668&FirstName=Susan&lastname=Costonis&username=SusanCostonis&institution=TTS&password=....costonis&confirmPassword=....costonis&email=Susan_Costonis@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=15002&FirstName=Asaad&lastname=Faquir&username=AsaadFaquir&institution=TTS&password=....faquir&confirmPassword=....faquir&email=Asaad_Faquir@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=15050&FirstName=Joseph&lastname=Tinucci&username=JosephTinucci&institution=TTS&password=....tinucci&confirmPassword=....tinucci&email=Joseph_Tinucci@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=15167&FirstName=Troy&lastname=Evans&username=TroyEvans&institution=TTS&password=....evans&confirmPassword=....evans&email=Troy_Evans@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=16304&FirstName=Mark&lastname=Solomon&username=MarkSolomon&institution=TTS&password=....solomon&confirmPassword=....solomon&email=Mark_Solomon@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=16305&FirstName=Jim&lastname=McCain&username=JimMcCain&institution=TTS&password=....mccain&confirmPassword=....mccain&email=Jim_McCain@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=16323&FirstName=Carolyn&lastname=Dowdy&username=CarolynDowdy&institution=TTS&password=....dowdy&confirmPassword=....dowdy&email=Carolyn_Dowdy@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=16913&FirstName=Denis&lastname=Kelly&username=DenisKelly&institution=TTS&password=....kelly&confirmPassword=....kelly&email=Denis_Kelly@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=17349&FirstName=Thomas&lastname=Pinkowish&username=ThomasPinkowish&institution=TTS&password=....pinkowish&confirmPassword=....pinkowish&email=Thomas_Pinkowish@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=18053&FirstName=Jim&lastname=DeMaio&username=JimDeMaio&institution=TTS&password=....demaio&confirmPassword=....demaio&email=Jim_DeMaio@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=18274&FirstName=Jerry&lastname=Smith&username=JerrySmith&institution=TTS&password=....smith&confirmPassword=....smith&email=Jerry_Smith@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=18276&FirstName=John&lastname=Behringer&username=JohnBehringer&institution=TTS&password=....behringer&confirmPassword=....behringer&email=John_Behringer@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=18277&FirstName=John&lastname=Knight&username=JohnKnight&institution=TTS&password=....knight&confirmPassword=....knight&email=John_Knight@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=18279&FirstName=Sophie&lastname=Kelley&username=SophieKelley&institution=TTS&password=....kelley&confirmPassword=....kelley&email=Sophie_Kelley@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=18281&FirstName=Terry&lastname=Saber&username=TerrySaber&institution=TTS&password=....saber&confirmPassword=....saber&email=Terry_Saber@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=18308&FirstName=Walt&lastname=Gerano&username=WaltGerano&institution=TTS&password=....gerano&confirmPassword=....gerano&email=Walt_Gerano@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=18309&FirstName=Chris&lastname=Carlson&username=ChrisCarlson&institution=TTS&password=....carlson&confirmPassword=....carlson&email=Chris_Carlson@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=19383&FirstName=Laurie &lastname=Appelbaum&username=Laurie Appelbaum&institution=TTS&password=....appelbaum&confirmPassword=....appelbaum&email=Laurie_Appelbaum@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=21565&FirstName=Sal&lastname=Fragapane&username=SalFragapane&institution=TTS&password=....fragapane&confirmPassword=....fragapane&email=Sal_Fragapane@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=23860&FirstName=Christy&lastname=Crawford&username=ChristyCrawford&institution=TTS&password=....crawford&confirmPassword=....crawford&email=Christy_Crawford@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=24203&FirstName=Ross&lastname=Blake&username=RossBlake&institution=TTS&password=....blake&confirmPassword=....blake&email=Ross_Blake@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=24210&FirstName=Sandy&lastname=Swartzberg&username=SandySwartzberg&institution=TTS&password=....swartzberg&confirmPassword=....swartzberg&email=Sandy_Swartzberg@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=25143&FirstName=Kris&lastname=Kavelaris&username=KrisKavelaris&institution=TTS&password=....kavelaris&confirmPassword=....kavelaris&email=Kris_Kavelaris@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=25154&FirstName=Tom &lastname=Hinkel&username=Tom Hinkel&institution=TTS&password=....hinkel&confirmPassword=....hinkel&email=Tom_Hinkel@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'
EXEC TTSWebinarsSeeder.dbo.[CallCreateUser] @qString = 'idWebUser=25517&FirstName=Crawford&lastname=Shelton&username=CrawfordShelton&institution=TTS&password=....shelton&confirmPassword=....shelton&email=Crawford_Shelton@ttstrain.com&usertype=1&title=na&city=city&addresstype=Billing&country=US&phone=555-555-5555&state=st&streetaddress=streetaddress&streetaddress2=streetaddress2&zip=55555'



INSERT [dbo].Presenter
        ( idUser ,
          Biography ,
          BiographyLong ,
          PhotoFull ,
          PhotoThumb
        )
SELECT *
  FROM BankWebinarsSeeder.[dbo].Presenter
GO
USE MembershipReboot
go
--SELECT 'INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES (''' + CAST([ID] AS VARCHAR(40)) + ''', ''http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE'', ''Presenter'')'
--  FROM [dbo].[UserAccounts] WHERE Email IN (

--'Carl_Pry@ttstrain.com'
--,'Honey_Shelton@ttstrain.com'
--,'Vin_DiCara@ttstrain.com'
--,'Patrice_Konarik@ttstrain.com'
--,'Tony_Brissette@ttstrain.com'
--,'Al_Herrman@ttstrain.com'
--,'Adam_LaBoda@ttstrain.com'
--,'Ann_Thomas@ttstrain.com'
--,'Dana_Sumner@ttstrain.com'
--,'Bob_Gregg@ttstrain.com'
--,'David_McGuinn@ttstrain.com'
--,'Deborah_Crawford@ttstrain.com'
--,'Douglas_Waldorf@ttstrain.com'
--,'Eileen_Iles@ttstrain.com'
--,'Elizabeth_Fast@ttstrain.com'
--,'Eric_North@ttstrain.com'
--,'Gary_Foltz@ttstrain.com'
--,'Greg_Souther@ttstrain.com'
--,'J.T._Turner@ttstrain.com'
--,'Janice_Branch@ttstrain.com'
--,'Jeffery_Johnson@ttstrain.com'
--,'Jeni_Wehrmeyer@ttstrain.com'
--,'Jennifer_Burke@ttstrain.com'
--,'Jim_Rechel@ttstrain.com'
--,'John_Barrickman@ttstrain.com'
--,'John_Oliver@ttstrain.com'
--,'Karl_Nelson@ttstrain.com'
--,'Kathy_Reed@ttstrain.com'
--,'Linda_Berke@ttstrain.com'
--,'Margaret_DeMarino@ttstrain.com'
--,'Mark_Trinkle@ttstrain.com'
--,'Mark_Tyrpin@ttstrain.com'
--,'Michael_Caldwell@ttstrain.com'
--,'Michael_Percy@ttstrain.com'
--,'Nicole_Durbin@ttstrain.com'
--,'Patricia_Cashman@ttstrain.com'
--,'Phil_Bruno@ttstrain.com'
--,'Ray_Adler@ttstrain.com'
--,'Richard_Peterson@ttstrain.com'
--,'Roger_Morin@ttstrain.com'
--,'Ron_Shapiro@ttstrain.com'
--,'Scott_Becker@ttstrain.com'
--,'Scott_Fink@ttstrain.com'
--,'Stan_Lange@ttstrain.com'
--,'Susan_Orr@ttstrain.com'
--,'Teresa_Allen@ttstrain.com'
--,'TerriD._Thomas@ttstrain.com'
--,'Nancy_Flynn@ttstrain.com'
--,'Anne_Lolley@ttstrain.com'
--,'Philip_Vassallo@ttstrain.com'
--,'Richard_Symansky@ttstrain.com'
--,'Susan_Costonis@ttstrain.com'
--,'Asaad_Faquir@ttstrain.com'
--,'Joseph_Tinucci@ttstrain.com'
--,'Troy_Evans@ttstrain.com'
--,'Mark_Solomon@ttstrain.com'
--,'Jim_McCain@ttstrain.com'
--,'Carolyn_Dowdy@ttstrain.com'
--,'Denis_Kelly@ttstrain.com'
--,'Thomas_Pinkowish@ttstrain.com'
--,'Jim_DeMaio@ttstrain.com'
--,'Jerry_Smith@ttstrain.com'
--,'John_Behringer@ttstrain.com'
--,'John_Knight@ttstrain.com'
--,'Sophie_Kelley@ttstrain.com'
--,'Terry_Saber@ttstrain.com'
--,'Walt_Gerano@ttstrain.com'
--,'Chris_Carlson@ttstrain.com'
--,'Laurie_Appelbaum@ttstrain.com'
--,'Sal_Fragapane@ttstrain.com'
--,'Christy_Crawford@ttstrain.com'
--,'Ross_Blake@ttstrain.com'
--,'Sandy_Swartzberg@ttstrain.com'
--,'Kris_Kavelaris@ttstrain.com'
--,'Tom_Hinkel@ttstrain.com'
--,'Crawford_Shelton@ttstrain.com')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('EB864B3F-0033-4CF7-89AD-00DDD19F37A5', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('5FA63B75-8925-45C7-86BF-0879AD5AD069', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('ED6B1D08-0890-4647-BDF2-0BFEEB33B466', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('50DA5DB7-05A7-4CB1-8ECC-0E6868F650EF', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('3881BC2F-0FCC-44D4-8FE8-0E9B6B38C779', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('C734FAD0-A57F-43A1-9148-0FA745A40FF2', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('C51B65D2-46DD-41E0-BDD3-0FE02EC32AF5', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('CF1327C6-EAB4-4B60-B7AA-1023EE05FDD2', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('6DAF427B-18B8-4BCD-B38B-12C449A8908E', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('6616815A-F2A2-4ED3-A056-2571A5C6CD73', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('A249AD1D-2B60-4AE7-AA87-27168B075B20', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('343CA12D-B478-402C-B4CE-2B9CA8787FEF', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('43A1D9C7-3EC0-4776-937F-2CB2D6D3647C', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('C4E30E91-92CD-454C-B84C-308186D2AB90', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('78C0F4B1-5B5F-4E20-BBB3-36DB0A29D6DA', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('ACA7391F-B620-452E-858C-3725EE8B2B1C', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('20D67D2A-6B4D-4E61-92AE-3E98976F428E', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('41577052-79C3-4434-ADAC-4143ADE91803', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('CAF03EC0-A278-455B-8AA1-421C733EBC47', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('3315E95D-2F8A-4B96-A8F8-479746552CBB', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('D4F4B140-8ED0-449D-8863-4C0924A02CA2', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('C9402060-3871-43B0-8D22-50E239D9EC63', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('D6B2EA88-F27D-42CF-BA9C-562306290D2A', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('9EC60E0C-8196-4FFA-9942-57F4C4537E56', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('CF9580B1-3E2D-4700-B266-5A0960594E73', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('00CE8BE4-51EA-4B40-BFEF-64CFEF330CF0', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('F3913CEA-9835-42A9-9818-669ADE312F79', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('135BF820-2A56-4296-A69C-68187D46BF5F', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('F8DE0752-C626-4ADB-B83C-697FC828C9DB', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('11AC2B86-CE38-4A7A-95E4-6DC3FB97EE98', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('FCF49946-4C84-417E-8263-6EC22B384E41', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('D0F6D493-DCD8-401A-B83B-6EF016712128', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('6BAFAAAB-273C-481B-A334-71FE39D39128', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('E20FC1CD-16EE-44FA-888D-720FB8D8CDE9', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('B39BF19F-78F2-4E09-91D0-72E56B63F03B', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('279800EA-BF0E-44A0-B253-755F636D2BD6', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('7A2910AC-1D08-47CB-8D31-7E5F7D30D227', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('ECDAE8D5-B963-4FB7-B205-803218858741', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('F9F2D2C8-BC5A-4DA6-89DB-80C2E9B521B5', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('907B38E1-A6A8-4BFC-A8C6-81BB6FEC345A', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('B4B4161B-07F4-44D8-A0D7-83B55C6BDE1C', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('D2401F8B-0D14-4F81-A01A-83D8EFB88F30', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('8A411002-F085-4CA3-B064-87521B170415', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('3B2CDA23-70CF-45A9-9B03-87CFC60F8449', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('BD2789F4-DF89-4AF5-B024-897F94C5A50A', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('0EE58A9B-A94D-47DB-B18A-8AFB6BA6FA7A', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('960171D7-453E-4BFE-A84C-8FBD01D20EFC', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('ADC82863-8858-474C-B648-989836095EDF', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('F2862D1B-9B15-4351-95A4-99CA64BBC8F4', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('9A8E6400-E638-49D2-AEF3-9BC40589CA4E', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('F8D58459-CA13-4AF8-9641-9DA05899452F', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('44E4DAE8-C56C-471D-806B-9F9718787BBB', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('734C668C-1D98-4314-8FDB-A13E5C6210DD', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('938E6882-4C79-4B4B-852E-A6F77E3AEA3B', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('2A9B4E92-C448-445C-BFCB-A8601770E590', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('2CDFD42A-4E2F-4EC9-8200-AB86B7010F46', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('631124D8-3933-4A4F-8CDA-B23551D86819', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('28C60937-C9AA-4267-86CD-B2D12181DA68', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('FB375F58-6304-4155-AD36-B3518618FCCB', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('EBBEB105-C13C-46C9-9479-BB54AE227BB0', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('0ED66583-502F-4E8A-851C-BB87F35ADCF8', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('61516556-B8DA-4D93-97AF-C125D16ACC47', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('24516E59-797A-47B2-B5F8-C239933D37E5', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('07C67439-F3F1-4C3A-8AAE-C66DA9A59B48', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('5C6EF0B8-3DA1-4306-95F5-CBDE7936EA1B', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('09D62374-2FA4-448B-8EC1-CE29A7442962', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('7092B76A-88C6-4567-9AFE-D0A1F6B0A445', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('BF0FEBBF-D9F4-4F57-A98A-D7AE526E38D8', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('B554A3FE-D2BE-44BA-A2CA-DDC6E4FBBC1A', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('FCD54913-2E67-4EBF-BA4C-E6A8BD1C2CBE', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('3CBE9574-68EF-452B-95C0-E76D81F97CC4', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('1D91018D-32F6-4645-9335-EC35000BCA3F', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('48B7AE3D-6E50-4E9D-9736-EC5194E3D3AC', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('6D67B505-F2AA-4DDF-A883-F632D679ECEC', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('0C1169DF-EA52-497C-B35A-FDD1291ACFF1', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')
INSERT INTO [dbo].[UserClaims] ([UserAccountID] ,[Type] ,[Value]) VALUES ('10882ED7-2527-4611-8981-FE94196A721D', 'http://schemas.microsoft.com/ws/2008/06/identity/claims/ROLE', 'Presenter')