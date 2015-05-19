/*************** set variables here ***************/ 

DECLARE  @webinarId INT;
DECLARE  @emailAddress NVARCHAR(150);
DECLARE  @quizId INT; -- set below in code
DECLARE  @orderId INT;

SET @webinarId = 1743;
SET @emailAddress = 'drogersbox-test1@yahoo.com.au';
SET @orderId = 13077

/*************** ****************** ***************/

-- get the id of the quiz
SELECT @quizId = qz.Id
FROM dbo.Quiz qz
INNER JOIN dbo.QuizUserOrder quo ON quo.idQuiz = qz.id
AND email = @emailAddress
AND QZ.idWebinar = @webinarId

/******** OVERALL SCORE *********/
SELECT Score, QuestionCount, DateQuizTaken, idOrder, Email
FROM [dbo].[QuizUserOrder] quo
WHERE quo.idQuiz = @quizId

/******** CORRECT ANSWERS *********/
SELECT results.QuestionNumber, results.UserAnswer, results.Correct
FROM 
	(SELECT qwq.QuestionNumber as QuestionNumber, qua.Letter as UserAnswer, qwo.CorrectAnswer as Correct
		FROM Quiz qz
		INNER JOIN [dbo].[QuizWithQuestion] qwq on qz.Id = qwq.idQuiz
		INNER JOIN [dbo].[Question] q on q.Id = qwq.idQuestion
		INNER JOIN [dbo].[QuestionWithOption] qwo on qwo.idQuestion = q.Id
		INNER JOIN [dbo].[QuizUserAnswer] qua on qua.idQuizQuestion = qwq.Id
		INNER JOIN [Option] o on o.Id = qwo.idOption
		where qz.idWebinar = @webinarId
		and email = @emailAddress
		and qwo.Letter = qua.Letter
	) as results
WHERE results.Correct = 1

/******** INCORRECT ANSWERS *********/

SELECT results.QuestionNumber, results.UserAnswer, results.Correct
FROM 
	(SELECT qwq.QuestionNumber as QuestionNumber, qua.Letter as UserAnswer, qwo.CorrectAnswer as Correct
		FROM Quiz qz
		INNER JOIN [dbo].[QuizWithQuestion] qwq on qz.Id = qwq.idQuiz
		INNER JOIN [dbo].[Question] q on q.Id = qwq.idQuestion
		INNER JOIN [dbo].[QuestionWithOption] qwo on qwo.idQuestion = q.Id
		INNER JOIN [dbo].[QuizUserAnswer] qua on qua.idQuizQuestion = qwq.Id
		INNER JOIN [Option] o on o.Id = qwo.idOption
		where qz.idWebinar = @webinarId
		and email = @emailAddress
		and qwo.Letter = qua.Letter
	) as results
WHERE results.Correct = 0

