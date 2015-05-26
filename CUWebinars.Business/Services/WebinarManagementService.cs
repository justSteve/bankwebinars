using System.Diagnostics;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.NotificationSystem.Event;
using FluentValidation;
using Ninject.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RazorEngine.Templating;

namespace CUWebinars.Business.Services
{
    public class WebinarManagementService : IEventSource, IWebinarManagementService
    {
        //private readonly IAffiliateRepository _affiliateRepository;
        private readonly IRegTypeRepository _regTypeRepository;
        //private readonly IOrderRepository _orderRepository;
        private readonly IRefDataRepository _refDataRepository;
        private readonly IWebinarRepository _webinarRepository;
        private readonly IWebinarFileRepository _webinarFileRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly ILogger _logger;
        private readonly IWebUserRepository _webUserRepository;
        private readonly TtsConfiguration _ttsConfig;
        private readonly IValidator<Webinar> _createWebinarValidator;
        private readonly IValidator<Webinar> _updateWebinarValidator;
        readonly List<IEvent> _events = new List<IEvent>();
        private bool _disposed;

        public WebinarManagementService(
            //IAffiliateRepository affiliateRepository,
            IRegTypeRepository regTypeRepository,
            //IOrderRepository orderRepository,
            IRefDataRepository refDataRepository,
            IWebUserRepository webUserRepository,
            IWebinarRepository webinarRepository,
            IWebinarFileRepository webinarFileRepository,
            IQuizRepository quizRepository,
            ILogger logger,
            TtsConfiguration ttsConfig,
            IValidator<Webinar> createWebinarValidator,
            IValidator<Webinar> updateWebinarValidator)
        {
            //_affiliateRepository = affiliateRepository;
            _regTypeRepository = regTypeRepository;
            //_orderRepository = orderRepository;
            _refDataRepository = refDataRepository;
            _ttsConfig = ttsConfig;
            _createWebinarValidator = createWebinarValidator;
            _updateWebinarValidator = updateWebinarValidator;
            _webinarFileRepository = webinarFileRepository;
            _quizRepository = quizRepository;
            _webinarRepository = webinarRepository;
            _logger = logger;
            _webUserRepository = webUserRepository;
        }
        public IEnumerable<Presenter> GetAllPresenters()
        {
            try
            {
                return _refDataRepository.GetAllPresenters();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetAllPresenters method", exception);
                throw;
            }

        }

        public IEnumerable<Topic> GetAllTopics()
        {
            return _webinarRepository.GetAllTopics();
        }

        public void AddAdditionalLocationsLookupPrice(AdditionalLocationsLookupPrice additionalLocationsLookupPrice)
        {
            // NOTE: db.SaveChanges is not called in the following method. 
            _webinarRepository.AddAdditionalLocationsLookupPrice(additionalLocationsLookupPrice);
        }

        public bool AddQuestionsToQuiz(int selectedWebinar, IEnumerable<Question> newQuestions)
        {
            var quiz = _quizRepository.GetQuizByWebinarId(selectedWebinar);

            // if there are any QuizUserAnswers for this quiz, this means someone has taken the quiz and no questions can be deleted.
            if (quiz.QuizWithQuestions.SelectMany(q => q.QuizUserAnswers).Any())
            {
                return false;
            }

            foreach (var question in newQuestions)
            {
                _quizRepository.AddQuestion(question);

                _quizRepository.SaveChanges();
            }

            return true;
        }

        public void AddWebinar(Webinar webinar)
        {
            _createWebinarValidator.ValidateAndThrow(webinar);

            _webinarRepository.Add(webinar);
        }

        public void AddWebinarFiles(IEnumerable<WebinarFile> webinarFiles)
        {
            _webinarFileRepository.AddRange(webinarFiles);
        }

        public void DeleteWebinar(int webinarId)
        {
            var webinar = GetWebinar(webinarId);
            _webinarRepository.Delete(webinar);
        }

        public void DeleteWebinarFiles(IEnumerable<WebinarFile> webinarFiles)
        {
            _webinarFileRepository.DeleteRange(webinarFiles);
        }

        public void DeleteRegTypeGroupXRef(Webinar webinar, int idRegTypeGroupXRef)
        {
            var regTypeGroupXRef = _webinarRepository.GetRegTypesGroupsXref(idRegTypeGroupXRef, webinar.idWebinar);

            webinar.RegTypesGroupsXref.Remove(regTypeGroupXRef);

            _webinarRepository.MarkForDeletion(regTypeGroupXRef);
        }

        public void DeleteWebinarTopicXref(Webinar webinar, int exisingTopicId)
        {
            var webinarTopicXRef = _webinarRepository.GetWebinarTopicXref(exisingTopicId, webinar.idWebinar);

            webinar.WebinarTopicXrefs.Remove(webinarTopicXRef);

            _webinarRepository.MarkForDeletion(webinarTopicXRef);
        }

        public void AddQuiz(int selectedWebinar, IEnumerable<Question> questions)
        {
            var quiz = _quizRepository.GetQuizByWebinarId(selectedWebinar);

            if (!ReferenceEquals(null, quiz))
                throw new InvalidOperationException(string.Format("Webinar with id {0} already has a quiz.", selectedWebinar));

            quiz = new Quiz
            {
                idWebinar = selectedWebinar,
                QuizCode =  RandomHelpers.GetUniqueCode(6)
            };
            _quizRepository.AddQuiz(quiz);

            _quizRepository.SaveChanges();

            foreach (var question in questions)
            {
                _quizRepository.AddQuestion(question);

                // new quiz was given an id < 0 at the client.
                question.QuizWithQuestions.Single(qwq => qwq.idQuiz < 0).idQuiz = quiz.Id;
            }

            _quizRepository.SaveChanges();

        }

        public Quiz GetQuizByCode(string quizCode)
        {
            return _quizRepository.GetQuizByCode(quizCode);
        }

        public QuestionCountAndWebinarId GetQuizQuestionCountAndWebinarId(string quizCode)
        {
            return _quizRepository.GetQuestionCountAndWebinarId(quizCode);
        }

        public Quiz GetQuizByQuizId(int quizId)
        {
            return _quizRepository.GetQuizByIdWithOptions(quizId);
        }

        public IList<QuizScore> GetQuizScoreForUser(int quizId, string email, int orderId)
        {
            var quiz = _quizRepository.GetQuizByIdWithOptionsAndUserAnswers(quizId);

            // ReSharper disable once TooWideLocalVariableScope
            bool isCorrect;
            var questionResult = new Dictionary<int, KeyValuePair<char, bool>>();
            IList<QuizScore> quizScores = new List<QuizScore>();

            foreach (var quizUserOrder in quiz.QuizUserOrders.Where(quo => quo.Email == email && quo.idQuiz == quizId && quo.idOrder == orderId))
            {
                int talley = 0;

                QuizUserOrder order = quizUserOrder; // to avoid unpredicted behaviour if different versions of the compiler compile this code.

                questionResult = new Dictionary<int, KeyValuePair<char, bool>>();

                foreach (var quizWithQuestion in quiz.QuizWithQuestions.Where(qwq => qwq.idQuiz == order.idQuiz))
                {
                    foreach (var quizUserAnswer in quizWithQuestion.QuizUserAnswers.Where(qua => qua.idQuizUserOrder == order.Id))
                    {
                        isCorrect = false;

                        if (quizUserAnswer.Letter.Contains(quizUserAnswer.QuizWithQuestion.Question.QuestionWithOptions.Single(qwo => qwo.CorrectAnswer).Letter))
                        {
                            isCorrect = true;
                            talley++;
                        }

                        questionResult.Add(quizWithQuestion.QuestionNumber, new KeyValuePair<char, bool>(quizUserAnswer.Letter.First(), isCorrect));
                    }
                }
                quizScores.Add(new QuizScore { QuestionResult = questionResult, TotalCorrectAnswerCount = talley, TotalQuestionCount = quiz.QuizWithQuestions.Count });
            }

            return quizScores;
        }

        public bool RemoveQuestionsFromQuiz(IEnumerable<int> deletedQuestions, int quizId)
        {
            var quiz = _quizRepository.GetQuizByIdWithOptions(quizId);

            // if there are any QuizUserAnswers for this quiz, this means someone has taken the quiz and no questions can be deleted.
            if (quiz.QuizWithQuestions.SelectMany(q => q.QuizUserAnswers).Any())
            {
                return false;
            }

            foreach (var deletedQuestion in deletedQuestions)
            {
                var quizWithQuestion = quiz.QuizWithQuestions.FirstOrDefault(q => q.Id == deletedQuestion);

                if (!ReferenceEquals(null, quizWithQuestion))
                {
                    _quizRepository.DeleteQuizWithQuestion(quizWithQuestion);
                }
            }
            _quizRepository.SaveChanges();

            return true;
        }

        public bool UpdateQuestions(IEnumerable<EditedQuestion> editedQuestions, int quizId)
        {
            var quiz = _quizRepository.GetQuizByIdWithOptionsText(quizId);

            // if there are any QuizUserAnswers for this quiz, this means someone has taken the quiz and no questions can be deleted.
            if (quiz.QuizWithQuestions.SelectMany(q => q.QuizUserAnswers).Any())
            {
                return false;
            }

            foreach (var editedQuestion in editedQuestions)
            {
                var question = quiz.QuizWithQuestions.Select(q => q.Question).Single(q => q.Id == editedQuestion.QuestionId);

                // get deleted options list
                var deletedOptions = editedQuestion.EditedOptions.Where(e => e.EditType == EditType.Deleted);
                // get added options list
                var addedOptions = editedQuestion.EditedOptions.Where(e => e.EditType == EditType.Added);
                // get edited options list
                var editedOptions = editedQuestion.EditedOptions.Where(e => e.EditType == EditType.Edited);

                // deal with option edits
                foreach (var editableOption in editedOptions)
                {
                    if (editableOption.NewCorrectStatus != editableOption.OriginalCorrectStatus ||
                        editableOption.NewOptionLetter != editableOption.OriginalOptionLetter ||
                        editableOption.NewOptionText.Trim() != editableOption.OriginalOptionText.Trim())
                    {
                        var questionWithOption = question.QuestionWithOptions.SingleOrDefault(qwo => qwo.Id == editableOption.OptionId);
                        if (ReferenceEquals(null, questionWithOption)) continue;

                        if (editableOption.NewCorrectStatus != editableOption.OriginalCorrectStatus)
                        {
                            questionWithOption.CorrectAnswer = editableOption.NewCorrectStatus;
                        }

                        if (editableOption.NewOptionLetter != editableOption.OriginalOptionLetter)
                        {
                            questionWithOption.Letter = editableOption.NewOptionLetter;
                        }

                        if (editableOption.NewOptionText.Trim() != editableOption.OriginalOptionText.Trim())
                        {
                            questionWithOption.Option.Text = editableOption.NewOptionText.Trim();
                        }
                    }
                }

                // deal with option deletions
                foreach (var deletedOption in deletedOptions)
                {
                    var questionWithOption = question.QuestionWithOptions.SingleOrDefault(q => q.Id == deletedOption.OptionId);

                    if (!ReferenceEquals(null, questionWithOption))
                    {
                        question.QuestionWithOptions.Remove(questionWithOption);
                    }

                    _quizRepository.DeleteQuizWithOption(questionWithOption);
                }

                // deal with option additions
                foreach (var addedOption in addedOptions)
                {
                    var newOption = new Option {Text = addedOption.NewOptionText};

                    var questionWithOption = new QuestionWithOption
                    {
                        CorrectAnswer = addedOption.NewCorrectStatus,
                        Option = newOption,
                        idQuestion = question.Id,
                        Letter = addedOption.NewOptionLetter,
                    };

                    question.QuestionWithOptions.Add(questionWithOption);
                }

                if (!ReferenceEquals(null, question))
                {
                    if (editedQuestion.NewQuestionNumber != editedQuestion.OriginalQuestionNumber)
                    {
                        var quizWithQuestion = question.QuizWithQuestions.SingleOrDefault(q => q.idQuestion == question.Id && q.idQuiz == quizId);

                        if (!ReferenceEquals(null, quizWithQuestion))
                        {
                            quizWithQuestion.QuestionNumber = editedQuestion.NewQuestionNumber;
                        }
                    }

                    if (editedQuestion.NewQuestionText != editedQuestion.OriginalQuestionText)
                    {
                        question.Text = editedQuestion.NewQuestionText;
                    }
                }
            }
            _quizRepository.SaveChanges();

            return true;
        }

        public void CloneQuizForWebinar(Webinar webinar, int existingQuizId)
        {
            var quiz = _quizRepository.GetQuizByIdWithOptionsText(existingQuizId);
            _quizRepository.CloneQuiz(quiz, webinar.idWebinar);
        }

        public IEnumerable<Webinar> GetByTopic(int topicId)
        {
            return _webinarRepository.GetByTopic(topicId);
        }

        public Quiz GetQuizByWebinarId(int idWebinar)
        {
            return _quizRepository.GetQuizByWebinarId(idWebinar);
        }

        public void UpdateWebinarFiles(IEnumerable<WebinarFile> webinarFiles)
        {
            _webinarFileRepository.UpdateRange(webinarFiles);
        }

        public Webinar GetCompliancePerspectives()
        {
            GetRegistrants_CP();
            return null;
        }

        public int GetRegTypeByLableAndWebinar(string registrationType, int idWebinar)
        {
            var regType = _webinarRepository.GetRegTypeByLableAndWebinar(registrationType, idWebinar);
            return regType;
        }

        public void SynchToLegacy()
        {
            _webinarRepository.SynchToLegacy();
        }

        public int GetRegTypeByACS(string registrationType, int idWebinar)
        {
           return _webinarRepository.GetRegTypeByACS(registrationType, idWebinar);
        }

        private IList<WebUser> GetRegistrants_CP()
        {
            return null;

        }
        public IEnumerable<Topic> GetTopicsPerWebinar(int idWebinar)
        {
            return _webinarRepository.GetTopicsPerWebinar(idWebinar).ToList();
        }
        public IEnumerable<WebinarFile> GetWebinarFilesPerWebinar(int idWebinar)
        {
            return _webinarRepository.GetWebinarFilesPerWebinar(idWebinar).ToList();
        }
        public IEnumerable<Webinar> GetRecordedWebinars()
        {
            return _webinarRepository.GetRecorded().ToList();
        }
        public IEnumerable<Webinar> GetUpcomingWebinars()
        {
            return _webinarRepository.GetUpcoming().ToList();
        }

        public IEnumerable<RegTypesGroup> GetRegTypeGroupsForWebinars(int idWebinar)
        {
            return _webinarRepository.GetRegTypeGroupsForWebinars(idWebinar);
        }

        public IEnumerable<RegTypesGroup> GetUpcomingRegTypesForWebinars()
        {
            return _webinarRepository.GetUpcomingRegTypesForWebinars();
        }

        public Webinar GetWebinar(int id)
        {
            return _webinarRepository.FindByIdLoaded(id);
        }

        public Webinar GetWebinarThin(int id)
        {
            return _webinarRepository.FindById(id);
        }

        public IEnumerable<Webinar> GetWebinarByPresenterLastName(string lastName)
        {
            return _webinarRepository.FindByPresenterLastName(lastName);
        }

        public IEnumerable<Webinar> GetWebinarByDescription(string topicDescription)
        {
            return _webinarRepository.FindByDescription(topicDescription);
        }

        public WebinarFile GetWebinarFile(int idWebinarFile)
        {
            return _webinarFileRepository.FindById(idWebinarFile);
        }

        public Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id)
        {
            try
            {
                return _webinarRepository.GetWebinarByIdIncludingAllWebinarsByPresenter(id);
                //return webinar;
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Failed attempt to retrieve webinar with id: " + id.ToString(), exception);
                return null;
            }

        }

        public int SaveChanges()
        {
            return _webinarRepository.SaveChanges();
        }


        public IDictionary<RegType, bool> FindRegTypesByWebinarId(int webinarId)
        {
            return _regTypeRepository.FindRegTypesByWebinarId(webinarId, false);
        }

        public IEnumerable<AdditionalLocationsLookupPrice> GetAdditionalLocationsLookupPricesForWebinar(int idWebinar)
        {
            return _webinarRepository.GetAdditionalLocationsLookupPricesForWebinar(idWebinar);
        }

        public IEnumerable<IEvent> GetEvents()
        {
            return _events;
        }

        public void Clear()
        {
            _events.Clear();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();


                _regTypeRepository.Dispose();
                _webinarRepository.Dispose();
                _webUserRepository.Dispose();
                
                _disposed = true;
            }
        }

    //    string IWebinarManagementService.CreateCalendarEvent(string title,
    //string body,
    //DateTime startDate,
    //double duration,
    //string location,
    //string organizer,
    //string eventId,
    //bool allDayEvent)
    //    {
    //        try
    //        {
    //            return CreateCalendarEvent(title, body, startDate, duration, location, organizer, eventId, allDayEvent);
    //        }
    //        catch (Exception exception)
    //        {
    //            _logger.ErrorException(string.Format("Title:{0},body:{1},startDate:{2},duration{3},location{4},organizer{5},eventId{6},allDayEvent" +
    //                                                 "{7}", title, body, startDate.ToString("yyyy-MM-dd-hh-mm-ss-fff-tt"), duration, location,
    //                                                 organizer, eventId, allDayEvent),
    //                                                 exception
    //                                                 );
    //            throw;
    //        }
    //    }


        public IEnumerable<Webinar> GetAllActive()
        {
            try
            {
                return _webinarRepository.GetAllActive().ToList();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetAllActive method", exception);
                throw;
            }
        }

        public void UpdateWebinar(Webinar webinar)
        {
            _updateWebinarValidator.ValidateAndThrow(webinar);
            _webinarRepository.Update(webinar);
        }
    }
}
